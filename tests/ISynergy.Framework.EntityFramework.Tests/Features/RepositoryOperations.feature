Feature: Repository Operations
    As a developer using Entity Framework repositories
    I want to perform CRUD operations on entities
    So that I can manage data persistence effectively

Background:
    Given the repository is initialized
    And the database context is configured

Scenario: Creating a new entity
    Given I have a new test entity
    When I add the entity to the repository
    Then the entity should be saved successfully
    And the entity should have a database ID assigned

Scenario: Reading an entity by ID
    Given an entity exists in the database
    When I query the entity by its ID
    Then the entity should be retrieved successfully
    And the entity properties should match the saved values

Scenario: Updating an existing entity
    Given an entity exists in the database
    When I modify the entity properties
    And I save the changes
    Then the entity should be updated successfully
    And the modified properties should be persisted

Scenario: Deleting an entity
    Given an entity exists in the database
    When I delete the entity from the repository
    Then the entity should be removed from the database
    And querying for the entity should return null

Scenario: Finding entities by predicate
    Given multiple entities exist in the database
    When I query entities with a filter predicate
    Then only matching entities should be returned
    And the result count should match the filter criteria

Scenario: Checking entity existence
    Given an entity exists in the database
    When I check if the entity exists by ID
    Then the existence check should return true
    And checking for a non-existent ID should return false
