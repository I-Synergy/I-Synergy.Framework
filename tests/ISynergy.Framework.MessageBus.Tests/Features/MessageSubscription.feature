Feature: Message Subscription
    As a developer using the message bus
    I want to subscribe to topics and receive messages
    So that I can process incoming events

Background:
    Given the message bus is configured

Scenario: Subscribing to a topic and receiving a message
    Given I have subscribed to topic "orders"
    When a message is published to topic "orders"
    Then I should receive the message
    And the received message count should be 1

Scenario: Multiple subscribers receive the same message
    Given subscriber "A" has subscribed to topic "orders"
    And subscriber "B" has subscribed to topic "orders"
 When a message is published to topic "orders"
    Then both subscribers should receive the message

Scenario: Subscriber only receives messages from subscribed topics
    Given I have subscribed to topic "orders"
    When a message is published to topic "notifications"
    Then I should not receive any messages

Scenario: Unsubscribing from a topic stops message delivery
    Given I have subscribed to topic "orders"
    When I unsubscribe from topic "orders"
    And a message is published to topic "orders"
    Then I should not receive any messages

Scenario: Subscribing with null handler throws exception
    When I attempt to subscribe to topic "orders" with a null handler
    Then an ArgumentNullException should be thrown
