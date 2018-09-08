using ISynergy.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Entities.Accounts
{
    public interface IModule : IBaseClass
    {
        Guid Module_Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        bool IsActive { get; set; }
        string Memo { get; set; }
        string InputFirst { get; set; }
        string InputLast { get; set; }
    }
}
