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