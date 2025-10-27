# NOTE: This feature file is provided as a template/example.
# Step definitions need to be implemented to make these scenarios executable.
# See SimpleCommandExample.feature for a working example.

Feature: Query Dispatching
 As a developer using the CQRS framework
    I want to dispatch queries through the query dispatcher
    So that data retrieval operations are properly executed

Background:
    Given the CQRS query system is initialized

Scenario: Dispatching a simple query
    Given I have a query with parameter "Test Parameter"
    When I dispatch the query
  Then the query should return "Query Result: Test Parameter"

Scenario: Dispatching a query without a registered handler
 Given I have a query without a registered handler
 When I attempt to dispatch the unhandled query
    Then an InvalidOperationException should be thrown for query

Scenario: Dispatching queries with cancellation
 Given I have a query with cancellation token
    And the cancellation token is cancelled
    When I attempt to dispatch the query with cancellation
    Then the operation should be cancelled
    And a cancellation exception should be raised

Scenario: Query returns expected data type
    Given I have a query that returns string data
    When I dispatch the query
    Then the result should be of type string
    And the result should not be null or empty
