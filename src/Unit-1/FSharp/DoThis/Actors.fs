namespace WinTail

open System
open Akka.Actor
open Akka.FSharp
open Messages

module Actors =
    
    type Command = 
    | Start
    | Continue
    | Message of string
    | Exit


    // in top of Actors.fs before consoleReaderActor
    let (|EmptyMessage|MessageLengthIsEven|MessageLengthIsOdd|) (msg:string) =
        match msg.Length, msg.Length % 2 with
        | 0,_ -> EmptyMessage
        | _,0 -> MessageLengthIsEven
        | _,_ -> MessageLengthIsOdd

    // in consoleReaderActor,
    let doPrintInstructions () =
        Console.WriteLine "Write whatever you want into the console!"
        Console.WriteLine "Some entries will pass validation, and some won't...\n\n"
        Console.WriteLine "Type 'exit' to quit this application at any time.\n"


    let consoleReaderActor (consoleWriter: IActorRef) (mailbox: Actor<_>) message = 

        // Reads input from console, validates it, then signals appropriate response
        // (continue processing, error, success, etc.).
        let getAndValidateInput () =
            let line = Console.ReadLine()
            match line.ToLower() with
            | "exit" -> mailbox.Context.System.Terminate() |> ignore
            | input ->
                match input with
                | EmptyMessage ->
                    consoleWriter <! InputError ("No input received.", ErrorType.Empty)
                | MessageLengthIsEven ->
                    consoleWriter <! InputSuccess ("Thank you! Message was valid.")                    
                | _ ->
                    consoleWriter <! InputError ("Invalid: input had odd number of characters.", ErrorType.Validation)
                mailbox.Self  <! Continue

        match box message with
          | :? Command as command ->
              match command with
              | Start -> doPrintInstructions ()
              | _ -> ()
          | _ -> raise(Exception("Unexpected code"))
           
        getAndValidateInput()

    let consoleWriterActor message = 
      
        let printInColor color message =
            Console.ForegroundColor <- color
            Console.WriteLine (message.ToString ())
            Console.ResetColor ()
        
        match box message with
        | :? InputResult as inputResult ->
            match inputResult with
            | InputError (reason,_) -> printInColor ConsoleColor.Red reason
            | InputSuccess reason -> printInColor ConsoleColor.Green reason
        | _ -> raise(Exception("Unexpected code"))