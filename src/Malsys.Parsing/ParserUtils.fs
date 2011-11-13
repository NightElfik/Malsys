module Malsys.Parsing.ParserUtils

open Lexer
open Malsys.Ast
open Malsys.Compilers
open Microsoft.FSharp.Text.Lexing

let setInitialBuffPos (lexbuf : LexBuffer<_>) sourceName =
    lexbuf.EndPos <- { pos_bol = 0;
        pos_fname = sourceName;
        pos_cnum = 0;
        pos_lnum = 1 }
    lexbuf

let parseHelper (parseFun : (LexBuffer<_> -> Parser.token) -> LexBuffer<_> -> 'b) (comments : ResizeArray<Comment>) (lexbuf : LexBuffer<_>) (msgs : MessagesCollection) sourceName =
    msgs.DefaultSourceName <- sourceName;
    MessagesLogger.ThreadStatic.ErrorLogger <- msgs;
    parseFun (Lexer.tokenize (msgs, comments)) (setInitialBuffPos lexbuf sourceName)

// Can not write:
//
// let parseLsystemStatements = parseHelper Parser.ParseLsystemStatements
//
// because of error:
// Value restriction. The value 'parseLsystemStatements' has been inferred to have generic type
// val parseLsystemStatements : ('_a -> LexBuffer<char> -> '_b -> string -> InputBlock) when '_a :> ResizeArray<Comment> and '_b :> MessagesCollection
// Either make the arguments to 'parseLsystemStatements' explicit or, if you do not intend for it to be generic, add a type annotation.


let ParseLsystemStatements comments lexbuf msgs sourceName =
    parseHelper Parser.ParseInput comments lexbuf msgs sourceName


let ParseBinding lexbuf msgs sourceName =
    let mutable comments = new ResizeArray<Comment>() in
    parseHelper Parser.ParseBinding comments lexbuf msgs sourceName