namespace FSharp.Javascript.Web

open System.Web.Mvc

open Models

type TestController() =
    inherit Controller()

    member this.Index() =
        base.View()

    member this.FirstTestModel() =
        let model = { Id = 0; IsSelected = false; Name = ""; DateOfBirth = None; PickANumber = 5; Email = "" }
        base.View(model)

    member this.FirstTestModelSubmit(model:FirstTestModel) =
        if base.ModelState.IsValid = false then
            base.View("FirstTestModel", model)
        else
            base.View("Success")


    member this.ValidateEmail (id:int, email:string) =
        base.Json(Some("Invalid Email"))
