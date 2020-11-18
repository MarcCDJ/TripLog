using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using TripLog.Models;

namespace TripLog.Services
{
    public class TripLogApiDataService : BaseHttpService, ITripLogApiDataService
    {
        readonly Uri _baseUri;
        readonly IDictionary<string, string> _headers;

        public TripLogApiDataService(Uri baseUri, string authToken)
        {
            _baseUri = baseUri;
            _headers = new Dictionary<string, string>();
            _headers.Add("Authorization", "Bearer " + authToken);

            // TODO: WORKING HERE
            //Debug.WriteLine(authToken.ToString());
        }

        public async Task<TripLogEntry> AddEntryAsync(TripLogEntry entry)
        {
            var url = new Uri(_baseUri, "/api/entry");
            var response = await SendRequestAsync<TripLogEntry>(url, HttpMethod.Post, _headers,  entry).ConfigureAwait(false);
            return response;
        }

        public async Task<IList<TripLogEntry>> GetEntriesAsync()
        {
            var url = new Uri(_baseUri, "/api/entry");
            var response = await SendRequestAsync<TripLogEntry[]>(url, HttpMethod.Get, _headers).ConfigureAwait(false);
            return response;
        }
    }
}
