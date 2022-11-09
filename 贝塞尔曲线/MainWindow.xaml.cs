using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace 贝塞尔曲线
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region
        /// <summary>
        /// 画板宽度
        /// </summary>
        double BoardWidth { get; set; }
        /// <summary>
        /// 画板高度
        /// </summary>
        double BoardHeight { get; set; }
        /// <summary>
        /// 垂直（纵向）边距（画图区域距离左右两边长度）
        /// </summary>
        double VerticalMargin { get; set; }
        /// <summary>
        /// 平行（横向）边距（画图区域距离左右两边长度）
        /// </summary>
        double HorizontalMargin { get; set; }
        /// <summary>
        /// 水平刻度间距像素
        /// </summary>
        double horizontalBetween { get; set; }
        /// <summary>
        /// 垂直刻度间距像素
        /// </summary>
        double verticalBetween { get; set; }

        /// <summary>
        ///     x轴最大值
        /// </summary>
        public double MaxX { get; set; }

        /// <summary>
        ///     y轴最大值
        /// </summary>
        public double MaxY { get; set; }

        /// <summary>
        ///     x轴最小值
        /// </summary>
        public double MinX { get; set; }

        /// <summary>
        ///     y轴最小值
        /// </summary>
        public double MinY { get; set; }

        /// <summary>
        /// 图表区域宽度
        /// </summary>
        double ChartWidth;
        /// <summary>
        /// 图表区域高度
        /// </summary>
        double CharHeight;
        /// <summary>
        /// 画图区域起点
        /// </summary>
        Point StartPostion;
        /// <summary>
        /// 画图区域终点
        /// </summary>
        Point EndPostion;
        /// <summary>
        /// X轴画图区域终点
        /// </summary>
        Point XEndPostion;
        /// <summary>
        /// 数据源
        /// </summary>
        PointCollection DataSourse;


        double MapLocationX = 0;
        double MapLocationY = 0;
        //鼠标按下去的位置
        Point startMovePosition;
        TranslateTransform totalTranslate = new TranslateTransform();
        TranslateTransform tempTranslate = new TranslateTransform();
        ScaleTransform totalScale = new ScaleTransform();
        Double scaleLevel = 1;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            DataSourse = GetCollPoint();
        }
        /// <summary>
        /// 双击右键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ClickCount)
            {
                case 2:
                    //this.NavigationService.Refresh();
                    //App.DoEvents();
                    
                    break;
                default:
                    break;
            }
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startMovePosition = e.GetPosition((Canvas)sender);
            
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            /*
            Point endMovePosition = e.GetPosition((Canvas)sender);
 
            totalTranslate.X += (endMovePosition.X - startMovePosition.X) / scaleLevel;
            totalTranslate.Y += (endMovePosition.Y - startMovePosition.Y) / scaleLevel;
            */
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentMousePosition = e.GetPosition((UIElement)sender);
            moushPonit.Content = currentMousePosition.X.ToString() + "," + currentMousePosition.Y.ToString();

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MapLocationX = MapLocationX + currentMousePosition.X - startMovePosition.X;
                MapLocationY = MapLocationY + currentMousePosition.Y - startMovePosition.Y;

                startMovePosition = currentMousePosition;

                Refresh();
                /*
                Point deltaPt = new Point(0, 0);
                deltaPt.X = (currentMousePosition.X - startMovePosition.X) / scaleLevel;
                deltaPt.Y = (currentMousePosition.Y - startMovePosition.Y) / scaleLevel;
 
                tempTranslate.X = totalTranslate.X + deltaPt.X;
                tempTranslate.Y = totalTranslate.Y + deltaPt.Y;
 
                TransformGroup tfGroup = new TransformGroup();
                tfGroup.Children.Add(tempTranslate);
                tfGroup.Children.Add(totalScale);
                foreach (UIElement ue in MainCanvas.Children)
                {
                    ue.RenderTransform = tfGroup;
                }*/
            }

        }

        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            InitCanvas();

            ////获取y最大值
            if (MaxY < 0.0001)
            {
                MaxY = DataSourse.Max(m => m.Y);
            }
            MinY = DataSourse.Min(m => m.Y);
            //MaxY = 200;
            //MinY = -50;
            //MaxX = 200;
            //MinX = -50;
            if (MaxX < 0.0001)
            {
                MaxX = DataSourse.Max(m => m.X);
            }
            MinX = DataSourse.Min(m => m.X);
            if (Math.Abs(MaxX) < 0.000001 || Math.Abs(MaxY) < 0.000001)
            {
                return;
            }

            DrawAxis();
            DrawXAxisTicks();
            DrawYAxisTicks();
            DrawPolyline();
        }
        bool IsInit = true;
        private void InitCanvas()
        {
            MainCanvas.Children.Clear();

            BoardWidth = MainCanvas.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            BoardHeight = MainCanvas.ActualHeight - SystemParameters.HorizontalScrollBarHeight;
            HorizontalMargin = 80;
            VerticalMargin = 80;
            
            ChartWidth = BoardWidth - 2 * HorizontalMargin;//画图区域宽度
            CharHeight = BoardHeight - 2 * VerticalMargin; //画图区域高度

            StartPostion = new Point(HorizontalMargin, VerticalMargin);
            EndPostion = new Point(BoardWidth - HorizontalMargin, BoardHeight - VerticalMargin);

            if (IsInit)
            {
                XEndPostion = new Point(BoardWidth - HorizontalMargin, BoardHeight - VerticalMargin);
                IsInit = false;
            }
        }

        private void DrawPolyline()
        {
            //var polyline = new Polyline();
            //foreach (var t in DataSourse)
            //{
            //    polyline.Points.Add(GetRealPoint(t));
            //}
            //polyline.Stroke = Brushes.Blue;

            List<Point> points =  new List<Point>();
            foreach (var t in DataSourse)
            {
                Point point = GetRealPoint(t);
                points.Add(point);
                if (t.Y != 0)
                {
                    MainCanvas.Children.Add(new Label { Width = 120, Height = 40, Margin = new Thickness(point.X+20, point.Y-20 , 0, 0),FontSize=18, Content =t.X.ToString()+","+ t.Y.ToString() });
                }
            }
            
            points.FindAll(a=>a.Y!= EndPostion.Y + MapLocationY).ForEach(lipt => MainCanvas.Children.Add(new Ellipse { Width = 4, Height = 4, Margin = new Thickness(lipt.X - 2, lipt.Y - 2, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Fill = Brushes.Red, ToolTip = string.Format("x:{0},y:{1} ", lipt.X, lipt.Y) }));
            
            StringBuilder data = new StringBuilder("M");
            data.AppendFormat("{0},{1} C", points[0].X, points[0].Y);
            for (int i = 1; i < points.Count; i++)
            {
                Point pre = new Point((points[i - 1].X + points[i].X) / 2, points[i - 1].Y);  //控制点
                Point next = new Point((points[i - 1].X + points[i].X) / 2, points[i].Y);     //控制点
                data.AppendFormat(" {0},{1} {2},{3} {4},{5}", pre.X, pre.Y, next.X, next.Y, points[i].X, points[i].Y);
            }
            Path path2 = new Path { Stroke = Brushes.DodgerBlue, StrokeThickness = 1, Data = Geometry.Parse(data.ToString()) };


            MainCanvas.Children.Add(path2);
        }

        private Point GetRealPoint(Point point)
        {
            var realX = StartPostion.X + ((point.X - MinX) - Math.Abs(MinX)) * ChartWidth / (MaxX - MinX) + MapLocationX;
            var realY = StartPostion.Y + (MaxY + Math.Abs(MinY) - point.Y) * CharHeight / (MaxY - MinY) + MapLocationY;
            return new Point(realX, realY);
        }

        /// <summary>
        ///  画y轴刻度
        /// </summary>
        private void DrawYAxisTicks()
        {
            if (MinY >= MaxY)
            {
                return;
            }
            if (verticalBetween < 0.0001)
            {
                verticalBetween = (MaxY - MinY) / 20;
            }
            for (var i = MinY; i <= MaxY + 0.01; i += verticalBetween)
            {
                var y = EndPostion.Y - i * CharHeight / (MaxY - MinY) + MapLocationY;
                //刻度不要
                //var marker = new Line
                //{
                //    X1 = StartPostion.X - 5,
                //    Y1 = y,
                //    X2 = StartPostion.X,
                //    Y2 = y,
                //    Stroke = Brushes.Red
                //};
                //MainCanvas.Children.Add(marker);
                if (y <= XEndPostion.Y)
                {
                    var XEnd = StartPostion.X + MaxX * ChartWidth / (MaxX - MinX) + MapLocationX;
                    var gridLine = new Line
                    {
                        X1 = StartPostion.X,
                        Y1 = y,
                        X2 = XEnd,
                        //X2 = XEndPostion.X+20,
                        Y2 = y,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.AliceBlue)
                    };
                    MainCanvas.Children.Add(gridLine);
                }
                //画y轴字符
                var markText = new TextBlock
                {
                    Text = i.ToString(CultureInfo.InvariantCulture),
                    Width = 30,
                    Foreground = Brushes.Yellow,
                    FontSize = 10,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    TextAlignment = TextAlignment.Right
                };
                MainCanvas.Children.Add(markText);
                Canvas.SetTop(markText, y - 10);
                Canvas.SetLeft(markText, 00);
            }
        }

        /// <summary>
        /// 画x轴标签
        /// </summary>
        private void DrawXAxisTicks()
        {
            if (MinX >= MaxX)
            {
                return;
            }
            if (horizontalBetween < 0.0001)
            {
                horizontalBetween = (MaxX - MinX) / 12;
            }
            for (var i = MinX; i <= MaxX + 0.01; i += horizontalBetween)
            {
                var x = StartPostion.X + i * ChartWidth / (MaxX - MinX) + MapLocationX;
                //刻度不要
                //var marker = new Line
                //{
                //    X1 = x,
                //    Y1 = XEndPostion.Y,
                //    X2 = x,
                //    Y2 = XEndPostion.Y + 4,
                //    Stroke = Brushes.Red
                //};
                //MainCanvas.Children.Add(marker);
                if (x >= StartPostion.X)
                {
                    var YStart = EndPostion.Y - MaxY * CharHeight / (MaxY - MinY) + MapLocationY;
                    var gridLine = new Line
                    {
                        X1 = x,
                        Y1 = YStart,
                        //Y1 = StartPostion.Y,
                        X2 = x,
                        Y2 = XEndPostion.Y,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.AliceBlue)
                    };
                    MainCanvas.Children.Add(gridLine);
                }
                //画x轴字符
                var text = i.ToString(CultureInfo.InvariantCulture);
                var markText = new TextBlock
                {
                    Text = text,
                    Width = 130,
                    Foreground = Brushes.Yellow,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    TextAlignment = TextAlignment.Left,
                    FontSize = 15
                };

                //Transform st = new SkewTransform(0, 0);
                //markText.RenderTransform = st;
                MainCanvas.Children.Add(markText);
                Canvas.SetTop(markText, XEndPostion.Y + 5);
                Canvas.SetLeft(markText, x);
            }


        }

        /// <summary>
        /// X轴Y轴
        /// </summary>
        private void DrawAxis()
        {
            var xaxis = new Line
            {
                X1 = StartPostion.X,
                Y1 = XEndPostion.Y,
                X2 = XEndPostion.X+20,
                Y2 = XEndPostion.Y,
                Stroke = new SolidColorBrush(Colors.Black)
            };
            MainCanvas.Children.Add(xaxis);

            var yaxis = new Line
            {
                X1 = StartPostion.X,
                Y1 = StartPostion.Y,
                X2 = StartPostion.X,
                Y2 = EndPostion.Y+20,
                Stroke = new SolidColorBrush(Colors.Black)
            };
            MainCanvas.Children.Add(yaxis);
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <returns></returns>
        private PointCollection GetCollPoint()
        {
            PointCollection myPointCollection = new PointCollection()
            {
                new Point(-20,0),
                new Point(-5,0),
                new Point(0,0),
                new Point(5,20),
                new Point(10,0),
                new Point(15,0),
                new Point(20,0),
                new Point(25,0),
                new Point(30,50),
                new Point(35,0),
                new Point(40,0),
                new Point(45,100),
                new Point(50,0),
                new Point(55,0),
                new Point(60,0),
                new Point(65,0),
                new Point(70,0),
                new Point(75,0),
                new Point(80,0),
                new Point(85,0),
                new Point(90,0),
                new Point(95,0),
                new Point(100,0),
            };

            return myPointCollection;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            

        }
        //(Canvas.GetLeft(MainCanvas) + wl + MainCanvas.Width) <= movBg.ActualWidth &&
        //&& (Canvas.GetTop(MainCanvas) + hl + MainCanvas.Height) <= movBg.ActualHeight
        //&&(MainCanvas.Width + wl) < MainCanvas.MaxWidth && (MainCanvas.Height + hl) < MainCanvas.MaxHeight
        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double val = e.Delta * 0.001;
            double wl = MainCanvas.Width * (val / 0.12) * 0.02;
            double hl = MainCanvas.Height * (val / 0.12) * 0.02;

            if (!((Canvas.GetLeft(MainCanvas) - wl / 2.0) > 0 )&& !((Canvas.GetBottom(MainCanvas) - hl / 2.0) > 0) )
            {
                MainCanvas.SetValue(Canvas.LeftProperty, Canvas.GetLeft(MainCanvas) - wl / 2.0);
                MainCanvas.SetValue(Canvas.BottomProperty, Canvas.GetBottom(MainCanvas) - hl / 2.0);
                MainCanvas.Width += wl;
                MainCanvas.Height += hl;
            }
            return;
        }

        private void DrawWheel(TransformGroup group, Point point, double scale)
        {
            Point pointToContent = group.Inverse.Transform(point);

            ScaleTransform scaleT = group.Children[0] as ScaleTransform;

            if (scaleT.ScaleX + scale < 1) return;

            scaleT.ScaleX += scale;

            scaleT.ScaleY += scale;

            TranslateTransform translateT = group.Children[1] as TranslateTransform;

            translateT.X = -1 * ((pointToContent.X * scaleT.ScaleX) - point.X);

            translateT.Y = -1 * ((pointToContent.Y * scaleT.ScaleY) - point.Y);

            //RaisePropertyChanged("TransGroup");
        }

        

        //Path path2 = null;
        //private void PaintLine(List<Point> points)
        //{
        //    StringBuilder data = new StringBuilder("M");
        //    data.AppendFormat("{0},{1} C", points[0].X, points[0].Y);
        //    for (int i = 1; i < points.Count; i++)
        //    {
        //        Point pre = new Point((points[i - 1].X + points[i].X) / 2, points[i - 1].Y);  //控制点
        //        Point next = new Point((points[i - 1].X + points[i].X) / 2, points[i].Y);     //控制点
        //        data.AppendFormat(" {0},{1} {2},{3} {4},{5}", pre.X, pre.Y, next.X, next.Y, points[i].X, points[i].Y);
        //    }

        //    path2 = new Path { Stroke = Brushes.DodgerBlue, StrokeThickness = 1, Data = Geometry.Parse(data.ToString()) };
        //    this.canvas2.Children.Add(path2);
        //}
        //Random rand = new Random(DateTime.Now.Millisecond);
        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    List<Point> points = new List<Point>();
        //    for (int i = 0; i < 10000; i++)
        //    {
        //        //if (i % 100 == 0)
        //        //{
        //        //    var ptx = i;
        //        //    var pty = rand.Next(-100, 0);
        //        //    points.Add(new Point(ptx, pty));
        //        //}
        //        //else if (i % 10 == 0)
        //        //{
        //        //    var ptx = i;
        //        //    var pty = 0;
        //        //    points.Add(new Point(ptx, pty));
        //        //}
        //        if (i % 100 == 0)
        //        {
        //            var ptx =i;
        //            var pty = rand.Next(100, 300);
        //            points.Add(new Point(ptx, pty));
        //        }
        //    }
        //    PaintLine(points);
        //}
    }
}
