module FSHarp.Javascript.Mvc.Utilities

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.ExprShape
open System
open System.Web.Routing


let getRouteFromExpression expr =
    let getControllerName (name:string) =
        if name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) then
            name.Remove(name.Length - 10, 10)
        else
            name

    match expr with
    | Patterns.Lambda(v, Patterns.Call(Some(controller), m, args)) ->
        let dict = new RouteValueDictionary()
        dict.Add("controller", getControllerName(v.Type.Name))
        dict.Add("action", m.Name)

        dict
            
    | _ -> failwith "Invalid Route"