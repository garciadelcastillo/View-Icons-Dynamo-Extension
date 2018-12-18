using System;
using System.Windows;
using System.Windows.Controls;
using Dynamo.Wpf.Extensions;

using System.Linq;
using System.Collections;
using Dynamo.Wpf.Services;

using Dynamo.Graph.Nodes;
using Dynamo.Controls;
using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace SampleViewExtension
{
    /// <summary>
    /// The View Extension framework for Dynamo allows you to extend
    /// the Dynamo UI by registering custom MenuItems. A ViewExtension has 
    /// two components, an assembly containing a class that implements 
    /// IViewExtension, and an ViewExtensionDefintion xml file used to 
    /// instruct Dynamo where to find the class containing the
    /// IViewExtension implementation. The ViewExtensionDefinition xml file must
    /// be located in your [dynamo]\viewExtensions folder.
    /// 
    /// This sample demonstrates an IViewExtension implementation which 
    /// shows a modeless window when its MenuItem is clicked. 
    /// The Window created tracks the number of nodes in the current workspace, 
    /// by handling the workspace's NodeAdded and NodeRemoved events.
    /// </summary>
    public class SampleViewExtension : IViewExtension
    {
        private MenuItem sampleMenuItem;
        private ViewLoadedParams loadedParams;

        public void Dispose()
        {
        }

        public void Startup(ViewStartupParams p)
        {
        }

        public void Loaded(ViewLoadedParams p)
        {
            // Save a reference to your loaded parameters.
            // You'll need these later when you want to use
            // the supplied workspaces

            sampleMenuItem = new MenuItem {Header = "Show View Extension Sample Window"};
            sampleMenuItem.Click += (sender, args) =>
            {
                var viewModel = new SampleWindowViewModel(p);
                var window = new SampleWindow
                {
                    // Set the data context for the main grid in the window.
                    MainGrid = { DataContext = viewModel },

                    // Set the owner of the window to the Dynamo window.
                    Owner = p.DynamoWindow
                };

                window.Left = window.Owner.Left + 400;
                window.Top = window.Owner.Top + 200;

                // Show a modeless window.
                window.Show();
            };
            p.AddMenuItem(MenuBarType.View, sampleMenuItem);

            // JL
            this.loadedParams = p;
            p.DynamoWindow.LayoutUpdated += DynamoWindow_ContentRendered;
        }

        private void DynamoWindow_ContentRendered(object sender, EventArgs e)
        {
            var nodeViews = this.loadedParams.DynamoWindow.FindVisualChildren<NodeView>();
            foreach (var nv in nodeViews)
            {
                ////if there is no existing label, add it.
                //if (nv.inputGrid.Children.OfType<TextBlock>().Where(x => x.Name == "typeLabel").Count() == 0)
                //{
                //    var nodeTypeLabel = new TextBlock();
                //    nodeTypeLabel.Name = "typeLabel";
                //    nodeTypeLabel.Text = nv.ViewModel.NodeModel.CreationName;
                //    nv.inputGrid.Children.Add(nodeTypeLabel);
                //}

                if (nv.inputGrid.Children.OfType<Image>().Count() == 0)
                {
                    // var src = @"/CoreNodeModelsWpf;component/Resources/MisingNode.png";  (from Core)
                    //string location = @"pack://application:,,,/MachinaDynamo;component/MachinaDynamo.Actions.Axes.Large.png";   // NOPE, crash
                    string location = @"/MachinaDynamo;component/Resources/MachinaDynamo.Actions.Axes.Large.png";  // Nope, no crash though
                    //string location = @"pack://application:,,,/MachinaDynamo;component/Resources/MachinaDynamo.Actions.Axes.Large.png";  // nope, also crash


                    // https://github.com/DynamoDS/Dynamo/blob/dec6240ded0c4369617775336b9af60c2aba4103/src/Libraries/CoreNodeModelsWpf/NodeViewCustomizations/DummyNode.cs
                    var img = new Image()
                    {
                        Stretch = System.Windows.Media.Stretch.None,
                        //Source = new BitmapImage(new Uri(@"C:\foo.png", UriKind.RelativeOrAbsolute))  // this works!
                        Source = new BitmapImage(new Uri(location, UriKind.RelativeOrAbsolute))
                        //Source = new BitmapImage(Properties.Resources.foo)
                    };

                    nv.inputGrid.Children.Add(img);

                    // Maybe htis works?? https://stackoverflow.com/questions/347614/wpf-image-resources
                }

                //NodeModel nm = nv.DataContext as NodeModel;
                //nv.ViewModel.NodeModel
            }

        }

        public void Shutdown()
        {
        }

        public string UniqueId
        {
            get
            {
                return Guid.NewGuid().ToString();
            }  
        } 

        public string Name
        {
            get
            {
                return "Sample View Extension";
            }
        } 

    }



    //https://github.com/mjkkirschner/DynamoSamples/blob/extensionWorkshop/src/NodeUISampleViewExtension/NodeUISampleViewExtension.cs
    public static class WPFextensions
    {
        ///https://stackoverflow.com/questions/10279092/how-to-get-children-of-a-wpf-container-by-type
        public static T GetChildOfType<T>(this DependencyObject depObj)
    where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
        //https://stackoverflow.com/questions/974598/find-all-controls-in-wpf-window-by-type/978352
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
