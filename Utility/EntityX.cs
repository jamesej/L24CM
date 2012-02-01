using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Data;
using System.Data.Objects;
using System.Data.Metadata.Edm;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace L24CM.Utility
{
    public static class EntityX
    {
        public static void SetKey(this ObjectContext ctx, EntityObject entity, string foreignName, string keyName, object value)
        {
            entity.SetKey(ctx.GetType().Name, foreignName, keyName, value);
        }

        public static void SetKey(this EntityObject entity, string contextTypeName, string foreignName, string keyName, object value)
        {
            SetKey(entity, contextTypeName, foreignName, foreignName, keyName, value);
        }
        public static void SetKey(this EntityObject entity, string contextTypeName, string propName, string foreignName, string keyName, object value)
        {
            EntityReference er = entity.GetType().GetProperty(propName + "Reference").GetValue(entity, null) as EntityReference;
            if (er.IsLoaded)
                throw new Exception("Trying to set EntityKey where " + propName + " is already loaded");
            else
                er.EntityKey = new EntityKey(contextTypeName + "." + foreignName + "Set", keyName, value);
        }

        public static T GetKey<T>(this EntityObject entity, string foreignName, string keyName)
        {
            EntityReference er = entity.GetType().GetProperty(foreignName + "Reference").GetValue(entity, null) as EntityReference;
            EntityObject eo = entity.GetType().GetProperty(foreignName).GetValue(entity, null) as EntityObject;
            if (er.EntityKey == null)
                er.Load();
            else if (!er.IsLoaded)
                return (T)er.EntityKey.EntityKeyValues[0].Value;

            return (eo == null ? default(T) : (T)eo.GetType().GetProperty(keyName).GetValue(eo, null));
        }

        public static T GetKey<T>(this EntityObject entity)
        {
            if (entity.EntityKey != null)
                return (T)entity.EntityKey.EntityKeyValues[0].Value;
            PropertyInfo pi = GetPrimaryKeyInfo(entity);
            return (T)pi.GetValue(entity, null);
        }

        public static T DetachedClone<T>(this T entity) where T : EntityObject
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, entity);
                stream.Position = 0;
                return formatter.Deserialize(stream) as T;
            }
        }

        public static PropertyInfo GetPrimaryKeyInfo(EntityObject entity)
        {
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo pI in properties)
            {
                System.Object[] attributes = pI.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    if (attribute is EdmScalarPropertyAttribute)
                    {
                        if ((attribute as EdmScalarPropertyAttribute).EntityKeyProperty == true)
                            return pI;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Ensures that the entity collection contains all and only the listed entities using the supplied key selector
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entities"></param>
        /// <param name="newEntities"></param>
        /// <param name="keySelector"></param>
        public static void UpdateMany<T, TKey>(this EntityCollection<T> entities, IEnumerable<T> newEntities, Func<T, TKey> keySelector)
            where T : class, IEntityWithRelationships
        {
            List<TKey> entKeys = entities.Select(keySelector).OrderBy(k => k).ToList();
            List<TKey> newEntKeys = newEntities.Select(keySelector).OrderBy(k => k).ToList();
            if (!entKeys.SequenceEqual(newEntKeys))
            {
                foreach (T entity in entities)
                    if (newEntKeys.BinarySearch(keySelector(entity)) < 0)
                        entities.Remove(entity);
                foreach (T newEntity in newEntities)
                    if (entKeys.BinarySearch(keySelector(newEntity)) < 0)
                        entities.Add(newEntity);
            }
        }
    }
}
