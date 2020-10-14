
//using EstiwTest.Migrations;
using EstiwTest.DB.Model;
using EstiwTest.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EstiwTest.DB
{
    public class TradeContext : DbContext
    {
        public TradeContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TradeContext, Configuration>());

        }

        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<Products> Products { get; set; }

        public static ItemCollection<Customers> GetCustomers()
        {
            ItemCollection<Customers> result;

            using (var db = new TradeContext())
            {
                var c = from b in db.Customers
                        select b;
                db.Customers.Load();


                result = new ItemCollection<Customers>(from t in db.Customers.Local
                                                      join p in db.Products on t.Id equals p.CustomersId into tp
                                                      from subt in tp.DefaultIfEmpty()
                                                          //group t by t.FirstName, t.
                                                      select new Customers()
                                                      {
                                                          Id = t.Id,
                                                          FirstName = t.FirstName,
                                                          LastName = t.LastName,
                                                          Phone = t.Phone,
                                                          Address = t.Address,
                                                          ProductCount = (from z in db.Products
                                                                          where z.CustomersId == t.Id
                                                                          select (int?)z.Count).Sum() ?? 0
                                                      });
                //считает количество товаров без загрузки самих товаров
                //result.ForEach(x => GetProductCount(x));

            }
            return result;
        }
        public static ItemCollection<Customers> GetCustomers(string search, string type)
        {
            ItemCollection<Customers> result;

            using (var db = new TradeContext())
            {
                var c = from b in db.Customers
                        select b;
                db.Customers.Load();


                result = new ItemCollection<Customers>(from t in db.Customers.Local
                                                      join p in db.Products on t.Id equals p.CustomersId into tp
                                                      where (type == "Имя" && t.FirstName.Contains(search)) || (type == "Фамилия" && t.LastName.Contains(search)) || (type == "Адрес" && t.Address.Contains(search)) || (type == "Телефон" && t.Phone.Contains(search))
                                                      from subt in tp.DefaultIfEmpty()
                                                      select new Customers()
                                                      {
                                                          Id = t.Id,
                                                          FirstName = t.FirstName,
                                                          LastName = t.LastName,
                                                          Phone = t.Phone,
                                                          Address = t.Address,
                                                          ProductCount = (from z in db.Products
                                                                          where z.CustomersId == t.Id
                                                                          select (int?)z.Count).Sum() ?? 0
                                                      });


            }
            return result;
        }

        public static ItemCollection<Products> GetProducts(int id)
        {
            ItemCollection<Products> result;
            using (var db = new TradeContext())
            {

                db.Products.Where(x => x.CustomersId == id).Load();
                result = new ItemCollection<Products>(db.Products.Local);

            }
            return result;
        }
        public static ItemCollection<Products> GetProducts(string search, string type)
        {
            ItemCollection<Products> result;
            using (var db = new TradeContext())
            {

                db.Products.Where(x => (type == "Товар" && x.Name.Contains(search))).Load();
                result = new ItemCollection<Products>(db.Products.Local);

            }
            return result;
        }

        public static int GetProductCount(Customers customer)
        {
            using (var db = new TradeContext())
            {
                var item = db.Customers.Find(customer.Id);
                var x = db.Entry(item)
                           .Collection(b => b.Products)
                           .Query()
                           .Count();
                var c = db.Entry(item)
                           .Collection(b => b.Products)
                           .Query()
                           .Sum(z => z.Count);
                return x + c;

            }
            return 0;
        }
        public static bool SaveCustomers(IEnumerable<Customers> itemsToSave, Customers itemToDelete)
        {
            try
            {
                using (var db = new TradeContext())
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
        public static bool SaveProducts(IEnumerable<Products> itemsToSave, Products itemToDelete)
        {
            try
            {
                using (var db = new TradeContext())
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
    }
}
