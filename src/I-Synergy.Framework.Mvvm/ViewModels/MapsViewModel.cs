using System.Collections.ObjectModel;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels
{
    public class MapsViewModel : ViewModelDialog<object>
    {
        public override string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Locations property value.
        /// </summary>
        public ObservableCollection<object> Locations
        {
            get { return GetValue<ObservableCollection<object>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the TimeToArrival property value.
        /// </summary>
        public double TimeToArrival
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DistcanceToArrival property value.
        /// </summary>
        public double DistcanceToArrival
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Content property value.
        /// </summary>
        public string Content
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Name property value.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Address property value.
        /// </summary>
        public string Address
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public MapsViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            string name, 
            string address)
            : base(context, commonServices, loggerFactory)
        {
            Locations = new ObservableCollection<object>();
            Title = address;
            Address = address;
            Name = name;
        }
    }
}
