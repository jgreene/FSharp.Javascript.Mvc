using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace FSharp.Javascript.Mvc
{
    public static class ValidationHelpers
    {

        const string script = @"<script type='text/javascript'>
$(document).ready(function(){{
    FormValidator.setupValidation({{ Form : '{0}', Prefix : '{1}', Type : '{2}', 
        get_Form : function() {{ return this.Form; }}, 
        get_Prefix : function(){{ return this.Prefix; }},
        get_Type : function(){{ return this.Type; }}
    }})
}})
</script>";
        public static MvcHtmlString FSharpValidation<TModel>(this HtmlHelper<TModel> helper)
        {
            Type type = typeof(TModel);

            return new MvcHtmlString(string.Format(script, helper.ViewContext.FormContext.FormId, "", type.FullName));
        }

        public static MvcHtmlString FSharpValidation<TModel, TProp>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProp>> expression)
        {
            string expressionString = ExpressionHelper.GetExpressionText((LambdaExpression)expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionString);

            var type = typeof(TProp);

            return new MvcHtmlString(string.Format(script, helper.ViewContext.FormContext.FormId, fullHtmlFieldName, type.FullName));
        }

        const string OuterError = "<div id=\"{0}\" class=\"{1}\">{2}</div>";
        const string InnerError = "<div>{0}</div>";
        public static MvcHtmlString FSharpValidationMessageFor<TModel, TProp>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProp>> expression)
        {
            string expressionString = ExpressionHelper.GetExpressionText((LambdaExpression)expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionString);

            if (helper.ViewData.ModelState.ContainsKey(fullHtmlFieldName) == false)
                return new MvcHtmlString(string.Format(OuterError, fullHtmlFieldName + "_validationMessage", "field-validation-valid", ""));

            var state = helper.ViewData.ModelState[fullHtmlFieldName];
            string errors = state.Errors.Aggregate(new StringBuilder(), (acc, error) => acc.Append(string.Format(InnerError, error.ErrorMessage))).ToString();

            return new MvcHtmlString(string.Format(OuterError, fullHtmlFieldName + "_validationMessage", "field-validation-error", errors));

        }
    }
}
