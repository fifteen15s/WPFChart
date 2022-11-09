using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPF曲线图
{
    /// <summary>SmoothCurveCtrl.xaml 的交互逻辑</summary>
    public partial class SmoothCurveCtrl
    {
        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<Point>), typeof(SmoothCurveCtrl), new PropertyMetadata(new ObservableCollection<Point>(), ItemsSourcePropertyChangeCallback));

        public SmoothCurveCtrl()
        {
            InitializeComponent();
        }


        public ObservableCollection<Point> ItemsSource
        {
            get { return (ObservableCollection<Point>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourcePropertyChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as SmoothCurveCtrl;
            if (uc != null)
            {
                if (uc.ItemsSource != null)
                {
                    uc.ItemsSource.CollectionChanged += uc.ItemsSource_CollectionChanged;
                }
            }
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HostGrid.Children.Clear();
            var originalPt = new Ellipse { Width = 0, Height = 0, Margin = new Thickness(0, 0, 0, 0), Fill = Brushes.Yellow };
            originalPt.HorizontalAlignment = HorizontalAlignment.Left;
            originalPt.VerticalAlignment = VerticalAlignment.Top;
            HostGrid.Children.Add(originalPt);
            DrawPath(ItemsSource);
        }

        private void DrawPath(ObservableCollection<Point> itemsSource)
        {
            var orderSource = from pt in itemsSource orderby pt.X ascending select pt;
            var li = orderSource.ToList();


            var pf = new PathFigure { StartPoint = li[0] };
            for (var i = 0; i < li.Count - 1; i++)
            {
                int current = i, last = i - 1, next = i + 1, next2 = i + 2;
                if (last == -1)
                {
                    last = 0;
                }
                if (next == li.Count)
                {
                    next = li.Count - 1;
                }
                if (next2 == li.Count)
                {
                    next2 = li.Count - 1;
                }
                var bzs = GetBezierSegment(li[current], li[last], li[next], li[next2]);
                pf.Segments.Add(bzs);
            }
            //画点的圆圈
            li.ForEach(lipt => HostGrid.Children.Add(new Ellipse { Width = 4, Height = 4, Margin = new Thickness(lipt.X - 2, lipt.Y - 2, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Fill = Brushes.Red, ToolTip = string.Format("x:{0},y:{1} ", lipt.X, lipt.Y) }));


            //添加曲线到图上
            var pfc = new PathFigureCollection { pf };
            var pg = new PathGeometry(pfc);
            var path = new Path { StrokeThickness = 1, Stroke = Brushes.Green, Data = pg };
            HostGrid.Children.Add(path);
        }
        /// <summary>
        /// 获得贝塞尔曲线
        /// </summary>
        /// <param name="currentPt">当前点</param>
        /// <param name="lastPt">上一个点</param>
        /// <param name="nextPt1">下一个点1</param>
        /// <param name="nextPt2">下一个点2</param>
        /// <returns></returns>
        private BezierSegment GetBezierSegment(Point currentPt, Point lastPt, Point nextPt1, Point nextPt2)
        {
            //计算中点
            var lastC = GetCenterPoint(lastPt, currentPt);
            var nextC1 = GetCenterPoint(currentPt, nextPt1); //贝塞尔控制点
            var nextC2 = GetCenterPoint(nextPt1, nextPt2);

            //计算相邻中点连线跟目的点的垂足
            //效果并不算太好，因为可能点在两个线上或者线的延长线上，计算会有误差
            //所以就直接使用中点平移方法。
            //var C1 = GetFootPoint(lastC, nextC1, currentPt);
            //var C2 = GetFootPoint(nextC1, nextC2, nextPt1);


            //计算“相邻中点”的中点
            var c1 = GetCenterPoint(lastC, nextC1);
            var c2 = GetCenterPoint(nextC1, nextC2);


            //计算【"中点"的中点】需要的点位移
            var controlPtOffset1 = currentPt - c1;
            var controlPtOffset2 = nextPt1 - c2;

            //移动控制点
            var controlPt1 = nextC1 + controlPtOffset1;
            var controlPt2 = nextC1 + controlPtOffset2;

            //如果觉得曲线幅度太大，可以将控制点向当前点靠近一定的系数。
            controlPt1 = controlPt1 + 0 * (currentPt - controlPt1);
            controlPt2 = controlPt2 + 0 * (nextPt1 - controlPt2);

            var bzs = new BezierSegment(controlPt1, controlPt2, nextPt1, true);
            return bzs;
        }

        /// <summary>
        ///     过c点做A和B连线的垂足
        /// </summary>
        /// <param name="aPoint"></param>
        /// <param name="bPoint"></param>
        /// <param name="cPoint"></param>
        /// <returns></returns>
        private Point GetFootPoint(Point aPoint, Point bPoint, Point cPoint)
        {
            //设三点坐标是A，B，C，AB构成直线，C是线外的点
            //三点对边距离是a,b,c,垂足为D，
            //根据距离推导公式得：AD距离是（b平方-a平方+c平方）/2c
            //本人数学不好，可能没考虑点c在线ab上的情况
            var offsetADist = (Math.Pow(cPoint.X - aPoint.X, 2) + Math.Pow(cPoint.Y - aPoint.Y, 2) - Math.Pow(bPoint.X - cPoint.X, 2) - Math.Pow(bPoint.Y - cPoint.Y, 2) + Math.Pow(aPoint.X - bPoint.X, 2) + Math.Pow(aPoint.Y - bPoint.Y, 2)) / (2 * GetDistance(aPoint, bPoint));

            var v = bPoint - aPoint;
            var distab = GetDistance(aPoint, bPoint);
            var offsetVector = v * offsetADist / distab;
            return aPoint + offsetVector;
        }

        private Point GetCenterPoint(Point pt1, Point pt2)
        {
            return new Point((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
        }

        private double GetDistance(Point pt1, Point pt2)
        {
            return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));
        }
    }
}