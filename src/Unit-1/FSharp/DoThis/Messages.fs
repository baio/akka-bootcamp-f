module Messages
open Akka.Actor

// in Messages.fs
// User provided blank input.
type ErrorType =
| Empty 
| Validation

// Discrimination union for signaling that user input was valid or invalid.
type InputResult =
| InputSuccess of string
| InputError of string * errorType: ErrorType

type TailCommand =
| StartTail of filePath: string * reporterActor: IActorRef  //File to observe, actor to display contents
| StopTail of filePath: string                             

type FileCommand =
| FileWrite of fileName: string
| FileError of fileName: string * reason: string
| InitialRead of fileName: string * text: string