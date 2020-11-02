using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
            _refreshCommand ?? (_refreshCommand = new Command(LoadEntries));

        public override void Init()
        {
            LoadEntries();
        }

        readonly ITripLogDataService _tripLogService;

        public MainViewModel(INavService navService, ITripLogDataService tripLogService)
            : base(navService)
        {
            _tripLogService = tripLogService;
            LogEntries = new ObservableCollection<TripLogEntry>();
        }

        async void LoadEntries()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                var entries = await _tripLogService.GetEntriesAsync().ConfigureAwait(false);
                LogEntries = new ObservableCollection<TripLogEntry>(entries);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
