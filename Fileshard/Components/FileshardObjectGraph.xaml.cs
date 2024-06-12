using Fileshard.Service.Structs;
using System.Windows.Controls;
using System.Windows;
using System.Xml;
using Microsoft.Msagl.Drawing;
using Fileshard.Frontend.Helpers;

namespace Fileshard.Frontend.Components { 
    public partial class FileshardObjectGraph : UserControl
    {
        public FileshardObjectGraph()
        {
            InitializeComponent();
        }

        public FileshardObject FileshardObject
        {
            get { return (FileshardObject)GetValue(FileshardObjectProperty); }
            set { SetValue(FileshardObjectProperty, value); }
        }

        public static readonly DependencyProperty FileshardObjectProperty =
            DependencyProperty.Register("FileshardObject", typeof(FileshardObject), typeof(FileshardObjectGraph), new PropertyMetadata(null, OnFileshardObjectChanged));

        private static void OnFileshardObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FileshardObjectGraph;
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

        private void DispatchGraphUpdate(FileshardObject _selectedObject)
        {
            graphName.Graph = new Graph();
            new Thread(() =>
            {
                var graph = new Graph();
                graph.AddNode(_selectedObject.Id.ToString());
                graph.AddNode("name");
                var name = _selectedObject.Name.WrapAt(20).JoinLines();

                graph.AddNode(name);

                graph.AddEdge(_selectedObject.Id.ToString(), "name");
                graph.AddEdge("name", name);

                graph.AddNode("files");
                graph.AddEdge(_selectedObject.Id.ToString(), "files");


                foreach (var file in _selectedObject.Files)
                {
                    graph.AddNode(file.Id.ToString());
                    graph.AddEdge("files", file.Id.ToString());

                    var internalPath = file.InternalPath.WrapAt(25).JoinLines();
                    var n = graph.AddNode(internalPath);

                    graph.AddEdge(file.Id.ToString(), internalPath);

                    graph.AddNode("metas");
                    graph.AddEdge(file.Id.ToString(), "metas");

                    foreach (var meta in file.Metas)
                    {
                        graph.AddNode(meta.Key);
                        graph.AddEdge("metas", meta.Key);

                        graph.AddNode(meta.Value);
                        graph.AddEdge(meta.Key, meta.Value);
                    }
                }

                // Execute on UI Thread
                Dispatcher.Invoke(() => { graphName.Graph = graph; });
            }).Start();
        }
    }
}