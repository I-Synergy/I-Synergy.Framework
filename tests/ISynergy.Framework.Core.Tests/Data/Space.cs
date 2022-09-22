using ISynergy.Framework.Core.Tests.Enumerations;
using System;

namespace ISynergy.Framework.Core.Tests.Data
{
    public class Space
    {
        public Guid Id { get; set; }    
        public string Name { get; set; }
        public double SquareFeet { get; set; }
        public SpaceTypes Type { get; set; }
        
        public Space()
        {
            Id = Guid.NewGuid();
        }
    }
}
