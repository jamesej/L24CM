using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Models
{
    public class AddressedContent<T> where T : BaseContent
    {
        public ContentAddress Address { get; set; }
        public T Content { get; set; }
    }
}
