using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using WPF曲线图.ViewModel;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows.Threading;

namespace WPF曲线图
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window//, INotifyPropertyChanged
    {
        private ObservableCollection<Point> _pointSource = new ObservableCollection<Point>();
        NameViewModel _nameViewModel;


        public MainWindow()
        {
            InitializeComponent();
            _nameViewModel = base.DataContext as NameViewModel;

            
            DataContext = this;

        }
        #region 曲线图
        public ObservableCollection<Point> PointSource
        {
            get { return _pointSource; }
            set
            {
                _pointSource = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PointSource"));
                }
            }
        }
        Random rand = new Random(DateTime.Now.Millisecond);
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var ptx = rand.Next(1, 300);
            var pty = rand.Next(1, 300);
            PointSource.Add((new Point(ptx, pty)));
        }
        #endregion

        #region
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            _boolThread = true;
            _nameViewModel.UserName = "测试";
            _nameViewModel.CompanyName = "格致";
            
        }
        private Thread _thread;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //_thread = new Thread(show);
            //_thread.Start();
        }
        public bool _boolThread = false;
        public void show()
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    if (_boolThread)
                    {
                        return;
                    }
                    Dispatcher.Invoke(new Action(() =>
                    {
                        _nameViewModel.UserName = "测试" + i;
                        _nameViewModel.CompanyName = "格致" + i;
                    }));
                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {

                
            }
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            _boolThread = false;
            _thread = new Thread(show);
            _thread.Start();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion



    }
}
