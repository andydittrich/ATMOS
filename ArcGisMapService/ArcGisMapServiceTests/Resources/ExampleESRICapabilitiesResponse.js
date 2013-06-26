{
    "currentVersion": 10.1,
    "serviceDescription": "Test Map Service Description",
    "mapName": "Street Map Pro Data",
    "description": "Street Map USA",
    "copyrightText": "ESRI",
    "supportsDynamicLayers": false,
    "layers": [
        {
            "id": 0,
            "name": "Cities",
            "defaultVisibility": true,
            "parentLayerId": -1,
            "subLayerIds": null,
            "minScale": 0,
            "maxScale": 0
        },
        {
            "id": 1,
            "name": "States",
            "defaultVisibility": true,
            "parentLayerId": -1,
            "subLayerIds": null,
            "minScale": 0,
            "maxScale": 0
        },
        {
            "id": 2,
            "name": "Counties",
            "defaultVisibility": false,
            "parentLayerId": -1,
            "subLayerIds": [
                3,
                4
            ],
            "minScale": 0,
            "maxScale": 0
        },
        {
            "id": 3,
            "name": "Large Counties",
            "defaultVisibility": false,
            "parentLayerId": 2,
            "subLayerIds": null,
            "minScale": 0,
            "maxScale": 0
        },
        {
            "id": 4,
            "name": "Small Counties",
            "defaultVisibility": false,
            "parentLayerId": 2,
            "subLayerIds": null,
            "minScale": 0,
            "maxScale": 0
        }
    ],
    "spatialReference": {
        "wkid": 4326
    },
    "singleFusedMapCache": true,
    "tileInfo": {
        "rows": 512,
        "cols": 512,
        "dpi": 96,
        "format": "JPEG",
        "compressionQuality": 75,
        "origin": {
            "x": -130,
            "y": 50
        },
        "spatialReference": {
            "wkid": 4326
        },
        "lods": [
            {
                "level": 0,
                "resolution": 8.46,
                "scale": 32000
            },
            {
                "level": 1,
                "resolution": 4.23,
                "scale": 16000
            },
            {
                "level": 2,
                "resolution": 2.11,
                "scale": 8000
            },
            {
                "level": 3,
                "resolution": 1.05,
                "scale": 4000
            },
            {
                "level": 4,
                "resolution": 0.52,
                "scale": 2000
            }
        ]
    },
    "initialExtent": {
        "xmin": -109.55,
        "ymin": 25.76,
        "xmax": -86.39,
        "ymax": 49.94,
        "spatialReference": {
            "wkid": 4326
        }
    },
    "fullExtent": {
        "xmin": -130,
        "ymin": 24,
        "xmax": -65,
        "ymax": 50,
        "spatialReference": {
            "wkid": 4326
        }
    },
    "units": "esriDecimalDegrees",
    "supportedImageFormatTypes": "PNG32,PNG24,PNG,JPG,DIB,TIFF,EMF,PS,PDF,GIF,SVG,SVGZ",
    "documentInfo": {
        "Title": "StreetMap USA.mxd",
        "Author": "ESRI Data Team",
        "Comments": "ESRI Data and Maps 2004",
        "Subject": "Street level data for the US",
        "Category": "vector",
        "Keywords": "StreetMap USA"
    },
    "capabilities": "Map,Query,Data",
    "maxRecordCount": 1000,
    "maxImageHeight": 2048,
    "maxImageWidth": 2048,
    "minScale": 0,
    "maxScale": 0,
    "tileServers": ["http://myserver/arcgis/rest/services/basemap/MapServer","http://myserver2/arcgis/rest/services/basemap/MapServer"],
    "supportedQueryFormats": "JSON, AMF"
}