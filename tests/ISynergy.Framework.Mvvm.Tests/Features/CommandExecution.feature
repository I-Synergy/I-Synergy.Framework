Feature: Command Execution
  As a developer using the MVVM framework
    I want to execute commands in ViewModels
    So that user actions trigger appropriate business logic

Background:
    Given I have a test ViewModel

Scenario: Executing a synchronous RelayCommand
    Given the ViewModel has a RelayCommand
    When I execute the command
    Then the command handler should be invoked
    And the command execution count should be 1

Scenario: AsyncRelayCommand execution
    Given the ViewModel has an AsyncRelayCommand
    When I execute the async command
    Then the async command handler should be invoked
    And the command should complete successfully

Scenario: Command CanExecute validation prevents execution
    Given the ViewModel has a command with CanExecute logic
    And the CanExecute condition returns false
    When I attempt to execute the command
    Then the command should not execute
    And the execution count should remain 0

Scenario: Command with parameter execution
 Given the ViewModel has a RelayCommand that accepts a parameter
 When I execute the command with parameter "TestValue"
    Then the command handler should receive the parameter
    And the parameter value should be "TestValue"

@ignore
Scenario: AsyncRelayCommand with cancellation
  Given the ViewModel has a cancellable AsyncRelayCommand
When I start executing the async command
    And I cancel the command execution
    Then the command should be cancelled
    And the cancellation token should be triggered

Scenario: Command execution updates CanExecute
    Given the ViewModel has a command with dynamic CanExecute
 When a property that affects CanExecute changes
    Then CanExecuteChanged event should be raised
    And the command CanExecute should reflect the new state
