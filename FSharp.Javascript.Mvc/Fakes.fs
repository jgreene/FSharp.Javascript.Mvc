namespace FSharp.Javascript.Mvc.Fakes

open System.Web
open System.Web.SessionState
open System.Security.Principal
open System.Collections.Specialized
open System.IO
open System.Text


type FakeHttpContext(relativeUrl:string, 
                        httpMethod:string, 
                        principal:IPrincipal, 
                        form:NameValueCollection, 
                        queryString:NameValueCollection,
                        cookies:HttpCookieCollection,
                        session:SessionStateItemCollection) =
    inherit HttpContextBase()
    
    let httpRequest = new FakeHttpRequest(relativeUrl, httpMethod, form, queryString, cookies)
    let httpResponse = new FakeHttpResponse()
    let httpSession = new FakeHttpSessionState(session)

    new(relativeUrl:string) = FakeHttpContext(relativeUrl, "GET", null, null, null, null, null)
    new(relativeUrl:string, httpMethod:string) = FakeHttpContext(relativeUrl, httpMethod, null, new NameValueCollection(), new NameValueCollection(), null, null)

    override this.Request with get() = httpRequest :> HttpRequestBase

    override this.Response with get() = httpResponse :> HttpResponseBase

    override this.User with get() = principal and set(value) = failwith "Not implemented"

    override this.Session with get() = httpSession :> HttpSessionStateBase

    override this.IsCustomErrorEnabled with get() = false

    

and FakeHttpRequest(relativeUrl:string, httpMethod:string, form:NameValueCollection, queryString:NameValueCollection, cookies:HttpCookieCollection) =
    inherit HttpRequestBase()
    let mutable contentType = ""

    override this.Form with get() = form
    override this.QueryString with get() = queryString
    override this.Cookies with get() = cookies
    override this.AppRelativeCurrentExecutionFilePath with get() = relativeUrl
    override this.PathInfo with get() = ""
    override this.HttpMethod with get() = httpMethod
    override this.ApplicationPath with get() = "/"
    override this.ServerVariables 
        with get() = 
                let coll = new NameValueCollection()
                if form <> null then
                    coll.Add(form)

                if queryString <> null then
                    coll.Add(queryString)

                coll

    override this.ValidateInput() = ()

    override this.RawUrl with get() = relativeUrl

    override this.ContentType with get() = contentType and set(value) = contentType <- value

    override this.Files with get() = new FakeHttpFileCollection() :> HttpFileCollectionBase
    

and FakeHttpResponse() =
    inherit HttpResponseBase()
    
    let mutable filter = new MemoryStream() :> Stream
    let mutable sw = new StreamWriter(filter)
    let mutable contentType = ""

    override this.Write(s:string) = sw.Write(s)

    override this.Output with get() = sw :> TextWriter

    override this.ApplyAppPathModifier(path:string) = path

    override this.Flush() = sw.Flush()

    override this.Filter 
        with get() = filter 
        and set(value) = 
            filter <- value
            sw <- new StreamWriter(filter)

    override this.ContentType with get() = contentType and set(value) = contentType <- value

    override this.ContentEncoding with get() = Encoding.Default

    

and FakePrincipal(identity:IIdentity, roles:string array) =
    interface IPrincipal with
        member this.Identity with get() = identity
        
        member this.IsInRole(role:string) =
            if roles = null then
                false
            else
                roles |> Array.exists (fun x -> x = role) 

and FakeHttpSessionState(sessionItems:SessionStateItemCollection) =
    inherit HttpSessionStateBase()


    override this.Add(name:string, value) =
        sessionItems.[name] <- value

    override this.Count with get() = sessionItems.Count
    
    override this.GetEnumerator() = sessionItems.GetEnumerator()

    override this.Keys = sessionItems.Keys

    override this.Item
        with get(i:string) = sessionItems.[i]
        and set (i:string) (value:obj) = sessionItems.[i] <- value

    override this.Item
        with get(i:int) = sessionItems.[i]
        and set (i:int) (value:obj) = sessionItems.[i] <- value

    override this.Remove(name) = sessionItems.Remove(name)

and FakeHttpFileCollection() =
    inherit HttpFileCollectionBase()

    override this.Count with get() = 0




