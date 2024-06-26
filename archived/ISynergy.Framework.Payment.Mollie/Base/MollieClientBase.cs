﻿using System;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Payment.Clients;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Payment.Mollie.Base
{
    /// <summary>
    /// Class MollieClientBase.
    /// Implements the <see cref="ClientBase" />
    /// </summary>
    /// <seealso cref="ClientBase" />
    public abstract class MollieClientBase : ClientBase
    {
        /// <summary>
        /// The client service
        /// </summary>
        protected readonly IMollieClientService _clientService;
        /// <summary>
        /// The mollie API options
        /// </summary>
        protected readonly MollieApiOptions _mollieApiOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MollieClientBase" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        protected MollieClientBase(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<MollieClientBase> logger) 
            : base(logger)
        {
            Argument.IsNotNull(options);

            _clientService = clientService;
            _mollieApiOptions = options.Value;
        }

        /// <summary>
        /// Validates the API key is oauth accesstoken.
        /// </summary>
        /// <param name="isConstructor">if set to <c>true</c> [is constructor].</param>
        /// <exception cref="InvalidOperationException">The provided token isn't an oauth token. You have invoked the method with oauth parameters thus an oauth accesstoken is required.</exception>
        /// <exception cref="ArgumentException">The provided token isn't an oauth token.</exception>
        protected void ValidateApiKeyIsOauthAccesstoken(bool isConstructor = false)
        {
            if (!_mollieApiOptions.ApiKey.StartsWith("access_"))
            {
                if (isConstructor)
                {
                    throw new InvalidOperationException(
                        "The provided token isn't an oauth token. You have invoked the method with oauth parameters thus an oauth accesstoken is required.");
                }

                throw new ArgumentException("The provided token isn't an oauth token.");
            }
        }
    }
}
