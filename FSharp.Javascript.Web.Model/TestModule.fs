module FSharp.Javascript.Web.TestModule

open FSharp.Javascript.Dom
open FSharp.Javascript.Jquery

type TestType =
    | One
    | Two

[<ReflectedDefinition>]
let main() =
    jquery(document).ready(fun () ->
        let r = One

        match r with
        | One -> alert(true)
        | Two -> alert(false)
    )

