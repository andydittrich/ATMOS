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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using ServicesBusinessLogic;
using ServicesDataAccess;
using ServiceLogic;
using ServiceLogicCommon;
using System.Data;
using System.Text;
using System.Web.UI.HtmlControls;

namespace PresentationFramework
{
    public partial class AdministerServicesForm : System.Web.UI.Page
    {
        // TODO: cancel when inserting on the service details view should just clear the form
        // TODO: client side form validation when inputting a service.  ensure url is not empty, and type is selected
        // TODO: use 'Select a type' or similar for default in type drop down list
        // TODO: make sure javascript events are in place to update help text after selecting 'new' in the service details view.

        protected void Page_Load(object sender, EventArgs e)
        {
            // start with the services details view in insert mode.
            if (IsPostBack == false)
            {
                ServicesDetailsView.ChangeMode(DetailsViewMode.Insert);
            }

            // produce a javascript array that maps service types to their help text.  This is used 
            // client-side to update the help text as the user selects different types.
            Services services = new Services();
            StringBuilder serviceHelpMap = new StringBuilder();
            foreach (string serviceType in services.getServices())
            {
                serviceHelpMap.Append("{");
                serviceHelpMap.AppendFormat("type: \"{0}\", help: \"{1}\"", serviceType, services.getServiceHelp(serviceType));
                serviceHelpMap.Append("},");
            }
            if (serviceHelpMap.Length > 0)
            {
                // strip off the trailing comma after the last array item to make the javascript syntactically correct
                serviceHelpMap.Remove(serviceHelpMap.Length - 1, 1);
                Page.ClientScript.RegisterArrayDeclaration("serviceHelpMap", serviceHelpMap.ToString());
            }

            // register the javascript method to update the help text with the drop down list's onchange event.
            Control control = ServicesDetailsView.FindControl("ServiceTypesDropDownList_Edit");
            DropDownList dropDownlist = control as DropDownList;
            if (dropDownlist != null)
            {
                dropDownlist.Attributes.Add("onchange", "updateHelpTextFromSelectControl(this);");
                // set the help text to the currently selected value (the javascript only sets it when the user changes the drop down list selection
                HelpText.Text = services.getServiceHelp(dropDownlist.SelectedValue);
            }
            control = ServicesDetailsView.FindControl("ServiceTypesDropDownList_Insert");
            dropDownlist = control as DropDownList;
            if (dropDownlist != null)
            {
                dropDownlist.Attributes.Add("onchange", "updateHelpTextFromSelectControl(this);");
                // set the help text to the currently selected value (the javascript only sets it when the user changes the drop down list selection
                HelpText.Text = services.getServiceHelp(dropDownlist.SelectedValue);
            }
        }

        // Called when the user selects a row in the ServicesGridView.  
        protected void ServicesGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // make sure the details view is in read only mode
            ServicesDetailsView.ChangeMode(DetailsViewMode.ReadOnly);

            // display the selected service capabilities
            int selectedServiceId = Int32.Parse (ServicesGridView.SelectedValue.ToString ());
            DisplayServiceDetails(selectedServiceId);
        }

        protected void DisplayServiceDetails(int serviceId)
        {
            ServicesBusinessLogic.ServicesBusinessLogic servicesLogic = new ServicesBusinessLogic.ServicesBusinessLogic();
            ServicesDataSet.ServicesDataTable serviceDataTable = servicesLogic.GetServiceById(serviceId);
            ServicesDataSet.ServicesRow serviceRow = serviceDataTable[0];
            Service service = new Service(serviceRow.Title, serviceRow.Url, serviceRow.Type, serviceRow.Version);

            Services services = new Services();
            var dataService = services.getService(service.Type);
            if (dataService == null)
            {
                // we couldn't find the service
                StringBuilder message = new StringBuilder("No service found for type ");
                message.Append(service.Type);
                throw new ArgumentException(message.ToString());
            }

            // set the service help text
            HelpText.Text = services.getServiceHelp(service.Type);

            // try using this as a map service
            var mapService = dataService as IMapService;
            if (mapService != null)
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Name");
                dataTable.Columns.Add("Title");

                foreach (Layer layer in mapService.requestLayers(service))
                {
                    // don't display layers if they don't have a name
                    if (layer.Name != null)
                    {
                        dataTable.Rows.Add(layer.Name, layer.Title);
                    }
                }

                LayersPreviewGridView.DataSource = dataTable;
                LayersPreviewGridView.DataBind();
            }
            else
            {
                // try using this as a tabular data service
                var tabularService = dataService as ITabularService;
                if (tabularService != null)
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Name");
                    dataTable.Columns.Add("Series Count", typeof(int));

                    foreach (SeriesGroup seriesGroup in tabularService.requestSeriesGroups(service))
                    {
                        dataTable.Rows.Add(seriesGroup.Name, seriesGroup.SeriesList.Count);
                    }

                    LayersPreviewGridView.DataSource = dataTable;
                    LayersPreviewGridView.DataBind();
                }
                else
                {
                    // this is not a service that we have support for
                    StringBuilder message = new StringBuilder("service ");
                    message.Append(service.Type);
                    message.Append(" is not a recognized service type.");
                    throw new ArgumentException(message.ToString());
                }
            }
        }

        // called after the user inserts a new service using the details view.  Used to handle 
        // exceptions and update the grid view.
        protected void ServicesDetailsView_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            if (e.Exception == null)
            {
                // call DataBind to retrieve the new database entries for the GridView
                ServicesGridView.DataBind();

                // select the inserted item.  We assume it is the last item on the last page of the grid view.  
                // First select the last page
                ServicesGridView.PageIndex = ServicesGridView.PageCount - 1;
                // call DataBind again to refresh the rows from the underlying data source
                ServicesGridView.DataBind();
                // now select the last row.
                ServicesGridView.SelectedIndex = ServicesGridView.Rows.Count - 1;

                // display the new service details too
                int selectedServiceId = Int32.Parse(ServicesGridView.SelectedValue.ToString());
                DisplayServiceDetails(selectedServiceId);
                Message.Visible = false;
            }
            else
            {
                Message.Visible = true;
                Message.Text = "Exception occurred when inserting the service: ";
                // The exception always indicates that an exception occurred in an invocation.  
                // The inner exception contains the real error details.
                if (e.Exception.InnerException != null)
                {
                    Message.Text += e.Exception.InnerException.Message;
                }
                e.ExceptionHandled = true;
                e.KeepInInsertMode = true;
            }
        }

        // called after the user edits a new service using the details view.  Used to handle 
        // exceptions and update the grid view.
        protected void ServicesDetailsView_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            if (e.Exception == null)
            {
                // display the new data in the GridView
                ServicesGridView.DataBind();
                Message.Visible = false;
            }
            else
            {
                Message.Visible = true;
                Message.Text = "Exception occurred when editing the service: ";
                // The exception always indicates that an exception occurred in an invocation.  
                // The inner exception contains the real error details.
                if (e.Exception.InnerException != null)
                {
                    Message.Text += e.Exception.InnerException.Message;
                }
                e.ExceptionHandled = true;
                e.KeepInEditMode = true;
            }
        }

        // occurs when data is bound to a row in the services grid view control.
        // used to add a javascript confirmation when the user clicks the delete button for a row
        protected void ServicesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // make sure this is a data row, not a header, footer, or command row
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // access the delete button.  We know the command field is the first cell, 
                // and that the control's commandName is Delete
                LinkButton deleteButton = null;
                foreach (Control control in e.Row.Cells[0].Controls)
                {
                    if (control is LinkButton)
                    {
                        LinkButton linkButton = (LinkButton)control;
                        if (linkButton.CommandName == "Delete")
                        {
                            deleteButton = linkButton;
                            break;
                        }
                    }
                }

                if (deleteButton != null)
                {
                    deleteButton.OnClientClick = "return confirm ('Are you sure you want to delete the service?');";
                }
            }
        }

        protected void ServicesDetailsView_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {
            switch (e.NewMode)
            {
                case DetailsViewMode.Insert:
                    // When the user selects 'new' to add a new service, clear the service capabilities gridview
                    LayersPreviewGridView.DataSource = null;
                    LayersPreviewGridView.DataBind();
                    break;
            }
        }

        protected void ServicesGridView_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            // make sure the details view is in insert mode
            ServicesDetailsView.ChangeMode(DetailsViewMode.Insert);

            // clear the service capabilities gridview
            LayersPreviewGridView.DataSource = null;
            LayersPreviewGridView.DataBind();
        }

        protected void ServicesGridView_PageIndexChanged(object sender, EventArgs e)
        {
            // put the details view back in insert mode
            ServicesDetailsView.ChangeMode(DetailsViewMode.Insert);

            // clear the service capabilities gridview
            LayersPreviewGridView.DataSource = null;
            LayersPreviewGridView.DataBind();
        }
    }
}