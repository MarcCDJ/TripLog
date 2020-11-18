using System.Collections.Generic;
using System.Threading.Tasks;
using TripLog.Models;

namespace TripLog.Services
{
    public interface ITripLogApiDataService
    {
        Task<IList<TripLogEntry>> GetEntriesAsync();
        Task<TripLogEntry> AddEntryAsync(TripLogEntry entry);
    }
}
