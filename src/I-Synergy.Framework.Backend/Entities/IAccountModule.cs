using System;

namespace ISynergy.Entities.Accounts
{
    public interface IAccountModule
    {
        Guid Account_Id { get; set; }
        Guid Module_Id { get; set; }
    }
}
