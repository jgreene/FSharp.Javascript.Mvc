module Validators

open FSharp.Javascript.Mvc.Validation
open Models



let setup() =

    let nameValidator = registerValidator <@ fun (x:FirstTestModel) -> x.Name @>

    nameValidator <@ fun (x:FirstTestModel) -> if x.IsSelected && x.Name = "" then Some "You must supply a name" else None @>

    nameValidator <@ fun (x:FirstTestModel) -> if x.IsSelected && x.Name.ToLower() <> "bob" then Some "You must be named bob" else None @>

    registerValidatorWithoutField <@ fun (x:FirstTestModel) ->
                                        if x.DateOfBirth.IsSome  then
                                            if x.DateOfBirth.Value >= System.DateTime.Now.AddYears(-18) then
                                                Some "You must be 18 years of age or older"
                                            else
                                                None
                                        else
                                            Some "You must enter your date of birth"
                                    @>

    registerValidatorWithoutField <@ fun (x:FirstTestModel) ->
                                    let result = {0..10} |> Seq.exists (fun num -> num = x.PickANumber)
                                    if result then None else Some "must be between 0 and 10"
                                @>

    registerValidator 
        <@ fun (x:Address) -> x.Address1 @> 
        <@ fun (x:Address) -> if x.Address1 = "" then Some "Address 1 is required." else None @>

    registerRemoteValidator
        <@ fun (x:FirstTestModel) -> x.Email @>
        <@ fun (x:FirstTestModel) -> fun (cont:FSharp.Javascript.Web.TestController) -> cont.ValidateEmail(x.Id, x.Email) @>

    registerValidator 
        <@ fun (x:SecondTestModel) -> x.FirstName @> 
        <@ fun (x:SecondTestModel) -> if x.FirstName = "" then Some "You must provide a first name" else None @>

    registerValidator 
        <@ fun (x:SecondTestModel) -> x.LastName @> 
        <@ fun (x:SecondTestModel) -> if x.LastName = "" then Some "You must provide a last name" else None @>
    
    registerValidator 
        <@ fun (x:SecondTestModel) -> x.LastName @> 
        <@ fun (x:SecondTestModel) -> if x.Addresses = null || x.Addresses.Length = 0 then Some "You must provide at least one address" else None  @>

    let zipValidator = 
        registerValidator 
            <@ fun (x:ThirdTestModel) -> x.Address.Zip @>

    zipValidator
        <@ fun x -> if x.Address.Zip = "" then Some "Zip is required" else None @>

    zipValidator
        <@ fun x -> if x.Address.Zip.Length <> 5 then Some "Zip must be 5 characters" else None @>