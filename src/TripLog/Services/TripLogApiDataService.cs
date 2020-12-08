using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using TripLog.Models;
using Xamarin.Essentials;

namespace TripLog.Services
{
    public class TripLogApiDataService : BaseHttpService, ITripLogApiDataService
    {
        readonly Uri _baseUri;
        readonly IDictionary<string, string> _headers;

        public TripLogApiDataService(Uri baseUri)
        {
            _baseUri = baseUri;
            _headers = new Dictionary<string, string>();
        }

        public async Task<TripLogEntry> AddEntryAsync(TripLogEntry entry)
        {
            string savedToken = await SecureStorage.GetAsync("AccessToken").ConfigureAwait(false);
            _headers.TryGetValue("Authorization", out string header);
            if (header is null)
            {
                _headers.Add("Authorization", "Bearer " + savedToken);
            }

            Debug.WriteLine("Now adding an trip log entry.");
            Debug.WriteLine("Using token: " + savedToken);
            var url = new Uri(_baseUri, "/api/entry");
            var response = await SendRequestAsync<TripLogEntry>(url, HttpMethod.Post, _headers,  entry).ConfigureAwait(false);
            return response;
        }

        public async Task<IList<TripLogEntry>> GetEntriesAsync()
        {
            string savedToken = await SecureStorage.GetAsync("AccessToken").ConfigureAwait(false);
            _headers.TryGetValue("Authorization", out string header);
            if (header is null)
            {
                _headers.Add("Authorization", "Bearer " + savedToken);
            }

            Debug.WriteLine("Now getting trip log entries.");
            Debug.WriteLine("Using token: " + savedToken);
            var url = new Uri(_baseUri, "/api/entry");
            var response = await SendRequestAsync<TripLogEntry[]>(url, HttpMethod.Get, _headers).ConfigureAwait(false);
            return response;
        }
    }
}
