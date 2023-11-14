using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareMaster
{
    public partial class Transaction : INotifyPropertyChanged
    {

        public override string ToString()
        {
            return $"Transaction ID: {id}, Item ID: {Item_Id}, Quantity: {Quantity}, Total: {Total:C}, Transaction Date: {Transaction_Date}, User ID: {User_Id}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
