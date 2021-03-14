using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models
{
    /// <summary>
    /// Class User.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the snowflake identifier.
        /// </summary>
        /// <value>The snowflake identifier.</value>
        public SnowflakeId SnowflakeId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the nullable int.
        /// </summary>
        /// <value>The nullable int.</value>
        public int? NullableInt { get; set; }

        /// <summary>
        /// Gets or sets the income.
        /// </summary>
        /// <value>The income.</value>
        public int Income { get; set; }

        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        /// <value>The profile.</value>
        public UserProfile Profile { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public UserState State { get; set; }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>The roles.</value>
        public List<Role> Roles { get; set; }

        /// <summary>
        /// Tests the method1.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TestMethod1()
        {
            return true;
        }

        /// <summary>
        /// Tests the method2.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TestMethod2(User other)
        {
            return true;
        }

        /// <summary>
        /// Tests the method3.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TestMethod3(User other)
        {
            return Id == other.Id;
        }

        /// <summary>
        /// Generates the sample models.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="allowNullableProfiles">if set to <c>true</c> [allow nullable profiles].</param>
        /// <returns>IList&lt;User&gt;.</returns>
        public static IList<User> GenerateSampleModels(int total, bool allowNullableProfiles = false)
        {
            var list = new List<User>();

            for (int i = 0; i < total; i++)
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    SnowflakeId = new SnowflakeId(((ulong)long.MaxValue + (ulong)i + 2UL)),
                    UserName = "User" + i,
                    Income = 1 + (i % 15) * 100
                };

                if (!allowNullableProfiles || (i % 8) != 5)
                {
                    user.Profile = new UserProfile
                    {
                        FirstName = "FirstName" + i,
                        LastName = "LastName" + i,
                        Age = (i % 50) + 18
                    };
                }

                user.Roles = new List<Role>(Role.StandardRoles);

                list.Add(user);
            }

            return list.ToArray();
        }
    }
}
