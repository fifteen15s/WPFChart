using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF曲线图.Model
{
    public class Name
    {
        string _userName;
        string _companyName;


        public string UserName { get { return _userName; } set { _userName = value; } }

        public string CompanyName { get { return _companyName; } set { _companyName = value; }
        }
    }
}
