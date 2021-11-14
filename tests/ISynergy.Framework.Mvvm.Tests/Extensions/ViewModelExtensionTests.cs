using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.Mvvm.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mvvm.Tests.Extensions
{
    [TestClass]
    public class ViewModelExtensionTests
    {
        [DataTestMethod]
        [DataRow(typeof(ViewModels.MapsViewModel), "MapsViewModel")]
        [DataRow(typeof(ViewModels.NoteViewModel), "NoteViewModel")]
        [DataRow(typeof(ViewModels.SelectionViewModel<TestClass>), "SelectionViewModel")]
        public void GetNameOfViewModelByTypeTest(Type viewModelType, string expectedName)
        {
            var result = viewModelType.GetViewModelName();
            Assert.AreEqual(expectedName, result);
        }

        [DataTestMethod]
        [DataRow(typeof(ViewModels.MapsViewModel), "ISynergy.Framework.Mvvm.ViewModels.MapsViewModel")]
        [DataRow(typeof(ViewModels.NoteViewModel), "ISynergy.Framework.Mvvm.ViewModels.NoteViewModel")]
        [DataRow(typeof(ViewModels.SelectionViewModel<TestClass>), "ISynergy.Framework.Mvvm.ViewModels.SelectionViewModel")]
        public void GetFullNameOfViewModelByTypeTest(Type viewModelType, string expectedName)
        {
            var result = viewModelType.GetViewModelFullName();
            Assert.AreEqual(expectedName, result);
        }
    }
}
