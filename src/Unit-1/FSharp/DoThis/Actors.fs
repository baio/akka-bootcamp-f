namespace WinTail

open System
open Akka.Actor
open Akka.FSharp

module Actors =
    
    type Command = 
    | Start
    | Continue
    | Message of string
    | Exit

    let consoleReaderActor (consoleWriter: IActorRef) (mailbox: Actor<_>) message = 
        let line = Console.ReadLine ()
        match line.ToLower() with
        | "exit" -> mailbox.Context.System.Terminate() |> ignore
        | input -> 
            // send input to the console writer to process and print
            // YOU NEED TO FILL IN HERE
            consoleWriter <! Message input

            // continue reading messages from the console
            // YOU NEED TO FILL IN HERE
            mailbox.Self <! Continue

    let consoleWriterActor (message : Command) = 
        let (|Even|Odd|) n = if n % 2 = 0 then Even else Odd
    
        let printInColor color message =
            Console.ForegroundColor <- color
            Console.WriteLine (message.ToString ())
            Console.ResetColor ()
        
        match message with
        | Message input ->
            match input.Length with 
                | 0    -> printInColor ConsoleColor.DarkYellow "Please provide an input.\n"
                | Even -> printInColor ConsoleColor.Red "Your string had an even # of characters.\n"
                | Odd  -> printInColor ConsoleColor.Green "Your string had an odd # of characters.\n"
        | _ ->
            raise (System.Exception("Unexpected code"))            
        