namespace ISynergy.Framework.Payment.Mollie.Models.Order
{
    /// <summary>
    /// Class OrderAddressDetails.
    /// Implements the <see cref="AddressObject" />
    /// </summary>
    /// <seealso cref="AddressObject" />
    public class OrderAddressDetails : AddressObject
    {
        /// <summary>
        /// The title of the person, for example Mr. or Mrs..
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// The given name (first name) of the person.
        /// </summary>
        /// <value>The name of the given.</value>
        public string GivenName { get; set; }

        /// <summary>
        /// The family name (surname) of the person.
        /// </summary>
        /// <value>The name of the family.</value>
        public string FamilyName { get; set; }

        /// <summary>
        /// The email address of the person.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// The phone number of the person. Some payment methods require this information. If you have it, you
        /// should pass it so that your customer does not have to enter it again in the checkout. Must be in
        /// the E.164 format. For example +31208202070.
        /// </summary>
        /// <value>The phone.</value>
        public string Phone { get; set; }
    }
}
