using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WPFDevelopers.Charts.Models;

namespace 扇形图.SampleViews
{
    /// <summary>
    /// ArcChartSampleView.xaml 的交互逻辑
    /// </summary>
    public partial class ArcChartSampleView : UserControl
    {
        /// <summary>
        /// ArcChart
        /// </summary>
        public ObservableCollection<PieSerise> ItemsSource
        {
            get { return (ObservableCollection<PieSerise>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<PieSerise>), typeof(ArcChartSampleView), new PropertyMetadata(null));
        public ArcChartSampleView()
        {
            InitializeComponent();
            Loaded += ArcChartSampleView_Loaded;
        }

        private void ArcChartSampleView_Loaded(object sender, RoutedEventArgs e)
        {
            ItemsSource = new ObservableCollection<PieSerise>();
            var collection1 = new ObservableCollection<PieSerise>();
            // Percentage 总和不能超过100
            collection1.Add(new PieSerise
            {
                Title = "氧气",
                Percentage = 10,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5B9BD5")),
            });
            collection1.Add(
                new PieSerise
                {
                    Title = "二氧化碳",
                    Percentage = 10,
                    PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4472C4")),
                });

            collection1.Add(new PieSerise
            {
                Title = "氮气",
                Percentage = 30,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007fff")),
            });

            collection1.Add(new PieSerise
            {
                Title = "水蒸气",
                Percentage = 20,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ED7D31")),
            });
            collection1.Add(new PieSerise
            {
                Title = "稀有气体",
                Percentage = 10,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC000")),
            });

            collection1.Add(new PieSerise
            {
                Title = "氯气",
                Percentage = 20,
                PieColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff033e")),
            });
            ItemsSource = collection1;
        }
    }
}
