using System;

namespace ISynergy.Entities.Accounts
{
    public interface IModule : IClassBase
    {
        Guid Module_Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        bool IsActive { get; set; }
    }
}
