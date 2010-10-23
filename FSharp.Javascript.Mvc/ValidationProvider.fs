namespace FSharp.Javascript.Mvc

open System.Web.Mvc
open FSharp.Javascript.Mvc.Validation

type FSharpModelValidator(validators:IValidator seq, metadata:ModelMetadata, context:ControllerContext) =
    inherit ModelValidator(metadata, context)

    override this.Validate(container:obj) =        
        let results = seq { for v in validators -> (v.errorField, v.validate metadata.Model) } |> Seq.filter (fun (f,x) -> x.IsSome) |> Seq.map (fun (f,x) -> (f, x.Value))

        seq { for (f,x) in results -> 
                        let result = new ModelValidationResult()
                        result.MemberName <- f
                        result.Message <- x
                        result }


type FSharpValidationProvider() =
    inherit ModelValidatorProvider()

    override this.GetValidators(metadata:ModelMetadata, context:ControllerContext) =
        let validators = getValidators metadata.ModelType
        
        seq { yield new FSharpModelValidator(validators, metadata, context) :> ModelValidator }