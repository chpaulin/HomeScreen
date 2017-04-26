using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtensions
    {
        public static T As<T>(this string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
