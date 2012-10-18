using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace L24CM.Serialization
{
    public class ExcludePropertiesContractResolver : DefaultContractResolver
    {
        IEnumerable<string> lstExclude;

        public ExcludePropertiesContractResolver(IEnumerable<string> excludedProperties)
        {
            lstExclude = excludedProperties;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization)
                .ToList()
                .FindAll(p => !lstExclude.Contains(p.PropertyName));
        }
    }
}
