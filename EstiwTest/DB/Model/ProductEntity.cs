using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstiwTest.DB
{
    public class Products:ItemBase
    {
        //[Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        
        public int CustomersId { get; set; }

        [ForeignKey("CustomersId")]
        public Customers Customers { get; set; }

        void Initialize()
        {
            AddValidationRule(nameof(Name), () => !string.IsNullOrWhiteSpace(Name), "Введите название товара.");
            AddValidationRule(nameof(Count), () => Count > 0, "Количество товара должно быть больше 0.");

        }
        public Products()
        {
            Initialize();
        }
    }
}
