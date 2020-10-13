using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstiwTest.DB
{
    public partial class Product : ItemBase
    {
        partial void Initialize()
        {
            AddValidationRule(nameof(Name), () => !string.IsNullOrWhiteSpace(Name), "Введите название товара.");
            //AddValidationRule(nameof(Price), () => , "Введите фамилию.");
            AddValidationRule(nameof(Count), () => Count>0, "Количество товара должно быть больше 0.");
            
        }
        public Product()
        {
            Initialize();
        }
    }
}
