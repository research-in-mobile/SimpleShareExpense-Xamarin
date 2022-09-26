using Split.ViewModels;
using Xamarin.Forms;

namespace Split.Views
{
	public partial class HubPage : ContentPage
	{
		private HubPageViewModel _vm;

		public HubPage()
		{
			InitializeComponent();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			_vm = (HubPageViewModel)this.BindingContext;
		}

		private void Event_Tapped(object sender, System.EventArgs e)
		{
			var obj = (BindableObject)sender;
			_vm.EventSelectedCommand.Execute(obj.BindingContext);
		}

		private void DeleteButtonClicked(object sender, System.EventArgs e)
		{
			var obj = (BindableObject)sender;
			_vm.DeleteEventCommand.Execute(obj.BindingContext);
		}
	}
}
