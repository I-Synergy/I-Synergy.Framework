using ISynergy.Framework.Mvvm.Models.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.Mvvm.Extensions.Tests
{
    [TestClass]
    public class ViewModelExtensionTests
    {
        [DataTestMethod]
        [DataRow(typeof(ViewModels.MapsViewModel), "MapsViewModel")]
        [DataRow(typeof(ViewModels.NoteViewModel), "NoteViewModel")]
        public void GetNameOfViewModelByTypeTest(Type viewModelType, string expectedName)
        {
            string result = viewModelType.GetViewModelName();
            Assert.AreEqual(expectedName, result);
        }

        [DataTestMethod]
        [DataRow(typeof(ViewModels.MapsViewModel), "ISynergy.Framework.Mvvm.ViewModels.MapsViewModel")]
        [DataRow(typeof(ViewModels.NoteViewModel), "ISynergy.Framework.Mvvm.ViewModels.NoteViewModel")]
        public void GetFullNameOfViewModelByTypeTest(Type viewModelType, string expectedName)
        {
            string result = viewModelType.GetViewModelFullName();
            Assert.AreEqual(expectedName, result);
        }
    }
}
