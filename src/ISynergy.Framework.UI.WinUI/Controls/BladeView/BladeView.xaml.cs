using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.UI.Controls;

public partial class BladeView : UserControl
{
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<UIElement>), typeof(BladeView), new PropertyMetadata(new ObservableCollection<UIElement>()));

    public ObservableCollection<UIElement> ItemsSource
    {
        get { return (ObservableCollection<UIElement>)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public double DisabledOpacity
    {
        get { return (double)GetValue(DisabledOpacityProperty); }
        set { SetValue(DisabledOpacityProperty, value); }
    }

    public static readonly DependencyProperty DisabledOpacityProperty = DependencyProperty.Register(nameof(DisabledOpacity), typeof(double), typeof(BladeView), new PropertyMetadata(0.75));

    public Brush DisabledBackground
    {
        get { return (Brush)GetValue(DisabledBackgroundProperty); }
        set { SetValue(DisabledBackgroundProperty, value); }
    }

    public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(nameof(DisabledBackground), typeof(Brush), typeof(BladeView), new PropertyMetadata(Application.Current.Resources.ThemeDictionaries["ApplicationPageBackgroundThemeBrush"] as Style));

    public double Spacing
    {
        get { return (double)GetValue(SpacingProperty); }
        set { SetValue(SpacingProperty, value); }
    }

    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(BladeView), new PropertyMetadata(10));

    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(BladeView), new PropertyMetadata(Orientation.Horizontal));

    public HorizontalAlignment HorizontalBladeAlignment
    {
        get { return (HorizontalAlignment)GetValue(HorizontalBladeAlignmentProperty); }
        set { SetValue(HorizontalBladeAlignmentProperty, value); }
    }

    public static readonly DependencyProperty HorizontalBladeAlignmentProperty = DependencyProperty.Register(nameof(HorizontalBladeAlignment), typeof(HorizontalAlignment), typeof(BladeView), new PropertyMetadata(HorizontalAlignment.Right));

    public Thickness InnerPadding
    {
        get { return (Thickness)GetValue(InnerMarginProperty); }
        set { SetValue(InnerMarginProperty, value); }
    }

    public static readonly DependencyProperty InnerMarginProperty = DependencyProperty.Register(nameof(InnerPadding), typeof(Thickness), typeof(BladeView), new PropertyMetadata(new Thickness(8)));

    public double BladeHeight
    {
        get { return (double)GetValue(BladeHeightProperty); }
        set { SetValue(BladeHeightProperty, value); }
    }

    public static readonly DependencyProperty BladeHeightProperty = DependencyProperty.Register(nameof(BladeHeight), typeof(double), typeof(BladeView), new PropertyMetadata(0d));

    public int AutoCollapseThreshold
    {
        get { return (int)GetValue(AutoCollapseThresholdProperty); }
        set { SetValue(AutoCollapseThresholdProperty, value); }
    }

    public static readonly DependencyProperty AutoCollapseThresholdProperty = DependencyProperty.Register(nameof(AutoCollapseThreshold), typeof(int), typeof(BladeView), new PropertyMetadata(0));

    public BladeView()
    {
        this.InitializeComponent();
        this.Visibility = Visibility.Collapsed;
        this.CornerRadius = new CornerRadius(8);
        this.InnerPadding = new Thickness(8);
        this.HorizontalBladeAlignment = HorizontalAlignment.Right;
        this.Orientation = Orientation.Horizontal;
        this.Spacing = 10;
        this.DisabledBackground = Application.Current.Resources.ThemeDictionaries["ApplicationPageBackgroundThemeBrush"] as Brush;
        this.DisabledOpacity = 0.75;
        this.DataContextChanged += BladeView_DataContextChanged;
        this.SizeChanged += BladeViewEx_SizeChanged;
    }

    private void BladeViewEx_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Height > 0)
        {
            BladeHeight = e.NewSize.Height -
                (InnerPadding.Top + InnerPadding.Bottom + Margin.Top + Margin.Bottom);
        }
    }

    private void BladeView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (this.DataContext is IViewModelBladeView bladeView)
            bladeView.Blades.CollectionChanged += Blades_CollectionChanged;
    }

    private void Blades_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (this.DataContext is IViewModelBladeView bladeView)
        {
            if (bladeView.Blades.Count < 1)
                this.Visibility = Visibility.Collapsed;
            else
                this.Visibility = Visibility.Visible;

            if (AutoCollapseThreshold > 0 && bladeView.Blades.Count > AutoCollapseThreshold)
            {
                for (int i = 0; i < (bladeView.Blades.Count - AutoCollapseThreshold); i++)
                    ((UIElement)bladeView.Blades[i]).Visibility = Visibility.Collapsed;

                for (int i = AutoCollapseThreshold - 1; i < bladeView.Blades.Count - 1; i++)
                    ((UIElement)bladeView.Blades[i]).Visibility = Visibility.Visible;
            }
            else
            {
                foreach (UIElement item in bladeView.Blades.EnsureNotNull())
                    item.Visibility = Visibility.Visible;
            }

            for (int i = 0; i < bladeView.Blades.Count - 1; i++)
            {
                if (i == bladeView.Blades.Count - 1)
                    ((UIElement)bladeView.Blades[i]).Opacity = 1;
                else
                    ((UIElement)bladeView.Blades[i]).Opacity = DisabledOpacity;
            }
        }
    }
}
