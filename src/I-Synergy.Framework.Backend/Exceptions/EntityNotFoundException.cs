using System;

namespace ISynergy.Exceptions
{
    [Serializable]
    public partial class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
            : base()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}