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

<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="AdministerServices.aspx.cs"
    Inherits="PresentationFramework.AdministerServicesForm" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .style2
        {
            width: 300px;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManager ID="AdministrationScriptManager" runat="server">
        <Scripts>
            <asp:ScriptReference Path="../Scripts/Administration.js"/>
        </Scripts>
    </asp:ScriptManager>
    Add a new data source using the Service details form below, or update existing services by selecting them from the list at the bottom.
    <table style="width: 100%;">
        <tr>
            <td class="style2">
                <h3>
                    Service Details:</h3>
            </td>
            <td class="style2">
                <h3>Service Help:
                </h3>
            </td>
            <td>
                <h3>
                    Service Capabilities:</h3>
            </td>
        </tr>
        <tr>
            <td class="style2" valign="top">
                <asp:DetailsView ID="ServicesDetailsView" runat="server" AutoGenerateRows="False"
                    DataKeyNames="ServiceID" DataSourceID="ServiceDetailDataSource" Height="50px"
                    Width="125px" OnItemInserted="ServicesDetailsView_ItemInserted" 
                    Style="margin-right: 20px" 
                    onitemupdated="ServicesDetailsView_ItemUpdated" 
                    onmodechanging="ServicesDetailsView_ModeChanging">
                    <Fields>
                        <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" >
                        <ControlStyle Width="200px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Url" HeaderText="Url" SortExpression="Url" >
                        <ControlStyle Width="200px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Version" HeaderText="Version" 
                            SortExpression="Version" >
                        <ControlStyle Width="200px" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Type" SortExpression="Type">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ServiceTypesDropDownList_Edit" Runat="server" 
                                    SelectedValue='<%# Bind ("Type") %>' 
                                    DataSourceID="SupportedServicesDataSource" ClientIDMode="Static">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ServiceTypesDropDownList_Insert" Runat="server" 
                                    SelectedValue='<%# Bind ("Type") %>' 
                                    DataSourceID="SupportedServicesDataSource" ClientIDMode="Static">
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label Runat="server" Text='<%# Bind("Type") %>' ID="ServiceTypesLabel"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="True" ShowInsertButton="True" />
                    </Fields>
                </asp:DetailsView>
                <asp:ObjectDataSource ID="SupportedServicesDataSource" runat="server" 
                    OldValuesParameterFormatString="original_{0}" SelectMethod="getServices" 
                    TypeName="ServiceLogic.Services"></asp:ObjectDataSource>
                <asp:ObjectDataSource ID="ServiceDetailDataSource" runat="server" DeleteMethod="DeleteService"
                    InsertMethod="AddService" SelectMethod="GetServiceById" TypeName="ServicesBusinessLogic.ServicesBusinessLogic"
                    UpdateMethod="UpdateService">
                    <DeleteParameters>
                        <asp:Parameter Name="serviceId" Type="Int32" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:Parameter Name="title" Type="String" />
                        <asp:Parameter Name="url" Type="String" />
                        <asp:Parameter Name="type" Type="String" />
                        <asp:Parameter Name="version" Type="String" />
                    </InsertParameters>
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ServicesGridView" Name="serviceId" PropertyName="SelectedValue"
                            Type="Int32" DefaultValue="" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="serviceId" Type="Int32" />
                        <asp:Parameter Name="title" Type="String" />
                        <asp:Parameter Name="url" Type="String" />
                        <asp:Parameter Name="type" Type="String" />
                        <asp:Parameter Name="version" Type="String" />
                    </UpdateParameters>
                </asp:ObjectDataSource>
                <asp:Label ID="Message" runat="server" Visible="False"></asp:Label>
            </td>
            <td class="style2" valign="top">
                <asp:Label ID="HelpText" runat="server" ClientIDMode="Static"></asp:Label>
            </td>
            <td class="style2" valign="top">
                <asp:GridView ID="LayersPreviewGridView" runat="server">
                </asp:GridView>
            </td>
        </tr>
    </table>
    <br />
    <h3>
        Available Services:</h3>
    <asp:GridView ID="ServicesGridView" runat="server" AllowSorting="True" AutoGenerateColumns="False"
        DataKeyNames="ServiceID" DataSourceID="ServicesDataSource" AllowPaging="True"
        OnSelectedIndexChanged="ServicesGridView_SelectedIndexChanged" 
        OnRowDataBound="ServicesGridView_RowDataBound" 
        onpageindexchanged="ServicesGridView_PageIndexChanged" 
        onrowdeleted="ServicesGridView_RowDeleted">
        <Columns>
            <asp:CommandField ShowDeleteButton="True" ShowSelectButton="True" />
            <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
            <asp:HyperLinkField DataNavigateUrlFields="Url" DataTextField="Url" 
                HeaderText="Url" />
            <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" />
            <asp:BoundField DataField="Version" HeaderText="Version" 
                SortExpression="Version" />
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="ServicesDataSource" runat="server" DeleteMethod="DeleteService"
        InsertMethod="AddService" SelectMethod="GetServices" TypeName="ServicesBusinessLogic.ServicesBusinessLogic"
        UpdateMethod="UpdateService">
        <DeleteParameters>
            <asp:Parameter Name="serviceId" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="title" Type="String" />
            <asp:Parameter Name="url" Type="String" />
            <asp:Parameter Name="version" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="serviceId" Type="Int32" />
            <asp:Parameter Name="title" Type="String" />
            <asp:Parameter Name="url" Type="String" />
            <asp:Parameter Name="version" Type="String" />
        </UpdateParameters>
    </asp:ObjectDataSource>
</asp:Content>
