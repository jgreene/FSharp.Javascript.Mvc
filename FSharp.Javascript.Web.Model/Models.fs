module Models

open System.Web.Mvc

type FirstTestModel = {
    Id : int;
    IsSelected : bool;
    Name : string;
    DateOfBirth : System.DateTime option;
    PickANumber : int;
    Email : string;
}

type SecondTestModel = {
    FirstName : string
    LastName : string
    Addresses : Address array
}
and Address = {
    Address1 : string
    Address2 : string
    Zip : string
}