using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Split.Entities;
using Split.Helpers;
using Split.Models;
using Split.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Split.ViewModels
{
    

    public class BuildEventPageViewModel : BaseViewModel
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly IPageDialogService _pageDialogService;


        private Event _event;
        public Event Event
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }

        DateTime _eventStartDate;
        public DateTime EventStartDate
        {
            get => _eventStartDate;
            set
            {
                if (_eventStartDate != value && value != null)
                {
                    _eventStartDate = value;
                    UpdateEndDate();

                    RaisePropertyChanged();
                }

            }
        }

        DateTime _eventEndDate;
        public DateTime EventEndDate
        {
            get => _eventEndDate;
            set
            {
                if (_eventEndDate != value && value != null)
                {
                    _eventEndDate = value;
                    UpdateStartDate();

                    RaisePropertyChanged();
                }
            }
        }

        private TimeSpan _eventSpan;
        public TimeSpan EventSpan
        {
            get => _eventSpan;
            set => SetProperty(ref _eventSpan, value);
        }

        private ObservableCollection<HostUserModel> _guests;
        public ObservableCollection<HostUserModel> Guests
        {
            get => _guests;
            set => SetProperty(ref _guests, value);
        }

        private ObservableCollection<User> _hosts;
        public ObservableCollection<User> Hosts
        {
            get => _hosts;
            set => SetProperty(ref _hosts, value);
        }

        private EntryModel _entityName = new EntryModel(ValidateName, "Please enter a name");
        public EntryModel EntityName
        {
            get => _entityName;
            set => SetProperty(ref _entityName, value);
        }


        private Dictionary<string, TransactionStrategy> _transactionStrategies;
        public List<string> TransactionStrategies => _transactionStrategies?.Keys.ToList();

        private TransactionStrategy _transactionStrategySelected;
        private int _selectedTransactionStrategyIndex;
        public int SelectedTransactionStrategyIndex
        {
            get => _selectedTransactionStrategyIndex;
            set
            {
                if (value >= 0)
                {
                    _selectedTransactionStrategyIndex = value;
                    RaisePropertyChanged();

                    _transactionStrategies.TryGetValue(TransactionStrategies[value], out _transactionStrategySelected);
                    Event.TransactionStrategy = _transactionStrategySelected;
                }
            }
        }


        public DelegateCommand AddGuestsFromFriendsCommand { get; set; }
        public DelegateCommand AddGuestCommand { get; set; }
        public DelegateCommand<object> AddToFriendsCommand { get; set; }
        public DelegateCommand<object> AddHostCommand { get; set; }

        public DelegateCommand<object> DeleteGuestCommand { get; set; }
        public DelegateCommand<object> RemoveHostCommand { get; set; }
        public DelegateCommand<object> ToggleHostCommand { get; set; }

        public DelegateCommand BuildEventCommand { get; set; }

        public BuildEventPageViewModel(
            IBaseViewModelServiceProvider baseServiceProvider,
            IPageDialogService pageDialogService,
            IEventService eventService,
            IUserService userService)
            : base(baseServiceProvider)
        {
            _eventService = eventService;
            _userService = userService;
            _pageDialogService = pageDialogService;

            Initialize();
        }

        private void Initialize()
        {
            Title = "Create Event";
            EventStartDate = DateTime.Now;

            Guests = new ObservableCollection<HostUserModel>();
            Hosts = new ObservableCollection<User>();


            AddGuestCommand = new DelegateCommand(() =>
            {
                if (!EntityName.IsValidEntry)
                {
                    EntityName.ShowErrorMessage = true;
                    return;
                }

                AddGuest(new HostUserModel(EntityName.Entry));
            })
            .ObservesProperty(() => IsBusy);

            AddHostCommand = new DelegateCommand<object>((obj) =>
                     AddHost((HostUserModel)obj))
                    .ObservesProperty(() => IsBusy);

            DeleteGuestCommand = new DelegateCommand<object>((obj) => RemoveGuest((HostUserModel)obj));

            RemoveHostCommand = new DelegateCommand<object>((obj) => RemoveHost((HostUserModel)obj));

            BuildEventCommand = new DelegateCommand(() =>
            {
                if (!CanSaveEvent())
                {
                    _pageDialogService.DisplayAlertAsync("Missing Something", "Title and atleast one personal is required to continue.", "Ok");
                    return;
                }
                    

                if (IsNew)
                    CreateEventAsync();
                else
                    UpdateEventAsync();

            })
            .ObservesProperty(() => IsBusy);

            ToggleHostCommand = new DelegateCommand<object>((obj) => ToggleHost((HostUserModel)obj));

            AddGuestsFromFriendsCommand = new DelegateCommand(async () =>
            {
                if (IsBusy)
                    return;

                using (Busy())
                {
                    await _pageDialogService.DisplayAlertAsync("Coming Soon", "Sorry, This feature is not available at the moment.", "Close");
                }
            });

        }

        protected override async Task RefreshAsync()
        {
            using (Busy())
            {
                await base.RefreshAsync();

                EventStartDate = Event.StartDate;
                EventEndDate = Event.EndDate;

                Hosts = new ObservableCollection<User>(Event.Hosts);
                Guests = new ObservableCollection<HostUserModel>();

                var guests = new List<HostUserModel>();

                foreach (var guest in Event.Guests)
                {
                    guests.Add(new HostUserModel(guest));
                }

                foreach (var host in Hosts)
                {
                   Hosts.ForEach(h =>
                   {
                       guests.FirstOrDefault(g => g.GUID == h.GUID).IsHost = true;
                   });
                }

                Guests = new ObservableCollection<HostUserModel>(guests.OrderByDescending(g => g.IsHost).ThenBy(g => g.Name));
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            var hasEvent = parameters.TryGetValue("Event", out Event evnt);

            if (_transactionStrategies == null)
            {
                _transactionStrategies = _eventService?.GetTransactionStrategies();
                RaisePropertyChanged(nameof(TransactionStrategies));
            }

            if (hasEvent)
            {
                IsNew = false;

                Event = evnt;

                var key = _transactionStrategies.FirstOrDefault(x => x.Value == Event.TransactionStrategy).Key;
                SelectedTransactionStrategyIndex = TransactionStrategies.IndexOf(key);

                await RefreshAsync();
            }
            else
            {
                IsNew = true;

                Event = new Event();
                SelectedTransactionStrategyIndex = 0;

            }
        }

        protected override async Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = null)
        {
            if (parameters == null)
                parameters = new NavigationParameters();

            parameters.Add("Event", Event);

            await base.GoBackAsync(parameters);
        }

        private void ToggleHost(HostUserModel user)
        {
            user.IsHost = !user.IsHost;

            if(!user.IsHost)
            {
                RemoveHost(user);
            }
        }

        private async void RemoveGuest(HostUserModel user)
        {
            Guests.Remove(user);
            Hosts.Remove(user);

            await _userService.DeleteUserAsync(user);
        }

        private void RemoveHost(HostUserModel user)
        {
            if (Hosts.Count > 1)
                Hosts.Remove(user);
        }

        private bool CanSaveEvent()
        {
            if (Guests.Count < 1)
                return false;

            if (Hosts.Count < 1)
                return false;

            if (String.IsNullOrWhiteSpace(Event.Title))
                return false;

            return true;
        }

        private async void CreateEventAsync()
        {
            using (Busy())
            {
                UpdateEventEntity();

                await _eventService.AddEventAsync(Event);

                await GoBackAsync();
            }
        }

        private async void UpdateEventAsync()
        {
            using (Busy())
            {
                UpdateHosts();
                UpdateEventEntity();

                await _eventService.UpdateEventAsync(Event);

                await GoBackAsync();
            }
        }

        private void AddGuest(HostUserModel user)
        {
            Guests.Add(user);
            EntityName.Entry = string.Empty;
            EntityName.ShowErrorMessage = false;

            if (Hosts.Count <= 0)
            {
                AddHost(user);
            }
        }

        private void AddHost(HostUserModel user)
        {
            Hosts.Add(user);
            user.IsHost = true;
        }


        private TimeSpan CalculateTimeSpan(DateTime from, DateTime to)
        {
            if (to.Date < from.Date)
                return new TimeSpan();

            return to - from;
        }

        private void UpdateEndDate()
        {
            if (EventEndDate < EventStartDate)
            {
                EventEndDate = EventStartDate;
            }

            EventSpan = CalculateTimeSpan(EventStartDate.Date, EventEndDate.Date);
        }

        private void UpdateStartDate()
        {
            if (EventStartDate > EventEndDate)
            {
                EventStartDate = EventEndDate;
            }

            EventSpan = CalculateTimeSpan(EventStartDate.Date, EventEndDate.Date);
        }

        private void UpdateEventEntity()
        {
            Event.StartDate = EventStartDate;
            Event.EndDate = EventEndDate;

            Event.Hosts = Hosts.OrderBy(g => g.Name).ToList();
            Event.Guests = HostUserModelsToUsers(Guests)
                                .OrderBy(g => g.Name)
                                .ToList();

            //Event.Guests = Event.Guests.OrderByDescending(g => g.Name).ToList();

            Event.TransactionStrategy = _transactionStrategySelected;

            Event.Expenses = ExpenseHelper.UpdateCostAllocations(Event.Guests, Event.Expenses);

        }

        private void UpdateHosts()
        {
            Hosts = new ObservableCollection<User>();
            Guests.ForEach(g =>
            {
                if (g.IsHost)
                    Hosts.Add(g);
            });
        }

        private static Func<string, bool> ValidateName = entry =>
        {
            return !string.IsNullOrWhiteSpace(entry);
        };

        private static Func<ICollection<HostUserModel>, ICollection<User>> HostUserModelsToUsers = hostUsers =>
        {
           var users = new Collection<User>();

           foreach (var hostUser in hostUsers)
           {
               users.Add(new HostUserModel(hostUser));
           }

           return users;
        };
    }


}
