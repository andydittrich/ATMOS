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

<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="MapAccess.aspx.cs" Inherits="PresentationFramework.MapAccess" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="PresentationFramework" Namespace="PresentationFramework" TagPrefix="custom" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script charset="UTF-8" type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=7.0"></script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Panel ID="BingMapPanel" runat="server" ClientIDMode="Static" 
        Height="600px" Visible="False" Width="900px">
    </asp:Panel>
    <div>
        <asp:ScriptManager ID="MapScriptManager" runat="server">
            <Scripts>
                <asp:ScriptReference Path="./Scripts/BingMap.js" />
            </Scripts>
        </asp:ScriptManager>
        <asp:Wizard ID="MapAccessWizard" runat="server" 
            onfinishbuttonclick="MapAccessWizard_FinishButtonClick" 
            onnextbuttonclick="MapAccessWizard_NextButtonClick">
            <WizardSteps>
                <asp:WizardStep ID="WizardStep1" runat="server" Title="Step 1 - Select Map Layers">
                    Select map layers from the available services below.  Expand the service to list the available layers.
                    <custom:ServiceTreeView ID="ServicesTreeView" runat="server"
                        ontreenodepopulate="ServicesTreeView_TreeNodePopulate" 
                        PopulateNodesFromClient="False">
                        <Nodes>
                            <custom:ServiceTreeNode PopulateOnDemand="True" Text="Services" Value="Services">
                            </custom:ServiceTreeNode>
                        </Nodes>
                    </custom:ServiceTreeView>
                </asp:WizardStep>
                <asp:WizardStep ID="WizardStep2" runat="server" Title="Step 2 - Confirm Selection">
                    Selected Layers (click links for direct access to metadata):
                    <asp:GridView ID="SelectedLayersGridView" runat="server" 
                        AutoGenerateColumns="False">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="ServiceUrl" DataTextField="Service" 
                                HeaderText="Service" />
                            <asp:BoundField DataField="Type" HeaderText="Service Type" />
                            <asp:HyperLinkField DataNavigateUrlFields="LayerMetadataUrl" 
                                DataTextField="Layer" HeaderText="Layer" />
                            <asp:BoundField DataField="BoundingBox" HeaderText="Bounding Box" NullDisplayText="N/A" DataFormatString="{0:F3}" />
                        </Columns>
                    </asp:GridView>
                </asp:WizardStep>
            </WizardSteps>
        </asp:Wizard>
    </div>
</asp:Content>
