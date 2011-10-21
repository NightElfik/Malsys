module Malsys.Parsing.MessagesLogger

open Malsys
open Malsys.Compilers
open Microsoft.FSharp.Text.Parsing


type internal ThreadStatic() =

    [<System.ThreadStatic;DefaultValue>]
    static val mutable private lasErrorPos :  Malsys.Position

    [<System.ThreadStatic;DefaultValue>]
    static val mutable private errorLogger : MessagesCollection

    static member LasErrorPos
        with get() = ThreadStatic.lasErrorPos
        and set(v) = ThreadStatic.lasErrorPos <- v

    static member ErrorLogger
        with get() = ThreadStatic.errorLogger
        and set(v) = ThreadStatic.errorLogger <- v


let logMessage msgType (parseState : IParseState) msg =
    ThreadStatic.ErrorLogger.AddMessage(msg, msgType, new Position(parseState.ResultRange))

let logMessagePos msgType (pos : Malsys.Position) msg =
    ThreadStatic.ErrorLogger.AddMessage(msg, msgType, pos)

let setErrPos (pos : Malsys.Position) =
    ThreadStatic.LasErrorPos <- pos

let getLastErrPos =
    ThreadStatic.LasErrorPos
