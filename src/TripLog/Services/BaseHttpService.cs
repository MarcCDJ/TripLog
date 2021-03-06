﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TripLog.Services
{
    public abstract class BaseHttpService
    {
#pragma warning disable CA1822 // Mark members as static
        protected async Task<T> SendRequestAsync<T>(
#pragma warning restore CA1822 // Mark members as static
        Uri url,
        HttpMethod httpMethod = null,
        IDictionary<string, string> headers = null,
        object requestData = null)
        {
            var result = default(T);
            // Default to GET
            var method = httpMethod ?? HttpMethod.Get;
            // Serialize request data
            var data = requestData == null
                ? null
                : JsonConvert.SerializeObject(requestData);
            using (var request = new HttpRequestMessage(method, url))
            {
                // Add request data to request 
                if (data != null)
                {
                    request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                }

                // Add headers to request 
                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        request.Headers.Add(h.Key, h.Value);
                    }
                }

                // Get response
                using (var client = new HttpClient())
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var content = response.Content == null
                        ? null
                        : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        result = JsonConvert.DeserializeObject<T>(content);
                    }
                    else
                    {
                        Debug.WriteLine($"Response was not successful: {response.Content}");
                    }
                }
            }
            return result;
        }
    }
}
