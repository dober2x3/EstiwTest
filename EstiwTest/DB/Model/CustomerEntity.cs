using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EstiwTest.DB
{
    public class Customers:ItemBase
    {
        //[Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string FirstName { get; set; }
        [MaxLength(200)]
        public string LastName { get; set; }
        [MaxLength(12)]
        public string Phone { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }

        //[ForeignKey("CustomerId")]
        public ObservableCollection<Products> Products { get; set; }


        void Initialize()
        {
            AddValidationRule(nameof(FirstName), () => !string.IsNullOrWhiteSpace(FirstName), "Введите имя.");
            AddValidationRule(nameof(LastName), () => !string.IsNullOrWhiteSpace(LastName), "Введите фамилию.");
            AddValidationRule(nameof(Phone), () =>
            {
                if (!string.IsNullOrWhiteSpace(Phone))
                    return true;
                if (Phone != null && Regex.Match(Phone, @"^[0-9\-\+]{9,15}$").Success)
                    return true;
                return false;
            }, "Введите телефон.");
            AddValidationRule(nameof(Address), () => !string.IsNullOrWhiteSpace(Address), "Введите адрес.");
        }
        [NotMapped]
        public string FullName { get { return FirstName + " " + LastName; } }
        [NotMapped]
        public int ProductCount { get; set; }

        public Customers()
        {
            Initialize();
        }
    }
}
