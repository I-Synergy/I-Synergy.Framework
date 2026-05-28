Feature: Property Change Notification
    As a developer using the MVVM framework
    I want ViewModels to notify property changes
    So that the UI can update automatically

Background:
 Given I have an ObservableObject

Scenario: Property change raises PropertyChanged event
    Given the object has a property "Name"
    When I change the property value to "New Name"
    Then PropertyChanged event should be raised
    And the event should specify property "Name"

Scenario: Multiple property changes raise multiple events
    Given the object has properties "FirstName" and "LastName"
    When I change "FirstName" to "John"
    And I change "LastName" to "Doe"
    Then PropertyChanged should be raised 4 times
    And both property names should be notified

Scenario: SetProperty only raises event when value changes
    Given the object has a property with value "Original"
    When I set the property to "Original" again
    Then PropertyChanged event should not be raised

Scenario: Computed property updates when dependency changes
    Given the object has a computed property "FullName"
    And "FullName" depends on "FirstName" and "LastName"
    When I change "FirstName" to "Jane"
    Then PropertyChanged should be raised for "FirstName"
    And PropertyChanged should be raised for "FullName"

@ignore
Scenario: Property validation on change
    Given the object has a validated property
    When I set the property to an invalid value
    Then PropertyChanged event should be raised
 And validation errors should be set
