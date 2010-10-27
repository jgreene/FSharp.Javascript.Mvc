using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FSharp.Javascript.Mvc
{
    public static class FScriptHelpers
    {
        const string ScriptTag = "<script type=\"text/javascript\" src=\"{0}\"></script>";
        public static MvcHtmlString GetCompiledModule(this HtmlHelper helper, Type moduleType)
        {
            if (moduleType == null)
                throw new NullReferenceException("moduleType cannot be null");

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            return new MvcHtmlString(string.Format(ScriptTag, urlHelper.Action("GetCompiledModule", "FScript", new { typ = moduleType.FullName + "," + moduleType.Assembly.FullName })));
        }

        public static MvcHtmlString GetRequiredFScriptFiles(this HtmlHelper helper)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            return new MvcHtmlString(string.Format(ScriptTag, urlHelper.Action("GetRequiredScripts", "FScript")));
        }
    }
}
