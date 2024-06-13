using Fileshard.Service.Structs;
using LiveCharts;
using LiveCharts.Wpf;
using MoreLinq.Extensions;
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

namespace Fileshard.Frontend.Components
{
    /// <summary>
    /// Interaction logic for FileshardObjectTagGraph.xaml
    /// </summary>
    public partial class FileshardObjectTagGraph : UserControl
    {
        public FileshardObject Object
        {
            get { return (FileshardObject)GetValue(FileshardObjectProperty); }
            set { SetValue(FileshardObjectProperty, value); }
        }

        public static readonly DependencyProperty FileshardObjectProperty =
            DependencyProperty.Register("Object", typeof(FileshardObject), typeof(FileshardObjectTagGraph), new PropertyMetadata(null, OnFileshardObjectChanged));

        private static void OnFileshardObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FileshardObjectTagGraph;
            var newFileshardObject = e.NewValue as FileshardObject;
            control?.UpdateUI(newFileshardObject);
        }

        private void UpdateUI(FileshardObject newFileshardObject)
        {
            if (newFileshardObject != null)
            {
                DispatchGraphUpdate(newFileshardObject);
            }
        }

        private void DispatchGraphUpdate(FileshardObject _selectedObject) {
            if (_selectedObject == null) return;

            List<String> labels = new List<string>();

            SeriesCollection = new SeriesCollection();

            _selectedObject.Tags.OrderBy(t => t.Weight).GroupBy(t => t.Tag.NamespaceId).ForEach(ns =>
            {
                var series = new RowSeries
                {
                    Title = ns.First().Tag.NamespaceId.ToString(),
                    Values = new ChartValues<float>()
                };

                ns.ForEach(t => series.Values.Add(t.Weight));
                ns.ForEach(t => labels.Add(t.Tag.Name));

                SeriesCollection.Add(series);
            });

            seriesCol.Series = SeriesCollection;

            seriesLabels.Labels = labels;
            seriesFmter.LabelFormatter = Formatter = value => value.ToString("N");
        }


        public FileshardObjectTagGraph()
        {
            InitializeComponent();

            /*DataContext = this;*/
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
    }
}
