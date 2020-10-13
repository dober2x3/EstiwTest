using EstiwTest.DB;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EstiwTest.DB
{



    public partial class Customer : ItemBase, IEditableObject
    {


        partial void Initialize()
        {
            AddValidationRule(nameof(FirstName), () => !string.IsNullOrWhiteSpace(FirstName), "Введите имя.");
            AddValidationRule(nameof(LastName), () => !string.IsNullOrWhiteSpace(LastName), "Введите фамилию.");
            AddValidationRule(nameof(Phone), () =>
            {
                if (!string.IsNullOrWhiteSpace(Phone))
                    return true;
                if (Phone!=null&&Regex.Match(Phone, @"^[0-9\-\+]{9,15}$").Success)
                    return true;
                return false;
            }, "Введите телефон.");
            AddValidationRule(nameof(Address), () => !string.IsNullOrWhiteSpace(Address), "Введите адрес.");
        }

        public string FullName { get { return FirstName + " " + LastName; } }
        public int ProductCount { get; set; }



        bool inEdit = false;
        Customer item;
        void IEditableObject.BeginEdit()
        {
            if (inEdit)
                return;

            inEdit = true;
            item = this;
        }

        void IEditableObject.EndEdit()
        {
            if (!inEdit)
                return;

            inEdit = false;
            item = null;
        }

        void IEditableObject.CancelEdit()
        {
            if (!inEdit)
                return;

            inEdit = false;
            this.FirstName = item.FirstName;
            this.LastName = item.LastName;
            this.Phone = item.Phone;
            this.Address = item.Address;
        }
    }
}
