namespace FSharp.Javascript.Mvc.Tests

open System
open System.Web.Mvc
open NUnit.Framework
open System.Collections.Specialized

open FSharp.Javascript.Mvc
open Models

open ModelBinderHelper

[<TestFixture>]
type FirstTestModelTests() =
    
    [<Test>]
    member this.``Can bind FirstTestModel DateOfBirth``() =
        let result, 
            modelState = bindModel<FirstTestModel> ["Id", "5";
                                                    "Name", "";
                                                    "IsSelected", "asdf";
                                                    "DateOfBirth", "10/06/2005";
                                                    "PickANumber", "11";
                                                    "Email",""]

        
        Assert.True(result.DateOfBirth.IsSome)
        Assert.IsTrue(modelState.IsValid = false)
        
        



