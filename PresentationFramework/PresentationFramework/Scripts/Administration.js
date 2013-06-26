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

// This is a collection of scripts used on the Administration.aspx page.

function updateHelpTextFromSelectControl (selectControl) {
    // get the currently selected type from the passed in select control
    selectedIndex = selectControl.selectedIndex;
    selectedType = selectControl.options[selectedIndex].value;
    updateHelpText(selectedType);
}

function updateHelpText(serviceType) {
    // look up the help text for this type from the serviceHelpMap.  This map is produced dynamically 
    // when the page loads from the code behind.  It maps 'type' to 'help', and should look 
    // something like this:
    // var serviceHelpMap = =[{ type: "ArcGISMapService", help: "ArcGIS help text" },
    //                        { type: "WebMapService", help: "Web Map Service help text" }];
    if (serviceHelpMap != null) {
        for (index in serviceHelpMap) {
            serviceHelp = serviceHelpMap[index];
            if (serviceHelp.type == selectedType) {
                helpText = document.getElementById("HelpText");
                helpText.innerText = serviceHelp.help;
            }
        }
    }
}

// make this script play nicely with the ASP.NET ScriptManager.  Tell it when it is done loading.  
// This must be the last line in the script
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
