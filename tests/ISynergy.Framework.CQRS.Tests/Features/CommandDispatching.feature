# NOTE: This feature file is provided as a template/example.
# Step definitions need to be implemented to make these scenarios executable.
# See SimpleCommandExample.feature for a working example.

Feature: Command Dispatching
    As a developer using the CQRS framework
    I want to dispatch commands through the command dispatcher
    So that commands are properly handled and executed

Background:
    Given the CQRS system is initialized with dependency injection

Scenario: Dispatching a simple command
    Given I have a command with data "Test Data"
    When I dispatch the command
    Then the command should be handled successfully
    And the command handler should have executed

Scenario: Dispatching a command with result
    Given I have a command with result that expects input "Sample Input"
    When I dispatch the command with result
    Then the result should be "Result: Sample Input"

Scenario: Dispatching a command without a registered handler
    Given I have a command without a registered handler
    When I attempt to dispatch the unhandled command
    Then an InvalidOperationException should be thrown
    And the exception message should indicate missing handler

Scenario: Dispatching multiple commands in sequence
 Given I have multiple commands to dispatch
    When I dispatch all commands in sequence
    Then all commands should be handled successfully
    And the execution order should be preserved
