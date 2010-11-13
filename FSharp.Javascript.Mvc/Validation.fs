module FSharp.Javascript.Mvc.Validation

open System
open System.Web.Mvc
open System.Collections.Generic
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.ExprShape
open Microsoft.FSharp.Linq.QuotationEvaluation
open FormValidator
open FSharp.Javascript.Mvc.Fakes
open FSharp.Javascript.Mvc.Utilities

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

    
let mutable validators = new ResizeArray<IValidator>()

let private getPropertyType (typ:System.Type) =
    match typ with
    | typ when typ.Name = "FSharpOption`1" -> 
        let innerTyp = typ.GetGenericArguments().[0]
        innerTyp.Name + " option"
    | _ -> typ.Name

let private getProperties<'a, 'b> (expression:Expr<'a -> 'b>) = 
    let typ = typeof<'a>

    let primitiveTypes = [typeof<string>;typeof<bool>;typeof<int>;typeof<single>;typeof<System.DateTime>;typeof<float>;typeof<decimal>;typeof<double>;]
    let isPrimitive (typ:System.Type) =
        typ.IsArray || (typ.IsGenericType) || (primitiveTypes |> List.exists (fun x -> typ.IsAssignableFrom(x) || x = typ))

    let getPropertyInfos expr =
        let rec loop expr acc =
            match expr with
            | Patterns.PropertyGet(e, p, xs) ->
                if e.IsSome then
                    let result = loop e.Value acc
                    p::result
                else
                    p::acc
            | _ -> acc

        (loop expr []) |> List.rev

    let rec loop expr acc =
        match expr with
        | Patterns.PropertyGet(e, p, xs) -> 
            
            if e.IsSome then
                let properties = getPropertyInfos expr
                let props = ref []
                let cont = ref true
                let propertyType = ref null
                for p in properties do
                    if !cont then
                        if isPrimitive p.PropertyType then
                            cont := false
                            props := p::!props
                            propertyType := getPropertyType p.PropertyType
                        else
                            props := p::!props

                let name = String.Join(".", !props |> List.rev |> List.map (fun p -> p.Name))
                (name, !propertyType)::acc
                    
            else
                let propertyType = getPropertyType p.PropertyType
                if p.DeclaringType = typ then
                    (p.Name, propertyType)::acc
                else
                    acc
        
        | ShapeVar v -> acc
        | ShapeLambda (var, expr) ->
            loop expr acc
        | ShapeCombination(ob, list) ->
            let result = [for l in list do yield! loop l []]
            result@acc |> Seq.distinct |> Seq.toList

    let result = loop expression []
    result

let private getPropertyName (prop:Expr) =
    let rec loop expr acc =
        match expr with
        | Patterns.PropertyGet(e, p, xs) ->
            let xs' = [for x in xs do yield! loop x []]
            if e.IsSome then
                let e' = loop e.Value []
                p.Name::e'@xs'
            else
                p.Name::xs'
        | Patterns.Lambda(v, exp) ->
            loop exp acc
                

        | _ -> acc
            

    String.Join(".", (loop prop []) |> List.rev)
                                                                                

let getValidators (typ:Type) =
    validators |> Seq.filter (fun x -> x.typ = typ)

let getAllValidators () = validators


let registerValidator<'a,'b> (property:Expr<'a -> 'b>) (expr:Expr<'a -> string option>) =
    let properties = (getProperties expr) |> Seq.distinct |> Seq.toList
    let propertyName = getPropertyName property
    let validator = new Validator<'a>(propertyName, properties, expr) :> IValidator

    validators.Add(validator)

let registerValidatorWithoutField<'a> (expr:Expr<'a -> string option>) =
    let properties = (getProperties expr) |> Seq.distinct |> Seq.toList
    let propertyName = properties |> List.head |> fst
    let validator = new Validator<'a>(propertyName, properties, expr) :> IValidator

    validators.Add(validator)

let private getUrlInfo<'a, 'b, 'c>(expr:Expr<'a -> ('b  -> 'c)>) (name:string) =
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
            let controllerName = getControllerName(v'.Type.Name)
            let actionname = m.Name

            let url = getUrl controllerName actionname


            let parameters = m.GetParameters()
         
            let arguments = seq { for i in {0..(parameters.Length - 1) } -> (parameters.[i].Name, getArg(args.[i])) }
         

            let valueArgs = [for (a,b) in arguments -> Expr.NewTuple([Expr.Value(a);Expr.Value(b)])]

         
            let array = Expr.NewArray(typeof<Tuple<string,string>>, valueArgs)


         
            Expr.Cast<RemoteValidator>(Expr.NewRecord(typeof<RemoteValidator>, [Expr.Value(url); Expr.Value(name); array]))
            
    | _ -> failwith "Invalid Remote Validator"



let registerRemoteValidator<'a, 'b, 'c when 'b :> System.Web.Mvc.ControllerBase and 'c :> FSharp.Javascript.Mvc.Extensions.ValidationResult> (property:Expr) (expr:Expr<'a -> ('b  -> 'c)>) =
    

    let properties = (getProperties expr) |> Seq.distinct |> Seq.toList
    let propertyName = getPropertyName property

    let urlInfo = getUrlInfo expr propertyName

    let expr = <@ fun (x:'a) -> getRemoteValidationResult x %urlInfo @>

    let validator = new Validator<'a>(propertyName, properties, expr) :> IValidator

    validators.Add(validator)

let registerRemoteValidatorWithoutField<'a, 'b, 'c when 'b :> System.Web.Mvc.ControllerBase and 'c :> FSharp.Javascript.Mvc.Extensions.ValidationResult> (expr:Expr<'a -> ('b  -> 'c)>)  =
    

    let properties = (getProperties expr) |> Seq.distinct |> Seq.toList
    let propertyName = properties |> List.head |> fst

    let urlInfo = getUrlInfo expr propertyName

    let expr = <@ fun (x:'a) -> getRemoteValidationResult x %urlInfo @>

    let validator = new Validator<'a>(propertyName, properties, expr) :> IValidator

    validators.Add(validator)

let clearValidators () = validators.Clear()