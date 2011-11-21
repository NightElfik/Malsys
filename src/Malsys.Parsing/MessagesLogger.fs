module Malsys.Parsing.MessagesLogger

open Microsoft.FSharp.Text.Parsing

open Malsys
open Malsys.Parsing


type internal ThreadStatic() =

    [<System.ThreadStatic;DefaultValue>]
    static val mutable private lasErrorPos :  Malsys.Position

    [<System.ThreadStatic;DefaultValue>]
    static val mutable private errorLogger : ParserMessagesLogger

    static member LasErrorPos
        with get() = ThreadStatic.lasErrorPos
        and set(v) = ThreadStatic.lasErrorPos <- v

    static member ErrorLogger
        with get() = ThreadStatic.errorLogger
        and set(v) = ThreadStatic.errorLogger <- v


let logMessage msgType (parseState : IParseState) args =
    ThreadStatic.ErrorLogger.LogMessage(msgType, new Position(parseState.ResultRange), args)

let logMessagePos msgType pos args =
    ThreadStatic.ErrorLogger.LogMessage(msgType, pos, args)

let setErrPos (pos : Malsys.Position) =
    ThreadStatic.LasErrorPos <- pos

let getLastErrPos =
    ThreadStatic.LasErrorPos