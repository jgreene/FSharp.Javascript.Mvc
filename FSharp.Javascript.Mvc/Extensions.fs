module FSharp.Javascript.Mvc.Extensions

type ValidationResult(result:string option) =
    inherit System.Web.Mvc.JsonResult()
    do base.Data <- result

type System.Web.Mvc.Controller with
    member this.Validate(result:string option) =
        ValidationResult(result)