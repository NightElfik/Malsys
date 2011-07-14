module Malsys.Parser.ParserUtils

open Lexer
open Parser
open System.IO
open Microsoft.FSharp.Text.Lexing

let SetInitialBuffPos (lexbuf : LexBuffer<char>) filename =  
    lexbuf.EndPos <- { pos_bol = 0; 
        pos_fname = filename;  
        pos_cnum = 0; 
        pos_lnum = 1 }
        
let ParseFromString (str : string) =
    let lexbuff = LexBuffer<char>.FromString str
    SetInitialBuffPos lexbuff ""
    Parser.start Lexer.tokenize lexbuff

let ParseFromTextReader (textReader : TextReader) (filePath : string) =
    let lexbuff = LexBuffer<char>.FromTextReader textReader
    SetInitialBuffPos lexbuff filePath
    Parser.start Lexer.tokenize lexbuff

let ParseFromFile (filePath : string) =
    use reader = File.OpenText filePath
    let lexbuff = LexBuffer<char>.FromTextReader reader
    SetInitialBuffPos lexbuff filePath
    Parser.start Lexer.tokenize lexbuff
