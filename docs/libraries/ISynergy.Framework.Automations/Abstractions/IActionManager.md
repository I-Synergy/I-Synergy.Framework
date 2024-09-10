# IActionManager Interface

The IActionManager interface defines a set of methods for managing scheduled and delayed actions in an automation system. This interface is designed to provide a standardized way to interact with a repository of actions, allowing developers to create, retrieve, update, and delete actions, as well as manage their execution status.

The purpose of this code is to establish a contract for classes that will implement action management functionality. It doesn't contain the actual implementation but rather outlines the methods that must be available in any class that implements this interface.

The interface doesn't take any direct inputs or produce any outputs itself. Instead, it defines methods that will handle various operations on IAction objects, which likely represent individual automated tasks or actions.

The IActionManager interface achieves its purpose by declaring several asynchronous methods, each responsible for a specific operation:

- GetItemAsync: Retrieves a single action by its unique identifier (Guid).
- AddAsync: Adds a new action to the repository.
- RemoveAsync: Removes an action from the repository using its identifier.
- UpdateAsync: Updates an existing action in the repository.
- GetFirstUpcomingTaskAsync: Retrieves the next scheduled action to be executed.
- GetItemsAsync: Retrieves a list of actions, with an option to filter only active (non-executed) ones.
- SetActionExcecutedAsync: Marks an action as executed and records its completion time.
- GetTimePreviousCompletedTaskAsync: Retrieves the completion time of the previously executed task for a specific automation.

These methods allow for comprehensive management of actions, including creating new actions, retrieving existing ones, updating their details, and tracking their execution status. The interface is designed to work asynchronously, which is important for maintaining responsiveness in applications that might be dealing with a large number of actions or time-consuming operations.

The GetItemsAsync method is particularly noteworthy as it allows filtering of actions based on their active status, which can be useful for displaying only pending tasks or for reviewing completed ones.

Overall, this interface provides a structured approach to managing automated actions, enabling developers to create systems that can schedule, track, and execute tasks in a organized and efficient manner.