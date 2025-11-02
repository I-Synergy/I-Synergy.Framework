# ?? Remaining BDD Files to Create

## EntityFramework - Step Definitions Needed (3 files)

### 1. RepositoryOperationsSteps.cs
**Step Methods Needed** (~12 steps):
- GivenTheRepositoryIsInitialized()
- GivenTheDatabaseContextIsConfigured()
- GivenIHaveANewTestEntity()
- GivenAnEntityExistsInTheDatabase()
- GivenMultipleEntitiesExistInTheDatabase()
- WhenIAddTheEntityToTheRepository()
- WhenIQueryTheEntityByItsID()
- WhenIModifyTheEntityProperties()
- WhenISaveTheChanges()
- WhenIDeleteTheEntityFromTheRepository()
- WhenIQueryEntitiesWithAFilterPredicate()
- WhenICheckIfTheEntityExistsByID()
- ThenTheEntityShouldBeSavedSuccessfully()
- ThenTheEntityShouldHaveADatabaseIDAssigned()
- ThenTheEntityShouldBeRetrievedSuccessfully()
- And more...

### 2. TransactionManagementSteps.cs
**Step Methods Needed** (~10 steps):
- GivenIHaveStartedADatabaseTransaction()
- GivenIHaveStartedATransactionWithTimeout()
- WhenIAddMultipleEntitiesWithinTheTransaction()
- WhenICommitTheTransaction()
- WhenAnErrorOccursDuringSave()
- WhenIRollbackTheTransaction()
- ThenAllEntitiesShouldBeSavedSuccessfully()
- ThenTheTransactionShouldBeMarkedAsCommitted()
- And more...

### 3. QueryBuildingSteps.cs
**Step Methods Needed** (~8 steps):
- GivenMultipleTestEntitiesExistInTheDatabase()
- GivenEntitiesHaveRelatedNavigationProperties()
- WhenIBuildAQueryWithAWherePredicate()
- WhenIExecuteTheQuery()
- WhenIBuildAQueryWithInclude()
- WhenIBuildAQueryWithOrderBy()
- ThenOnlyEntitiesMatchingThePredicateShouldBeReturned()
- And more...

---

## Storage.Azure - All Files Needed

### Project Setup
1. ? Verify project exists: `tests\ISynergy.Framework.Storage.Azure.Tests\`
2. Add Reqnroll packages to `.csproj`
3. Create `reqnroll.json`

### Feature Files (2)

#### 1. BlobOperations.feature
**Scenarios** (6):
- Uploading a file to blob storage
- Downloading a file from blob storage
- Deleting a blob
- Checking if a blob exists
- Getting blob metadata
- Generating a SAS token for blob access

#### 2. ContainerManagement.feature
**Scenarios** (4):
- Creating a blob container
- Listing blobs in a container
- Deleting a blob container
- Setting container access level

### Fixtures
- `TestBlob.cs` - Blob data model
- `TestBlobContainer.cs` - Container model

### Shared Context
- `StorageTestContext.cs` - Shared state

### Step Definitions (2)

#### 1. BlobOperationsSteps.cs
**Methods** (~15):
- Given steps for blob setup
- When steps for upload/download/delete
- Then steps for verification

#### 2. ContainerManagementSteps.cs
**Methods** (~10):
- Given steps for container setup
- When steps for container operations
- Then steps for verification

---

## Estimated File Count

| Project | Files to Create | Lines of Code (est.) |
|---------|----------------|---------------------|
| EntityFramework | 3 step definition files | ~800 lines |
| Storage.Azure | 2 features + 2 steps + 2 fixtures + 1 context + config | ~1000 lines |
| **TOTAL** | **10 files** | **~1800 lines** |

---

## Creation Strategy

### Batch 1: EntityFramework Step Definitions
- RepositoryOperationsSteps.cs
- TransactionManagementSteps.cs  
- QueryBuildingSteps.cs

### Batch 2: Storage.Azure Setup
- Add Reqnroll to project
- Create reqnroll.json
- Create fixtures and context

### Batch 3: Storage.Azure Features
- BlobOperations.feature
- ContainerManagement.feature

### Batch 4: Storage.Azure Step Definitions
- BlobOperationsSteps.cs
- ContainerManagementSteps.cs

### Batch 5: Testing & Validation
- Build all projects
- Run all tests
- Fix any issues
- Update documentation

---

## Would You Like Me To:

**A**: Create all EntityFramework step definitions now (Batch 1)
**B**: Create all Storage.Azure files now (Batches 2-4)
**C**: Create everything in one go (all remaining files)
**D**: Pause here and test what we have so far

**Recommendation**: Option A (finish EntityFramework completely first)
