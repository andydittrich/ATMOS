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

// this script creates a new map control, and adds or removes layers on it.

var map = null;

// Munich: var mapCoords = new Microsoft.Maps.Location(48.45926, 10.89843);
// New Mexico: var mapCoords = new Microsoft.Maps.Location(34.597042, -106.259766);
// Nevada:
var mapCoords = new Microsoft.Maps.Location(39.554883, -116.960449);

function GetMap() {
    // construct a map.
    mapAttributes = { credentials: /* TODO: insert your credentials here */,
                      center: mapCoords,
                      mapTypeId: Microsoft.Maps.MapTypeId.aerial,
                      zoom: 6 };
    map = new Microsoft.Maps.Map(document.getElementById("BingMapPanel"), mapAttributes);

    // add a tile layer for each active layer specified.  the activeLayers array should be 
    // constructed dynamically by the code-behind.  Each entry in the array should be an
    // object with 'serviceID' and 'layer' members to specify the service and layer to
    // retrieve, respectively.  An optional options object may be specified. For example:
    // var activeLayers = [{ serviceID: 3, layer: "mrcog10", options: {opacity: 0.5}},
    //                     { serviceID: 22, layer: "Map Background - Major Roads"}];
    if (activeLayers != null) {
        for (index in activeLayers) {
            activeLayer = activeLayers[index];
            AddTileLayer(activeLayer.serviceID, activeLayer.layer, activeLayer.options);
        }
    }
}

function AddTileLayer(serviceID, layer, options) {
    // create a tile layer.  The UriConstructor defines the source, and is called for each tile.  
    // In this case, we provide a javascript method that is called passing in the tile parameter.  
    // This method returns the url to retrieve the tile.  The Url is an HttpHandler that forwards.
    // the request to the underlying service.  We create a closure here to allow including the 
    // service ID and layer name in the request.
    var constructorMethod = function (tile) { return "./BingMapTile.axd?x=" + tile.x + "&y=" + tile.y + "&lod=" + tile.levelOfDetail + "&serviceID=" + serviceID + "&layer=" + layer; }
    var tileSource = new Microsoft.Maps.TileSource({ uriConstructor: constructorMethod })
    // construct the layer options.  Create a new one if it doesn't exist
    if (options == null) {
        options = { }
    }
    // set the mercator to our tile source
    options.mercator = tileSource;
    // set a longer timeout, in ms, because some services (like New Mexico RGIS) are really slow
    options.downloadTimeout = 3 * 60 * 1000;
    // if the opacity was not set, set it to 1 here
    if (options.opacity == null) {
        options.opacity = 1;
    }
    var tileLayer = new Microsoft.Maps.TileLayer(options);
    map.entities.push(tileLayer);
}

function HideTileLayer(serviceID) {
    // Remove the associated entity from the map
}

function ServiceChecked(checkbox, serviceID) {
    if (checkbox.checked) {
        AddTileLayer(serviceID);
    } else {
        HideTileLayer(serviceID);
    }
}

// make this script play nicely with the ASP.NET ScriptManager.  Tell it when it is done loading.  This must be the last line in the script
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
