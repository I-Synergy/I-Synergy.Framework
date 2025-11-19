Feature: Automation Workflow Execution
    As a developer using the automation framework
    I want to define and execute automated workflows
    So that I can automate complex business processes

Background:
    Given the automation service is initialized
    And I have a valid customer object

Scenario: Execute automation with conditions met
    Given I have an automation with age validation condition
    And the customer age is 18 or older
    When I execute the automation
    Then the automation should succeed
    And all actions should be executed

Scenario: Execute automation with conditions not met
    Given I have an automation with age validation condition
    And the customer age is below 18
    When I execute the automation
    Then the automation should fail
    And no actions should be executed

Scenario: Execute automation with delay action
    Given I have an automation with a delay action of 2 seconds
    When I execute the automation
    Then the automation should succeed
    And the execution time should be approximately 2 seconds

Scenario: Execute automation with command actions
    Given I have an automation with command actions
    And the commands modify customer properties
    When I execute the automation
    Then the automation should succeed
    And the customer properties should be modified

Scenario: Execute automation with repeat actions
    Given I have an automation with repeat actions
    And the repeat is configured to run until a condition
    When I execute the automation
    Then the automation should succeed
    And the action should repeat the specified number of times

Scenario: Execute automation with timeout
    Given I have an automation with a 5 second timeout
    And the automation has actions exceeding the timeout
    When I execute the automation
    Then the automation should be cancelled
    And a timeout exception should be raised
