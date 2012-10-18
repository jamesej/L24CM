using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Models
{
    public class AddressedSummary<T> where T : Summary
    {
        public ContentAddress Address { get; set; }
        public T Summary { get; set; }
    }
}
