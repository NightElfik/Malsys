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

let parseLsystemStatements (lexbuf : LexBuffer<_>) (msgs : MessagesCollection) sourceName =
    let mutable comments = new ResizeArray<Comment>() in
    MessagesLogger.ThreadStatic.ErrorLogger <- msgs;
    Parser.parseLsystemStatements (Lexer.tokenize (msgs, comments)) (setInitialBuffPos lexbuf sourceName)

let parseExpression (lexbuf : LexBuffer<_>) (msgs : MessagesCollection) sourceName =
    let mutable comments = new ResizeArray<Comment>() in
    MessagesLogger.ThreadStatic.ErrorLogger <- msgs;
    Parser.parseExpression (Lexer.tokenize (msgs, comments)) (setInitialBuffPos lexbuf sourceName)

let parseExprInteractiveStatements (lexbuf : LexBuffer<_>) (msgs : MessagesCollection) sourceName =
    let mutable comments = new ResizeArray<Comment>() in
    MessagesLogger.ThreadStatic.ErrorLogger <- msgs;
    Parser.parseExprInteractiveStatements (Lexer.tokenize (msgs, comments)) (setInitialBuffPos lexbuf sourceName)
