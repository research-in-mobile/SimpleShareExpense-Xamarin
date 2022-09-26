using DryIoc;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Split.Helpers;
using Split.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Split.ViewModels
{
	public class BaseViewModel : BindableBase, INavigationAware, IInitializeAsync, IDestructible
	{
		protected readonly INavigationService NavigationService;
		protected readonly IErrorManagementService ErrorManagementService;
		protected readonly ILogService LogService;


		protected CancellationTokenSource CTS = new CancellationTokenSource();
		protected CancellationToken CT { get; }

		private string _title;
		public string Title
		{
			get { return _title; }
			set { SetProperty(ref _title, value); }
		}

		private bool _isNew;
		public bool IsNew
		{
			get => _isNew;
			set => SetProperty(ref _isNew, value);
		}

		private bool _hasChanged;
		public bool HasChanged
		{
			get => _hasChanged;
			set => SetProperty(ref _hasChanged, value);
		}


		public DelegateCommand GoBackCommand { get; set; }
		public DelegateCommand RefreshCommand { get; set; }
		public DelegateCommand<string> NavigateToCommand { get; }

		public BaseViewModel(IBaseViewModelServiceProvider baseViewModelServiceProvider)
		{
			NavigationService = baseViewModelServiceProvider.NavigationService;
			ErrorManagementService = baseViewModelServiceProvider.ErrorManagementService;
			LogService = baseViewModelServiceProvider.LogService;

			CT = CTS.Token;
			IsNew = false;
			HasChanged = false;

			RefreshCommand = new DelegateCommand(() => RefreshAsync());
			GoBackCommand = new DelegateCommand(async () => await GoBackAsync(null));
		}
	

		public virtual Task InitializeAsync(INavigationParameters parameters)
		{
			return Task.CompletedTask;
		}

		public virtual void Destroy()
		{
			CancelAllTasks();
		}

		protected virtual Task RefreshAsync()
		{
			return Task.CompletedTask;
		}

		protected void CancelAllTasks()
		{
			if (!CTS.IsCancellationRequested)
				CTS.Cancel();
		}

		protected async Task TaskLoaderBufferAsync<T>(IEnumerable<Task<T>> tasks, CancellationToken ct)
		{
			List<Task<T>> taskList = tasks.ToList();

			while (taskList.Count > 0)
			{
				Task<T> finishedTask = await Task.WhenAny(taskList);
				taskList.Remove(finishedTask);
			}
		}

		#region Run Safe
		protected virtual void OnError(Exception ex)
		{
			switch (ex)
			{
				case NavigationException _ when ex.InnerException is ContainerException containerException:
					ErrorManagementService.HandleError(containerException);
					break;
				//case InvalidCastException thing when thing. //CarouselView.FormsPlugin.Android.CarouselViewRenderer
				case OperationCanceledException _: // NOTE: Suppress these exceptions. Includes TaskCanceledException.
					return;
				default:
					ErrorManagementService.HandleError(ex);
					break;
			}
		}

		protected void RunSafe(Action action) => RunSafe(action, OnError);

		protected void RunSafe(Action action, Action<Exception> handleErrorAction)
		{
			try
			{
				action.Invoke();
			}
			catch (Exception ex)
			{
				handleErrorAction?.Invoke(ex);
			}
		}

		protected Task RunSafeAsync(Func<Task> task) => RunSafeAsync(task, OnError);

		protected async Task RunSafeAsync(Func<Task> task, Action<Exception> handleErrorAction)
		{
			try
			{
				await task().ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				handleErrorAction?.Invoke(ex);
			}
		}

		protected Task<T> RunSafeAsync<T>(Func<Task<T>> task) => RunSafeAsync(task, OnError);

		protected async Task<T> RunSafeAsync<T>(Func<Task<T>> task, Action<Exception> handleErrorAction)
		{
			try
			{
				return await task().ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				handleErrorAction?.Invoke(ex);
			}
			return default(T);
		}
		#endregion

		#region Navigation Base
		public virtual void OnNavigatedFrom(INavigationParameters parameters)
		{

		}

		public virtual void OnNavigatedTo(INavigationParameters parameters)
		{

		}

		protected virtual async Task NavigateAsync(string page, INavigationParameters parameters = null, bool useModalNavigation = false)
		{
			var result = await NavigationService.NavigateAsync(page, parameters, useModalNavigation);
			if (result.Success) return;
			throw result.Exception;
		}

		protected virtual async Task NavigateSafeAsync(string page, INavigationParameters parameters = null, bool useModalNavigation = false)
		{
			if (IsBusy)
				return;

			await RunSafeAsync(async () =>
			{
				using (Busy())
				{
					await NavigateAsync(page, parameters, useModalNavigation);
				}
			});
		}

		protected virtual async Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = null)
		{
			var result = await NavigationService.GoBackAsync(parameters, useModalNavigation);
			if (result.Success) return;
			throw result.Exception;
		}
		#endregion

		#region Busy Mechanism
		private static readonly Guid _defaultTracker = new Guid("A7848922-C10A-4D6A-9D82-4987F638F718");
		private IList<Guid> _busyLocks = new List<Guid>();

		public bool IsBusy
		{
			get => _busyLocks.Any();
			set
			{
				if (value && !_busyLocks.Contains(_defaultTracker))
				{
					_busyLocks.Add(_defaultTracker);
					RaisePropertyChanged(nameof(IsBusy));
				}

				if (!value && _busyLocks.Contains(_defaultTracker))
				{
					_busyLocks.Remove(_defaultTracker);
					RaisePropertyChanged(nameof(IsBusy));
				}
			}
		}

		protected BusyHelper Busy() => new BusyHelper(this);

		private void ForceUnlock()
		{
			_busyLocks = new List<Guid>();
		}

		protected sealed class BusyHelper : IDisposable
		{
			private readonly BaseViewModel _viewModel;
			private readonly Guid _tracker;

			public BusyHelper(BaseViewModel viewModel)
			{
				_viewModel = viewModel;
				_tracker = Guid.NewGuid();
				_viewModel._busyLocks.Add(_tracker);
				_viewModel.RaisePropertyChanged(nameof(IsBusy));
			}

			public void Dispose()
			{
				_viewModel._busyLocks.Remove(_tracker);
				_viewModel.RaisePropertyChanged(nameof(IsBusy));
			}
		}
		#endregion
	}
}
