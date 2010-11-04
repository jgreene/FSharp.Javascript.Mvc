module ModelBinderHelper

open System
open System.Web.Mvc
open NUnit.Framework
open System.Collections.Specialized

open FSharp.Javascript.Mvc

let print input = System.Console.WriteLine((sprintf "%A" input))

let getForm (input:seq<(string * string)>) =
        let formCollection = new NameValueCollection()

        input |> Seq.iter (fun (key,value) ->
                            formCollection.Add(key, value)
                        )

        formCollection

let getValueProvider (input:seq<(string * string)>) =
    let form = getForm input
    new NameValueCollectionValueProvider(form, null)

type TestControllerFactory() =
    inherit System.Web.Mvc.DefaultControllerFactory()

    override this.GetControllerInstance(rc, typ:System.Type) =
        let cont = System.Activator.CreateInstance(typ) :?> ControllerBase
        cont.ControllerContext <- new ControllerContext(rc, cont)
        cont :> IController



let bindModel<'a> (input:seq<(string * string)>) =
    System.Web.Mvc.ControllerBuilder.Current.SetControllerFactory(new TestControllerFactory())
    System.Web.Routing.RouteTable.Routes.Clear()
    FSharp.Javascript.Web.MvcApplication.RegisterRoutes(System.Web.Routing.RouteTable.Routes)
    Validation.clearValidators()
    Validators.setup()
    Setup.initialize()

    let typ = typeof<'a>
    let valueProvider = getValueProvider input
    let modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typ)

    let bindingContext = new ModelBindingContext()
    bindingContext.ModelName <- ""
    bindingContext.ValueProvider <- valueProvider
    bindingContext.ModelMetadata <- modelMetadata

    let controllerContext = new ControllerContext()
    controllerContext.Controller <- new FSharp.Javascript.Web.TestController()
    controllerContext.Controller.ViewData <- new ViewDataDictionary()
    controllerContext.Controller.ViewData.ModelState.Merge(bindingContext.ModelState)

    let binder = new RecordDefaultModelBinder()
    let result = binder.BindModel(controllerContext, bindingContext)

    bindingContext.ModelState |> Seq.iter (fun s -> 
                                                    print (sprintf "%A : %A" s.Key (s.Value.Errors |> Seq.map (fun e -> e.ErrorMessage)))
                                            )

    print "--------------------------------------------------------------------------------"

    print result

    ((result :?> 'a), bindingContext.ModelState)