Feature: ViewModel Lifecycle
    As a developer using the MVVM framework
    I want ViewModels to have proper lifecycle management
    So that resources are initialized and disposed correctly

Background:
    Given I have a ViewModel

Scenario: ViewModel initialization
 When the ViewModel is created
    Then the IsInitialized property should be false
    When I call InitializeAsync
    Then the IsInitialized property should be true
    And initialization logic should have executed

Scenario: ViewModel disposal
    Given the ViewModel is initialized
 When I dispose the ViewModel
    Then Dispose should be called
    And resources should be cleaned up
    And the ViewModel should not be usable

Scenario: ViewModel busy state management
    Given the ViewModel has an async operation
    When the operation starts
    Then IsBusy should be true
    When the operation completes
    Then IsBusy should be false

Scenario: ViewModel title and subtitle binding
    Given the ViewModel has Title and Subtitle properties
    When I set Title to "Main View"
    And I set Subtitle to "Dashboard"
    Then PropertyChanged should be raised for "Title"
    And PropertyChanged should be raised for "Subtitle"
