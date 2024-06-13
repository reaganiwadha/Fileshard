using Fileshard.Service.Structs;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Msagl.Drawing;
using Fileshard.Frontend.Helpers;
using Microsoft.Msagl.Core.Routing;

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
                graph.Attr.OptimizeLabelPositions = true;
                graph.Attr.LayerDirection = LayerDirection.TB;
                graph.CreateLayoutSettings().EdgeRoutingSettings.EdgeRoutingMode = EdgeRoutingMode.StraightLine;

                graph.AddNode(_selectedObject.Id.ToString());
                graph.AddNode("name");
                var name = _selectedObject.Name.WrapAt(20).JoinLines();

                graph.AddNode(name);

                graph.AddEdge(_selectedObject.Id.ToString(), "name");
                graph.AddEdge("name", name);

                graph.AddNode("files");
                graph.AddEdge(_selectedObject.Id.ToString(), "files");
/*
                graph.AddNode("tags");
                graph.AddEdge(_selectedObject.Id.ToString(), "tags");*/
/*
                foreach (var tag in _selectedObject.Tags)
                {
                    var tagName = tag.Tag.Name.WrapAt(20).JoinLines();
                    graph.AddNode(tagName);
                    graph.AddEdge("tags", tagName);
                }
*/
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
                        var parts = meta.Key.Split(':');

                        string value = null;
                        if (meta.TimeValue != null)
                        {
                            value = meta.TimeValue.ToString();
                        }
                        else if (meta.Value != null)
                        {
                            value = meta.Value;
                        }
                        else if (meta.LongValue != null)
                        {
                            value = meta.LongValue.ToString();
                        }

                        AddNodesAndEdges(graph, parts, 0, "metas", value);
                    }
                }

                // Execute on UI Thread
                Dispatcher.Invoke(() => { graphName.Graph = graph; });
            }).Start();
        }

        void AddNodesAndEdges(Graph graph, string[] parts, int index, string parentNode, string value)
        {
            if (index >= parts.Length)
            {
                if (value != null)
                {
                    graph.AddNode(value);
                    graph.AddEdge(parentNode, value);
                }
                return;
            }

            string currentNode = parts[index];
            graph.AddNode(currentNode);
            graph.AddEdge(parentNode, currentNode);

            AddNodesAndEdges(graph, parts, index + 1, currentNode, value);
        }
    }
}