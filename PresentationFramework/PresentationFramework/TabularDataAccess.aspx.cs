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
using ServicesDataAccess;
using ServiceLogicCommon;
using ServiceLogic;
using System.Text;
using System.Data;
using DataProcessors;
using DataProcessorCommon;

// TODO: provide links to metadata where possible.
// TODO: rename this to ATMOS
namespace PresentationFramework
{
    public partial class TabularDataAccess : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // called when the nodes in the service tree view are being populated.  This allows us to 
        // populate the service nodes by querying the database, or to populate the series nodes by 
        // querying the service.
        protected void ServicesTreeView_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            ServiceTreeNode serviceTreeNode = e.Node as ServiceTreeNode;
            // if this is a valid node, and there aren't already child nodes
            if (serviceTreeNode != null && serviceTreeNode.ChildNodes.Count == 0)
            {
                // populate new nodes, depending on the depth.  Nodes at depth 1 are services, and nodes at depths 2 through n are series.
                switch (serviceTreeNode.Depth)
                {
                    case 0:
                        // this is the root node, its children are serivces.
                        populateServices(serviceTreeNode);
                        break;

                    case 1:
                        // this is a service node, its children are series groups
                        populateSeriesGroups(serviceTreeNode);
                        break;

                    case 2:
                        // this is a seriesGroup node, its children are series
                        populateSeries(serviceTreeNode);
                        break;

                    default:
                        // ignore nodes at other depths.
                        break;
                }
            }
        }

        protected void populateServices(ServiceTreeNode rootNode)
        {
            // get a list of services from the database
            ServicesBusinessLogic.ServicesBusinessLogic servicesLogic = new ServicesBusinessLogic.ServicesBusinessLogic();
            ServicesDataSet.ServicesDataTable serviceTable = servicesLogic.GetServices();
            Services services = new Services();
            foreach (ServicesDataSet.ServicesRow serviceRow in serviceTable)
            {
                // only display tabular services in the tree
                var tabularService = services.getService(serviceRow.Type) as ITabularService;
                if (tabularService != null)
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

        protected void populateSeriesGroups(ServiceTreeNode serviceNode)
        {
            // get information about the service
            ServicesBusinessLogic.ServicesBusinessLogic servicesLogic = new ServicesBusinessLogic.ServicesBusinessLogic();
            ServicesDataSet.ServicesDataTable servicesTable = servicesLogic.GetServiceById((int)serviceNode.ServiceID);
            ServicesDataSet.ServicesRow serviceRow = servicesTable[0];
            Service service = new Service(serviceRow.Title, serviceRow.Url, serviceRow.Type, serviceRow.Version);

            // request the set of series for this service
            try
            {
                Services services = new Services();
                var tabularService = services.getService(service.Type) as ITabularService;
                if (tabularService != null)
                {
                    foreach (SeriesGroup seriesGroup in tabularService.requestSeriesGroups(service))
                    {
                        // create a node for this series group.
                        ServiceTreeNode seriesGroupNode = new ServiceTreeNode(seriesGroup.Name, seriesGroup.InternalName, serviceNode.ServiceID);
                        seriesGroupNode.SerializableData = null;
                        seriesGroupNode.ShowCheckBox = false;
                        seriesGroupNode.SelectAction = TreeNodeSelectAction.Expand;

                        // Allow flexibility for tabular data services to retrieve the series data 
                        // along with the series groups.  If the seriesGroup's series list is empty,
                        // then populate it on demand when expanded.  Otherwise, populate it now.
                        if (seriesGroup.SeriesList.Count == 0)
                        {
                            // we will need the seriesGroup to populate the series, so store it in the view state.
                            seriesGroupNode.SerializableData = seriesGroup; 
                            seriesGroupNode.PopulateOnDemand = true;
                        }
                        else
                        {
                            // add a node for each series in this group
                            foreach (Series series in seriesGroup.SeriesList)
                            {
                                ServiceTreeNode seriesNode = new ServiceTreeNode(series.Name, series.InternalName, serviceNode.ServiceID);
                                seriesNode.SerializableData = series;
                                seriesNode.ShowCheckBox = true;
                                seriesNode.SelectAction = TreeNodeSelectAction.Expand;
                                seriesGroupNode.ChildNodes.Add(seriesNode);
                            }
                        }

                        serviceNode.ChildNodes.Add(seriesGroupNode);
                    }
                }
                else
                {
                    // we couldn't find the service
                    // create a new node to display an error message
                    StringBuilder message = new StringBuilder("No tabular data service found for type ");
                    message.Append(service.Type);
                    TreeNode errorNode = new TreeNode(message.ToString());
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

        protected void populateSeries(ServiceTreeNode seriesGroupNode)
        {
            // get information about the service
            ServicesBusinessLogic.ServicesBusinessLogic servicesLogic = new ServicesBusinessLogic.ServicesBusinessLogic();
            ServicesDataSet.ServicesDataTable servicesTable = servicesLogic.GetServiceById((int)seriesGroupNode.ServiceID);
            ServicesDataSet.ServicesRow serviceRow = servicesTable[0];
            Service service = new Service(serviceRow.Title, serviceRow.Url, serviceRow.Type, serviceRow.Version);

            // request the set of series for this service
            try
            {
                Services services = new Services();
                var tabularService = services.getService(service.Type) as ITabularService;
                if (tabularService != null)
                {
                    SeriesGroup seriesGroup = seriesGroupNode.SerializableData as SeriesGroup;
                    if (seriesGroup != null)
                    {
                        // add a node for each series in this group
                        foreach (Series series in tabularService.requestSeries(service, seriesGroup))
                        {
                            ServiceTreeNode seriesNode = new ServiceTreeNode(series.Name, series.InternalName, seriesGroupNode.ServiceID);
                            seriesNode.SerializableData = series;
                            seriesNode.ShowCheckBox = true;
                            seriesNode.SelectAction = TreeNodeSelectAction.None;
                            seriesGroupNode.ChildNodes.Add(seriesNode);
                        }
                    }
                    else
                    {
                        // create a new node to display an error message
                        TreeNode errorNode = new TreeNode("Error: Unable to retrieve series group information");
                        errorNode.SelectAction = TreeNodeSelectAction.None;
                        seriesGroupNode.ChildNodes.Add(errorNode);
                    }
                }
                else
                {
                    // we couldn't find the service
                    // create a new node to display an error message
                    StringBuilder message = new StringBuilder("No tabular data service found for type ");
                    message.Append(service.Type);
                    TreeNode errorNode = new TreeNode(message.ToString());
                    errorNode.SelectAction = TreeNodeSelectAction.None;
                    seriesGroupNode.ChildNodes.Add(errorNode);
                }
            }
            catch (Exception e)
            {
                // create a new node to display an error message
                TreeNode errorNode = new TreeNode("Error: " + e.Message);
                errorNode.SelectAction = TreeNodeSelectAction.None;
                seriesGroupNode.ChildNodes.Add(errorNode);
            }
        }

        protected void TabularDataWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (TabularDataWizard.WizardSteps[e.NextStepIndex].ID == "WizardStepSelectTimeRange")
            {
                // construct a data table to display information about the selected services
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Service");
                dataTable.Columns.Add("ServiceUrl");
                dataTable.Columns.Add("Type");
                dataTable.Columns.Add("SeriesGroup");
                dataTable.Columns.Add("SeriesGroupMetadataUrl");
                dataTable.Columns.Add("Series");
                dataTable.Columns.Add("SeriesMetadataUrl");
                dataTable.Columns.Add("Units");
                dataTable.Columns.Add("SampleCount", typeof(int));
                dataTable.Columns.Add("StartDate", typeof(DateTimeOffset));
                dataTable.Columns.Add("EndDate", typeof(DateTimeOffset));

                foreach (ServiceTreeNode node in ServicesTreeView.CheckedNodes)
                {
                    Series series = node.SerializableData as Series;
                    if (series != null)
                    {
                        ServicesBusinessLogic.ServicesBusinessLogic servicesLogic = new ServicesBusinessLogic.ServicesBusinessLogic();
                        ServicesDataSet.ServicesDataTable servicesTable = servicesLogic.GetServiceById((int)node.ServiceID);
                        ServicesDataSet.ServicesRow serviceRow = servicesTable[0];

                        dataTable.Rows.Add(serviceRow.Title,        // Service
                                           serviceRow.Url,          // link to service information
                                           serviceRow.Type,         // Service Type
                                           node.Parent.Text,        // Series Group
                                           null,                    // link to Series Group metadata
                                           series.Name,             // Series
                                           null,                    // link to Series metadata
                                           series.Units,            // Units
                                           series.SampleCount,      // Sample Count
                                           series.DateRange.Start,  // Start Date
                                           series.DateRange.End);   // End Date
                    }
                    else
                    {
                        StringBuilder message = new StringBuilder("series is null for checked node.  Service: ");
                        message.Append(node.ServiceID);
                        message.Append(", text: ");
                        message.Append(node.Text);
                        message.Append(", value: ");
                        message.Append(node.Value);
                        throw new ArgumentNullException(message.ToString());
                    }
                }

                SelectedSeriesGridView.DataSource = dataTable;
                SelectedSeriesGridView.DataBind();
            }
        }

        protected void TabularDataWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            DateTime startDate = DateTime.Parse (StartDateTextBox.Text);
            DateTime endDate = DateTime.Parse (EndDateTextBox.Text);
            Services services = new Services ();
            TableCombiners combiners = new TableCombiners();
            DataTable combinedDataTable = new DataTable();

            // clear the message
            Message.Text = String.Empty;

            // retrieve the data for each selected series.  We go back to the treeview here because 
            // the series is serialized in the view state for each node.
            foreach (ServiceTreeNode node in ServicesTreeView.CheckedNodes)
            {
                Series series = node.SerializableData as Series;
                if (series != null)
                {
                    // get information about the service
                    ServicesBusinessLogic.ServicesBusinessLogic servicesLogic = new ServicesBusinessLogic.ServicesBusinessLogic();
                    ServicesDataSet.ServicesDataTable servicesTable = servicesLogic.GetServiceById((int)node.ServiceID);
                    ServicesDataSet.ServicesRow serviceRow = servicesTable[0];
                    Service service = new Service(serviceRow.Title, serviceRow.Url, serviceRow.Type, serviceRow.Version);

                    ITabularService tabularService = services.getService (service.Type) as ITabularService;
                    if (tabularService != null)
                    {
                        // construct a request to retrieve the data
                        TableRequest tableRequest = new TableRequest();
                        tableRequest.StartDate = startDate;
                        tableRequest.EndDate = endDate;
                        tableRequest.SeriesInternalName = series.InternalName;
                        tableRequest.SeriesGroupInternalName = node.Parent.Value;

                        DataTable seriesData = tabularService.requestData(service, tableRequest);

                        if (seriesData != null)
                        {
                            // the retrieved data may not have a unique name.  Since we will combine it with 
                            // data from other sources, we need to be able to uniquely identify it.  So add 
                            // the service name and series group name as a prefix to each column (except Time).
                            StringBuilder prefix = new StringBuilder();
                            prefix.AppendFormat("{0}-{1}-", service.Title, node.Parent.Text);
                            foreach (DataColumn column in seriesData.Columns)
                            {
                                if (column.ColumnName != "Time")
                                {
                                    column.ColumnName = prefix.ToString() + column.ColumnName;
                                }
                            }

                            // use the selected data combiner to combine this data into the master table
                            ITableCombiner combiner = combiners.getTableCombiner(TableCombinersDropDownList.SelectedValue);
                            combinedDataTable = combiner.combine(combinedDataTable, seriesData);
                        }
                        else
                        {
                            // display a message indicating that some data couldn't be retrieved
                            Message.Text += String.Format("\nWarning: No Data retrieved for {0} - {1} - {2}", service.Title, node.Parent.Text, series.Name);
                            Message.Visible = true;
                        }
                    }
                    else
                    {
                        // we couldn't find the service
                        // create a new node to display an error message
                        StringBuilder message = new StringBuilder("No tabular data service found for type ");
                        message.Append(service.Type);
                        throw new ArgumentException(message.ToString());
                    }
                }
                else
                {
                    StringBuilder message = new StringBuilder("series is null for checked node.  Service: ");
                    message.Append(node.ServiceID);
                    message.Append(", text: ");
                    message.Append(node.Text);
                    message.Append(", value: ");
                    message.Append(node.Value);
                    throw new ArgumentNullException(message.ToString());
                }
            }

            // Now we are ready to bind the data to the grid.  We could use AutoGenerateColumns, but 
            // that doesn't populate the Columns collection, which we need when saving data below.  
            // So we create each column 'manually' here
            TabularDataGrid.Columns.Clear();
            foreach (DataColumn column in combinedDataTable.Columns)
            {
                BoundField boundField = new BoundField();
                boundField.DataField = column.ColumnName;
                boundField.HeaderText = column.ColumnName;
                TabularDataGrid.Columns.Add(boundField);
            }

            TabularDataGrid.DataSource = combinedDataTable;
            TabularDataGrid.DataBind();
            TabularDataGrid.Visible = true;
            SaveButton.Visible = true;
        }

        protected void TabularDataWizard_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
        {
            // make sure the data table and message are hidden
            Message.Visible = false;
            TabularDataGrid.Visible = false;
            SaveButton.Visible = false;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // write the csv file using a StringBuilder in memory.  This will avoid having to create temporary files on the server.
            StringBuilder csvFileContents = new StringBuilder();
            foreach (DataControlField column in TabularDataGrid.Columns)
            {
                // replace commas in the header with dashes to support CSV format.
                csvFileContents.AppendFormat("{0},", column.HeaderText.Replace(',', '-'));
            }
            csvFileContents.Append(Environment.NewLine);
            foreach (GridViewRow row in TabularDataGrid.Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    csvFileContents.AppendFormat("{0},", cell.Text);
                }
                csvFileContents.Append(Environment.NewLine);
            }

            // transmit the file back to the client, and indicate that it is a CSV attachment
            HttpResponse response = HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "text/csv";
            response.AddHeader("Content-Disposition", "attachment; filename=seriesData.csv;");
            response.Write(csvFileContents.ToString());
            response.Flush();

            response.End();
        }
    }
}