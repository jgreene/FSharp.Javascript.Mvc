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

        /// <summary>
        /// Outputs a <script></script> tag that initializes validation for the current form model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static MvcHtmlString Validate<TModel>(this FSharpHelper<TModel> helper)
        {
            Type type = typeof(TModel);

            return new MvcHtmlString(string.Format(script, helper.HtmlHelper.ViewContext.FormContext.FormId, "", type.FullName));
        }

        /// <summary>
        /// Outputs a <script></script> tag that initializes validation for the selected form model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString Validate<TModel, TProp>(this FSharpHelper<TModel> helper, Expression<Func<TModel, TProp>> expression)
        {
            string expressionString = ExpressionHelper.GetExpressionText((LambdaExpression)expression);
            string fullHtmlFieldName = helper.HtmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionString);

            var type = typeof(TProp);

            return new MvcHtmlString(string.Format(script, helper.HtmlHelper.ViewContext.FormContext.FormId, fullHtmlFieldName, type.FullName));
        }

        const string OuterError = "<div id=\"{0}\" class=\"validationMessageFor {1}\">{2}</div>";
        const string InnerError = "<span>{0}</span>";

        /// <summary>
        /// Outputs validation errors.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString ValidationMessageFor<TModel, TProp>(this FSharpHelper<TModel> helper, Expression<Func<TModel, TProp>> expression)
        {
            string expressionString = ExpressionHelper.GetExpressionText((LambdaExpression)expression);
            string fullHtmlFieldName = helper.HtmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionString);

            if (helper.HtmlHelper.ViewData.ModelState.ContainsKey(fullHtmlFieldName) == false)
                return new MvcHtmlString(string.Format(OuterError, fullHtmlFieldName + "_validationMessage", "field-validation-valid", ""));

            var state = helper.HtmlHelper.ViewData.ModelState[fullHtmlFieldName];
            string errors = state.Errors.Aggregate(new StringBuilder(), (acc, error) => acc.Append(string.Format(InnerError, error.ErrorMessage))).ToString();

            return new MvcHtmlString(string.Format(OuterError, fullHtmlFieldName + "_validationMessage", "field-validation-error", errors));

        }
    }
}
