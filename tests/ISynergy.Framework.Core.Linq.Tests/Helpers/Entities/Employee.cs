using System;
using System.ComponentModel.DataAnnotations.Schema;
using Linq.PropertyTranslator.Core;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    /// <summary>
    /// Class Employee.
    /// Implements the <see cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Entity" />
    public class Employee : Entity
    {
        /// <summary>
        /// The full name expr
        /// </summary>
        private static readonly CompiledExpressionMap<Employee, string> FullNameExpr =
            DefaultTranslationOf<Employee>.Property(e => e.FullName).Is(e => e.FirstName + " " + e.LastName);

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        /// <value>The employee number.</value>
        public int EmployeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the hire date.
        /// </summary>
        /// <value>The hire date.</value>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// Gets or sets the company identifier.
        /// </summary>
        /// <value>The company identifier.</value>
        public long? CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the country identifier.
        /// </summary>
        /// <value>The country identifier.</value>
        public long? CountryId { get; set; }

        /// <summary>
        /// Gets or sets the function identifier.
        /// </summary>
        /// <value>The function identifier.</value>
        public long? FunctionId { get; set; }

        /// <summary>
        /// Gets or sets the sub function identifier.
        /// </summary>
        /// <value>The sub function identifier.</value>
        public long? SubFunctionId { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>The company.</value>
        public Company Company { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        public Country Country { get; set; }

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        /// <value>The function.</value>
        public Function Function { get; set; }

        /// <summary>
        /// Gets or sets the sub function.
        /// </summary>
        /// <value>The sub function.</value>
        public SubFunction SubFunction { get; set; }

        /// <summary>
        /// Gets or sets the assigned.
        /// </summary>
        /// <value>The assigned.</value>
        public int? Assigned { get; set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        [NotMapped]
        public string FullName => FullNameExpr.Evaluate(this);
    }
}
