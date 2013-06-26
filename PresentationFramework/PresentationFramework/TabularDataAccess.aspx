<!-- // Copyright 2012 Andrew Dittrich
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
-->

<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="TabularDataAccess.aspx.cs" Inherits="PresentationFramework.TabularDataAccess" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="PresentationFramework" Namespace="PresentationFramework" TagPrefix="custom" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="Server" />
    <div>
        <asp:Wizard ID="TabularDataWizard" runat="server" 
            onnextbuttonclick="TabularDataWizard_NextButtonClick" 
            onfinishbuttonclick="TabularDataWizard_FinishButtonClick" 
            FinishCompleteButtonText="Get Data" 
            onpreviousbuttonclick="TabularDataWizard_PreviousButtonClick">
            <WizardSteps>
                <asp:WizardStep ID="WizardStepSelectServices" runat="server" Title="Step 1 - Select Services">
                    Select the data from the available services below.  Expand the service to list the available data series.
                    <custom:ServiceTreeView ID="ServicesTreeView" runat="server"
                        ontreenodepopulate="ServicesTreeView_TreeNodePopulate" 
                        PopulateNodesFromClient="False">
                        <Nodes>
                            <custom:ServiceTreeNode PopulateOnDemand="True" Text="Services" Value="Services">
                            </custom:ServiceTreeNode>
                        </Nodes>
                    </custom:ServiceTreeView>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStepSelectTimeRange" runat="server" Title="Step 2 - Select Time Range">
                    Selected Services (click links for direct access to metadata):<br />
                    <asp:GridView ID="SelectedSeriesGridView" runat="server" 
                        AutoGenerateColumns="False">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="ServiceUrl" DataTextField="Service" 
                                HeaderText="Service" />
                            <asp:BoundField DataField="Type" HeaderText="Service Type" />
                            <asp:HyperLinkField DataNavigateUrlFields="SeriesGroupMetadataUrl" DataTextField="SeriesGroup"
                                HeaderText="Series Group" />
                            <asp:HyperLinkField DataNavigateUrlFields="SeriesMetadataUrl" DataTextField="Series"
                                HeaderText="Series" />
                            <asp:BoundField DataField="Units" HeaderText="Units" />
                            <asp:BoundField DataField="SampleCount" HeaderText="Sample Count" />
                            <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:d}" />
                            <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:d}" />
                        </Columns>
                    </asp:GridView>
                    Select a date range:<br />Start Date:<asp:TextBox ID="StartDateTextBox" runat="server"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="StartDateCalendarExtender" runat="server" TargetControlID="StartDateTextBox"></ajaxToolkit:CalendarExtender>
                    End Date:<asp:TextBox ID="EndDateTextBox" runat="server"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="EndDateCalendarExtender" runat="server" TargetControlID="EndDateTextBox"></ajaxToolkit:CalendarExtender>
                    <br />
                    Select a data combination method:<br />
                    <asp:DropDownList ID="TableCombinersDropDownList" runat="server" 
                        DataSourceID="CombinersDataSource">
                    </asp:DropDownList>
                    <!-- TODO: post processing step? -->
                    <!-- TODO: output format (HTML table, CSV file, XLS file, graph, etc.)  Preferably dynamic based on a set of plugins -->
                </asp:WizardStep>
            </WizardSteps>
        </asp:Wizard>
        <asp:Label ID="Message" runat="server"></asp:Label>
        <br />
        <asp:GridView ID="TabularDataGrid" runat="server" Visible="False" 
            AutoGenerateColumns="False">
        </asp:GridView>
        <asp:Button ID="SaveButton" runat="server" onclick="SaveButton_Click" 
            Text="Save" Visible="false"/>
        <asp:ObjectDataSource ID="CombinersDataSource" runat="server" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="getTableCombiners" 
            TypeName="DataProcessors.TableCombiners"></asp:ObjectDataSource>
    </div>
</asp:Content>
