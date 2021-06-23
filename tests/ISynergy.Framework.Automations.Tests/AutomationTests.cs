using Microsoft.VisualStudio.TestTools.UnitTesting;
using ISynergy.Framework.Automations.Conditions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Automations.States;
using ISynergy.Framework.Automations.Tests.Data;
using ISynergy.Framework.Mvvm.Commands;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Automations.Tests
{
    [TestClass()]
    public class AutomationTests
    {
        [TestMethod()]
        public void AutomationDefaultInActiveTest()
        {
            var automation = new Automation();
            Assert.IsFalse(automation.IsActive);
        }

        [TestMethod()]
        public void AutomationActiveTest()
        {
            var automation = new Automation();
            automation.IsActive = true;
            Assert.IsTrue(automation.IsActive);
        }

        [TestMethod]
        public async Task AutomationScenario1TestAsync()
        {
            var customer1 = new Customer()
            {
                Name = "Test1",
                Email = "admin@demo.com",
                Age = 21
            };

            var customer2 = new Customer()
            {
                Name = "Test2",
                Age = 16
            };

            var automation = new Automation();
            automation.IsActive = true;
            automation.Conditions.Add(new Condition<Customer>(automation.AutomationId, (e) => !string.IsNullOrEmpty(e.Name)));
            automation.Conditions.Add(new Condition<Customer>(automation.AutomationId, (e) => !string.IsNullOrEmpty(e.Email)));
            automation.Conditions.Add(new Condition<Customer>(automation.AutomationId, (e) => e.Age >= 18));

            Assert.IsTrue(await automation.ExecuteAsync(customer1));
            Assert.IsFalse(await automation.ExecuteAsync(customer2));
        }

        [TestMethod]
        public async Task AutomationScenario2TestAsync()
        {
            var customer1 = new Customer()
            {
                Name = "Test1",
                Email = "admin@demo.com",
                Age = 21
            };

            var automation = new Automation();
            automation.IsActive = true;
            automation.Conditions.Add(new Condition<Customer>(automation.AutomationId, (e) => !string.IsNullOrEmpty(e.Name)));
            automation.Conditions.Add(new Condition<Customer>(automation.AutomationId, (e) => !string.IsNullOrEmpty(e.Email)));
            automation.Conditions.Add(new Condition<Customer>(automation.AutomationId, (e) => e.Age >= 18));

            var command = new Command<Customer>((e) =>
            {
                e.Age = 16;
                automation.IsActive = false;
            });

            var command2 = new Command<Customer>((e) =>
            {
                e.Age += 1;
            });

            automation.Actions.Add(new CommandAction(automation.AutomationId, command, customer1));
            automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(5)));
            automation.Actions.Add(new RepeatAction(automation.AutomationId, 3));
            automation.Actions.Add(new CommandAction(automation.AutomationId, command2, customer1));
            automation.Actions.Add(new RepeatAction<Customer>(automation.AutomationId, RepeatTypes.Until, (e) => e.Age >= 35, 10));

            Assert.IsTrue(await automation.ExecuteAsync(customer1));
            Assert.IsFalse(await automation.ExecuteAsync(customer1));
        }
    }
}