using EstiwTest.DB;
using EstiwTest.DB.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
    public class ProductsViewModel : ViewModelBase
    {
        private ItemCollection<Products> products;
        private Products currentProduct;
        private Customers customer;
        public RelayCommand SearchCommand { get; set; }
        public Products CurrentProduct
        {
            get => currentProduct;
            set
            {
                Set(ref currentProduct, value);
                SaveCommand.RaiseCanExecuteChanged();
                DelCommand.RaiseCanExecuteChanged();
            }
        }
        public Customers Customer { get => customer; set => Set(ref customer, value); }
        public ItemCollection<Products> Products
        {
            get => products;
            set
            {
                Set(ref products, value);
            }
        }
        public string CurrentSearch { get; set; }
        public string SearchText { get; set; }
        public CollectionView ProductsView { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DelCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public ProductsViewModel(Customers customer)
        {
            Customer = customer;
            
            RefreshCommand = new RelayCommand(Refresh);
            BackCommand = new RelayCommand(Back);
            SaveCommand = new RelayCommand(Save, () => { return Products == null ? false : (Products.Count(x => x.IsChanged) > 0 && Products.CollectionIsValid); });
            DelCommand = new RelayCommand(Del, () => { return CurrentProduct != null; });
            SearchCommand = new RelayCommand(Search);
            Refresh();


        }
        private void Search()
        {
            if (Products != null)
                Products.OnCollectionChangeStateEvent -= Customers_PropertyChanged;
            Products = TradeContext.GetProducts(SearchText, CurrentSearch);
            if (Products != null)
                Products.OnCollectionChangeStateEvent += Customers_PropertyChanged;
            Products.ForEach(x => x.AcceptChanges());
            ProductsView = (CollectionView)CollectionViewSource.GetDefaultView(Products);
        }
        private void Back()
        {
            MessengerInstance.Send(new BackMessage());
        }
        private void Customers_PropertyChanged(object sender, bool IsValid, bool IsChanged)
        {
            if (IsValid || IsChanged)
                SaveCommand.RaiseCanExecuteChanged();

        }
        public void Save()
        {
            Products.Where(x => x.IsChanged).ForEach(z => z.CustomersId = Customer.Id);
            TradeContext.SaveProducts(Products.Where(x => x.IsChanged), null);
            Products.ForEach(x => x.AcceptChanges());
            SaveCommand.RaiseCanExecuteChanged();

        }

        public void Del()
        {
            if (MessageBox.Show("������� " + CurrentProduct.Name, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (TradeContext.SaveProducts(null, CurrentProduct))
                {
                    Products.Remove(CurrentProduct);
                    CurrentProduct = null;
                }
            }
            Products.ForEach(x => x.AcceptChanges());

        }

        public void Refresh()
        {
            if (Products != null)
                Products.OnCollectionChangeStateEvent -= Customers_PropertyChanged;
            Products = TradeContext.GetProducts(Customer.Id);
            if (Products != null)
                Products.OnCollectionChangeStateEvent += Customers_PropertyChanged;

            Products.ForEach(x => x.AcceptChanges());
            ProductsView = (CollectionView)CollectionViewSource.GetDefaultView(Products);
        }
    }
}