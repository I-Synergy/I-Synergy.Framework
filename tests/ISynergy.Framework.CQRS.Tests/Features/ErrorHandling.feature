# NOTE: This feature file is provided as a template/example.
# Step definitions need to be implemented to make these scenarios executable.
# See SimpleCommandExample.feature for a working example.

Feature: CQRS Error Handling
    As a developer using the CQRS framework
    I want proper error handling for commands and queries
    So that failures are managed gracefully and diagnostically

Scenario: Command handler throws exception
    Given I have a command that will cause an exception
 When I dispatch the failing command
    Then the exception should propagate to the caller
    And the exception details should be preserved

Scenario: Query handler returns null result
    Given I have a query that returns null
    When I dispatch the null-returning query
    Then the result should be null
    And no exception should be thrown

Scenario: Command validation failure
 Given I have an invalid command
    When I attempt to dispatch the invalid command
    Then a validation exception should be thrown
    And the validation errors should be accessible

Scenario: Logging integration for command failures
    Given logging is configured for the CQRS system
    And I have a command that will fail
    When I dispatch the command and it fails
    Then an error should be logged
    And the log should contain command details
    And the log should contain the exception information
