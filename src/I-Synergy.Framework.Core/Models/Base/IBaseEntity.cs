using ISynergy.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Entities.Base
{
    public interface IBaseEntity : IBaseClass
    {
        string Memo { get; set; }
        string InputFirst { get; set; }
        string InputLast { get; set; }
    }
}
