
module ConsoleApplication1

open System
open Microsoft.FSharp.Control.WebExtensions
open System.Text.RegularExpressions

type Page = {
    Uri : System.Uri
    Contents : string
    Response : Map<string,string>
    PageLinks : System.Uri list
    RequestStart : DateTime
    RequestEnd : DateTime
}

//memoized function
let getParser (regexes:string list) =
    let regex = regexes |> List.reduce (fun acc item -> acc + "|" + item)
    let compiledRegex = new Regex(regex, RegexOptions.IgnorePatternWhitespace + RegexOptions.IgnoreCase + RegexOptions.Multiline + RegexOptions.Compiled)
    (fun x -> [for m in compiledRegex.Matches(x) do yield m.Value])

let linkParser = getParser ["href=[\'\"]?([^\'\" >]+)"; "src=[\'\"]?([^\'\" >]+)"]

let getLinks contents (baseUri:System.Uri) =
    let baseUrl = baseUri.Scheme + "://" + baseUri.Authority
    let getUri url =
        let innerGet innerUrl =
            try
                Some(new System.Uri(innerUrl))
            with
            | _ -> None

        let result = innerGet url
        if result.IsSome then
            result
        else
            innerGet (baseUrl + url)

    let results = linkParser contents
    results |> List.map (fun x -> x.Replace("href=\"", "").Replace("src=\"", "")) |> List.map(fun x -> getUri x) |> List.filter(fun x -> x.IsSome) |> List.map(fun x-> x.Value)

let rec Download url credentials =
    let rec loop url acc =
        async {
            try
                let uri = new System.Uri(url)
                let client = new System.Net.WebClient()
                client.Credentials <- credentials
                client.Encoding <- System.Text.Encoding.GetEncoding("utf-8")
                let requestStart = DateTime.Now
                let! html = client.AsyncDownloadString(uri)
                let page = { Uri = uri; 
                            Contents = html; 
                            Response = Map.ofSeq  (seq { for key in client.ResponseHeaders.AllKeys do yield (key, client.ResponseHeaders.[key]) });
                            RequestStart = requestStart; 
                            RequestEnd = DateTime.Now;
                            PageLinks = getLinks html uri  }

                let results = [for l in (page.PageLinks |> List.filter (fun x -> (acc |> List.exists (fun y -> y.Uri = x)) = false)) do yield! loop l.AbsoluteUri acc]

                return page::results
            with
            | _ -> return []


        }

    loop url []

let credentials = new System.Net.NetworkCredential("teva-user", "teva-usr")

let url = "http://rctest.sharedsolutions.com/"

let asyncResult = Async.Parallel [(Download url credentials)]

let result = (Async.RunSynchronously asyncResult)
        



//let url = "http://localhost:2996/campaigndetail.aspx/index/2"
//
//let result = Download url
//
//let blah = Async.Parallel [result]
//
//
//
//printfn "%A" (Async.RunSynchronously blah)