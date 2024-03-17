using ISynergy.Framework.Wopi.Models;
using ISynergy.Framework.Wopi.Options;
using ISynergy.Framework.Wopi.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISynergy.Framework.Wopi.Services
{
    public class WopiDiscoveryService : IWopiDiscoveryService
    {
        private readonly IMemoryCache memoryCache;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly WopiOptions wopiOptions;

        public WopiDiscoveryService(IMemoryCache memoryCache, IOptions<WopiOptions> wopiOptions, IHttpClientFactory httpClientFactory)
        {
            this.memoryCache = memoryCache;
            this.wopiOptions = wopiOptions.Value;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Gets the discovery information from WOPI discovery and caches it appropriately
        /// </summary>
        public async Task<List<WopiAction>> GetActionsAsync()
        {
            // Determine if the discovery data is cached
            if (!memoryCache.TryGetValue(Constants.DiscoveryData, out _))
            {
                await RefreshAsync();
            }

            return memoryCache.Get<List<WopiAction>>(Constants.DiscoveryData);
        }

        public async Task RefreshAsync()
        {

            // Use the Wopi Discovery endpoint to get the data
            HttpClient client = httpClientFactory.CreateClient(Constants.HttpClientDefault);

            using (HttpResponseMessage response = await client.GetAsync(wopiOptions.DiscoveryUrl))
            {
                if (response.IsSuccessStatusCode)
                {
                    var actions = new List<WopiAction>();

                    // Read the xml string from the response
                    var xmlString = await response.Content.ReadAsStringAsync();

                    // Parse the xml string into Xml
                    var discoXml = XDocument.Parse(xmlString);

                    // Convert the discovery xml into list of WopiApp
                    var xapps = discoXml.Descendants("app");
                    foreach (var xapp in xapps)
                    {
                        // Parse the actions for the app
                        var xactions = xapp.Descendants("action");
                        foreach (var xaction in xactions)
                        {
                            actions.Add(new WopiAction()
                            {
                                app = xapp.Attribute(nameof(WopiAction.name)).Value,
                                favIconUrl = xapp.Attribute(nameof(WopiAction.favIconUrl)).Value,
                                checkLicense = Convert.ToBoolean(xapp.Attribute(nameof(WopiAction.checkLicense)).Value),
                                name = xaction.Attribute(nameof(WopiAction.name)).Value,
                                ext = (xaction.Attribute(nameof(WopiAction.ext)) != null) ? xaction.Attribute(nameof(WopiAction.ext)).Value : string.Empty,
                                progid = (xaction.Attribute(nameof(WopiAction.progid)) != null) ? xaction.Attribute(nameof(WopiAction.progid)).Value : string.Empty,
                                @default = (xaction.Attribute(nameof(WopiAction.@default)) != null) ? true : false,
                                urlsrc = xaction.Attribute(nameof(WopiAction.urlsrc)).Value,
                                requires = (xaction.Attribute(nameof(WopiAction.requires)) != null) ? xaction.Attribute(nameof(WopiAction.requires)).Value : string.Empty
                            });
                        }

                        // Cache the discovey data for an hour
                        await memoryCache.GetOrCreateAsync(Constants.DiscoveryData, entry =>
                        {
                            entry.SlidingExpiration = TimeSpan.FromHours(1);
                            return Task.FromResult(actions);
                        });
                    }

                    // Convert the discovery xml into list of WopiApp
                    var proof = discoXml.Descendants("proof-key").FirstOrDefault();
                    var wopiProof = new WopiProof()
                    {
                        value = proof.Attribute(nameof(WopiProof.value)).Value,
                        modulus = proof.Attribute(nameof(WopiProof.modulus)).Value,
                        exponent = proof.Attribute(nameof(WopiProof.exponent)).Value,
                        oldvalue = proof.Attribute(nameof(WopiProof.oldvalue)).Value,
                        oldmodulus = proof.Attribute(nameof(WopiProof.oldmodulus)).Value,
                        oldexponent = proof.Attribute(nameof(WopiProof.oldexponent)).Value
                    };

                    // Add to cache for 20min
                    await memoryCache.GetOrCreateAsync(Constants.WopiProof, entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromMinutes(20);
                        return Task.FromResult(wopiProof);
                    });
                }
            }

        }

        /// <summary>
        /// Forms the correct action url for the file and host
        /// </summary>
        public string GetActionUrl(WopiAction action, string fileId, string authority)
        {
            // Initialize the urlsrc
            var urlsrc = action.urlsrc;

            // Look through the action placeholders
            var placeholderCount = 0;

            foreach (var placeholder in WopiUrlPlaceholders.Placeholders)
            {
                if (urlsrc.Contains(placeholder))
                {
                    // Replace the placeholder value accordingly
                    var ph = WopiUrlPlaceholders.GetPlaceholderValue(placeholder);

                    if (!string.IsNullOrEmpty(ph))
                    {
                        urlsrc = urlsrc.Replace(placeholder, ph + "&");
                        placeholderCount++;
                    }
                    else
                        urlsrc = urlsrc.Replace(placeholder, ph);
                }
            }

            // Add the WOPISrc to the end of the request
            urlsrc += ((placeholderCount > 0) ? "" : "?") + string.Format("WOPISrc=https://{0}/wopi/files/{1}", authority, fileId);
            return urlsrc;
        }
    }
}
