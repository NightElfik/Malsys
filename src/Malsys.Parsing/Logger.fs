module Malsys.Parsing.Logger

open Microsoft.FSharp.Text.Parsing

open Malsys
open Malsys.Parsing


type internal ThreadStatic() =

    [<System.ThreadStatic;DefaultValue>]
    static val mutable private lastErrorPos : Malsys.PositionRange

    [<System.ThreadStatic;DefaultValue>]
    static val mutable private logger : Malsys.IMessageLogger

    static member LastErrorPos
        with get() = ThreadStatic.lastErrorPos
        and set(v) = ThreadStatic.lastErrorPos <- v

    static member MessageLogger
        with get() = ThreadStatic.logger
        and set(v) = ThreadStatic.logger <- v


let logMessagePos msgType (pos : PositionRange) args =
    ThreadStatic.MessageLogger.LogMessage(msgType, pos, args)

let setErrPos (pos : Malsys.PositionRange) =
    ThreadStatic.LastErrorPos <- pos

let getLastErrPos =
    ThreadStatic.LastErrorPos
