namespace FSharp.Javascript.Mvc

open System
open System.Web.Mvc
open Microsoft.FSharp.Reflection
open System.ComponentModel
open System.Globalization

type RecordDefaultModelBinder() =
    inherit OptionBinder()

    let isrec = FSharpType.IsRecord

    let createSubPropertyName (prefix:string) (propertyName:string) =
            if prefix = null || prefix = "" then
                propertyName
            elif propertyName = null || propertyName = "" then
                prefix
            else
                prefix + "." + propertyName

    let getDefaultValue (typ:System.Type) =
                if typ.IsValueType then
                    Activator.CreateInstance(typ)
                else
                    null

    let getUserResourceString (cc:ControllerContext) (resourceName:string) =
        let ResourceClassKey = ""
        if String.IsNullOrEmpty(ResourceClassKey) && cc <> null && cc.HttpContext <> null then
            cc.HttpContext.GetGlobalResourceObject(ResourceClassKey, resourceName, CultureInfo.CurrentUICulture) :?> string
        else
            null

    let getValueInvalidResource cc =
        let result = getUserResourceString cc "PropertyValueInvalid"
        if String.IsNullOrEmpty(result) then
            "The value '{0}' is not valid for {1}."
        else
            result

    

    override this.BindModel(cc,bc) =
        
        if isrec bc.ModelType then

            let bc = 
                if String.IsNullOrEmpty(bc.ModelName) = false && (bc.ValueProvider.ContainsPrefix(bc.ModelName)) = false && bc.FallbackToEmptyPrefix then
                    let newBc = new ModelBindingContext()
                    newBc.ModelMetadata <- bc.ModelMetadata
                    newBc.ModelState <- bc.ModelState
                    newBc.PropertyFilter <- bc.PropertyFilter
                    newBc.ValueProvider <- bc.ValueProvider

                    newBc
                else
                    bc


            let fields = (FSharpType.GetRecordFields bc.ModelType) |> Array.map (fun f -> (f, f.PropertyType))
            let fieldTypes = fields |> Array.map (fun (f,t) -> t)

            let fieldValues = fields |> Array.map (fun (f,t) ->
                                                        let fullPropertyKey = createSubPropertyName bc.ModelName f.Name
                                                        let defaultValue = getDefaultValue f.PropertyType
                                                        if (bc.ValueProvider.ContainsPrefix fullPropertyKey) = false then
                                                            defaultValue
                                                        else
                                                            let binder = ModelBinders.Binders.GetBinder(t)
                                                            let propertyMetadata = bc.PropertyMetadata.[f.Name]
                                                            propertyMetadata.Model <- defaultValue
                                                            
                                                            let innerBindingContext = new ModelBindingContext()
                                                            innerBindingContext.ModelMetadata <- propertyMetadata
                                                            innerBindingContext.ModelName <- fullPropertyKey
                                                            innerBindingContext.ModelState <- bc.ModelState
                                                            innerBindingContext.ValueProvider <- bc.ValueProvider

                                                            let newPropertyValue = binder.BindModel(cc,innerBindingContext)
                                                            propertyMetadata.Model <- newPropertyValue

                                                            let modelState = bc.ModelState.[fullPropertyKey]
                                                            
                                                            if modelState <> null then
                                                                modelState.Errors
                                                                |> Seq.filter (fun err -> (String.IsNullOrEmpty(err.ErrorMessage)) && err.Exception <> null)
                                                                |> Seq.iter (fun err -> if err.Exception <> null then
                                                                                            match err.Exception with
                                                                                            | :? FormatException ->
                                                                                                let displayName = propertyMetadata.GetDisplayName()
                                                                                                let errorMessageTemplate = getValueInvalidResource cc
                                                                                                let errorMessage = String.Format(CultureInfo.CurrentCulture, errorMessageTemplate, modelState.Value.AttemptedValue, displayName)
                                                                                                modelState.Errors.Remove(err) |> ignore
                                                                                                modelState.Errors.Add(errorMessage) |> ignore
                                                                                            | _ -> ()
                                                                                ) |> ignore

                                                            propertyMetadata.Model
                                                                                             
                                                    )

            let constr = bc.ModelType.GetConstructor(fieldTypes)
            let result = constr.Invoke(fieldValues)
            

            let bindAttr = base.GetTypeDescriptor(cc,bc).GetAttributes().[typeof<BindAttribute>] :?> BindAttribute
            let propertyFilter =
                if bindAttr <> null then
                    new System.Predicate<string>(fun propertyName -> bindAttr.IsPropertyAllowed(propertyName) && bc.PropertyFilter.Invoke(propertyName))
                else
                    bc.PropertyFilter

            let newBc = new ModelBindingContext()
            newBc.ModelMetadata <- ModelMetadataProviders.Current.GetMetadataForType( new Func<obj>(fun () -> result), bc.ModelType)
            newBc.ModelName <- bc.ModelName
            newBc.ModelState <- bc.ModelState
            newBc.PropertyFilter <- bc.PropertyFilter
            newBc.ValueProvider <- bc.ValueProvider
                

            let bc = newBc
            this.OnModelUpdated(cc, bc)

            result

        else
            base.BindModel(cc,bc)