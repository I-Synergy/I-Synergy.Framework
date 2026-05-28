Feature: HTTP Client Extensions
    As an API integration developer
    I want to use enhanced HTTP client capabilities
    So that I can monitor and measure API performance

Scenario: Making HTTP request with timing
    Given I have an HTTP client configured
    When I make a GET request to an endpoint
    Then the response should include timing information
    And the timing should be greater than zero

Scenario: Handling successful HTTP responses
    Given I have an HTTP client configured
    When I make a request to a successful endpoint
    Then the response status should be successful
    And the response content should be retrievable

Scenario: Handling failed HTTP responses
    Given I have an HTTP client configured
    When I make a request to a non-existent endpoint
    Then the response status should indicate failure
And appropriate error information should be available

Scenario: Measuring API response times
    Given I have an HTTP client with performance monitoring
    When I make multiple requests to an endpoint
    Then each response should have timing metrics
    And I can compare performance across requests
