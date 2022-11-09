using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF曲线图.Model;

namespace WPF曲线图.ViewModel
{
    public class NameViewModel: INotifyPropertyChanged
    {
        public NameViewModel()
        {
            _userName = new Name() { UserName = "清晨的粥", CompanyName = "GZ" };
        }
        Name _userName;

        

        public string UserName { get { return _userName.UserName; } set { _userName.UserName = value;RaisePropertyChanged("UserName"); } }

        public string CompanyName { get { return _userName.CompanyName; } set { _userName.CompanyName = value; RaisePropertyChanged("CompanyName"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
