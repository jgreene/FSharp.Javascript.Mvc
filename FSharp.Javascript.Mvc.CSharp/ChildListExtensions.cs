using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using System.Web.WebPages;


namespace FSharp.Javascript.Mvc.Helpers
{
    public static class ChildListExtensions
    {
        public static HtmlHelper<TItem> GetChildHelper<TModel, TItem>(this FSharpHelper<TModel> helper, TItem item)
        {
            var page = new ViewPage<TItem>();
            page.ViewData = new ViewDataDictionary<TItem>(item);
            var itemHelper = new HtmlHelper<TItem>(helper.HtmlHelper.ViewContext, page);
            return itemHelper;
        }

        public static HelperResult ChildListFor<TModel, TProp, TItem>(this FSharpHelper<TModel> helper, Expression<Func<TModel, TProp>> prefixName, IEnumerable<TItem> items, Func<HtmlHelper<TItem>, HelperResult> action)
        {
            return new HelperResult((writer) =>
            {
                var collectionName = helper.HtmlHelper.NameFor(prefixName).ToHtmlString();
                foreach (var item in items)
                {
                    var itemHelper = helper.GetChildHelper(item);


                    using (itemHelper.FSharp().BeginCollectionItem(collectionName))
                    {
                        action(itemHelper).WriteTo(writer);
                    }
                }
            });
        }
    }
}
