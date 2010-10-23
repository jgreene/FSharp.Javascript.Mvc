module FSharp.Javascript.Mvc.Setup

open System.Web.Mvc
open FSharp.Javascript.Mvc

let initialize () =
    ModelBinders.Binders.DefaultBinder <- new RecordDefaultModelBinder()

    ModelValidatorProviders.Providers.Clear()

    ModelValidatorProviders.Providers.Add(new FSharpValidationProvider())