using System;
using System.Windows;
using System.Windows.Controls;

namespace ArtRecommenderSystem.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для RangeSlider.xaml
    /// </summary>
    public partial class RangeSlider : UserControl
    {
        private const int ThumbMargin = 5;
        private double _stepWidth;

        public RangeSlider()
        {
            InitializeComponent();
            Loaded += Slider_Loaded;
        }

        private void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            LowerSlider.ValueChanged += LowerSlider_ValueChanged;
            UpperSlider.ValueChanged += UpperSlider_ValueChanged;

            _stepWidth = (ActualWidth - 2 * ThumbMargin) / (Maximum - Minimum);
            UpdateHighlightMargin();
        }

        private void LowerSlider_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            UpperSlider.Value = Math.Max(UpperSlider.Value, LowerSlider.Value);
            UpdateHighlightMargin();
        }

        private void UpperSlider_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            LowerSlider.Value = Math.Min(UpperSlider.Value, LowerSlider.Value);
            UpdateHighlightMargin();
        }

        private void UpdateHighlightMargin()
        {
            HighlightMargin =
                new Thickness(ThumbMargin + (LowerValue - Minimum) * _stepWidth, 0,
                    (Maximum - UpperValue) * _stepWidth, 0);
        }

        public double Minimum
        {
            get => (double) GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double),
                typeof(RangeSlider), new UIPropertyMetadata(0d));

        public double LowerValue
        {
            get => (double) GetValue(LowerValueProperty);
            set => SetValue(LowerValueProperty, value);
        }

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double),
                typeof(RangeSlider), new UIPropertyMetadata(0d));

        public double UpperValue
        {
            get => (double) GetValue(UpperValueProperty);
            set => SetValue(UpperValueProperty, value);
        }

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double),
                typeof(RangeSlider), new UIPropertyMetadata(0d));

        public double Maximum
        {
            get => (double) GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double),
                typeof(RangeSlider), new UIPropertyMetadata(1d));

        public Thickness HighlightMargin
        {
            get => (Thickness) GetValue(HighlightMarginProperty);
            set => SetValue(HighlightMarginProperty, value);
        }

        public static readonly DependencyProperty HighlightMarginProperty =
            DependencyProperty.Register("HighlightMargin", typeof(Thickness),
                typeof(RangeSlider),
                new UIPropertyMetadata(new Thickness(5, 0, 5, 0)));
    }
}
