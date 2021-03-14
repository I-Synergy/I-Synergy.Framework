using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Models.Tests;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.Mvvm.Extensions.Tests
{
    public class ViewModelExtensionTests
    {
        [Theory]
        [InlineData(typeof(ISynergy.Framework.Mvvm.ViewModels.MapsViewModel), "MapsViewModel")]
        [InlineData(typeof(ISynergy.Framework.Mvvm.ViewModels.NoteViewModel), "NoteViewModel")]
        [InlineData(typeof(ISynergy.Framework.Mvvm.ViewModels.SelectionViewModel<TestClass>), "SelectionViewModel")]
        public void GetNameOfViewModelByTypeTest(Type viewModelType, string expectedName)
        {
            var result = viewModelType.GetViewModelName();
            Assert.Equal(expectedName, result);
        }

        [Theory]
        [InlineData(typeof(ISynergy.Framework.Mvvm.ViewModels.MapsViewModel), "ISynergy.Framework.Mvvm.ViewModels.MapsViewModel")]
        [InlineData(typeof(ISynergy.Framework.Mvvm.ViewModels.NoteViewModel), "ISynergy.Framework.Mvvm.ViewModels.NoteViewModel")]
        [InlineData(typeof(ISynergy.Framework.Mvvm.ViewModels.SelectionViewModel<TestClass>), "ISynergy.Framework.Mvvm.ViewModels.SelectionViewModel")]
        public void GetFullNameOfViewModelByTypeTest(Type viewModelType, string expectedName)
        {
            var result = viewModelType.GetViewModelFullName();
            Assert.Equal(expectedName, result);
        }
    }
}
