namespace FSharp.Javascript.Mvc

open System.Web.Mvc

type FSharpHelper(helper:HtmlHelper) =
    member this.HtmlHelper = helper

type FSharpHelper<'a>(helper:HtmlHelper<'a>) =
    inherit FSharpHelper(helper)
    member this.HtmlHelper = helper
