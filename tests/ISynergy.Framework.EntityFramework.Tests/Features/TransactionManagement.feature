Feature: Transaction Management
  As a developer using Entity Framework
    I want to manage database transactions
    So that I can ensure data consistency and handle errors properly

Background:
    Given the database context is configured
    And the repository is initialized

@ignore
Scenario: Committing a successful transaction
    Given I have started a database transaction
    When I add multiple entities within the transaction
    And I commit the transaction
    Then all entities should be saved successfully
    And the transaction should be marked as committed

@ignore
Scenario: Rolling back a failed transaction
    Given I have started a database transaction
    When I add an entity within the transaction
    And an error occurs during save
    And I rollback the transaction
    Then no entities should be saved to the database
    And the transaction should be rolled back

@ignore
Scenario: Nested transactions
    Given I have started a parent transaction
    When I create a nested transaction
    And I save entities in both transactions
    And I commit the nested transaction
    Then the nested changes should be visible to the parent
    And committing the parent should save all changes

@ignore
Scenario: Transaction timeout handling
    Given I have started a transaction with a 1 second timeout
    When the operation exceeds the timeout duration
    Then a transaction timeout exception should be thrown
    And no changes should be persisted

@ignore
Scenario: Concurrent transaction handling
    Given I have two parallel transactions
    When both transactions modify the same entity
    And both attempt to commit
    Then one transaction should succeed
    And the other should detect a concurrency conflict
