using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using Akavache;
using TripLog.Models;
using TripLog.Services;
using Xamarin.Forms;

namespace TripLog.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        ObservableCollection<TripLogEntry> _logEntries;
        public ObservableCollection<TripLogEntry> LogEntries
        {
            get => _logEntries;
            set
            {
                _logEntries = value;
                OnPropertyChanged();
            }
        }

        public Command<TripLogEntry> ViewCommand =>
            new Command<TripLogEntry>(async entry =>
                await NavService.NavigateTo<DetailViewModel, TripLogEntry>(entry).ConfigureAwait(false));

        public Command NewCommand =>
            new Command(async () =>
                await NavService.NavigateTo<NewEntryViewModel>().ConfigureAwait(false));

        Command _refreshCommand;
        public Command RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new Command(RefreshEntries));

        public Command SignOutCommand =>
            new Command(async () =>
                await _authService.SignOutAsync().ConfigureAwait(false));

        public override void Init()
        {
            LoadEntries();
            Debug.WriteLine("Finished loading entries: " + LogEntries.Count);
        }

        readonly ITripLogApiDataService _tripLogService;
        readonly IAuthService _authService;
        readonly IBlobCache _cache;

        public MainViewModel(INavService navService, IAuthService authService, ITripLogApiDataService tripLogService, IBlobCache cache)
            : base(navService)
        {
            _tripLogService = tripLogService;
            _authService = authService;
            _cache = cache;
            LogEntries = new ObservableCollection<TripLogEntry>();
        }

        void RefreshEntries()
        {
            _cache.InvalidateAll();
            _cache.Vacuum();
            LoadEntries();
        }

        void LoadEntries()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                // Load from local cache and then immediately load from API
                _cache.GetAndFetchLatest("entries", async ()
                        => await _tripLogService.GetEntriesAsync().ConfigureAwait(false)
                    )
                    .Subscribe(entries =>
                    {
                        Debug.WriteLine("*** Subscription ran! ***");
                        Debug.WriteLine("*** Entries returned: " + entries?.Count + " ***");
                        LogEntries = new ObservableCollection<TripLogEntry>(entries);
                        IsBusy = false;
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading entries: " + ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
