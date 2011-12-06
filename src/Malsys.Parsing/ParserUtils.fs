module Malsys.Parsing.ParserUtils

open Microsoft.FSharp.Text.Lexing
open Lexer
open Malsys
open Malsys.Ast
open Malsys.Parsing

let setInitialBuffPos (lexbuf : LexBuffer<_>) sourceName =
    lexbuf.EndPos <- { pos_bol = 0;
        pos_fname = sourceName;
        pos_cnum = 0;
        pos_lnum = 1 }
    lexbuf

let parseHelper (parseFun : (LexBuffer<_> -> Parser.token) -> LexBuffer<_> -> 'b) (comments : ResizeArray<Comment>) (lexbuf : LexBuffer<_>) (logger : MessageLogger) sourceName =
    logger.DefaultSourceName <- sourceName;
    Logger.ThreadStatic.MessageLogger <- logger;
    parseFun (Lexer.tokenize (logger, comments)) (setInitialBuffPos lexbuf sourceName)

// Can not write:
//
// let parseLsystemStatements = parseHelper Parser.ParseLsystemStatements
//
// because of error:
// Value restriction. The value 'parseLsystemStatements' has been inferred to have generic type
// val parseLsystemStatements : ('_a -> LexBuffer<char> -> '_b -> string -> InputBlock) when '_a :> ResizeArray<Comment> and '_b :> MessagesCollection
// Either make the arguments to 'parseLsystemStatements' explicit or, if you do not intend for it to be generic, add a type annotation.


let ParseInput comments lexbuf msgs sourceName =
    parseHelper Parser.ParseInput comments lexbuf msgs sourceName

let ParseInputNoComents lexbuf msgs sourceName =
    let mutable comments = new ResizeArray<Comment>() in
    parseHelper Parser.ParseInput comments lexbuf msgs sourceName



let ParseExpression lexbuf msgs sourceName =
    let mutable comments = new ResizeArray<Comment>() in
    parseHelper Parser.ParseExpression comments lexbuf msgs sourceName

let ParseSymbols lexbuf msgs sourceName =
    let mutable comments = new ResizeArray<Comment>() in
    parseHelper Parser.ParseSymbols comments lexbuf msgs sourceName
