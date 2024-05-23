using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.Core.Abstractions;

public interface IMigration
{
    void Up();
    void Down();
    string MigrationVersion { get; }
}
