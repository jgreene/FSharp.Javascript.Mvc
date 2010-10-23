namespace FSharp.Javascript.Mvc.Tests

open System
open NUnit.Framework
open System.Reflection
open Microsoft.FSharp.Reflection

[<TestFixture>]
type OptionBinderTests() =

    [<Test>]
    member this.``Can dynamically instantiate option``() =
        let getDefault (typ:System.Type) =
            if typ.IsValueType then
                Activator.CreateInstance(typ)
            else
                null

        let print input = System.Console.WriteLine((sprintf "%A" input))

        let optionType = typedefof<option<_>>
        let conversionType = typeof<option<System.DateTime>>
        let innerType = conversionType.GetGenericArguments().[0]
        let genericType = optionType.MakeGenericType(innerType)

        let innerValue = DateTime.MinValue

        let result = 
            try
                Activator.CreateInstance(genericType, innerValue)
            with
            | _ -> Activator.CreateInstance(genericType)

        
        print result
        print (result.GetType().FullName)
            


