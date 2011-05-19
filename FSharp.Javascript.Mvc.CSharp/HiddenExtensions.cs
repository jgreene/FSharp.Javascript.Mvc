using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using Microsoft.Web.Mvc;

namespace FSharp.Javascript.Mvc.Helpers
{
    public static class IntouchHiddenExtensions
    {
        public static MvcHtmlString HiddenFor<TModel, TProp>(this FSharpHelper<TModel> helper, Expression<Func<TModel, TProp>> exp)
        {
            var name = helper.HtmlHelper.NameFor(exp).ToHtmlString();

            return helper.HtmlHelper.Hidden(name);
        }

    }
}
