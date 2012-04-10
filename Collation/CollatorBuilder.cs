using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Collation
{
    public class CollatorBuilder
    {
        public static ICollatorFactory Factory { get; set; }

        static CollatorBuilder()
        {
            Factory = new DefaultCollatorFactory();
        }
    }
}
