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

<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="PresentationFramework.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        About
    </h2>
    <p>
        ATMOS is a framework that allows consistent and convenient access to map and tabular data sources.  It is primarily focused on Climate Change data, and providing researches access to a variety of data all in one location.  The framework is extensible through a collection of plugins that allow access to the ever growing collection of data sources available.

        This software was created to support work on a Master's Thesis titled "ATMOS: A Data Collection and Presentaiton Framework for the Nevada Climate Change Portal".  Please direct any questions or comments to <a href=mailto:andy.dittrich@gmail.com>Andy Dittrich</a>.
    </p>
</asp:Content>
