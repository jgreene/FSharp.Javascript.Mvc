module FSharp.Javascript.Mvc.Validation

open System
open System.Web.Mvc
open System.Collections.Generic
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.ExprShape
open Microsoft.FSharp.Linq.QuotationEvaluation
open FormValidator

type IValidator =
    abstract typ : Type;
    abstract errorField : string
    abstract properties : (string * string) list;
    abstract expr : Microsoft.FSharp.Quotations.Expr
    abstract javascript : string;
    abstract validate : obj -> string option

type Validator<'a>(errorField, properties, expr: Microsoft.FSharp.Quotations.Expr<'a -> string option>) =
    let validator = expr.Compile()()
    interface IValidator with

        member this.typ = typeof<'a>
        member this.errorField = errorField
        member this.properties = properties
        member this.expr = expr :> Microsoft.FSharp.Quotations.Expr
        
        member this.javascript = (FSharp.Javascript.Converter.convert expr).Replace(";", "")

        member this.validate model = 
            let model = model :?> 'a
            validator(model)

let private validators = new System.Collections.Concurrent.ConcurrentBag<IValidator>();

let private getPropertyType (typ:System.Type) =
    match typ with
    | typ when typ.Name = "FSharpOption`1" -> 
        let innerTyp = typ.GetGenericArguments().[0]
        innerTyp.Name + " option"
    | _ -> typ.Name

let private getProperties<'a, 'b> (expression:Expr<'a -> 'b>) = 
    let typ = typeof<'a>

    let rec loop expr acc =
        match expr with
//        | Patterns.Value(x,y) ->
//            acc
//        | Patterns.Lambda(x,e) -> loop e acc
        | Patterns.PropertyGet(e, p, xs) -> 
            if e.IsSome then
                let e' = loop e.Value acc
                if p.DeclaringType = typ then
                    (p.Name, getPropertyType p.PropertyType)::e'
                else
                    e'
            else
                if p.DeclaringType = typ then
                    (p.Name, getPropertyType p.PropertyType)::acc
                else
                    acc

//        | Patterns.IfThenElse(s,b,e) ->
//            let s' = loop s acc
//            let b' = loop b s'
//            loop e b'
//        | Patterns.Application(l,r) ->
//            let l' = loop l acc
//            let r' = loop r acc
//            r'
//        | Patterns.NewUnionCase(i, args) ->
//            let args' = [for a in args do yield! (loop a [])]
//            args'@acc
//        | Patterns.Call(e,m, args) ->
//            let args' = [for a in args do yield! (loop a [])]
//            if e.IsSome then
//                let e' = loop e.Value acc
//                args'@e'
//            else
//                args'@acc
//        | Patterns.Let(v, value, after) ->
//            let value' = loop value acc
//            let after' = loop after value'
//            after'
//        | Patterns.Quote(x) ->
//            loop x acc
//        | Patterns.Coerce(x, t) ->
//            loop x acc
        
        | ShapeVar v -> acc
        | ShapeLambda (var, expr) ->
            loop expr acc
        | ShapeCombination(ob, list) ->
            let result = [for l in list do yield! loop l []]
            result@acc

    let result = loop expression []
    result
                                                                                

let getValidators (typ:Type) =
    validators |> Seq.filter (fun x -> x.typ = typ)


let registerValidator<'a>(expr:Expr<'a -> string option>) =
    let properties = (getProperties expr) |> Seq.distinct |> Seq.toList
    let propertyName = properties |> List.head |> fst
    let validator = new Validator<'a>(propertyName, properties, expr) :> IValidator

    validators.Add(validator)

let private getUrlInfo<'a, 'b, 'c>(expr:Expr<'a -> ('b  -> 'c)>) =
    let getControllerName (name:string) =
        if name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) then
            name.Remove(name.Length - 10, 10)
        else
            name

    let getArg arg =
        match arg with
        | Patterns.PropertyGet(a,b,c) -> b.Name
        | _ -> failwith "Invalid argument"
             
    match expr with
    | Patterns.Lambda(v, Patterns.Lambda(v', Patterns.Call(Some(controller), m, args))) ->
         let controllerName = getControllerName(v.Type.Name)
         let actionname = m.Name

         let parameters = m.GetParameters()
         
         let arguments = seq { for i in {0..(parameters.Length - 1) } -> (parameters.[i].Name, getArg(args.[i])) }


         let valueArgs = [for (a,b) in arguments -> Expr.NewTuple([Expr.Value(a);Expr.Value(b)])]

         
         let array = Expr.NewArray(typeof<Tuple<string,string>>, valueArgs)
         
         Expr.Cast<RemoteValidator>(Expr.NewRecord(typeof<RemoteValidator>, [Expr.Value("/test/ValidateEmail"); array]))
            
    | _ -> failwith "Invalid Remote Validator"



let registerRemoteValidator<'a, 'b, 'c>(expr:Expr<'a -> ('b  -> 'c)>)  =
    

    let properties = (getProperties expr) |> Seq.distinct |> Seq.toList
    let propertyName = properties |> List.head |> fst

    let urlInfo = getUrlInfo expr

    let expr = <@ fun (x:'a) -> getRemoteValidationResult x %urlInfo @>

    let validator = new Validator<'a>(propertyName, properties, expr) :> IValidator

    validators.Add(validator)


let getJavascriptForValidators (typ:Type) =
    let validators = getValidators typ
    
    let getProperties props =
        let props = props |> List.map (fun (name,typ) -> "{ Item1 : \"" + name + "\", Item2 : \"" + typ + "\" }")
        String.Join(",", props)

    let script = [for v in validators -> sprintf "{ ErrorField : '%s', FieldNames : [%s], Validator : %s, 
    get_ErrorField : function () { return this.ErrorField; }, 
    get_FieldNames : function(){ return this.FieldNames; }, 
    get_Validator : function(){ return this.Validator; } }" v.errorField (getProperties v.properties) (v.javascript)]
    let result = script |> String.concat ","
    "[" + result + "]"




//type ValidationBinder() =
//    inherit System.Web.Mvc.DefaultModelBinder()
//
//    override this.BindModel(controllerContext:ControllerContext, bindingContext:ModelBindingContext) =
//        let model = base.BindModel(controllerContext, bindingContext)
//        
//        model




//type FSharpModelValidatorProvider() =
//    inherit System.Web.Mvc.ModelValidatorProvider()
//    override this.GetValidators(metadata, context) =

    