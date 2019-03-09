using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Background.Tasks
{
    internal class JobFactory : IJobFactory
    {
        IJob IJobFactory.GetJobInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }
    }
}
