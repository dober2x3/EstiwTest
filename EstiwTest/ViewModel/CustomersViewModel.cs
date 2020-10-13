using EstiwTest.DB;
using EstiwTest.DB.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

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

    public class CustomersViewModel : ViewModelBase
    {
       

        private ItemCollection<Customer> customers = new ItemCollection<Customer>();
        private Customer currentCustomer;

        public Customer CurrentCustomer
        {
            get => currentCustomer;
            set
            {
                Set(ref currentCustomer, value);
                SaveCommand.RaiseCanExecuteChanged();
                DelCommand.RaiseCanExecuteChanged();
            }
        }
        public ItemCollection<Customer> Customers
        {
            get => customers;
            set
            {
                Set(ref customers, value);
            }
        }
        public string CurrentSearch { get; set; }
        public string SearchText { get; set; }
        public CollectionView CustomersView { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DelCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand OpenProductsCommand { get; set; }
        public CustomersViewModel()
        {


            RefreshCommand = new RelayCommand(Refresh);
            SaveCommand = new RelayCommand(Save, () => { return Customers == null ? false : (Customers.Count(x => x.IsChanged) > 0 && Customers.CollectionIsValid); });
            DelCommand = new RelayCommand(Del, () => { return CurrentCustomer != null; });
            SearchCommand = new RelayCommand(Search);
            OpenProductsCommand = new RelayCommand(OpenProducts);
            Refresh();

        }

        private void Search()
        {
            if (Customers != null)
                Customers.OnCollectionChangeStateEvent -= Customers_PropertyChanged;
            Customers = EstivProvider.GetCustomers(SearchText,CurrentSearch);
            if (Customers != null)
                Customers.OnCollectionChangeStateEvent += Customers_PropertyChanged;
            Customers.ForEach(x => x.AcceptChanges());
            CustomersView = (CollectionView)CollectionViewSource.GetDefaultView(Customers);
        }

        private void Customers_PropertyChanged(object sender, bool IsValid, bool IsChanged)
        {
            if (IsValid || IsChanged)
                SaveCommand.RaiseCanExecuteChanged();

        }

        private void OpenProducts()
        {
            Messenger.Default.Send(new OpenProductsMessage { CurrentCustomer = CurrentCustomer });
        }

        public void Save()
        {

            EstivProvider.SaveCustomers(Customers.Where(x => x.IsChanged), null);
            Customers.ForEach(x => x.AcceptChanges());
            SaveCommand.RaiseCanExecuteChanged();

        }

        public void Del()
        {
            if (MessageBox.Show("Удилить " + CurrentCustomer.FirstName, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (EstivProvider.SaveCustomers(null, CurrentCustomer))
                {
                    Customers.Remove(CurrentCustomer);
                    CurrentCustomer = null;
                }
            }
            Customers.ForEach(x => x.AcceptChanges());

        }

        public void Refresh()
        {
            if (Customers != null)
                Customers.OnCollectionChangeStateEvent -= Customers_PropertyChanged;
            Customers = EstivProvider.GetCustomers();
            if (Customers != null)
                Customers.OnCollectionChangeStateEvent += Customers_PropertyChanged;
            Customers.ForEach(x => x.AcceptChanges());
            CustomersView = (CollectionView)CollectionViewSource.GetDefaultView(Customers);
        }

        public void RefreshProducts()
        {
            CurrentCustomer.ProductCount = EstivProvider.GetProductCount(CurrentCustomer);
        }
    }

}