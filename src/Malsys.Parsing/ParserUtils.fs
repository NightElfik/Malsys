module Malsys.Parsing.ParserUtils

open Lexer
open Microsoft.FSharp.Text.Lexing

let setInitialBuffPos (lexbuf : LexBuffer<_>) sourceName =
    lexbuf.EndPos <- { pos_bol = 0;
        pos_fname = sourceName;
        pos_cnum = 0;
        pos_lnum = 1 }
    lexbuf

let parseLsystemStatements (lexbuf : LexBuffer<_>) sourceName =
    Parser.parseLsystemStatements Lexer.tokenize (setInitialBuffPos lexbuf sourceName)