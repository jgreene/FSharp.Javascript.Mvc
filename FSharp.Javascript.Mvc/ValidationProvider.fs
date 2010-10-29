namespace FSharp.Javascript.Mvc

open System.Web.Mvc

type FSharpModelValidator(validators:IValidator seq, metadata:ModelMetadata, context:ControllerContext) =
    inherit ModelValidator(metadata, context)

    let createSubPropertyName (prefix:string) (propertyName:string) =
        if prefix = null || prefix = "" then
            propertyName
        elif propertyName = null || propertyName = "" then
            prefix
        else
            prefix + "." + propertyName

    override this.Validate(container:obj) =        
        let modelState = context.Controller.ViewData.ModelState
        
        let shouldValidate fields =
            fields |> Seq.fold (fun acc (name, typ) -> 
                                    let key = createSubPropertyName metadata.PropertyName name
                                    acc && modelState.IsValidField(name)) true
        
        let results = seq { for v in validators -> (v.errorField, if shouldValidate (v.properties) then v.validate metadata.Model else None) } 
                        |> Seq.filter (fun (f,x) -> x.IsSome) 
                        |> Seq.map (fun (f,x) -> (f, x.Value))

        seq { for (f,x) in results -> 
                        let result = new ModelValidationResult()
                        result.MemberName <- f
                        result.Message <- x
                        result }


type FSharpValidationProvider() =
    inherit ModelValidatorProvider()

    override this.GetValidators(metadata:ModelMetadata, context:ControllerContext) =
        let allValidators = Validation.getValidators metadata.ModelType
        //let validators = allValidators  |> Seq.filter (fun v -> v.errorField = metadata.PropertyName)
        
        seq { yield new FSharpModelValidator(allValidators, metadata, context) :> ModelValidator }