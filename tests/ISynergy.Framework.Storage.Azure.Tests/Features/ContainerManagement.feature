Feature: Container Management
    As a developer using Azure Blob Storage
    I want to manage blob containers
    So that I can organize and control access to blobs

Background:
    Given the blob storage service is configured

Scenario: Creating a blob container
    Given I have a container name
    When I create a new container
    Then the container should be created successfully
    And the container should exist in storage

Scenario: Listing blobs in a container
    Given a container with multiple blobs exists
    When I list the blobs in the container
    Then all blobs should be returned
    And the blob count should match the expected number

Scenario: Deleting a blob container
    Given a container exists
    When I delete the container
    Then the container should be removed from storage
    And checking for the container should return false

Scenario: Setting container access level
    Given a container exists
    When I set the container access level to public
    Then the container access level should be updated
    And blobs should be publicly accessible
