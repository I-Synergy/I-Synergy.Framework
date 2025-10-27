Feature: Result Pattern API Responses
    As an API developer
    I want to use the Result pattern with ASP.NET Core
    So that I can provide consistent and type-safe API responses

Background:
    Given I have an API controller using Result pattern

Scenario: Returning successful result with data
    Given I have a successful result with product data
    When I match the result to an action result
    Then the response should be an OK result
    And the response should contain the product data

Scenario: Returning successful result without data
    Given I have a successful result without data
    When I match the result to an action result
    Then the response should be a NoContent result

Scenario: Returning failed result
    Given I have a failed result
    When I match the result to an action result
    Then the response should be a NotFound result

Scenario: Result pattern with custom error handling
    Given I have a result with error messages
When I match the result with error handling
    Then the response should include error details
    And the response should have appropriate status code

Scenario: Chaining result operations
    Given I have multiple result operations
    When I chain the operations together
    Then all operations should execute in sequence
    And the final result should reflect the chain outcome
