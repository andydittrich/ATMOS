// Copyright 2012 Andrew Dittrich
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.using System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ServicesDataAccess;
using System.Text;
using ServiceLogicCommon;
using ServiceLogic;
using System.Web.UI.HtmlControls;

namespace PresentationFramework
{
    // TODO: remove the annoying postback when expanding services
    // TODO: hide the map if the user goes back to select more services, or hide the wizard.
    // TODO: create javascript functionality to control z-order, visibility, and transparency of layers.
    public partial class MapAccess : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // add an onload method to the page body to execute the javascript method that retrieves 
            // the Bing Map.  This can't be done declaratively because the body is part of the master
            // page, not the content page.
            // TODO: apply this to the div that contains the map, and trigger it when the div is shown when the wizard finishes.
            HtmlGenericControl body = (HtmlGenericControl)Page.Master.FindControl("MasterPageBody");
            body.Attributes.Add("onload", "GetMap();"); 
        }

        // called when the nodes in the service tree view are being populated.  This allows us to 
        // populate the service nodes by querying the database, or to populate the layer nodes by 
        // querying the service.
        protected void ServicesTreeView_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            ServiceTreeNode serviceTreeNode = e.Node as ServiceTreeNode;
            // if this is a valid node, and there aren't already child nodes
            if (serviceTreeNode != null && serviceTreeNode.ChildNodes.Count == 0)
            {
                // populate new nodes, depending on the depth.  Nodes at depth 1 are services, and nodes at depths 2 through n are layers.
                switch (serviceTreeNode.Depth)
                {
                    case 0:
                        // this is the root node, its children are serivces.
                        populateServices(serviceTreeNode);
                        break;

                    case 1:
                        // this is a service node, its children are layers
                        populateLayers(serviceTreeNode);
                        break;

                    default:
                        // ignore nodes at other depths.
                        break;
                }
            }
        }

        protected void populateServices (ServiceTreeNode rootNode)
        {
            // get a list of services from the database
            ServicesBusinessLogic.ServicesBusinessLogic servicesLogic = new ServicesBusinessLogic.ServicesBusinessLogic ();
            ServicesDataSet.ServicesDataTable serviceTable = servicesLogic.GetServices ();
            Services services = new Services();
            foreach (ServicesDataSet.ServicesRow serviceRow in serviceTable)
            {
                var mapService = services.getService(serviceRow.Type) as IMapService;
                if (mapService != null)
                {
                    // create a new node for this service and add it to the tree
                    ServiceTreeNode serviceNode = new ServiceTreeNode(serviceRow.Title, serviceRow.ServiceID);
                    serviceNode.PopulateOnDemand = true;
                    serviceNode.Expanded = false;
                    serviceNode.SelectAction = TreeNodeSelectAction.Expand;
                    rootNode.ChildNodes.Add(serviceNode);
                }
            }
        }

        protected void populateLayers(ServiceTreeNode serviceNode)
        {
            // get information about the service
            ServicesBusinessLogic.ServicesBusinessLogic servicesLogic = new ServicesBusinessLogic.ServicesBusinessLogic();
            ServicesDataSet.ServicesDataTable servicesTable = servicesLogic.GetServiceById((int)serviceNode.ServiceID);
            ServicesDataSet.ServicesRow serviceRow = servicesTable[0];
            Service service = new Service(serviceRow.Title, serviceRow.Url, serviceRow.Type, serviceRow.Version);

            // request the set of layers for this service
            try
            {
                Services services = new Services();
                var mapService = services.getService(service.Type) as IMapService;
                if (mapService != null)
                {
                    foreach (Layer layer in mapService.requestLayers(service))
                    {
                        // create a new node for the top level layer and recusrively for all sublayers
                        populateLayers(serviceNode, serviceNode.ServiceID, layer);
                    }
                }
                else
                {
                    // we couldn't find the service
                    // create a new node to display an error message
                    StringBuilder message = new StringBuilder("No map service found for type ");
                    message.Append(service.Type);
                    TreeNode errorNode = new TreeNode(message.ToString ());
                    errorNode.SelectAction = TreeNodeSelectAction.None;
                    serviceNode.ChildNodes.Add(errorNode);
                }
            }
            catch (Exception e)
            {
                // create a new node to display an error message
                TreeNode errorNode = new TreeNode("Error: " + e.Message);
                errorNode.SelectAction = TreeNodeSelectAction.None;
                serviceNode.ChildNodes.Add(errorNode);
            }
        }

        protected void populateLayers(TreeNode parentNode, long serviceID, Layer layer)
        {
            // create a node for this layer.
            ServiceTreeNode layerNode = new ServiceTreeNode (layer.Title, layer.Name, serviceID);
            layerNode.ShowCheckBox = layer.Requestable;
            layerNode.SerializableData = layer;
            layerNode.SelectAction = TreeNodeSelectAction.Expand;
            parentNode.ChildNodes.Add(layerNode);

            // recursively add all child layers
            foreach (Layer sublayer in layer.Layers)
            {
                populateLayers(layerNode, serviceID, sublayer);
            }
        }

        protected void MapAccessWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            DataTable selectedLayers = new DataTable();
            selectedLayers.Columns.Add("Service");
            selectedLayers.Columns.Add("ServiceUrl");
            selectedLayers.Columns.Add("Type");
            selectedLayers.Columns.Add("Layer");
            selectedLayers.Columns.Add("LayerMetadataUrl");
            selectedLayers.Columns.Add("BoundingBox", typeof(BoundingBox));

            // iterate through the tree view to determine which nodes are checked
            foreach (ServiceTreeNode node in ServicesTreeView.CheckedNodes)
            {
                Layer layer = node.SerializableData as Layer;
                if (layer != null)
                {
                    ServicesBusinessLogic.ServicesBusinessLogic servicesLogic = new ServicesBusinessLogic.ServicesBusinessLogic();
                    ServicesDataSet.ServicesDataTable servicesTable = servicesLogic.GetServiceById((int)node.ServiceID);
                    ServicesDataSet.ServicesRow serviceRow = servicesTable[0];

                    selectedLayers.Rows.Add(serviceRow.Title,   // Service
                                            serviceRow.Url,     // Link to Service information
                                            serviceRow.Type,    // Service Type
                                            layer.Title,        // Layer
                                            null,               // TODO: link to Layer metadata
                                            layer.BoundingBox); // Bounding Box
                }
                else
                {
                    StringBuilder message = new StringBuilder("layer is null for checked node.  Service: ");
                    message.Append(node.ServiceID);
                    message.Append(", text: ");
                    message.Append(node.Text);
                    message.Append(", value: ");
                    message.Append(node.Value);
                    throw new ArgumentNullException(message.ToString());
                }
            }

            SelectedLayersGridView.DataSource = selectedLayers;
            SelectedLayersGridView.DataBind();
        }

        protected void MapAccessWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            BingMapPanel.Visible = true;
            BingMapPanel.Style.Add(HtmlTextWriterStyle.Position, "relative");

            // assemble a javascript array declaration to indicate which layers are checked.
            StringBuilder activeLayers = new StringBuilder();

            // iterate through the tree view to determine which nodes are checked
            foreach (ServiceTreeNode node in ServicesTreeView.CheckedNodes)
            {
                Layer layer = node.SerializableData as Layer;
                if (layer != null)
                {
                    activeLayers.Append("{");
                    activeLayers.AppendFormat("serviceID: {0}, layer: \"{1}\"", node.ServiceID, layer.Name);
                    activeLayers.Append("},");
                }
            }
            if (activeLayers.Length > 0)
            {
                // strip off the trailing comma after the last array item to make the javascript syntactically correct
                activeLayers.Remove(activeLayers.Length - 1, 1);
                Page.ClientScript.RegisterArrayDeclaration("activeLayers", activeLayers.ToString());
            }
        }
    }
}