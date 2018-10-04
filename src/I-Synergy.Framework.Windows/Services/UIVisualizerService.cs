using DryIoc;
using System;

namespace ISynergy.Services
{
    public class UIVisualizerService : UIVisualizerServiceBase
    {
        public UIVisualizerService(IContainer container)
            : base(container)
        {
        }
    }
}
