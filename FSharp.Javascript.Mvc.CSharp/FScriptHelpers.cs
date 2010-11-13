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

        /// <summary>
        /// Translates a module from F# to javascript.  This should only be called on F# modules that are intended to become javascript.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public static MvcHtmlString GetCompiledModule(this FSharpHelper helper, Type moduleType)
        {
            if (moduleType == null)
                throw new NullReferenceException("moduleType cannot be null");

            var urlHelper = new UrlHelper(helper.HtmlHelper.ViewContext.RequestContext);

            return new MvcHtmlString(string.Format(ScriptTag, urlHelper.Action("GetCompiledModule", "FScript", new { typ = moduleType.FullName + "," + moduleType.Assembly.FullName })));
        }

        /// <summary>
        /// Builds a <script> tag that gets all of the required javascript for running F# programs in the browser, while also adding support for the F# validation framework.
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static MvcHtmlString GetRequiredScripts(this FSharpHelper helper)
        {
            var urlHelper = new UrlHelper(helper.HtmlHelper.ViewContext.RequestContext);

            return new MvcHtmlString(string.Format(ScriptTag, urlHelper.Action("GetRequiredScripts", "FScript")));
        }

        /// <summary>
        /// Builds a <script> tag that gets all of the validators registered with the F# validation framework.
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static MvcHtmlString GetAllValidators(this FSharpHelper helper)
        {
            var urlHelper = new UrlHelper(helper.HtmlHelper.ViewContext.RequestContext);

            return new MvcHtmlString(string.Format(ScriptTag, urlHelper.Action("GetAllValidators", "FScript")));
        }
    }
}
