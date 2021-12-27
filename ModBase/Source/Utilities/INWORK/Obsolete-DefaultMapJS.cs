//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Botman
//{
//    class Default
//    {

//        public static string MapFile = "var mapinfo = {\n" +

//    "regionsize: 512,\n" +
//    "chunksize: 16,\n" +
//    "tilesize: 128,\n" +
//    "maxzoom: 4\n" +
//"}\n" +

//    "function InitMap()\n" +
//    "{\n" +
//        // ===============================================================================================
//        // 7dtd coordinate transformations

//        "SDTD_Projection = {\n" +
//            "project: function(latlng) {\n" +

//                "return new L.Point(\n" +
//                    "(latlng.lat) / Math.pow(2, mapinfo.maxzoom),\n" +
//                    "(latlng.lng) / Math.pow(2, mapinfo.maxzoom));\n" +
//            "},\n" +

//        "unproject: function(point) {\n" +
//                "return new L.LatLng(\n" +
//                    "point.x * Math.pow(2, mapinfo.maxzoom),\n" +
//                    "point.y * Math.pow(2, mapinfo.maxzoom));\n" +
//            "}\n" +
//        "};\n" +

//        "SDTD_CRS = L.extend({ }, L.CRS.Simple, {\n" +
//            "projection: SDTD_Projection,\n" +

//"        transformation: new L.Transformation(1, 0, -1, 0),\n" +

//"        scale: function(zoom) {\n" +
//"                return Math.pow(2, zoom);\n" +
//"            }\n" +

//"        });\n" +

//        // ===============================================================================================
//        // Map and basic tile layers

//"        map = L.map('tab_map', {\n" +
//"            zoomControl: false,\n" + // Added by Zoomslider
//"        zoomsliderControl: true,\n" +
//"        attributionControl: false,\n" +
//"        crs: SDTD_CRS\n" +


//"    }).setView([0, 0], Math.max(0, mapinfo.maxzoom - 5));\n" +


//"        var initTime = new Date().getTime();\n" +
//"        var tileLayer = GetSdtdTileLayer(mapinfo, initTime);\n" +
//"        var tileLayerMiniMap = GetSdtdTileLayer(mapinfo, initTime, true);\n" +

//        // player icon
//"        var playerIcon = L.icon({\n" +
//"        iconUrl: '/static/leaflet/images/marker-survivor.png',\n" +
//"        iconRetinaUrl: '/static/leaflet/images/marker-survivor-2x.png',\n" +
//"        iconSize: [25, 48],\n" +
//"        iconAnchor: [12, 24],\n" +
//"        popupAnchor: [0, -20]\n" +


//"    });\n" +

//        // hostile icon
//"        var hostileIcon = L.icon({\n" +
//"            iconUrl: '/static/leaflet/images/marker-zombie.png',\n" +

//"        iconRetinaUrl: '/static/leaflet/images/marker-zombie-2x.png',\n" +
//"        iconSize: [25, 33],\n" +
//"        iconAnchor: [12, 16],\n" +
//"        popupAnchor: [0, -10]\n" +


//"    });\n" +

//        // animal icon
//"        var animalIcon = L.icon({\n" +
//"            iconUrl: '/static/leaflet/images/marker-animal.png',\n" +
//"        iconRetinaUrl: '/static/leaflet/images/marker-animal-2x.png',\n" +
//"        iconSize: [25, 26],\n" +
//"        iconAnchor: [12, 13],\n" +
//"        popupAnchor: [0, -10]\n" +


//"    });\n" +




//        // ===============================================================================================
//        // Overlays and controls

//"        var playersOnlineMarkerGroup = L.markerClusterGroup({\n" +
//"            maxClusterRadius: function(zoom) { return zoom >= mapinfo.maxzoom ? 10 : 50; }\n" +
//"        });\n" +
//"        var playersOfflineMarkerGroup = L.markerClusterGroup({\n" +
//"            maxClusterRadius: function(zoom) { return zoom >= mapinfo.maxzoom ? 10 : 50; }\n" +
//"        });\n" +
//"        var hostilesMarkerGroup = L.markerClusterGroup({\n" +
//"            maxClusterRadius: function(zoom) { return zoom >= mapinfo.maxzoom ? 10 : 50; }\n" +
//"        });\n" +
//"        var animalsMarkerGroup = L.markerClusterGroup({\n" +
//"            maxClusterRadius: function(zoom) { return zoom >= mapinfo.maxzoom ? 10 : 50; }\n" +
//"        });\n" +

//"        var densityMismatchMarkerGroupAir = L.markerClusterGroup({\n" +
//"            maxClusterRadius: function(zoom) { return zoom >= mapinfo.maxzoom ? 10 : 50; }\n" +
//"        });\n" +
//"        var densityMismatchMarkerGroupTerrain = L.markerClusterGroup({\n" +
//"            maxClusterRadius: function(zoom) { return zoom >= mapinfo.maxzoom ? 10 : 50; }\n" +
//"        });\n" +
//"        var densityMismatchMarkerGroupNonTerrain = L.markerClusterGroup({\n" +
//"            maxClusterRadius: function(zoom) { return zoom >= mapinfo.maxzoom ? 10 : 50; }\n" +
//"        });\n" +


//"        var layerControl = L.control.layers({\n" +
////"Map": tileLayer
//"        }, null, {\n" +
//"            collapsed: false\n" +

//"        }\n" +
//"	);\n" +

//"        var layerCount = 0;\n" +


//        "tileLayer.addTo(map);\n" +

//"        new L.Control.Coordinates({ }).addTo(map);\n" +

//"        new L.Control.ReloadTiles({\n" +
//"        autoreload_enable: true,\n" +
//"        autoreload_minInterval: 30,\n" +
//"        autoreload_interval: 120,\n" +
//"        autoreload_defaultOn: false,\n" +
//"        layers: [tileLayer, tileLayerMiniMap]\n" +


//"    }).addTo(map);\n" +

//"    layerControl.addOverlay(GetRegionLayer (mapinfo), \"Region files\");\n" +

//"    layerCount++;\n" +

//"    var miniMap = new L.Control.MiniMap(tileLayerMiniMap, {\n" +

// "       zoomLevelOffset: -6,\n" +
//"        toggleDisplay: true\n" +

//"    }).addTo(map);\n" +

//"var measure = L.control.measure({\n" +
//"        units: {\n" +

//"        sdtdMeters: {\n" +

//"            factor: 0.00001,\n" +
//"            display: 'XMeters',\n" +
//"            decimals: 0\n" +

//"            },\n" +
//"            sdtdSqMeters: {\n" +
//"                factor: 0.000000001,\n" +
//"                display: 'XSqMeters',\n" +
//"                decimals: 0\n" +

//"            }\n" +

//"        },\n" +
//"        primaryLengthUnit: \"sdtdMeters\",\n" +
//"		primaryAreaUnit: \"sdtdSqMeters\",\n" +
////activeColor: "#ABE67E",
////completedColor: "#C8F2BE",
//"		position: \"bottomleft\"\n" +
//"	});\n" +
////measure.addTo(map);

//"	new L.Control.GameTime({}).addTo(map);\n" +

//"	if (HasPermission (\"webapi.getlandclaims\")) {\n" +
//"		layerControl.addOverlay(GetLandClaimsLayer (map, mapinfo), \"Land claims\");\n" +

//"        layerCount++;\n" +
//"	}\n" +

//"	if (HasPermission (\"webapi.gethostilelocation\")) {\n" +
//"		layerControl.addOverlay(hostilesMarkerGroup, \"Hostiles (<span id='mapControlHostileCount'>0</span>)\");\n" +
//"		layerCount++;\n" +
//"	}\n" +

//"	if (HasPermission (\"webapi.getanimalslocation\")) {\n" +
//"		layerControl.addOverlay(animalsMarkerGroup, \"Animals (<span id='mapControlAnimalsCount'>0</span>)\");\n" +
//"		layerCount++;\n" +
//"	}\n" +

//"	if (HasPermission (\"webapi.getplayerslocation\")) {\n" +
//"		layerControl.addOverlay(playersOfflineMarkerGroup, \"Players (offline) (<span id='mapControlOfflineCount'>0</span>)\");\n" +

//"        layerControl.addOverlay(playersOnlineMarkerGroup, \"Players (online) (<span id='mapControlOnlineCount'>0</span>)\");\n" +

//"        layerCount++;\n" +
//"	}\n" +

//"	if (layerCount > 0) {\n" +
//"		layerControl.addTo(map);\n" +
//"	}\n" +




//"	var hostilesMappingList = { };\n" +
//"var animalsMappingList = { };\n" +
//"var playersMappingList = { }; \n" +


//// ===============================================================================================
//// Player markers

//"// BOTMAN RESET REGIONS\n" +

//"	$(\".leaflet-popup-pane\").on('click.action', '.inventoryButton', function(event)\n" +

//"{\n" +
//"    ShowInventoryDialog($(this).data('steamid'));\n" +
//"});\n" +

//"	var updatingMarkers = false;\n" +


//"var setPlayerMarkers = function(data) {\n" +
//"		var onlineIds = [];\n" +
//"updatingMarkers = true;\n" +
//"		$.each(data, function(key, val) {\n" +
//"    var marker;\n" +
//"    if (playersMappingList.hasOwnProperty(val.steamid))\n" +
//"    {\n" +
//"        marker = playersMappingList[val.steamid].currentPosMarker;\n" +
//"    }\n" +
//"    else\n" +
//"    {\n" +
//"        marker = L.marker([val.position.x, val.position.z], { icon: playerIcon}).bindPopup(\n" +
//"\"Player: \" + $(\"<div>\").text(val.name).html() +\n" +
//"(HasPermission(\"webapi.getplayerinventory\") ?\n" +
//"\"<br/><a class='inventoryButton' data-steamid='\" + val.steamid + \"'>Show inventory</a>\"\n" +
//": \"\")\n" +
//");\n" +
//"        marker.on(\"move\", function(e) {\n" +
//"            if (this.isPopupOpen())\n" +
//"            {\n" +
//"                map.flyTo(e.latlng, map.getZoom());\n" +
//"            }\n" +
//"        });\n" +
//"        playersMappingList[val.steamid] = { online: !val.online };\n" +
//"    }\n" +

//"    if (val.online)\n" +
//"    {\n" +
//"        onlineIds.push(val.steamid);\n" +
//"    }\n" +

//"    oldpos = marker.getLatLng();\n" +
//"    if (playersMappingList[val.steamid].online != val.online)\n" +
//"    {\n" +
//"        if (playersMappingList[val.steamid].online)\n" +
//"        {\n" +
//"            playersOnlineMarkerGroup.removeLayer(marker);\n" +
//"            playersOfflineMarkerGroup.addLayer(marker);\n" +
//"        }\n" +
//"        else\n" +
//"        {\n" +
//"            playersOfflineMarkerGroup.removeLayer(marker);\n" +
//"            playersOnlineMarkerGroup.addLayer(marker);\n" +
//"        }\n" +
//"    }\n" +
//"    if (oldpos.lat != val.position.x || oldpos.lng != val.position.z)\n" +
//"    {\n" +
//"        marker.setLatLng([val.position.x, val.position.z]);\n" +
//"        if (val.online)\n" +
//"        {\n" +
//"            marker.setOpacity(1.0);\n" +
//"        }\n" +
//"        else\n" +
//"        {\n" +
//"            marker.setOpacity(0.5);\n" +
//"        }\n" +
//"    }\n" +

//"    val.currentPosMarker = marker;\n" +
//"    playersMappingList[val.steamid] = val;\n" +
//"});\n" +

//"		var online = 0;\n" +
//"var offline = 0;\n" +
//"		$.each(playersMappingList, function (key, val) {\n" +
//"    if (val.online && onlineIds.indexOf(key) < 0)\n" +
//"    {\n" +
//"        var marker = val.currentPosMarker;\n" +
//"        playersOnlineMarkerGroup.removeLayer(marker);\n" +
//"        playersOfflineMarkerGroup.addLayer(marker);\n" +
//"        val.online = false;\n" +
//"    }\n" +
//"    if (val.online)\n" +
//"    {\n" +
//"        online++;\n" +
//"    }\n" +
//"    else\n" +
//"    {\n" +
//"        offline++;\n" +
//"    }\n" +
//"});\n" +

//"		updatingMarkers = false;\n" +

//"		$( \"#mapControlOnlineCount\" ).text(online );\n" +
//"		$( \"#mapControlOfflineCount\" ).text(offline );\n" +
//"	}\n" +

//"	var updatePlayerTimeout;\n" +
//"var playerUpdateCount = -1;\n" +
//"var updatePlayerEvent = function() {\n" +
//"		playerUpdateCount++;\n" +

//"		$.getJSON( \"../api/getplayerslocation\" + ((playerUpdateCount % 15) == 0 ? \"?offline=true\" : \"\"))\n" +
//"		.done(setPlayerMarkers)\n" +
//"		.fail(function(jqxhr, textStatus, error) {\n" +
//"    console.log(\"Error fetching players list\");\n" +
//"})\n" +
//"		.always(function() {\n" +
//"    updatePlayerTimeout = window.setTimeout(updatePlayerEvent, 4000);\n" +
//"});\n" +
//"	}\n" +

//"	tabs.on(\"tabbedcontenttabopened\", function(event, data)\n" +
//"{\n" +
//"    if (data.newTab === \"#tab_map\")\n" +
//"    {\n" +
//"        if (HasPermission(\"webapi.getplayerslocation\"))\n" +
//"        {\n" +
//"            updatePlayerEvent();\n" +
//"        }\n" +
//"    }\n" +
//"    else\n" +
//"    {\n" +
//"        window.clearTimeout(updatePlayerTimeout);\n" +
//"    }\n" +
//"});\n" +

//"	if (tabs.tabbedContent (\"isTabOpen\", \"tab_map\")) {\n" +
//"		if (HasPermission (\"webapi.getplayerslocation\")) {\n" +
//"			updatePlayerEvent();\n" +
//"		}\n" +
//"	}\n" +




//// ===============================================================================================
//// Hostiles markers

//"	var setHostileMarkers = function(data) {\n" +
//"		updatingMarkersHostile = true;\n" +

//"		var hostileCount = 0;\n" +

//"hostilesMarkerGroup.clearLayers();\n" +

//"		$.each(data, function(key, val) {\n" +
//"    var marker;\n" +
//"    if (hostilesMappingList.hasOwnProperty(val.id))\n" +
//"    {\n" +
//"        marker = hostilesMappingList[val.id].currentPosMarker;\n" +
//"    }\n" +
//"    else\n" +
//"    {\n" +
//"        marker = L.marker([val.position.x, val.position.z], { icon: hostileIcon}).bindPopup(\n" +
//"\"Hostile: \" + val.name\n" +
//");\n" +
//"        //hostilesMappingList[val.id] = { };\n" +
//"        hostilesMarkerGroup.addLayer(marker);\n" +
//"    }\n" +

//"    var bAbort = false;\n" +

//"    oldpos = marker.getLatLng();\n" +

//"    //if ( oldpos.lat != val.position.x || oldpos.lng != val.position.z ) {\n" +
//"    //	hostilesMarkerGroup.removeLayer(marker);\n" +
//"    marker.setLatLng([val.position.x, val.position.z]);\n" +
//"    marker.setOpacity(1.0);\n" +
//"    hostilesMarkerGroup.addLayer(marker);\n" +
//"    //}\n" +

//"    val.currentPosMarker = marker;\n" +
//"    hostilesMappingList[val.id] = val;\n" +

//"    hostileCount++;\n" +
//"});\n" +

//"		$( \"#mapControlHostileCount\" ).text(hostileCount );\n" +

//"updatingMarkersHostile = false;\n" +
//"	}\n" +

//"	var updateHostileTimeout;\n" +
//"var updateHostileEvent = function() {\n" +
//"		$.getJSON( \"../api/gethostilelocation\")\n" +
//"		.done(setHostileMarkers)\n" +
//"		.fail(function(jqxhr, textStatus, error) {\n" +
//"    console.log(\"Error fetching hostile list\");\n" +
//"})\n" +
//"		.always(function() {\n" +
//"    updateHostileTimeout = window.setTimeout(updateHostileEvent, 4000);\n" +
//"});\n" +
//"	}\n" +

//"	tabs.on(\"tabbedcontenttabopened\", function(event, data)\n" +
//"{\n" +
//"    if (data.newTab === \"#tab_map\")\n" +
//"    {\n" +
//"        if (HasPermission(\"webapi.gethostilelocation\"))\n" +
//"        {\n" +
//"            updateHostileEvent();\n" +
//"        }\n" +
//"    }\n" +
//"    else\n" +
//"    {\n" +
//"        window.clearTimeout(updateHostileTimeout);\n" +
//"    }\n" +
//"});\n" +

//"	if (tabs.tabbedContent (\"isTabOpen\", \"tab_map\")) {\n" +
//"		if (HasPermission (\"webapi.gethostilelocation\")) {\n" +
//"			updateHostileEvent();\n" +
//"		}\n" +
//"	}\n" +



//// ===============================================================================================
//// Animals markers

//"	var setAnimalMarkers = function(data) {\n" +
//"		updatingMarkersAnimals = true;\n" +

//"		var animalsCount = 0;\n" +

//"animalsMarkerGroup.clearLayers();\n" +

//"		$.each(data, function(key, val) {\n" +
//"    var marker;\n" +
//"    if (animalsMappingList.hasOwnProperty(val.id))\n" +
//"    {\n" +
//"        marker = animalsMappingList[val.id].currentPosMarker;\n" +
//"    }\n" +
//"    else\n" +
//"    {\n" +
//"        marker = L.marker([val.position.x, val.position.z], { icon: animalIcon}).bindPopup(\n" +
//"\"Animal: \" + val.name\n" +
//");\n" +
//"        //animalsMappingList[val.id] = { };\n" +
//"        animalsMarkerGroup.addLayer(marker);\n" +
//"    }\n" +

//"    var bAbort = false;\n" +

//"    oldpos = marker.getLatLng();\n" +

//"    //if ( oldpos.lat != val.position.x || oldpos.lng != val.position.z ) {\n" +
//"    //	animalsMarkerGroup.removeLayer(marker);\n" +
//"    marker.setLatLng([val.position.x, val.position.z]);\n" +
//"    marker.setOpacity(1.0);\n" +
//"    animalsMarkerGroup.addLayer(marker);\n" +
//"    //}\n" +

//"    val.currentPosMarker = marker;\n" +
//"    animalsMappingList[val.id] = val;\n" +

//"    animalsCount++;\n" +
//"});\n" +

//"		$( \"#mapControlAnimalsCount\" ).text(animalsCount );\n" +

//"updatingMarkersAnimals = false;\n" +
//"	}\n" +

//"	var updateAnimalsTimeout;\n" +
//"var updateAnimalsEvent = function() {\n" +
//"		$.getJSON( \"../api/getanimalslocation\")\n" +
//"		.done(setAnimalMarkers)\n" +
//"		.fail(function(jqxhr, textStatus, error) {\n" +
//"    console.log(\"Error fetching animals list\");\n" +
//"})\n" +
//"		.always(function() {\n" +
//"    updateAnimalsTimeout = window.setTimeout(updateAnimalsEvent, 4000);\n" +
//"});\n" +
//"	}\n" +

//"	tabs.on(\"tabbedcontenttabopened\", function(event, data)\n" +
//"{\n" +
//"    if (data.newTab === \"#tab_map\")\n" +
//"    {\n" +
//"        if (HasPermission(\"webapi.getanimalslocation\"))\n" +
//"        {\n" +
//"            updateAnimalsEvent();\n" +
//"        }\n" +
//"    }\n" +
//"    else\n" +
//"    {\n" +
//"        window.clearTimeout(updateAnimalsTimeout);\n" +
//"    }\n" +
//"});\n" +

//"	if (tabs.tabbedContent (\"isTabOpen\", \"tab_map\")) {\n" +
//"		if (HasPermission (\"webapi.getanimalslocation\")) {\n" +
//"			updateAnimalsEvent();\n" +
//"		}\n" +
//"	}\n" +

//"	// =============================================================================================== \n" +
//"	// Density markers\n" +

//"	var setDensityMarkers = function(data) {\n" +
//"		var densityCountAir = 0;\n" +
//"var densityCountTerrain = 0;\n" +
//"var densityCountNonTerrain = 0;\n" +

//"densityMismatchMarkerGroupAir.clearLayers();\n" +
//"		densityMismatchMarkerGroupTerrain.clearLayers();\n" +
//"		densityMismatchMarkerGroupNonTerrain.clearLayers();\n" +


//"		var downloadCsv = true;\n" +
//"var downloadJson = false;\n" +

//"		if (downloadJson) {\n" +
//"			var jsonAir = [];\n" +
//"var jsonTerrain = [];\n" +
//"var jsonNonTerrain = [];\n" +
//"		}\n" +
//"		if (downloadCsv) {\n" +
//"			var csvAir = \"x;y;z;Density;IsTerrain;BvType\";\n" +
//"var csvTerrain = \"x;y;z;Density;IsTerrain;BvType\";\n" +
//"var csvNonTerrain = \"x;y;z;Density;IsTerrain;BvType\";\n" +
//"		}\n" +

//"		$.each(data, function(key, val) {\n" +
//"    if (val.bvtype == 0)\n" +
//"    {\n" +
//"        marker = L.marker([val.x, val.z]).bindPopup(\n" +
//"            \"Density Mismatch: <br>Position: \" + val.x + \" \" + val.y + \" \" + val.z + \"<br>Density: \" + val.density + \"<br>isTerrain: \" + val.terrain + \"<br>bv.type: \" + val.bvtype);\n" +
//"        densityMismatchMarkerGroupAir.addLayer(marker);\n" +
//"        densityCountAir++;\n" +
//"        if (downloadJson)\n" +
//"        {\n" +
//"            jsonAir.push(val);\n" +
//"        }\n" +
//"        if (downloadCsv)\n" +
//"        {\n" +
//"            csvAir += val.x + \";\" + val.y + \";\" + val.z + \";\" + val.density + \";\" + val.terrain + \";\" + val.bvtype + \"\";\n" +
//"        }\n" +
//"    }\n" +
//"    else if (val.terrain)\n" +
//"    {\n" +
//"        marker = L.marker([val.x, val.z]).bindPopup(\n" +
//"            \"Density Mismatch: <br>Position: \" + val.x + \" \" + val.y + \" \" + val.z + \"<br>Density: \" + val.density + \"<br>isTerrain: \" + val.terrain + \"<br>bv.type: \" + val.bvtype\n" +
//"        );\n" +
//"        densityMismatchMarkerGroupTerrain.addLayer(marker);\n" +
//"        densityCountTerrain++;\n" +
//"        if (downloadJson)\n" +
//"        {\n" +
//"            jsonTerrain.push(val);\n" +
//"        }\n" +
//"        if (downloadCsv)\n" +
//"        {\n" +
//"            csvTerrain += val.x + \";\" + val.y + \";\" + val.z + \";\" + val.density + \";\" + val.terrain + \";\" + val.bvtype + \"\";\n" +
//"        }\n" +
//"    }\n" +
//"    else\n" +
//"    {\n" +
//"        marker = L.marker([val.x, val.z]).bindPopup(\n" +
//"            \"Density Mismatch: <br>Position: \" + val.x + \" \" + val.y + \" \" + val.z + \"<br>Density: \" + val.density + \"<br>isTerrain: \" + val.terrain + \"<br>bv.type: \" + val.bvtype\n" +
//"        );\n" +
//"        densityMismatchMarkerGroupNonTerrain.addLayer(marker);\n" +
//"        densityCountNonTerrain++;\n" +
//"        if (downloadJson)\n" +
//"        {\n" +
//"            jsonNonTerrain.push(val);\n" +
//"        }\n" +
//"        if (downloadCsv)\n" +
//"        {\n" +
//"            csvNonTerrain += val.x + \";\" + val.y + \";\" + val.z + \";\" + val.density + \";\" + val.terrain + \";\" + val.bvtype + \"\";\n" +
//"        }\n" +
//"    }\n" +
//"});\n" +

//"		layerControl.addOverlay(densityMismatchMarkerGroupAir, \"Density Mismatches Air (<span id='mapControlDensityCountAir'>0</span>)\");\n" +
//"		layerControl.addOverlay(densityMismatchMarkerGroupTerrain, \"Density Mismatches Terrain (<span id='mapControlDensityCountTerrain'>0</span>)\");\n" +
//"		layerControl.addOverlay(densityMismatchMarkerGroupNonTerrain, \"Density Mismatches NonTerrain (<span id='mapControlDensityCountNonTerrain'>0</span>)\");\n" +

//"		$( \"#mapControlDensityCountAir\" ).text(densityCountAir );\n" +
//"		$( \"#mapControlDensityCountTerrain\" ).text(densityCountTerrain );\n" +
//"		$( \"#mapControlDensityCountNonTerrain\" ).text(densityCountNonTerrain );\n" +

//"		if (downloadJson) {\n" +
//"			download(\"air-negative-density.json\", JSON.stringify(jsonAir, null, '\t'));\n" +
//"			download(\"terrain-positive-density.json\", JSON.stringify(jsonTerrain, null, '\t'));\n" +
//"			download(\"nonterrain-negative-density.json\", JSON.stringify(jsonNonTerrain, null, '\t'));\n" +
//"		}\n" +
//"		if (downloadCsv) {\n" +
//"			download(\"air-negative-density.csv\", csvAir);\n" +
//"download(\"terrain-positive-density.csv\", csvTerrain);\n" +
//"download(\"nonterrain-negative-density.csv\", csvNonTerrain);\n" +
//"		}\n" +

//"		function download(filename, text)\n" +
//"{\n" +
//"    var element = document.createElement('a');\n" +
//"    var file = new Blob([text], { type: 'text/plain'});\n" +
//"    element.href = URL.createObjectURL(file);\n" +
//"    element.download = filename;\n" +

//"    element.style.display = 'none';\n" +
//"    document.body.appendChild(element);\n" +

//"    element.click();\n" +

//"    document.body.removeChild(element);\n" +
//"}\n" +
//"	}\n" +

//"	$.getJSON(\"densitymismatch.json\")\n" +
//"	.done(setDensityMarkers)\n" +
//"	.fail(function(jqxhr, textStatus, error) {\n" +
//"    console.log(\"Error fetching density mismatch list\");\n" +
//"});\n" +

//"}\n" +





//"function StartMapModule()\n" +
//"{\n" +
//"	$.getJSON(\"../map/mapinfo.json\")\n" +
//"    .done(function(data) {\n" +
//"        mapinfo.tilesize = data.blockSize;\n" +
//"        mapinfo.maxzoom = data.maxZoom;\n" +
//"    })\n" +
//"	.fail(function(jqxhr, textStatus, error) {\n" +
//"        console.log(\"Error fetching map information\");\n" +
//"    })\n" +
//"	.always(function() {\n" +
//"        InitMap();\n" +
//"});\n" +
//"}\n";

//    }
//}
