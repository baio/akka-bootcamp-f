module Messages

// in Messages.fs
// User provided blank input.
type ErrorType =
| Empty 
| Validation

// Discrimination union for signaling that user input was valid or invalid.
type InputResult =
| InputSuccess of string
| InputError of string * errorType: ErrorType