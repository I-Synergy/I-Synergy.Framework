using ISynergy.Services;
using ISynergy.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ISynergy.ViewModels
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
        /// Gets or sets the Token property value.
        /// </summary>
        public string Token => "hF8EPqW3h22fX9BAQyU1~0IckNGlmrR9tLsNHtBrP6g~AnhUPQAJSywzA2Wj53uJ2pfv8_JzJTPP2V4gAkttD4AOFbJWL51QQ9Lj5F8i0to3";
      
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
        
        public MapsViewModel(IContext context, IBusyService busy, string name, string address)
            : base(context, busy)
        {
            Locations = new ObservableCollection<object>();
            Title = address;
            Address = address;
            Name = name;
        }

        public override Task SubmitAsync(object e)
        {
            throw new NotImplementedException();
        }
    }
}