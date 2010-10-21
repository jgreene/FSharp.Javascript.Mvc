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
            FormValidator.setupValidation({{ Form : '{0}', Prefix : '{1}', Validators : {2}, 
get_Form : function() {{ return this.Form; }}, 
get_Prefix : function(){{ return this.Prefix; }},
get_Validators : function(){{ return this.Validators; }}
}})
        }})
        </script>";
        public static string FSharpValidation<TModel>(this HtmlHelper<TModel> helper)
        {
            Type type = typeof(TModel);

            return string.Format(script, helper.ViewContext.FormContext.FormId, "", Validation.getJavascriptForValidators(type));
        }





        public static string FSharpValidation<TModel, TProp>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProp>> expression)
        {
            string expressionString = ExpressionHelper.GetExpressionText((LambdaExpression)expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionString);

            var type = typeof(TProp);

            return string.Format(script, helper.ViewContext.FormContext.FormId, fullHtmlFieldName, Validation.getJavascriptForValidators(type));
        }
    }
}
