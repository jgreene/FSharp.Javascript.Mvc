using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FSharp.Javascript.Mvc
{
    public static class FScriptHelpers
    {
        public static FSharpHelper FSharp(this HtmlHelper helper)
        {
            return new FSharpHelper(helper);
        }

        public static FSharpHelper<T> FSharp<T>(this HtmlHelper<T> helper)
        {
            return new FSharpHelper<T>(helper);
        }

        const string ScriptTag = "<script type=\"text/javascript\" src=\"{0}\"></script>";
        public static MvcHtmlString GetCompiledModule(this FSharpHelper helper, Type moduleType)
        {
            if (moduleType == null)
                throw new NullReferenceException("moduleType cannot be null");

            var urlHelper = new UrlHelper(helper.HtmlHelper.ViewContext.RequestContext);

            return new MvcHtmlString(string.Format(ScriptTag, urlHelper.Action("GetCompiledModule", "FScript", new { typ = moduleType.FullName + "," + moduleType.Assembly.FullName })));
        }

        public static MvcHtmlString GetRequiredScripts(this FSharpHelper helper)
        {
            var urlHelper = new UrlHelper(helper.HtmlHelper.ViewContext.RequestContext);

            return new MvcHtmlString(string.Format(ScriptTag, urlHelper.Action("GetRequiredScripts", "FScript")));
        }

        public static MvcHtmlString GetAllValidators(this FSharpHelper helper)
        {
            var urlHelper = new UrlHelper(helper.HtmlHelper.ViewContext.RequestContext);

            return new MvcHtmlString(string.Format(ScriptTag, urlHelper.Action("GetAllValidators", "FScript")));
        }
    }
}
