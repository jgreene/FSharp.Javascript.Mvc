namespace FSharp.Javascript.Web.Model

open System.Web.Mvc

type CanvasController() =
    inherit Controller()

    member this.Index() = base.View("Tetris")

    member this.CanvasTest() = base.View()