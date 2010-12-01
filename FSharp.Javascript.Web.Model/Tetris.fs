module FSharp.Javascript.Web.Tetris

open FSharp.Javascript.Dom
open FSharp.Javascript.Jquery
open FSharp.Javascript.Library
open FSharp.Javascript.Canvas

type block = {
    x : float
    y : float
    color : string
}

type piece  = 
    | I
    | J
    | L
    | O
    | S
    | T
    | Z

type pieceRotation = 
    | Top
    | Right
    | Bottom
    | Left

type pieceState = {
    x : float
    y : float
    piece : piece
    rotation : pieceRotation
}

type gameStatus =
    | Playing
    | Paused
    | GameOver

type gameState = {
    blocks : block list
    piece : pieceState option
    level : float
    status : gameStatus
    score : float
    lines : float
}

[<ReflectedDefinition>]
let getColor = function
    | I -> "cyan"
    | J -> "blue"
    | L -> "orange"
    | O -> "yellow"
    | S -> "green"
    | T -> "purple"
    | Z -> "red"

[<ReflectedDefinition>]
let getRandomPiece () = 
    let num = Math.floor(Math.random() * (7.0))
    match num with
    | 0.0 -> I
    | 1.0 -> J
    | 2.0 -> L
    | 3.0 -> O
    | 4.0 -> S
    | 5.0 -> T
    | 6.0 -> Z
    | _ -> failwith "This should never happen...haha"

[<ReflectedDefinition>]
let getRandomRotation () =
    let num = Math.floor(Math.random() * (4.0))
    match num with
    | 0.0 -> Top
    | 1.0 -> Right
    | 2.0 -> Bottom
    | 3.0 -> Left
    | _ -> alert(num); failwith "random fail"

[<ReflectedDefinition>]
let getNextRotation rotation =
    match rotation with
    | Top -> Right
    | Right -> Bottom
    | Bottom -> Left
    | Left -> Top

[<ReflectedDefinition>]
let log message =
    let elem = jquery("#log")
    let html = elem.html()
    let result = html + "<br/>" + message.ToString()
    elem.html(result) |> ignore

[<ReflectedDefinition>]
let main() = 
    jquery(document).ready(fun () ->
        
            
        let canvas = getCanvasById "canvas"
        let ctx = canvas.getContext "2d"
        let block_size = 24.0
        let columns = 14.0
        let rows = 19.0
        let width = block_size * columns
        let height = block_size * rows
        canvas.width <- width
        canvas.height <- height

        let rowSequence = [
            for r in 0.0..(rows - 1.0) -> 
                [for c in 0.0..(columns - 1.0) -> { x = c * block_size; y = r * block_size; color = "black" }]
        ]

        let clear() =
            ctx.fillStyle <- "black"
            ctx.clearRect(0.0, 0.0, width, height)
            ctx.beginPath()
            ctx.rect(0.0, 0.0, width, height)
            ctx.closePath()
            ctx.fill()

        let getImage (color:string) =
            jquery("#images").find("." + color).get(0) :?> HtmlImageElement

        let drawBlock (block:block) =
            
            ctx.fillStyle <- block.color
            ctx.beginPath()
            ctx.drawImage(getImage block.color, block.x, block.y)
            ctx.closePath()
            ctx.fill()

        let getBlock x y color =
            { x = x; y = y; color = color }

        let getBlockLeft (block:block) =
            { block with x = block.x - block_size; }

        let getBlockRight (block:block) =
            { block with x = block.x + block_size; }

        let getBlockTop (block:block) =
            { block with y = block.y - block_size; }

        let getBlockBottom (block:block) =
            { block with y = block.y + block_size; }

        let getPieceFromState (pieceState:pieceState) =
            match (pieceState.piece, pieceState.rotation) with
            | (I, Top) | (I, Bottom) -> 
                let color = "cyan"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockTop initialBlock
                let b2 = getBlockTop b1
                let b3 = getBlockBottom initialBlock
                [initialBlock; b1; b2; b3]
            | (I, Right) | (I, Left) ->
                let color = "cyan"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockRight initialBlock
                let b2 = getBlockLeft initialBlock
                let b3 = getBlockRight b1
                [initialBlock; b1; b2; b3]
            | (J, Top) ->
                let color = "blue"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockRight initialBlock
                let b2 = getBlockLeft initialBlock
                let b3 = getBlockTop b2
                [initialBlock; b1; b2; b3]
            | (J, Right) ->
                let color = "blue"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockBottom initialBlock
                let b2 = getBlockTop initialBlock
                let b3 = getBlockRight b2
                [initialBlock; b1; b2; b3]
            | (J, Bottom) ->
                let color = "blue"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockLeft initialBlock
                let b2 = getBlockRight initialBlock
                let b3 = getBlockBottom b2
                [initialBlock; b1; b2; b3]
            | (J, Left) ->
                let color = "blue"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockTop initialBlock
                let b2 = getBlockBottom initialBlock
                let b3 = getBlockLeft b2
                [initialBlock; b1; b2; b3]
            | (L, Top) ->
                let color = "orange"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockLeft initialBlock
                let b2 = getBlockRight initialBlock
                let b3 = getBlockTop b2
                [initialBlock; b1; b2; b3]
            | (L, Right) ->
                let color = "orange"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockTop initialBlock
                let b2 = getBlockBottom initialBlock
                let b3 = getBlockRight b2
                [initialBlock; b1; b2; b3]
            | (L, Bottom) ->
                let color = "orange"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockRight initialBlock
                let b2 = getBlockLeft initialBlock
                let b3 = getBlockBottom b2
                [initialBlock; b1; b2; b3]
            | (L, Left) ->
                let color = "orange"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockBottom initialBlock
                let b2 = getBlockTop initialBlock
                let b3 = getBlockLeft b2
                [initialBlock; b1; b2; b3]
            | (O, Top) | (O, Right) | (O, Bottom) | (O, Left) ->
                let color = "yellow"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockBottom initialBlock
                let b2 = getBlockRight initialBlock
                let b3 = getBlockBottom b2
                [initialBlock; b1; b2; b3]
            | (S, Top) | (S, Bottom) ->
                let color = "green"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockLeft initialBlock
                let b2 = getBlockTop initialBlock
                let b3 = getBlockRight b2
                [initialBlock; b1; b2; b3]
            | (S, Right) | (S, Left) ->
                let color = "green"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockTop initialBlock
                let b2 = getBlockRight initialBlock
                let b3 = getBlockBottom b2
                [initialBlock; b1; b2; b3]
            | (T, Top) ->
                let color = "purple"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockLeft initialBlock
                let b2 = getBlockRight initialBlock
                let b3 = getBlockTop initialBlock
                [initialBlock; b1; b2; b3]
            | (T, Right) ->
                let color = "purple"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockBottom initialBlock
                let b2 = getBlockRight initialBlock
                let b3 = getBlockTop initialBlock
                [initialBlock; b1; b2; b3]
            | (T, Bottom) ->
                let color = "purple"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockLeft initialBlock
                let b2 = getBlockRight initialBlock
                let b3 = getBlockBottom initialBlock
                [initialBlock; b1; b2; b3]
            | (T, Left) ->
                let color = "purple"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockLeft initialBlock
                let b2 = getBlockBottom initialBlock
                let b3 = getBlockTop initialBlock
                [initialBlock; b1; b2; b3]
            | (Z, Top) | (Z, Bottom) ->
                let color = "red"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockLeft initialBlock
                let b2 = getBlockBottom initialBlock
                let b3 = getBlockRight b2
                [initialBlock; b1; b2; b3]
            | (Z, Right) | (Z, Left) ->
                let color = "red"
                let initialBlock = getBlock pieceState.x pieceState.y color
                let b1 = getBlockTop initialBlock
                let b2 = getBlockLeft initialBlock
                let b3 = getBlockBottom b2
                [initialBlock; b1; b2; b3]

        let getNewPieceState () =
            { x = (width / 2.0); y = 0.0; piece = getRandomPiece (); rotation = getRandomRotation () }

        let drawPiece (pieceState:pieceState) =
            let pieces = getPieceFromState pieceState
            pieces |> List.iter drawBlock

        let intersectsBlocks (currentBlocks:block list) (pieces:block list) =
            currentBlocks
            |> List.exists (fun block -> 
                pieces 
                |> List.exists(fun pieceBlock ->
                    block.x = pieceBlock.x && block.y = pieceBlock.y
                )
            )

        let intersectsRightWall (pieces:block list) = 
            pieces |> List.exists (fun block -> (block.x + block_size) > width)

        let intersectsLeftWall (pieces:block list) = 
            pieces |> List.exists (fun block -> block.x < 0.0)

        let intersectsFloor (pieces:block list) =
            pieces
            |> List.exists (fun block ->
                (block.y + block_size) > height
            )

        let getLoweredBlocks (removedRows:(float * block list) list) (pieces:block list) =
            if removedRows.Length = 0 then
                pieces
            else
                let ys = removedRows |> List.map (fun (y, r) -> y)
                pieces |> List.map (fun block ->
                    let innerYs = ys |> List.filter (fun y -> y > block.y) in
                    if innerYs.Length = 0 then
                        block
                    else
                        let moveDownYs = (float innerYs.Length) * block_size in
                        { block with y = block.y + moveDownYs }
                )
                        

        let getRemovedRows (pieces: block list) =
            let removedRows = [
                for r in rowSequence do
                    let yPosition = ref 0.0
                    let result = r 
                                |> List.forall (fun block -> 
                                    yPosition := block.y
                                    pieces 
                                    |> List.exists (fun existingBlock -> 
                                        block.x = existingBlock.x && block.y = existingBlock.y
                                    )
                                )
                    if result then yield (!yPosition, r)
            ]

            let result = pieces 
                            |> List.filter (fun block -> 
                                let temp = seq { for (y,r) in removedRows -> r |> List.exists (fun rowBlock -> block.x = rowBlock.x && block.y = rowBlock.y) }
                                (temp |> Seq.forall (fun x -> x = false))
                            )

            (removedRows, getLoweredBlocks removedRows result)

        let countTetris (removedRows:(float * block list) list) =
            let lastY = ref 0.0
            let rows = ref 0.0
            let tetris = ref 0.0
            [for (y,r) in removedRows do
                if !lastY + block_size = y then do
                    rows := !rows + 1.0
                    if !rows = 3.0 then do
                        tetris := !tetris + 1.0
                        rows := 0.0

                lastY := y] |> ignore

            let rowCount = (float removedRows.Length) - (!tetris * 4.0)
            (rowCount, !tetris)

        let calculateLevel (lines:float) =
            Math.floor(lines / 10.0) + 1.0
                


        let moveDown (state:gameState) =

            let blocksAreAtRoof (pieces:block list) =
                pieces |> List.exists (fun block ->  block.y = 0.0) 

            if state.piece.IsSome then
                let pieceState = state.piece.Value
                let newPieceState = { pieceState with y = pieceState.y + block_size; }

                let newPieces = getPieceFromState newPieceState
                let intersects = intersectsBlocks state.blocks newPieces
                //landed on the bottom or landed on blocks
                if intersects || intersectsFloor newPieces then
                    let pieces = (getPieceFromState pieceState)
                    //landed on blocks and is to high - game over
                    if intersects && blocksAreAtRoof pieces then
                        { state with piece = None; status = GameOver; blocks = pieces@state.blocks; }
                    else
                        //remove full lines and calculate level/score
                        let blocks = (getPieceFromState pieceState)@state.blocks
                        let rowsRemoved, newBlocks = getRemovedRows blocks
                        let rowCount, tetris = countTetris rowsRemoved
                        let lines = state.lines + rowCount + (tetris * 4.0)
                        let score = (rowCount * 100.0) + (tetris * 1000.0)
                        let level = calculateLevel lines
                        { state with piece = None; blocks = newBlocks; score = state.score + score; level = level; lines = lines }
                else
                    { state with piece = Some newPieceState }
            else
                { state with piece = Some (getNewPieceState ()) }

        let moveRight (state:gameState) =
            if state.piece.IsSome then
                let pieceState = state.piece.Value
                let newPieceState = { pieceState with x = pieceState.x + block_size; }
                let newPieces = getPieceFromState newPieceState
                if intersectsRightWall newPieces || intersectsBlocks state.blocks newPieces then
                    state
                else
                    { state with piece = Some newPieceState; }
            else
                state

        let moveLeft (state:gameState) =
           if state.piece.IsSome then
                let pieceState = state.piece.Value
                let newPieceState = { pieceState with x = pieceState.x - block_size; }
                let newPieces = getPieceFromState newPieceState
                if intersectsLeftWall newPieces || intersectsBlocks state.blocks newPieces then
                    state
                else
                    { state with piece = Some newPieceState; }
            else
                state

        let rotate (state:gameState) =
            if state.piece.IsSome then
                let statePiece = state.piece.Value
                let newPieceState = { statePiece with rotation = (getNextRotation statePiece.rotation) }
                let newPieces = getPieceFromState newPieceState
                if intersectsRightWall newPieces || intersectsLeftWall newPieces || intersectsBlocks state.blocks newPieces || intersectsFloor newPieces then
                    state
                else
                    { state with piece = Some newPieceState }
            else
                state

        let drawTetris (state:gameState) =
            clear()
            state.blocks |> List.iter drawBlock

            let scoreText = "Level: " + state.level.ToString() + "<br/>" + "Lines: " + state.lines.ToString() + "<br/>" + "Score: " + state.score.ToString()
            jquery("#score").html(scoreText) |> ignore

            if state.piece.IsSome then do
                drawPiece state.piece.Value
            ()
        
        let unbind() = 
                jquery(document).unbind("keydown") |> ignore

        let rec gameLoop (state:gameState) =
            let newState = ref (moveDown state)
            drawTetris !newState

            
            unbind()
            jquery(document).keydown(fun (e:event) ->
                //log e.keyCode
                
                match e.keyCode with
                | 37.0 -> newState := moveLeft !newState
                | 38.0 -> newState := rotate !newState
                | 39.0 -> newState := moveRight !newState
                | 40.0 -> newState := moveDown !newState
                //space
                | 32.0 -> 
                    newState := [0..19] |> List.fold (fun acc next -> if acc.piece.IsSome then moveDown acc else acc) !newState
                    unbind()
                | _ -> ()

                
                drawTetris !newState
            ) |> ignore

            let timeOutFunc () = gameLoop !newState |> ignore
            
            if (!newState).status = Playing then
                window.setTimeout(timeOutFunc, 800.0 - (50.0 * (!newState).level)) |> ignore
            elif (!newState).status = GameOver then
                unbind()
                log "Game over"


        let initialState = { blocks = []; piece = None; level = 1.0; status = Playing; score = 0.0; lines = 0.0 }
        gameLoop (initialState)
        ()
    )