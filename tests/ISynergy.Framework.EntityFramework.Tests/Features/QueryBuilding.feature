Feature: Query Building
    As a developer using Entity Framework
    I want to build complex queries using LINQ
    So that I can retrieve data efficiently with filtering and sorting

Background:
    Given the database context is configured
    And multiple test entities exist in the database

Scenario: Building queries with Where clause
  When I build a query with a Where predicate
    And I execute the query
    Then only entities matching the predicate should be returned
    And the query should be translated to SQL correctly

Scenario: Building queries with Include for eager loading
    Given entities have related navigation properties
    When I build a query with Include for related entities
    And I execute the query
    Then the related entities should be loaded
  And no additional database queries should be executed

Scenario: Building queries with OrderBy
    When I build a query with OrderBy ascending
    And I execute the query
    Then the results should be sorted in ascending order
    And the first result should have the minimum value

Scenario: Building complex queries with multiple operations
    When I build a query with Where, OrderBy, and Take
    And I execute the query
    Then the results should be filtered, sorted, and limited
    And the query performance should be optimized
