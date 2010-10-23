namespace FSharp.Javascript.Mvc

open System
open System.Web.Mvc
open Microsoft.FSharp.Reflection
open System.ComponentModel


type OptionConverter(typ:System.Type) =
    inherit TypeConverter()
    




type OptionBinder() =
    inherit DefaultModelBinder()

        override this.BindModel(cc:ControllerContext, bc:ModelBindingContext) =
            let getDefault (typ:System.Type) =
                if typ.IsValueType then
                    Activator.CreateInstance(typ)
                else
                    null

            if bc.ModelType.IsGenericType && bc.ModelType.GetGenericTypeDefinition().Equals(typedefof<option<_>>) then
                let optionType = typedefof<option<_>>
                let conversionType = bc.ModelType
                let innerType = conversionType.GetGenericArguments().[0]
                let genericType = optionType.MakeGenericType(innerType)
                let result = 
                    try
                        let defaultValue = getDefault innerType
                        let propertyMetadata = new ModelMetadata(ModelMetadataProviders.Current, conversionType, new Func<obj>(fun () -> defaultValue), innerType, bc.ModelName)
                        
                        let newBc = new ModelBindingContext()
                        newBc.ModelMetadata <- propertyMetadata
                        newBc.ModelName <- bc.ModelName
                        newBc.ModelState <- bc.ModelState
                        newBc.PropertyFilter <- bc.PropertyFilter
                        newBc.ValueProvider <- bc.ValueProvider
                        let innerValue = this.BindModel(cc, newBc)
                        if innerValue = null || innerValue.Equals((getDefault innerType)) then
                            getDefault genericType
                        else
                            Activator.CreateInstance(genericType, innerValue)
                    with
                    | ex -> getDefault genericType

                result
            else
                base.BindModel(cc,bc)


