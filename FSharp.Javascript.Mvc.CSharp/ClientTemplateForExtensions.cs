using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using System.Linq.Expressions;
using System.Web.WebPages;

namespace FSharp.Javascript.Mvc.Helpers
{
    public static class ClientTemplateForExtensions
    {
        const string ClientScript = @"
	<script type='text/javascript'>
		function {0}(model, after){{
			var appendSelector = '{1}';
			var collectionName = '{2}';
			var uniqueId = '{3}';
			var templateId = '{4}';
			mvcAddTemplate(appendSelector, collectionName, uniqueId, templateId, model, after);
		}}
	</script>
";
        public static HelperResult ClientTemplateFor<TModel, TProp>(this FSharpHelper<TModel> helper, string appendSelector, string clientFunctionName, Expression<Func<TModel, IEnumerable<TProp>>> prefixName, Func<HtmlHelper<TProp>, HelperResult> action) where TProp : new()
        {
            var collectionName = helper.HtmlHelper.NameFor(prefixName).ToHtmlString();
            var functionName = string.IsNullOrEmpty(clientFunctionName) ? "add" + collectionName : clientFunctionName;

            var item = new TProp();
            var page = new ViewPage<TProp>();
            page.ViewData = new ViewDataDictionary<TProp>(item);
            var itemHelper = new HtmlHelper<TProp>(helper.HtmlHelper.ViewContext, page);

            var templateId = Guid.NewGuid().ToString();



            return new HelperResult((writer) =>
            {
                writer.WriteLine(string.Format("<script type='text/html' id='{0}' style='display:none;'>", templateId));

                using (itemHelper.FSharp().BeginCollectionItem(collectionName))
                {
                    var fieldId = itemHelper.ViewData.TemplateInfo.HtmlFieldPrefix.Replace(collectionName, "").Replace("[", "").Replace("]", "");

                    action(itemHelper).WriteTo(writer);
                    writer.WriteLine("</script>");
                    writer.WriteLine(string.Format(ClientScript, functionName, appendSelector, collectionName, fieldId, templateId));
                }
            });
        }
    }
}
