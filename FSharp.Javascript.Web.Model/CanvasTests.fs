module FSharp.Javascript.Web.CanvasTests

open FSharp.Javascript.Dom
open FSharp.Javascript.Jquery
open FSharp.Javascript.Library
open FSharp.Javascript.Canvas

type Circle = {
    x : float
    y : float
    radius : float
    transparency : float
}

[<ReflectedDefinition>]
let main() = 
    jquery(document).ready(fun () ->
        let canvas = getCanvasById "canvas"
        let jCanvas = jquery(canvas)
        let ctx = canvas.getContext "2d"
        let width = float 320
        let height = float 500
        canvas.width <- width
        canvas.height <- height

        let clear () =
            ctx.fillStyle <- "#d0e7f9"
            ctx.clearRect(0.0, 0.0, width, height)
            ctx.beginPath()
            ctx.rect(0.0, 0.0, width, height)
            ctx.closePath()
            ctx.fill()

        let getNewCircle () =
            { x = Math.random() * width; y = Math.random() * height; radius = Math.random() * 100.0; transparency = Math.random() / 2.0 }

        let drawCircles (circles:Circle list) =
            circles 
            |> List.iter (fun circle ->
                ctx.fillStyle <- "rgba(255,255,255, " + circle.transparency.ToString() + ")"
                ctx.beginPath()
                ctx.arc(circle.x, circle.y, circle.radius, 0.0, Math.PI * 2.0, true)
                ctx.closePath()
                ctx.fill()
            )

        let getMovedCircles deltaY (circles:Circle list) =
            circles 
            |> List.map (fun circle ->
                if (circle.y - circle.x) > height then
                    let newCircle = getNewCircle ()
                    { newCircle with y = 0.0 - newCircle.radius }
                else
                    { circle with y = circle.y + deltaY }
            )
        
        let rec gameLoop circles =
            clear()
            let newCircles = getMovedCircles 5.0 circles
            drawCircles newCircles
            let timeOutFunc () = gameLoop newCircles |> ignore
            window.setTimeout(timeOutFunc, 1000.0 / 50.0) |> ignore

        let startingCircles = 
            List.init 10 (fun i -> 
                getNewCircle ()
            )

        gameLoop startingCircles
        ()

    )