using System;
using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;
using Dynamo.Models;
using Dynamo.Wpf.Extensions;
using Dynamo.Search.SearchElements;
using System.Collections;
using System.Linq;


namespace SampleViewExtension
{
    public class SampleWindowViewModel : NotificationObject, IDisposable
    {
        private string activeNodeTypes;
        private ReadyParams readyParams;
        private DynamoModel model;

        // Displays active nodes in the workspace
        public string ActiveNodeTypes
        {
            get
            {
                activeNodeTypes = getNodeTypes();
                return activeNodeTypes;
            }
        }

        // Helper function that builds string of active nodes
        public string getNodeTypes()
        {
            string output = "Active nodes 1.4:\n";

            foreach (NodeModel node in readyParams.CurrentWorkspaceModel.Nodes)
            {
                string nickName = node.Name == "" ? node.CreationName : node.Name;
                output += nickName + "\n";
            }


            // JL testing here: fetch the iconames
            output += "\nIcons:\n";

            // https://github.com/DynamoDS/Dynamo/blob/dec6240ded0c4369617775336b9af60c2aba4103/test/DynamoCoreWpfTests/IconsTests.cs
            IEnumerable searchEntries = model.SearchModel.SearchEntries.OfType<NodeSearchElement>();
            foreach (var entry in searchEntries)
            {
                var searchEle = entry as NodeSearchElement;
                

                //ElementTypes typpes = searchEle.ElementType;
                //TypeCode code = typpes.GetTypeCode();
                if (String.IsNullOrEmpty(searchEle.IconName))
                {
                    output += $"NO ICON for {searchEle.FullName}\n";
                }
                else
                {
                    string location = "pack://application:,,,/" + searchEle.Assembly + ";component/" + searchEle.IconName;
                    //Uri oUri = new Uri(, UriKind.RelativeOrAbsolute);
                    output += $"{searchEle.FullName} has icon on {searchEle.IconName}; {location}\n";
                }
            }

            return output;
        }

        public SampleWindowViewModel(ReadyParams p)
        {
            readyParams = p;
            p.CurrentWorkspaceModel.NodeAdded += CurrentWorkspaceModel_NodesChanged;
            p.CurrentWorkspaceModel.NodeRemoved += CurrentWorkspaceModel_NodesChanged;

            // JL: fetching the DynamoModel from the params...
            var vlp = p as ViewLoadedParams;
            var view = vlp.DynamoWindow.DataContext as Dynamo.ViewModels.DynamoViewModel;
            this.model = view.Model;
        }

        private void CurrentWorkspaceModel_NodesChanged(NodeModel obj)
        {
            RaisePropertyChanged("ActiveNodeTypes");
        }

        public void Dispose()
        {
            readyParams.CurrentWorkspaceModel.NodeAdded -= CurrentWorkspaceModel_NodesChanged;
            readyParams.CurrentWorkspaceModel.NodeRemoved -= CurrentWorkspaceModel_NodesChanged;
        }
    }
}
