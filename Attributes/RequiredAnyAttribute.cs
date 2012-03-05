using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace L24CM.Attributes
{
    /// <summary>
    /// This attribute validates a model that at least one of a group of properties has
    /// a value for every member of the group
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequiredAnyAttribute : ValidationAttribute
    {
        string attributeGroups = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attributeGroups">A series of names of properties of the class, separated first by '|' for each validation group, then with ',' within the group</param>
        public RequiredAnyAttribute(string attributeGroups)
        {
            this.attributeGroups = attributeGroups;
        }

        // Needed to make AllowMultiple work for an MVC validation attribute
        object _typeId = new object();
        public override object TypeId
        {
            get
            {
                return this._typeId;
            }
        }

        public override bool IsValid(object value)
        {
            return attributeGroups.Split('|')
                .Any(ag => ag.Split(',')
                    .All(a => value.GetType()
                                    .GetProperty(a.Trim())
                                    .GetValue(value, null) != null));
        }
    }
}
