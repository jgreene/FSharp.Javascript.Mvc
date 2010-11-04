module FormValidator

open System.Text
open System.IO
open System.Web.Mvc
open System.Web.Routing
open FSharp.Javascript.Mvc.Fakes

open FSharp.Javascript.Dom
open FSharp.Javascript.Jquery

open FSharp.Javascript.Library

open FSharp.Javascript.Mvc.Utilities


type FormValidator<'a> = {
    Form : string
    Prefix : string
    Type : string
} 

type Validator<'a> = {
    Type : string
    ErrorField : string
    FieldNames : (string * string) array
    Validator : 'a -> string option
}

type RemoteValidator = {
    url : string
    errorField : string
    arguments : (string * string) array
}

let getRemoteValidationResult<'a> (model:'a) (validator:RemoteValidator) = 
    let typ = typeof<'a>

    let args = validator.arguments |> Array.map (fun (a,b) -> 
                                                    let prop = typ.GetProperty(b)
                                                    (a, b, prop.GetValue(model, null)))

    let context = new FakeHttpContext("~" + validator.url, "POST")
    
    let routes = System.Web.Routing.RouteTable.Routes
    let routeData = routes.GetRouteData(context)
    

    for (a,b,v) in args do
        if routeData.Values.ContainsKey(a) then
            routeData.Values.[a] <- v
        else
            routeData.Values.Add(a, v)

    let stringResult = renderRouteToString context routeData
    
    if stringResult.Contains("Value") = false then
        None
    else
        let result = stringResult.Split(':').[1].Replace("\"", "").Replace("}", "")
        Some result

//onCompleteValidation is a hack to have asynchronous remote validators
let getFormModel<'a> (formValidator : FormValidator<'a>) (onCompleteValidation) = new obj() :?> 'a

let getValueFromModel (model:'a) (property:string) = ""

let setValueOnModel (model:'a) (property:string) (value:obj) = ()

let currentValidators<'a> () = [||] : 'a array


[<ReflectedDefinition>]
let setupValidation<'a> (formValidator : FormValidator<'a>) =
    let form = jquery("#" + formValidator.Form)

    let getInputName fieldName = 
        if formValidator.Prefix = "" then
            "input[name='" + fieldName + "']"
        else
            "input[name='" + formValidator.Prefix + "." + fieldName + "']"

    let getElement (prop:string) =
        if formValidator.Prefix = "" then
            jquery("#" + prop)
        else
            jquery("#" + formValidator.Prefix + "." + prop)

    let getErrorElement (elem:jquery) =
        let id = elem.attr("id") + "_validationMessage"
        jquery("#" + id)

    let errors = ref Map.empty<string,string list>

    let addError property errorMessage =
        if errors.Value.ContainsKey property then
            let propertyErrors = errors.Value |> Map.tryFind property

            if propertyErrors.IsSome then
                if propertyErrors.Value |> List.exists (fun x -> x = errorMessage) then
                    ()
                else
                    errors := errors.Value.Remove(property).Add(property, (errorMessage::propertyErrors.Value))
            else
                ()
        else
            errors := errors.Value.Add(property, [errorMessage])

    

    let getErrors property =
        if errors.Value.ContainsKey property then
            errors.Value |> Map.find property
        else
            []

    let displayErrors (property:string)  =
        let elem = getElement property
        let errorElement = getErrorElement elem
        if errors.Value.ContainsKey property then
            
            let errs = errors.Value |> Map.tryFind property
            if errs.IsNone then
                errorElement.hide() |> ignore
            else
                let errorMessage = System.String.Join("<br/>", errs.Value)
                //let errorMessage = errs.Value |> List.fold (fun acc next -> next + "<br/>" + acc) ""
                errorElement.html(errorMessage).show() |> ignore
        else
            errorElement.hide() |> ignore

    let resetErrors (property:string) =
        if errors.Value.ContainsKey property then
            errors := errors.Value.Remove(property)
            displayErrors property

    let checkTypes (props:(string * string) array) (model:'a) =
        let getValue = getValueFromModel model
        let setValue (prop:string, value:obj) = setValueOnModel model prop value
        let result = ref true

        let error prop errorMessage =
            addError prop errorMessage
            displayErrors prop
            result := false

        props 
        |> Array.iter (fun (prop, typ) ->
            let value = getValue prop
                                        
            match typ with
            | "DateTime option" ->
                let parsed = System.DateTime.TryParse2(value)

                if parsed.IsNone && value <> "" then
                    error prop "Invalid DateTime option"
                else
                    setValue (prop, parsed)

            | "Boolean option" ->
                let parsed = System.Boolean.TryParse2(value)

                if parsed.IsNone && value <> "" then
                    error prop "Invalid Boolean option"
                else
                    setValue (prop, parsed)

            | x when x.Contains("Int") && x.Contains("option") ->
                let parsed = System.Int16.TryParse2(value)

                if parsed.IsNone && value <> "" then
                    error prop "Invalid Integer option"
                else
                    setValue (prop, parsed)

            | x when (x.Contains("Decimal") || x.Contains("Double") || x.Contains("Single")) && x.Contains("option") ->
                let parsed = System.Decimal.TryParse2(value)
                if parsed.IsNone then
                    error prop "Invalid Decimal"
                else
                    setValue (prop, parsed.Value)
                    
            | "DateTime" -> 
                let parsed = System.DateTime.TryParse2(value)
                if parsed.IsNone then
                    error prop "Invalid DateTime"

            | x when x.Contains("Int") ->
                let parsed = System.Int16.TryParse2(value)
                if parsed.IsNone then
                    error prop "Invalid Integer"
                else
                    setValue (prop, parsed.Value)
                    
            | "Decimal" | "Double" | "Single" ->
                let parsed = System.Decimal.TryParse2(value)
                if parsed.IsNone then
                    error prop "Invalid Decimal"
                else
                    setValue (prop, parsed.Value)
            | "Boolean" ->
                let parsed = System.Boolean.TryParse2(value)
                if parsed.IsNone then
                    error prop "Invalid Boolean"
                else
                    setValue (prop, parsed.Value)
                
                
            | _ -> ()


        )

        result.Value
            
    let formValidators = (currentValidators ()) |> Seq.filter (fun validator -> validator.Type = formValidator.Type)

    formValidators    
    |> Seq.iter (fun validator -> do
                    let field = validator.ErrorField
                    let properties = validator.FieldNames
                    let inputName = getInputName field
                    let input = form.find(inputName)

                    input.bind("FSharpValidate", 
                        (fun () -> 
                            let model = getFormModel formValidator (fun field error ->
                                                                    addError field error
                                                                    displayErrors field
                                                                )
                            if (checkTypes properties model) then
                            
                                let result = validator.Validator(model)
                            
                                match result with
                                | Some x -> 
                                    addError field x
                                    false
                                | None -> true
                            else
                                false
                                                    
                        )) |> ignore

                    input.blur(fun () ->
                        resetErrors field
                        input.triggerHandler("FSharpValidate") |> ignore
                        displayErrors field
                    ) |> ignore
                )

    form.submit(fun () ->

        formValidators
        |> Seq.iter (fun validator -> do
            let field = validator.ErrorField
            let inputName = getInputName field
            let input = form.find(inputName)

            resetErrors field

            input.triggerHandler("FSharpValidate") |> ignore
            displayErrors field
            )

        errors.Value.Count = 0
    ) |> ignore