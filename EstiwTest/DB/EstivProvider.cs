using EstiwTest.DB.Model;

using Lx.Data.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EstiwTest.DB
{

    public class EstivProvider
    {

        public static ItemCollection<Customer> GetCustomers()
        {
            ItemCollection<Customer> result;

            using (var db = new EstiwDBContainer())
            {
                var c = from b in db.Customers
                        select b;
                db.Customers.Load();
             

                result =new ItemCollection<Customer>( from t in db.Customers.Local
                          join p in db.Products on t.Id equals p.CustomerId into tp
                          from subt in tp.DefaultIfEmpty()
                         //group t by t.FirstName, t.
                         select new Customer()
                         {
                             Id = t.Id,
                             FirstName = t.FirstName,
                             LastName = t.LastName,
                             Phone = t.Phone,
                             Address = t.Address,
                             ProductCount = (from z in db.Products
                                             where z.CustomerId == t.Id
                                             select (int?)z.Count).Sum() ?? 0
                         });
                //считает количество товаров без загрузки самих товаров
                //result.ForEach(x => GetProductCount(x));

            }
            return result;
        }
        public static ItemCollection<Customer> GetCustomers(string search,string type)
        {
            ItemCollection<Customer> result;

            using (var db = new EstiwDBContainer())
            {
                var c = from b in db.Customers
                        select b;
                db.Customers.Load();


                result = new ItemCollection<Customer>(from t in db.Customers.Local
                                                      join p in db.Products on t.Id equals p.CustomerId into tp
                                                      //where (type=="Имя"&& t.FirstName.Contains(search))|| (type == "Имя" && t.FirstName.Contains(search)) || (type == "Имя" && t.FirstName.Contains(search)) || (type == "Имя" && t.FirstName.Contains(search)) ||
                                                      from subt in tp.DefaultIfEmpty()
                                                      select new Customer()
                                                      {
                                                          Id = t.Id,
                                                          FirstName = t.FirstName,
                                                          LastName = t.LastName,
                                                          Phone = t.Phone,
                                                          Address = t.Address,
                                                          ProductCount = (from z in db.Products
                                                                          where z.CustomerId == t.Id
                                                                          select (int?)z.Count).Sum() ?? 0
                                                      });


            }
            return result;
        }

        public static ItemCollection<Product> GetProducts(int id)
        {
            ItemCollection<Product> result;
            using (var db = new EstiwDBContainer())
            {

                db.Products.Where(x => x.CustomerId == id).Load();
                result = new ItemCollection<Product>(db.Products.Local);

            }
            return result;
        }

        public static int GetProductCount(Customer customer)
        {
            using (var db = new EstiwDBContainer())
            {
                var item = db.Customers.Find(customer.Id);
                var x = db.Entry(item)
                           .Collection(b => b.Product)
                           .Query()
                           .Count();
                var c = db.Entry(item)
                           .Collection(b => b.Product)
                           .Query()
                           .Sum(z => z.Count);
                return x + c;

            }
            return 0;
        }
        public static bool SaveCustomers(IEnumerable<Customer> itemsToSave, Customer itemToDelete)
        {
            try
            {
                using (var db = new EstiwDBContainer())
                {
                    if (itemsToSave != null)
                        foreach (var t in itemsToSave)
                        {
                            db.Customers.AddOrUpdate(t);
                        }
                    if (itemToDelete != null)
                    {
                        db.Entry(itemToDelete).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                    //db.Customers.Remove(itemToDelete);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }
        public static bool SaveProducts(IEnumerable<Product> itemsToSave, Product itemToDelete)
        {
            try
            {
                using (var db = new EstiwDBContainer())
                {
                    if (itemsToSave != null)
                        foreach (var t in itemsToSave)
                        {
                            db.Products.AddOrUpdate(t);
                        }
                    if (itemToDelete != null)
                    {
                        db.Entry(itemToDelete).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                    //db.Customers.Remove(itemToDelete);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        //private class EstiwContext : DbContext
        //{
        //    public EstiwContext()
        //       : base("EstiwDBContainer")
        //    { }
        //    public DbSet<Customer> Customers { get; set; }
        //    public DbSet<Product> Products { get; set; }

        //}
    }
}
