using Split.ViewModels;
using Xamarin.Forms;

namespace Split.Views
{
    public partial class EventPage : ContentPage
    {
		private EventPageViewModel _vm;
        public EventPage()
        {
            InitializeComponent();
        }

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			_vm = (EventPageViewModel)this.BindingContext;
		}

		private void DeleteExpenseButtonClicked(object sender, System.EventArgs e)
		{
			var obj = (BindableObject)sender;
			_vm.DeleteExpenseCommand.Execute(obj.BindingContext);
		}
	}
}
