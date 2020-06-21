﻿using System.Collections.Generic;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;

namespace Mollie.Sample.Models {
    /// <summary>
    /// Class OverviewModel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <autogeneratedoc />
    public class OverviewModel<T> where T : IResponseObject {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        /// <autogeneratedoc />
        public List<T> Items { get; set; }
        /// <summary>
        /// Gets or sets the navigation.
        /// </summary>
        /// <value>The navigation.</value>
        /// <autogeneratedoc />
        public OverviewNavigationLinksModel Navigation { get; set; }
    }
}