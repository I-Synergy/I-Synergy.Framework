using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Windows.Samples.Data;

namespace ISynergy.Framework.Windows.Samples.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChartView : IView
    {
        public ChartView()
        {
            this.InitializeComponent();

            //var charts = new ChartData().CreateXamarinSample();
            var charts = new ChartData().CreateQuickstart();
            this.chart1.Chart = charts[0];
            this.chart2.Chart = charts[1];
            this.chart3.Chart = charts[2];
            this.chart4.Chart = charts[3];
            this.chart5.Chart = charts[4];
            this.chart6.Chart = charts[5];
        }
    }
}
