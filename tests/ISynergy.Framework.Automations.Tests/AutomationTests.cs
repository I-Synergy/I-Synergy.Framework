using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Conditions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Automations.Services;
using ISynergy.Framework.Automations.Tests.Data;
using ISynergy.Framework.Automations.Triggers;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;

namespace ISynergy.Framework.Automations.Tests
{
    [TestClass()]
    public class AutomationTests
    {
        private readonly Customer _defaultCustomer;
        private readonly Automation _defaultAutomation;
        private readonly IAutomationService _automationService;

        /// <summary>
        /// Default constructor for the test.
        /// </summary>
        public AutomationTests()
        {
            _automationService = new AutomationService(
                new Mock<IAutomationManager>().Object,
                new Mock<IOptions<AutomationOptions>>().Object,
                new Mock<ILogger<AutomationService>>().Object);

            _defaultCustomer = new Customer()
            {
                Name = "Test1",
                Email = "admin@demo.com",
                Age = 18,
                Active = false
            };

            _defaultAutomation = new Automation();
            _defaultAutomation.IsActive = true;
            _defaultAutomation.Conditions.Add(new Condition<Customer>(_defaultAutomation.AutomationId, (e) => !string.IsNullOrEmpty(e.Name)));
            _defaultAutomation.Conditions.Add(new Condition<Customer>(_defaultAutomation.AutomationId, (e) => !string.IsNullOrEmpty(e.Email)));
            _defaultAutomation.Conditions.Add(new Condition<Customer>(_defaultAutomation.AutomationId, (e) => e.Age >= 18));
        }

        /// <summary>
        /// Scenario where new automation is default disabled.
        /// </summary>
        [TestMethod()]
        public void AutomationDefaultInActiveTest()
        {
            var _defaultAutomation = new Automation();
            Assert.IsFalse(_defaultAutomation.IsActive);
        }

        /// <summary>
        /// Scenario where new automation is activated.
        /// </summary>
        [TestMethod()]
        public void AutomationActiveTest()
        {
            var _defaultAutomation = new Automation();
            _defaultAutomation.IsActive = true;
            Assert.IsTrue(_defaultAutomation.IsActive);
        }

        /// <summary>
        /// Scenario where 2 customers are checked against the conditions after a delay of 1 second.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AutomationScenario1TestAsync()
        {
            _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, TimeSpan.FromSeconds(1)));

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var result1 = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
            stopwatch.Stop();

            // Assert should succeed because condition is met.
            Assert.IsTrue(result1.Succeeded);
            // Assert should succeed because condition is met and delay is applied.
            Assert.IsTrue(stopwatch.Elapsed > TimeSpan.FromSeconds(1));

            stopwatch.Reset();

            var customer2 = new Customer()
            {
                Name = "Test2",
                Age = 16
            };

            stopwatch.Start();
            var result2 = await _automationService.ExecuteAsync(_defaultAutomation, customer2);
            stopwatch.Stop();

            // Assert should fail because condition is not met. Age is below 18.
            Assert.IsFalse(result2.Succeeded);
            // Assert should fail because no condition is met to run the action.
            Assert.IsFalse(stopwatch.Elapsed > TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Scenario where _defaultAutomation is run and executed before timing out.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AutomationScenario2TestAsync()
        {
            var command = new Command<Customer>((e) =>
            {
                e.Age = 16;
                _defaultAutomation.IsActive = false;
            });

            var command2 = new Command<Customer>((e) =>
            {
                e.Age += 1;
            });

            _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command, _defaultCustomer));
            _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, TimeSpan.FromSeconds(5)));
            _defaultAutomation.Actions.Add(new RepeatPreviousAction(_defaultAutomation.AutomationId, 1));
            _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command2, _defaultCustomer));
            _defaultAutomation.Actions.Add(new RepeatPreviousAction<Customer>(_defaultAutomation.AutomationId, RepeatTypes.Until, (e) => e.Age >= 35, 10));

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var result1 = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
            stopwatch.Stop();

            // Assert should succeed because condition is met.
            Assert.IsTrue(result1.Succeeded);
            Assert.AreEqual(27, ((Customer)result1.Result).Age);
            // Assert should succeed because condition is met and delay is applied.
            Assert.IsTrue(stopwatch.Elapsed > TimeSpan.FromSeconds(5 * 2));

            stopwatch.Reset();

            stopwatch.Start();
            var result2 = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
            stopwatch.Stop();

            // Assert should fail because condition is not met. Age is below 18.
            Assert.IsFalse(result2.Succeeded);
            // Assert should fail because no condition is met to run the action.
            Assert.IsFalse(stopwatch.Elapsed > TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Scenario where _defaultAutomation is run and executed before timing out.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AutomationScenario3TestAsync()
        {
            var command = new Command<Customer>((e) =>
            {
                e.Age = 16;
            });

            _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command, _defaultCustomer));
            _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, TimeSpan.FromSeconds(3)));

            var result1 = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
            Assert.IsTrue(result1.Succeeded);
            Assert.AreEqual(16, ((Customer)result1.Result).Age);

            var result2 = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
            Assert.IsFalse(result2.Succeeded);
        }

        /// <summary>
        /// Scenario where _defaultAutomation trigger is set and handled.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public Task AutomationScenario4TestAsync()
        {
            var command = new Command<Customer>((e) =>
            {
                e.Age = 16;
            });

            _defaultAutomation.ExecutionTimeout = TimeSpan.FromSeconds(10);
            _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command, _defaultCustomer));
            _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, TimeSpan.FromSeconds(4)));

            var trigger = new IntegerTrigger(
                _defaultAutomation.AutomationId,
                () => new(_defaultCustomer, (IProperty<int>)_defaultCustomer.Properties[nameof(Customer.Age)]),
                65,
                18,
                async (age) =>
                {
                    var result = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
                    Assert.IsTrue(result.Succeeded);
                    Assert.AreEqual(21, age);
                });

            _defaultAutomation.Triggers.Add(trigger);
            _defaultCustomer.Age = 17;
            _defaultCustomer.Age = 21;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario where _defaultAutomation is cancelled by timeout.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AutomationScenario5TestAsync()
        {
            var automation = new Automation();
            automation.IsActive = true;
            automation.ExecutionTimeout = TimeSpan.FromSeconds(5);
            automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(10)));
            await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => _automationService.ExecuteAsync(automation, null));
        }

        /// <summary>
        /// Scenario where automation executes 5 delays.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AutomationScenario6TestAsync()
        {
            var stopwatch = new Stopwatch();
            var automation = new Automation();
            automation.IsActive = true;
            automation.ExecutionTimeout = TimeSpan.FromSeconds(10);
            automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(2)));
            automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(2)));
            automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(2)));

            stopwatch.Start();
            var result = await _automationService.ExecuteAsync(automation, null);
            stopwatch.Stop();

            Assert.IsTrue(result.Succeeded);
            // Assert should succeed because condition is met and delay is applied.
            Assert.IsTrue(stopwatch.Elapsed > TimeSpan.FromSeconds(3 * 2));
        }

        /// <summary>
        /// Scenario where boolean state trigger is executed.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public Task AutomationScenario7TestAsync()
        {
            var automation = new Automation();
            automation.IsActive = true;
            automation.ExecutionTimeout = TimeSpan.FromSeconds(10);

            var stateTrigger = new BooleanStateTrigger(
                automation.AutomationId,
                () => new(_defaultCustomer, _defaultCustomer.GetProperty(x => x.Active)),
                false,
                true,
                async (active) =>
                {
                    var result = await _automationService.ExecuteAsync(automation, _defaultCustomer);
                    Assert.IsTrue(result.Succeeded);
                    Assert.AreEqual(true, active);
                });

            automation.Triggers.Add(stateTrigger);
            automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(7)));

            _defaultCustomer.Active = true;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario where string state trigger is executed.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public Task AutomationScenario8TestAsync()
        {
            var automation = new Automation();
            automation.IsActive = true;
            automation.ExecutionTimeout = TimeSpan.FromSeconds(10);

            var name = "Test";

            var stateTrigger = new StringStateTrigger(
                automation.AutomationId,
                () => new(_defaultCustomer, _defaultCustomer.GetProperty(x => x.Name)),
                _defaultCustomer.Name,
                name,
                async (newName) =>
                {
                    var result = await _automationService.ExecuteAsync(automation, _defaultCustomer);
                    Assert.IsTrue(result.Succeeded);
                    Assert.AreEqual(name, newName);
                });

            automation.Triggers.Add(stateTrigger);
            automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(8)));

            _defaultCustomer.Name = name;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario where boolean state trigger is not executed because they are the same.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void AutomationScenario9Test()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var stateTrigger = new BooleanStateTrigger(
                new Automation().AutomationId,
                () => new(_defaultCustomer, _defaultCustomer.GetProperty(x => x.Active)),
                false,
                false,
                null);
            });
        }

        /// <summary>
        /// Scenario where event trigger is executed.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public Task AutomationScenario10TestAsync()
        {
            var automation = new Automation();
            automation.IsActive = true;
            automation.ExecutionTimeout = TimeSpan.FromSeconds(15);

            var stateTrigger = new EventTrigger<Customer>(
                automation.AutomationId,
                _defaultCustomer,
                (s) => _defaultCustomer.Registered += s,
                async (e) =>
                {
                    var result = await _automationService.ExecuteAsync(automation, e);
                    Assert.IsTrue(result.Succeeded);
                });

            automation.Triggers.Add(stateTrigger);
            automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(10)));

            _defaultCustomer.Register();

            return Task.CompletedTask;
        }
    }
}