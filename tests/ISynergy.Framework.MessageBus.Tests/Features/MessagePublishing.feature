Feature: Message Publishing
    As a developer using the message bus
  I want to publish messages to topics
    So that subscribers can receive and process them

Background:
    Given the message bus is configured
    And I have a test message

Scenario: Publishing a message to a topic
    When I publish the message to topic "orders"
    Then the message should be published successfully
    And the published message count should be 1

Scenario: Publishing multiple messages to the same topic
    When I publish the message to topic "orders"
    And I publish the message to topic "orders"
    And I publish the message to topic "orders"
    Then the published message count should be 3

Scenario: Publishing messages to different topics
    When I publish the message to topic "orders"
    And I publish the message to topic "notifications"
    Then messages should be published to 2 different topics

Scenario: Publishing a null message throws exception
    Given I have a null message
    When I attempt to publish the null message to topic "orders"
    Then an ArgumentNullException should be thrown

Scenario: Publishing to an empty topic name throws exception
    When I attempt to publish the message to topic ""
    Then an ArgumentException should be thrown
