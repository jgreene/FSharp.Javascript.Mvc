namespace FSharp.Javascript.Mvc

open System.Web.Mvc
open FSharp.Javascript.Converter
open FSharp.Javascript.Mvc.Validation


type FScriptController() =
    inherit Controller()
    

    let getEmbeddedScript (assembly:System.Reflection.Assembly) (name:string) =
        use stream = assembly.GetManifestResourceStream(name)
        use reader = new System.IO.StreamReader(stream)
        reader.ReadToEnd()

    let getRequiredScripts (cache:System.Web.Caching.Cache) =
        let key = "FScript"
        let scripts = cache.Get(key)
        if scripts <> null then
            scripts.ToString()
        else
            let typ = System.Type.GetType("FormValidator, FSharp.Javascript.Mvc")
            let getEmbeddedScript = getEmbeddedScript typ.Assembly
            let formValidatorScript = convertModule typ
            let scripts = ["FSharp.Javascript.js";
                            "FSharp.Javascript.Library.js";
                            "FSharp.Javascript.Dom.js";
                            "FSharp.Javascript.Jquery.js";
                            ]

            let newLine = System.Environment.NewLine

            let mainScript = scripts 
                            |> List.fold (fun acc name ->
                                            let script = getEmbeddedScript name
                                            acc + newLine + script
                                        ) ""

            let result = mainScript + newLine + formValidatorScript + newLine + (getEmbeddedScript "FSharp.Javascript.Validation.Utils.js")

            cache.Insert(key, result)
            result

    let getValidator (validator:IValidator) =
        let getProperties props =
            let props = props |> List.map (fun (name,typ) -> "{ Item1 : \"" + name + "\", Item2 : \"" + typ + "\" }")
            System.String.Join(",", props)

        sprintf "{ 
Type : '%s', 
ErrorField : '%s', 
FieldNames : [%s], 
Validator : %s,
get_Type : function() { return this.Type },
get_ErrorField : function() { return this.ErrorField },
get_FieldNames : function() { return this.FieldNames },
get_Validator : function () { return this.Validator }
}"
            validator.typ.FullName 
            validator.errorField 
            (getProperties validator.properties)
            validator.javascript

    let getValidators () =
        let validators = Validation.getAllValidators ()
        let jsValidators = validators |> Seq.map getValidator
        let result = System.String.Join(",", jsValidators)
        sprintf "[%s]" result


    member this.GetCompiledModule(typ:string) =
        this.Response.AddHeader("Expires", System.DateTime.Now.AddDays(1.0).ToString())
        let javascript = this.HttpContext.Cache.Get(typ) :?> string
        base.Content(javascript, "application/x-javascript")

    
    member this.GetRequiredScripts() =
        try
            this.Response.AddHeader("Expires", System.DateTime.Now.AddDays(1.0).ToString())
            let script = getRequiredScripts this.HttpContext.Cache
            base.Content(script, "application/x-javascript")
        with
        | ex -> base.Content(ex.ToString(), "application/x-javascript")

    member this.GetAllValidators() =
        this.Response.AddHeader("Expires", System.DateTime.Now.AddDays(1.0).ToString())
        let script = sprintf "$(document).ready(function(){ FormValidator.currentValidators = function () { return %s } })" (getValidators ())
        
        base.Content(script, "application/x-javascript")
        
        
        


        

