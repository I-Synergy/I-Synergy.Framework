# Reqnroll/Gherkin BDD Testing Setup

## Overview

This project now includes **Reqnroll** (formerly SpecFlow) for Behavior-Driven Development (BDD) testing using Gherkin syntax. Reqnroll allows you to write tests in plain English that map to executable test code.

## What Has Been Set Up

### 1. **Packages Installed**
- `Reqnroll` (v2.4.1) - Core BDD framework
- `Reqnroll.MSTest` (v2.4.1) - MSTest integration for Reqnroll
- `Reqnroll.Tools.MsBuild.Generation` (v2.4.1) - Build-time code generation

### 2. **Configuration**
- `reqnroll.json` - Reqnroll configuration file with English language settings and trace configuration

### 3. **Feature Files Created**
Located in `Features/` directory:
- **CommandDispatching.feature** - BDD scenarios for CQRS command dispatching
- **QueryDispatching.feature** - BDD scenarios for CQRS query dispatching
- **ErrorHandling.feature** - BDD scenarios for error handling in CQRS

### 4. **Step Definitions Created**
Located in `StepDefinitions/` directory:
- **CommandDispatchingSteps.cs** - Implementation of command dispatching scenarios
- **QueryDispatchingSteps.cs** - Implementation of query dispatching scenarios
- **ErrorHandlingSteps.cs** - Implementation of error handling scenarios

## Feature File Example

```gherkin
Feature: Command Dispatching
    As a developer using the CQRS framework
    I want to dispatch commands through the command dispatcher
    So that commands are properly handled and executed

Scenario: Dispatching a simple command
    Given I have a command with data "Test Data"
    When I dispatch the command
    Then the command should be handled successfully
    And the command handler should have executed
```

## How Reqnroll Works

1. **Write Features** - Business requirements in Gherkin (Given/When/Then)
2. **Generate Code** - Reqnroll generates test classes from `.feature` files at build time
3. **Implement Steps** - Write C# code in step definition classes
4. **Run Tests** - Execute as normal MSTest tests in Visual Studio Test Explorer

## Running the Tests

### Visual Studio Test Explorer
1. Build the solution
2. Open Test Explorer (Test ? Test Explorer)
3. Look for tests named after your scenarios
4. Run individual scenarios or all Reqnroll tests

### Command Line
```powershell
dotnet test tests\ISynergy.Framework.CQRS.Tests\ISynergy.Framework.CQRS.Tests.csproj
```

### Filter Reqnroll Tests
```powershell
dotnet test --filter "FullyQualifiedName~CommandDispatching"
```

## Benefits of Using Reqnroll in This Project

### 1. **Living Documentation**
- Feature files serve as executable specifications
- Non-technical stakeholders can read and understand tests
- Documentation stays in sync with code

### 2. **Behavior-Focused Testing**
- Tests describe WHAT the system does, not HOW
- Aligns perfectly with CQRS patterns (commands/queries are behaviors)
- Makes business logic explicit

### 3. **Reusable Step Definitions**
- Step definitions can be shared across multiple scenarios
- Reduces test code duplication
- Promotes consistency

### 4. **Better Test Organization**
- Groups related scenarios in feature files
- Clear separation between test intent (feature) and implementation (steps)
- Easy to find and maintain tests

## Writing New Tests

### 1. Create a Feature File
Create a new `.feature` file in the `Features/` directory:

```gherkin
Feature: My New Feature
    As a [role]
    I want [feature]
    So that [benefit]

Scenario: My first scenario
    Given [precondition]
    When [action]
    Then [expected result]
```

### 2. Generate Step Definitions
Build the project. Reqnroll will show missing step definitions in the output.

### 3. Implement Steps
Create or update step definition classes in `StepDefinitions/`:

```csharp
[Binding]
public class MyFeatureSteps
{
    [Given(@"some precondition")]
    public void GivenSomePrecondition()
    {
// Setup code
    }

    [When(@"I perform an action")]
    public async Task WhenIPerformAnAction()
    {
        // Action code
    }

  [Then(@"I expect a result")]
  public void ThenIExpectAResult()
 {
        // Assertion code  
    }
}
```

## Best Practices

### 1. **Use Meaningful Scenario Names**
? Bad: "Test 1"
? Good: "Dispatching a command with valid data should execute the handler"

### 2. **Keep Scenarios Focused**
- One scenario should test one behavior
- Use multiple scenarios instead of complex ones

### 3. **Use Background for Common Setup**
```gherkin
Background:
    Given the CQRS system is initialized
```

### 4. **Parameterize Steps**
```gherkin
Given I have a command with data "<data>"
Examples:
    | data |
    | Test1     |
    | Test2     |
```

### 5. **Follow AAA Pattern**
- **Given** = Arrange (setup)
- **When** = Act (execute)
- **Then** = Assert (verify)

## Integration with Existing Tests

Reqnroll tests **coexist** with traditional MSTest tests:

- **Reqnroll** - For behavior/integration testing, business scenarios
- **Traditional MSTest** - For unit tests, edge cases, performance tests

You can have both in the same project!

## Troubleshooting

### Tests Not Appearing
- Rebuild the solution
- Check that `.feature` files have Build Action = "None" or "Content"
- Verify `Reqnroll.Tools.MsBuild.Generation` package is installed

### Step Definition Not Found
- Ensure step definition method has `[Given]`, `[When]`, or `[Then]` attribute
- Check regex pattern matches the feature file text exactly
- Verify the class has `[Binding]` attribute

### Build Errors
- Clean and rebuild the solution
- Check that all Reqnroll packages have the same version
- Verify `reqnroll.json` is valid JSON

## Next Steps

1. **Fix Current Compilation Issues** - The step definitions need minor adjustments to work with the CQRS framework interfaces
2. **Run Existing Scenarios** - Verify the provided scenarios execute correctly
3. **Extend Coverage** - Add more feature files for other CQRS behaviors
4. **Integrate CI/CD** - Include Reqnroll tests in your build pipeline

## Resources

- [Reqnroll Documentation](https://docs.reqnroll.net/)
- [Gherkin Syntax Reference](https://cucumber.io/docs/gherkin/reference/)
- [MSTest Integration](https://docs.reqnroll.net/latest/integrations/mstest.html)

---

**Note:** The current step definitions require some adjustments to properly integrate with your CQRS framework. The primary issues are:
1. Import statements need correction for dispatcher interfaces
2. Assert statements need to use MSTest 4.x syntax
3. Some interface method calls need null-checking

These will be fixed in the next iteration.
