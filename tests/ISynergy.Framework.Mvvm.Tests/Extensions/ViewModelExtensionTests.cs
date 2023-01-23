using ISynergy.Framework.Mvvm.Models.Tests;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mvvm.Extensions.Tests
{
    [TestClass]
    public class ViewModelExtensionTests
    {
        [DataTestMethod]
        [DataRow(typeof(ISynergy.Framework.Mvvm.ViewModels.MapsViewModel), "MapsViewModel")]
        [DataRow(typeof(ISynergy.Framework.Mvvm.ViewModels.NoteViewModel), "NoteViewModel")]
        [DataRow(typeof(ISynergy.Framework.Mvvm.ViewModels.SelectionViewModel), "SelectionViewModel")]
        public void GetNameOfViewModelByTypeTest(Type viewModelType, string expectedName)
        {
            var result = viewModelType.GetViewModelName();
            Assert.AreEqual(expectedName, result);
        }

        [DataTestMethod]
        [DataRow(typeof(ISynergy.Framework.Mvvm.ViewModels.MapsViewModel), "ISynergy.Framework.Mvvm.ViewModels.MapsViewModel")]
        [DataRow(typeof(ISynergy.Framework.Mvvm.ViewModels.NoteViewModel), "ISynergy.Framework.Mvvm.ViewModels.NoteViewModel")]
        [DataRow(typeof(ISynergy.Framework.Mvvm.ViewModels.SelectionViewModel), "ISynergy.Framework.Mvvm.ViewModels.SelectionViewModel")]
        public void GetFullNameOfViewModelByTypeTest(Type viewModelType, string expectedName)
        {
            var result = viewModelType.GetViewModelFullName();
            Assert.AreEqual(expectedName, result);
        }
    }
}
