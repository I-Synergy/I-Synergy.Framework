Feature: Automation Triggers
    As a developer using the automation framework
    I want to define triggers that activate automations
    So that workflows execute automatically based on events

Background:
    Given the automation service is initialized
    And I have a valid customer object

@ignore
Scenario: Boolean state trigger activates automation
    Given I have an automation with a boolean state trigger
    And the trigger monitors the customer Active property
    And the trigger expects a change from false to true
    When the customer Active property changes to true
    Then the automation should be triggered
    And the automation should execute successfully

@ignore
Scenario: Integer trigger activates on value change
    Given I have an automation with an integer trigger
    And the trigger monitors the customer Age property
    And the trigger expects the age to change
    When the customer age changes to 21
    Then the automation should be triggered
    And the trigger callback should receive the new value

@ignore
Scenario: String state trigger activates on change
    Given I have an automation with a string state trigger
    And the trigger monitors the customer Name property
    When the customer name changes to a new value
    Then the automation should be triggered
    And the new name should be passed to the callback

@ignore
Scenario: Event trigger activates on custom event
    Given I have an automation with an event trigger
    And the trigger monitors the customer Registered event
    When the customer registration event is raised
    Then the automation should be triggered
    And the event arguments should be passed to the callback

Scenario: Trigger with invalid configuration throws exception
    Given I have a boolean state trigger
    And the from and to states are the same
    When I attempt to create the trigger
    Then an ArgumentException should be thrown
    And the exception should indicate invalid configuration
