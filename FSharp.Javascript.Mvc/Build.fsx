#r "../Dependencies/FSharp.Javascript.dll"
#r "bin/debug/FSharp.Javascript.Mvc.dll"

open System.IO
open FSharp.Javascript

System.Threading.ThreadPool.QueueUserWorkItem(fun _ ->
    let utilitiesPath = "../../FSharp.Javascript.Validation.Utils.js"

    let finalPath = "../../FSharp.Javascript.Validation.js"

    let utilitiesScript = File.ReadAllText(utilitiesPath)

    let formValidatorScript = Converter.convertModule (System.Type.GetType("FormValidator, FSharp.Javascript.Mvc"))

    let finalScript = formValidatorScript + System.Environment.NewLine + utilitiesScript

    File.WriteAllText(finalPath, finalScript)

) |> ignore

