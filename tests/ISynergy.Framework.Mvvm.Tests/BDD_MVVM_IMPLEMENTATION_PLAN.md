# ?? MVVM BDD Tests - Implementation Plan

## Overview
Creating BDD tests for **ISynergy.Framework.Mvvm** to complement existing unit tests with behavioral scenarios and living documentation.

---

## ? Features Created (3)

### 1. Command Execution (6 scenarios)
Tests command patterns and execution flow:
- ? Executing a synchronous RelayCommand
- ? AsyncRelayCommand execution
- ? Command CanExecute validation prevents execution
- ? Command with parameter execution
- ? AsyncRelayCommand with cancellation
- ? Command execution updates CanExecute

### 2. Property Change Notification (5 scenarios)
Tests INotifyPropertyChanged implementation:
- ? Property change raises PropertyChanged event
- ? Multiple property changes raise multiple events
- ? SetProperty only raises event when value changes
- ? Computed property updates when dependency changes
- ? Property validation on change

### 3. ViewModel Lifecycle (4 scenarios)
Tests ViewModel initialization and disposal:
- ? ViewModel initialization
- ? ViewModel disposal
- ? ViewModel busy state management
- ? ViewModel title and subtitle binding

---

## ?? Summary Statistics

**Total Scenarios**: 15  
**Total Features**: 3  
**Files Created**: 4 (reqnroll.json + 3 features)  
**Files Needed**: ~6 more (fixtures + 3 step definitions)

---

## ?? File Structure

```
tests/ISynergy.Framework.Mvvm.Tests/
??? ISynergy.Framework.Mvvm.Tests.csproj ? (updated)
??? reqnroll.json ?
??? Features/
?   ??? CommandExecution.feature ?
?   ??? PropertyChangeNotification.feature ?
?   ??? ViewModelLifecycle.feature ?
??? Fixtures/
?   ??? TestViewModel.cs ? (needed)
??? StepDefinitions/
    ??? MvvmTestContext.cs ? (needed)
    ??? CommandExecutionSteps.cs ? (needed)
    ??? PropertyChangeNotificationSteps.cs ? (needed)
    ??? ViewModelLifecycleSteps.cs ? (needed)
```

---

## ?? Design Philosophy

### Complementing Existing Unit Tests
The existing unit tests cover:
- ? Individual command functionality
- ? Specific ViewModel types
- ? Extension methods

The new BDD tests focus on:
- ? **Behavioral scenarios** - How features work together
- ? **Developer workflows** - Common usage patterns
- ? **Living documentation** - Readable specifications
- ? **Integration patterns** - Commands + ViewModels + Property notification

### Key Differences from Unit Tests
| Aspect | Unit Tests | BDD Tests |
|--------|------------|-----------|
| Focus | Individual methods | Complete behaviors |
| Style | Code-centric | Business-readable |
| Coverage | Technical details | User scenarios |
| Documentation | Code comments | Gherkin features |

---

## ?? Next Steps

### Immediate (Step Definitions):
1. ? Create `MvvmTestContext.cs` - Shared state
2. ? Create `TestViewModel.cs` - Test fixture
3. ? Create `CommandExecutionSteps.cs` - Command scenarios
4. ? Create `PropertyChangeNotificationSteps.cs` - Property notification scenarios
5. ? Create `ViewModelLifecycleSteps.cs` - Lifecycle scenarios

### Testing:
1. ? Build and run BDD tests
2. ? Verify all 15 scenarios pass
3. ? Update documentation

---

## ?? Benefits

### For Developers:
- ? **Clear examples** of how to use MVVM patterns correctly
- ? **Living documentation** that stays in sync with code
- ? **Onboarding tool** for new team members
- ? **API contract validation** ensuring patterns remain stable

### For Quality:
- ? **Regression protection** for MVVM behaviors
- ? **Integration testing** of MVVM components together
- ? **Behavioral validation** beyond unit test coverage

### For Maintenance:
- ? **Refactoring confidence** with comprehensive behavioral tests
- ? **Breaking change detection** in MVVM patterns
- ? **Documentation accuracy** enforced by tests

---

## ?? Progress

**Project Setup**: ? Complete  
**Feature Files**: ? Complete (3/3)  
**Step Definitions**: ? Pending (0/3)  
**Test Fixtures**: ? Pending (0/2)  
**Overall**: 40% complete

---

## ?? Combined BDD Test Coverage

With MVVM BDD tests, the framework will have:

| Project | Scenarios | Status |
|---------|-----------|--------|
| CQRS | 12 | ? Complete |
| AspNetCore | 9 | ? Complete |
| Automations | 7 | ? Complete |
| MessageBus | 10 | ? Complete |
| EntityFramework | 15 | ? Complete |
| Storage.Azure | 10 | ? Complete |
| **MVVM** | **15** | **? 40%** |
| **TOTAL** | **78** | **88% Complete** |

---

*Created: January 2025*  
*Framework: I-Synergy.Framework.Mvvm*  
*BDD Tool: Reqnroll 3.2.0*  
*.NET Version: 10.0*
