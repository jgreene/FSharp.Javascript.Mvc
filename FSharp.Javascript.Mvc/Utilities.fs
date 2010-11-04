module FSharp.Javascript.Mvc.Utilities


open System
open System.IO
open System.Web
open System.Web.Routing
open System.Web.Mvc

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.ExprShape

open FSharp.Javascript.Mvc.Fakes


let getRouteFromExpression expr =
    let getControllerName (name:string) =
        if name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) then
            name.Remove(name.Length - 10, 10)
        else
            name

    match expr with
    | Patterns.Lambda(v, Patterns.Call(Some(controller), m, args)) ->
        let dict = new RouteValueDictionary()
        dict.Add("controller", getControllerName(v.Type.Name))
        dict.Add("action", m.Name)

        dict
            
    | _ -> failwith "Invalid Route"

let getFakeUrlHelper () =
    let context = new FakeHttpContext("~/")
    let routes = System.Web.Routing.RouteTable.Routes
    let routeData = routes |> Seq.pick (fun x -> 
                                        let rd = x.GetRouteData(context)
                                        if rd = null then None else Some(rd))

    let requestContext = new Web.Routing.RequestContext(context, routeData)

    new UrlHelper(requestContext)

type CapturingResponseFilter(sink:Stream) =
    inherit Stream()

    let mem = new MemoryStream()
    let mutable position = 0L

    override this.CanRead with get() = true
    override this.CanSeek with get() = false
    override this.CanWrite with get() = false
    override this.Length with get() = 0L
    override this.Position with get() = position and set(value) = position <- value
    override this.Seek(offset, direction) = 0L
    override this.SetLength(length) = sink.SetLength(length)
    override this.Close() =
        sink.Close()
        mem.Close()

    override this.Flush() = sink.Flush()
    override this.Read(buffer, offset, count) =
        sink.Read(buffer, offset, count)

    override this.Write(buffer, offset, count) =
        mem.Write(buffer, 0, count)

    member this.GetContents(enc:System.Text.Encoding) =
        let buffer = Array.empty<byte>
        mem.Position <- 0L
        mem.Read(buffer, 0, buffer.Length) |> ignore
        enc.GetString(buffer, 0, buffer.Length)

let capture (context:HttpContextBase) (renderer:System.Action) =
    let resp = context.Response
    if renderer = null then
        ""
    else
        resp.Flush()
        let originalFilter = resp.Filter
        let innerFilter = new CapturingResponseFilter(resp.Filter)
        resp.Filter <- innerFilter
        renderer.Invoke()
        resp.Flush()
        let result = innerFilter.GetContents(resp.ContentEncoding)
        
        if originalFilter <> null then
            resp.Filter <- originalFilter

        result

type FakeIView() =
    interface IView with
        override this.Render(viewContext, writer) = ()

type FakeIViewDataContainer() =
    let mutable viewData = new ViewDataDictionary()
    interface IViewDataContainer with
        override this.ViewData with get() = viewData and set(value) = viewData <- value

let renderRouteToString (context:HttpContextBase) (routeData:RouteData) =
    
    let controllerName = routeData.GetRequiredString("controller")
    let actionName = routeData.GetRequiredString("action")

    let requestContext = new RequestContext(context, routeData)

    let factory = ControllerBuilder.Current.GetControllerFactory()
    let controller = factory.CreateController(requestContext, controllerName) :?> ControllerBase

    let ActionInvoker = new ControllerActionInvoker()
    let controllerContext = new ControllerContext(requestContext, controller)
    (controller).ControllerContext <- controllerContext

    let viewData = new ViewDataDictionary()
    let tempData = new TempDataDictionary()
    let sb = new System.Text.StringBuilder()
    let sw = new System.IO.StringWriter(sb)
    let helper = new HtmlHelper(new ViewContext(controllerContext, new FakeIView(), viewData, tempData, sw), FakeIViewDataContainer())


    ActionInvoker.InvokeAction(controllerContext, actionName) |> ignore

    let reader = new StreamReader(context.Response.OutputStream)

    reader.ReadToEnd()

//    let action = new System.Action(fun () -> ActionInvoker.InvokeAction(controllerContext, actionName) |> ignore )
//    capture context action
    
        

let getUrl (controller:string) (action:string) =
    let routeValue = new Web.Routing.RouteValueDictionary()
    routeValue.Add("controller", controller)
    routeValue.Add("action", action)

    

    let urlHelper = getFakeUrlHelper ()
    urlHelper.RouteUrl(routeValue)



