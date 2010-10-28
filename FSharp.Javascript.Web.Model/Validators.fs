module Validators

open FSharp.Javascript.Mvc.Validation

open Models



let setup() =

    registerValidator <@ fun (x:FirstTestModel) -> if x.IsSelected && x.Name = "" then Some "You must supply a name" else None @>

    registerValidator <@ fun (x:FirstTestModel) -> if x.IsSelected && x.Name.ToLower() <> "bob" then Some "You must be named bob" else None @>

    registerValidator <@ fun (x:FirstTestModel) ->
                        if x.DateOfBirth.IsSome  then
                            if x.DateOfBirth.Value >= System.DateTime.Now.AddYears(-18) then
                                Some "You must be 18 years of age or older"
                            else
                                None
                        else
                            Some "You must enter your date of birth"
                    @>

    registerValidator <@ fun (x:FirstTestModel) ->
                            let result = {0..10} |> Seq.exists (fun num -> num = x.PickANumber)
                            if result then None else Some "must be between 0 and 10"
                @>

    registerRemoteValidator <@ fun (x:FirstTestModel) -> fun (cont:FSharp.Javascript.Web.TestController) -> cont.ValidateEmail(x.Id,x.Email) @>