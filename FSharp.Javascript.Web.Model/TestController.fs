﻿namespace FSharp.Javascript.Web

open System.Web.Mvc
open FSharp.Javascript.Mvc.Extensions

open Models

type TestController() =
    inherit Controller()

    member this.Index() =
        base.View()

    member this.FirstTestModel() =
        let model = { Id = 0; IsSelected = false; Name = ""; DateOfBirth = Some System.DateTime.Now; PickANumber = 5; Email = ""; }
        base.View(model)

    member this.FirstTestModelSubmit(model:FirstTestModel) =
        let test = match model.DateOfBirth with
                    | Some x -> true
                    | None -> false

        if base.ModelState.IsValid = false then
            base.View("FirstTestModel", model)
        else
            base.View("Success")


    member this.ValidateEmail (id:int, email:string) =
        if email = "test@test.com" then
            base.Validate(None)
        else
            base.Validate(Some "Invalid Email")


type HomeController() =
    inherit Controller()

    member this.Index() = base.View()

        
