using System;
using System.ComponentModel;

namespace ISynergy
{
    public interface IModelBase : INotifyPropertyChanged
    {
        int Version { get; set; }
        DateTimeOffset CreatedDate { get; set; }
        DateTimeOffset? ChangedDate { get; set; }
        string CreatedBy { get; set; }
        string ChangedBy { get; set; }
    }
}