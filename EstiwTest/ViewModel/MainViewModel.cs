using EstiwTest.DB;
using GalaSoft.MvvmLight;
using System;

namespace EstiwTest.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public CustomersViewModel CustomersViewModel { get; set; }
        public object CurrentViewModel { get; set; }

        public MainViewModel()
        {
            CustomersViewModel = new CustomersViewModel();
            CurrentViewModel = CustomersViewModel;
            //System.Action<OpenProductsMessage> HandleOpenProductsMessage = () => { };
            MessengerInstance.Register<OpenProductsMessage>(this, HandleOpenProductsMessage);
            MessengerInstance.Register<BackMessage>(this, HandleBackMessage);
            /*            */
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        private void HandleBackMessage(BackMessage obj)
        {
            CurrentViewModel = CustomersViewModel;
            CustomersViewModel.RefreshProducts();
        }

        private void HandleOpenProductsMessage(OpenProductsMessage obj)
        {
            CurrentViewModel = new ProductsViewModel(obj.CurrentCustomer);
        }
    }
}