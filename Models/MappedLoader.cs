using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LoaderFunc = System.Func<L24CM.Models.RequestDataSpecification, object>;

namespace L24CM.Models
{
    public abstract class MappedLoader
    {
        LoaderDictionary loaders = new LoaderDictionary();

        public LoaderDictionary Loaders
        {
            get { return loaders; }
            set { loaders = value; }
        }

        public abstract void Load(RequestDataSpecification rds);
    }

    public class MappedLoader<T> : MappedLoader where T : class
    {
        public T Loadee { get; private set; }

        public MappedLoader(T loadee)
        {
            Loadee = loadee;
        }

        /// <summary>
        /// returns an action on an object which assigns that object to the specified property path of the loadee
        /// </summary>
        /// <param name="path">Property path of loadee</param>
        /// <returns>action which assigns an object to a specified property (or subproperty) of the loadee</returns>
        public Action<object> GetAssignerByPath(string path)
        {
            object currObject = Loadee;
            string[] pathEls = path.Split('.');
            for (int i = 0; i < pathEls.Length - 1; i++)
                currObject = currObject.GetType().GetProperty(pathEls[i]).GetValue(currObject, null);

            return o => currObject.GetType().GetProperty(pathEls.Last()).SetValue(currObject, o, null);
        }

        /// <summary>
        /// Load the loadee using the Loaders provided according to the RequestDataSpecification
        /// </summary>
        /// <param name="rds">Operations to apply to collections within loaded data</param>
        public override void Load(RequestDataSpecification rds)
        {
            if (rds.LoadPath == null) return;

            Dictionary<string, LoaderFunc> relevantLoaders = Loaders.Where(l => l.Key.StartsWith(rds.LoadPath)).ToDictionary(l => l.Key, l => l.Value);
            foreach (KeyValuePair<string, LoaderFunc> loader in relevantLoaders)
            {
                Action<object> assignToProperty = GetAssignerByPath(loader.Key);
                LoaderFunc loaderFunc = loader.Value;
                object loadedValue = loaderFunc(rds);
                assignToProperty(loadedValue);
            }
        }
    }
}
