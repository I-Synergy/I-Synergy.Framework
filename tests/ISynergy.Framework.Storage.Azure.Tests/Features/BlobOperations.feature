Feature: Blob Operations
    As a developer using Azure Blob Storage
    I want to perform file operations on blobs
    So that I can manage cloud storage effectively

Background:
    Given the blob storage service is configured
    And I have a test container

Scenario: Uploading a file to blob storage
    Given I have a file to upload
    When I upload the file to blob storage
    Then the file should be uploaded successfully
 And the blob should exist in the container

Scenario: Downloading a file from blob storage
    Given a blob exists in the container
    When I download the blob
    Then the blob content should be retrieved successfully
    And the content should match the uploaded file

Scenario: Deleting a blob
 Given a blob exists in the container
    When I delete the blob
    Then the blob should be removed from storage
 And checking for the blob should return false

Scenario: Checking if a blob exists
    Given a blob exists in the container
    When I check if the blob exists
  Then the existence check should return true
    And checking for a non-existent blob should return false

Scenario: Getting blob metadata
    Given a blob with metadata exists
    When I retrieve the blob metadata
    Then the metadata should be returned successfully
And the metadata properties should be accessible

Scenario: Generating a SAS token for blob access
    Given a blob exists in the container
    When I generate a SAS token for the blob
    Then a valid SAS URI should be returned
    And the SAS token should have appropriate permissions
