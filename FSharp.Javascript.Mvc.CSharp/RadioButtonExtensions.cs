using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Microsoft.Web.Mvc;
using System.Linq.Expressions;

namespace FSharp.Javascript.Mvc
{
    public static class RadioButtonExtensions
    {
        public static MvcHtmlString RadioButtonFor<TModel, TProp>(this FSharpHelper<TModel> helper, Expression<Func<TModel, TProp>> exp, bool isChecked, string trueOption, string falseOption)
        {
            var name = helper.HtmlHelper.NameFor(exp).ToHtmlString();

            var elem1 = helper.HtmlHelper.RadioButton(name, "True", isChecked);
            var label1 = helper.HtmlHelper.Label(name, trueOption);
            var elem2 = helper.HtmlHelper.RadioButton(name, "False", isChecked);
            var label2 = helper.HtmlHelper.Label(name, falseOption);

            return MvcHtmlString.Create("<div>" + elem1.ToHtmlString() + label1.ToHtmlString() + "</div><div>" + elem2.ToHtmlString() + label2.ToHtmlString() + "</div>");
        }
    }
}
