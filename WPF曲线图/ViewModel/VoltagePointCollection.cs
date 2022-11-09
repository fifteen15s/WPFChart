using Microsoft.Research.DynamicDataDisplay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF曲线图.ViewModel
{
    public class VoltagePointCollection : RingArray<VoltagePoint>
    {
        private const int TOTAL_POINTS = 3000;
        public VoltagePointCollection() : base(TOTAL_POINTS)//这里我设置了要显示多少值
        {
        }
    }

    public class VoltagePoint
    {
        public DateTime Date { get; set; }

        public double Voltage { get; set; }

        public VoltagePoint(DateTime date, double voltage)
        {
            Date = date;
            Voltage = voltage;
        }
    }
}
