using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.FSharp.Core;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace FSharp.Javascript.Mvc
{
    public static class OptionHelpers
    {
        public static T ValueOrDefault<T>(this FSharpOption<T> option)
        {
            if (FSharpOption<T>.get_IsSome(option))
            {
                return option.Value;
            }

            return default(T);
        }

        public static T ValueOrDefault<T>(this FSharpOption<T> option, T defaultValue)
        {
            if (FSharpOption<T>.get_IsSome(option))
            {
                return option.Value;
            }

            return defaultValue;
        }

        public static string StringOrEmpty<T>(this FSharpOption<T> option)
        {
            if (FSharpOption<T>.get_IsSome(option))
            {
                return option.Value.ToString();
            }

            return "";
        }

        public static string ToShortDateString(this FSharpOption<DateTime> option)
        {
            if (FSharpOption<DateTime>.get_IsSome(option))
            {
                return option.Value.ToShortDateString();
            }
            
            return "";
        }

        public static string ToShortDateString(this DateTime? option)
        {
            if (option.HasValue)
            {
                return option.Value.ToShortDateString();
            }

            return "";
        }
    }
}
