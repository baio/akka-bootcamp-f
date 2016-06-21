open System
open Akka.FSharp
open Akka.FSharp.Spawn
open Akka.Actor
open WinTail

let printInstructions () =
    Console.WriteLine "Write whatever you want into the console!"
    Console.Write "Some lines will appear as"
    Console.ForegroundColor <- ConsoleColor.Red
    Console.Write " red"
    Console.ResetColor ()
    Console.Write " and others will appear as"
    Console.ForegroundColor <- ConsoleColor.Green
    Console.Write " green! "
    Console.ResetColor ()
    Console.WriteLine ()
    Console.WriteLine ()
    Console.WriteLine "Type 'exit' to quit this application at any time.\n"

[<EntryPoint>]
let main argv = 
    // initialize an actor system
    // YOU NEED TO FILL IN HERE
    let myActorSystem = System.create "MyActorSystem" (Configuration.load ())
           
    // make your first actors using the 'spawn' function
    // YOU NEED TO FILL IN HERE
    let consoleWriterActor = spawn myActorSystem "consoleWriterActor" (actorOf Actors.consoleWriterActor)  
    
    //SupervisionStrategy used by tailCoordinatorActor
    let strategy () = Strategy.OneForOne((fun ex ->
        match ex with
        | :? ArithmeticException  -> Directive.Resume
        | :? NotSupportedException -> Directive.Stop
        | _ -> Directive.Restart), 10, TimeSpan.FromSeconds(30.))

    let tailCoordinatorActor = spawnOpt myActorSystem "tailCoordinatorActor" (actorOf2 Actors.tailCoordinatorActor) [ SpawnOption.SupervisorStrategy(strategy ()) ]
    
    // pass tailCoordinatorActor to fileValidatorActorProps (just adding one extra arg)
    let fileValidatorActor = spawn myActorSystem "validationActor" (actorOf2 (Actors.fileValidatorActor consoleWriterActor))

    let consoleReaderActor = spawn myActorSystem "consoleReaderActor" (actorOf2 (Actors.consoleReaderActor))

    // tell the consoleReader actor to begin
    // YOU NEED TO FILL IN HERE
    consoleReaderActor <! Actors.Start

    
    myActorSystem.WhenTerminated.Wait()
        
    0
    

