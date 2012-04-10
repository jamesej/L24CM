using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cyotek.Web.BbCodeFormatter;
using Newtonsoft.Json;
using System.Web.Mvc;
using L24CM.Binding;
using System.ComponentModel;
using System.Globalization;
using System.Web;

namespace L24CM.Models
{
    public class BbTextJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((value as BbText).Text);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(BbText).IsAssignableFrom(objectType);
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return (BbText)null;
            BbText bbt = Activator.CreateInstance(objectType, reader.Value as string) as BbText;
            return bbt;
        }
    }

    public class BbTextConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
           Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new BbText(value as string);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return (value as BbText).Text;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [JsonConverter(typeof(BbTextJsonConverter)), TypeConverter(typeof(BbTextConverter))]
    public class BbText
    {
        public static implicit operator BbText(string s)
        {
            if (s == null)
                return (BbText)null;
            return new BbText(s);
        }
        public static implicit operator string(BbText t)
        {
            if (t == null) return (string)null;
            return t.ToString();
        }

        public static BbText Empty
        {
            get
            {
                return new BbText("");
            }
        }

        string text = null;

        public string Text { get; set; }

        public BbText()
        { }
        public BbText(string s)
        {
            Text = s;
            text = BbCodeProcessor.Format(HttpUtility.HtmlEncode(s));
        }

        public override string ToString()
        {
            return text;
        }
    }
}
