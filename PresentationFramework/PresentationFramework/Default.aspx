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

<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="PresentationFramework._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p>
        Welcome to ATMOS, a framework that provides Access to Tabular and Map-based 
        Online Services.&nbsp; To get started, select <a href="MapAccess.aspx">Explore Maps</a> or <a href="TabularDataAccess.aspx">Explore Tabular 
        Data</a> above.  To add, remove, or change available services, log in, and select <a href="Account/AdministerServices.aspx">Administration</a>.</p>
</asp:Content>
