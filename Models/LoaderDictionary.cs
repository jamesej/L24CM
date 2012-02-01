using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Models
{
    public class LoaderDictionary : Dictionary<string, Func<RequestDataSpecification, object>>
    {
    }
}
