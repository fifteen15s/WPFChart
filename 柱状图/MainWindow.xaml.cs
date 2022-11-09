using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 柱状图
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigationWindow navigationWindow = new NavigationWindow();
            navigationWindow.Source = new Uri("BasicBarChartExample.xaml", UriKind.Relative); 
            navigationWindow.Show();
            this.Close();
            //NavigationService.GetNavigationService(this).Navigate(new Uri("BasicBarChartExample.xaml",UriKind.Relative));
        }
    }
}
