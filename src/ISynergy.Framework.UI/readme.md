# I-Synergy Framework UI

## Services

### ExceptionHandlerService
The ExceptionHandlerService is a helper class that manages what happens when errors (exceptions) occur in an application. Think of it as the "error manager" that decides how to respond when something goes wrong.

#### Purpose
The main purpose of this code is to provide a centralized way to handle errors that occur in the application. Instead of having error-handling code scattered throughout the application, this service takes care of all errors in one place, making the code more organized and consistent.

#### Inputs
This service takes in exceptions (errors) that occur in the application. An exception is an object that contains information about what went wrong, such as error messages and where the error happened. The service receives these exceptions through its HandleExceptionAsync method.

#### Outputs
The service doesn't return any values, but it produces several actions:

- It logs the error details for developers to review later
- It shows appropriate error messages to users
- It stops any "busy" indicators that might be showing
- It prevents the same error message from appearing multiple times

#### How It Works
When an error happens in the application, the HandleExceptionAsync method is called with the exception.\
Here's what happens next:
- First, the service checks if it's already handling an error to prevent getting stuck in a loop (where handling one error causes another error).
- It then checks if this is a repeat of the last error it handled. 
- If so, it ignores it to avoid annoying the user with the same message.

#### The service has several checks to ignore certain types of errors that don't need user attention:
- Canceled operations (when a user or the system intentionally stops a task)
- Certain input/output errors
- Some specific Windows-related errors

#### For errors that do need attention, the service:
- Logs the error details for developers
- Stops any "busy" indicators
- Shows an appropriate message to the user based on the type of error:
- For features not yet implemented, it shows a "future module" message
- For permission issues, it shows an "unauthorized access" message
- For file-related errors, it shows specific file messages
- For other errors, it shows a general error message

#### Important Logic Flows
The most important aspect of this code is how it filters different types of errors:
- It uses a flag (_isHandlingException) to prevent recursive error handling, which could cause the application to get stuck in a loop.
- It remembers the last error message shown (_lastErrorMessage) to avoid showing the same error repeatedly.
- It has specific handling for different error types, showing customized messages that make sense to users rather than technical error details.
- It uses a try-finally block to ensure the error handling flag is reset even if something goes wrong during the error handling process itself.

This service makes the application more user-friendly by translating technical errors into understandable messages and preventing error message overload, while still ensuring developers have the detailed information they need for troubleshooting.