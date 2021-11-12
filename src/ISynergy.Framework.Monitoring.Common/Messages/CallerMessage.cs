using System.Collections.Generic;

namespace ISynergy.Framework.Messages
{
    /// <summary>
    /// Class CallerMessage.
    /// </summary>
    public class CallerMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallerMessage" /> class.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="relations">The relations.</param>
        public CallerMessage(string phoneNumber, List<object> relations = default)
        {
            PhoneNumber = phoneNumber;
            Relations = relations;
        }

        /// <summary>
        /// Gets the relations.
        /// </summary>
        /// <value>The relations.</value>
        public List<object> Relations { get; }
        /// <summary>
        /// Gets the phone number.
        /// </summary>
        /// <value>The phone number.</value>
        public string PhoneNumber { get; }
    }
}
