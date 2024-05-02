﻿/*
 * Park Alerts System
 * Version 4.1
 * https://parks.dpaw.wa.gov.au/
 *
 * Interactive leaflet map for the public interface
 *
 * Copyright (c) 2017 Department of Biodiversity, Conservation and Attractions
 * Author: Mohammed Tajuddin, mohammed.tajuddin@dbca.wa.gov.au
 * Parks and Wildlife Service
 * Parks and Visitor Services
 * Use closure compiler to compile the code https://closure-compiler.appspot.com/home
 * Last update - 15/05/2018
 */
var baseUrl = document.location.origin;
var appUrl = baseUrl; // + "/alerts";

$(function () {

    //bind the datatable on DOM ready

    

            
    $('#alertDetails').on('shown.bs.modal', function (e) {
        var element = $(e.relatedTarget);
        var data = element.data("value");
        var url = appUrl + "/Home/ViewAlert/" + data;
        //fix the safari issue
        e.stopPropagation();
        e.preventDefault();
        $("#alertContent").load(url,
            function (response, status, xhr) {
                if (status === "error") {
                    var msg = "Sorry but there was an error loading data from the server!";
                    $("#error").html(msg + xhr.status + " " + xhr.statusText);
                    $(".loadingOverlay").hide();
                }
                else {
                    var fu = appUrl + "/Home/Share/" + data;
                    var tu = appUrl + "/Home/Index%23" + data;
                    var ttl = $("#ttl").text();
                    var fb = $("#fb"), tw= $("#twt");
                    fb.attr("href", "https://www.facebook.com/sharer/sharer.php?u=" + tu + "&quote="+ttl);
                    tw.attr("href", "https://twitter.com/intent/tweet?text=" + ttl +"%0A" + tu);
                   
                    $(".loadingOverlay").hide();
                    $('div#share-div').removeAttr('class', 'hidden').attr('class','pull-left');
                }
            });
       
    });


    //open print option
    $(document).on('click', '#prnt', function (e) {

        var pcontent = $('#alertContent').html();
        var mywindow = window.open('', 'PRINT', 'height=600,width=800');

        mywindow.document.write('<html><head><title>' + document.title + '</title>');
        mywindow.document.write('</head><body >');
        mywindow.document.write('<h3>' + document.title + '</h3><hr />');
        mywindow.document.write(pcontent);
        mywindow.document.write('</body></html>');

        mywindow.document.close(); // necessary for IE >= 10
        mywindow.focus(); // necessary for IE >= 10*/

        mywindow.print();
        mywindow.close();

        e.preventDefault();

    });

    $('#siteInfo').on('shown.bs.modal', function (e) {
        var element = $(e.relatedTarget);
        var data = element.data("value");
        var url = appUrl + "/Home/Details/" + data;
        //fix the safari issue
        e.stopPropagation();
        e.preventDefault();
        $("#siteContent").load(url);
    });

    //load help content
    $('#help').on('shown.bs.modal', function (e) {
       
        var url = appUrl + "/Home/Help/";
        $("#helpContent").load(url);
        //fix the safari issue
        e.stopPropagation();
        e.preventDefault();
    });
    
});


$(function(e){
 
        var lcontrol = new L.control.layers();
        var eb = new L.control.layers();
        //initialize map
        var map = null;
        var markers = [];
        var latlng = L.latLng(-30.81881, 116.16596);
       //set the layers
	    //var mapbox_accesstoken = "pk.eyJ1IjoiZHBhd2FzaSIsImEiOiJtVjY5WmlFIn0.whc76euXLk2PkyxOkZ5xlQ"
		//var mapbox_accesstoken = "pk.eyJ1IjoiZHBhd2FzaSIsImEiOiJjazB3MW9iZ24weHMxM2NxcGluMml1NW80In0.v7uQYd32Ue0yoH2Bw6Inmg"
        //var satMutant = L.tileLayer("https://api.mapbox.com/v4/mapbox.satellite/{z}/{x}/{y}.png?access_token={mbkey}", {mbkey: mapbox_accesstoken});
	    //var roadMutant = L.tileLayer("https://api.mapbox.com/v4/mapbox.streets/{z}/{x}/{y}.png?access_token={mbkey}", {mbkey: mapbox_accesstoken});
		//var terrainMutant = L.tileLayer("https://api.mapbox.com/v4/mapbox.outdoors/{z}/{x}/{y}.png?access_token={mbkey}", {mbkey: mapbox_accesstoken});
        //var hybridMutant = L.tileLayer("https://api.mapbox.com/v4/mapbox.streets-satellite/{z}/{x}/{y}.png?access_token={mbkey}", {mbkey: mapbox_accesstoken});
		
		// TP - 20/10/2020 Use new mapbox layers	
        var mapbox_accesstoken = "pk.eyJ1IjoiZHBhd2FzaSIsImEiOiJjamxmeHM5OGswdnA2M3FydmJ2YW9xa2Z6In0.wAnkinx2kKEZLxU1vzvyIw";
		var roadMutant = L.tileLayer("https://api.mapbox.com/styles/v1/mapbox/streets-v12/tiles/{z}/{x}/{y}?access_token={mbkey}", {mbkey: mapbox_accesstoken});
        var satMutant = L.tileLayer("https://api.mapbox.com/styles/v1/mapbox/satellite-v9/tiles/{z}/{x}/{y}?access_token={mbkey}", {mbkey: mapbox_accesstoken});
		var terrainMutant = L.tileLayer("https://api.mapbox.com/styles/v1/mapbox/outdoors-v12/tiles/{z}/{x}/{y}?access_token={mbkey}", {mbkey: mapbox_accesstoken});
        var hybridMutant = L.tileLayer("https://api.mapbox.com/styles/v1/mapbox/satellite-streets-v12/tiles/{z}/{x}/{y}?access_token={mbkey}", { mbkey: mapbox_accesstoken });
        var darkMutant = L.tileLayer("https://api.mapbox.com/styles/v1/mapbox/dark-v11/tiles/{z}/{x}/{y}?access_token={mbkey}", { mbkey: mapbox_accesstoken });
		
		
        
        //map = new L.map('lmap', {
        //    center: latlng, minZoom:0, maxZoom: 18, scrollwheel: false, layers: [roadMutant],
        //    streetViewControl: false, scrollWheelZoom: false, 
        //});
		// TP changed the layer to SatMutant 16/10/2020
		  map = new L.map('lmap', {
              center: latlng, minZoom: 0, maxZoom: 18, scrollwheel: false, fullscreenControl: true,
              fullscreenControlOptions: {
                  position: 'topleft' // Change as per your preference
              },
              layers: [roadMutant],
            streetViewControl: false, scrollWheelZoom: false, tileSize: 512
        });
		
		window.leafletmap = map;

        
        var mun = appUrl + "/media/Munda_Biddi_Trail.gpx";
        var bib = appUrl + "/media/Bibbulmun_Track.gpx";
        var cape = appUrl + "/media/Cape_to_Cape_Track.gpx";
        var wib = appUrl + "/media/WiilmanBilya.gpx";
        
        var mtrack = new L.GPX(mun, {
            async: true,
            marker_options: {
                startIconUrl: null,
                endIconUrl: null,
                shadowUrl: null
            },
            polyline_options: { color: '#0170FF' }
        }).on("loaded", function (e) {
           // map.fitBounds(e.target.getBounds());
            });
        var btrack = new L.GPX(bib, {
            async: true,
            marker_options: { startIconUrl: null, endIconUrl: null, shadowUrl: 'images/pin-shadow.png' },
            polyline_options: { color: '#8C211F' }
        }).on("loaded", function (e) {
            //map.fitBounds(e.target.getBounds());
            });
        var ctrack = new L.GPX(cape, {
            async: true,
            marker_options: {
                startIconUrl: null,
                endIconUrl: null,
                shadowUrl: null
            },
            polyline_options: { color: '#F58025' }

        }).on("loaded", function (e) {
           // map.fitBounds(e.target.getBounds());
        });
        var wtrack = new L.GPX(wib, {
            async: true,
            marker_options: {
                startIconUrl: null,
                endIconUrl: null,
                shadowUrl: null
            },
            polyline_options: { color: '#FF771C' }
        }).on("loaded", function (e) {
            // map.fitBounds(e.target.getBounds());
        });
        map.addLayer(mtrack);
        map.addLayer(btrack);
        map.addLayer(ctrack);
        map.addLayer(wtrack);
        //var kmlLayer = new L.KML("/media/Cape_to_Cape_Track.kml", { async: true });

        //kmlLayer.on("loaded", function (e) {
        //    map.fitBounds(e.target.getBounds());
        //});

        //map.addLayer(kmlLayer);
   
       // map.dragging.disable();
       // map.touchZoom.disable();
       // map.scrollWheelZoom.disable();

        map.on('focus', function () { map.scrollWheelZoom.enable(); });
        map.on('blur', function () { map.scrollWheelZoom.disable(); });

    var baseLayers = {
                "Streets": roadMutant,
                "Satellite": satMutant,
                "Terrain": terrainMutant,
                "Hybrid": hybridMutant,
                "Dark": darkMutant,
            
        };

    map.addControl(new L.Control.Layers(baseLayers, { 'Munda Biddi': mtrack, 'Cape to Cape': ctrack, 'Bibbulmun': btrack, 'Wiilman Bilya': wtrack }, { collapsed: true }));
   

   // map.addLayer(kmlLayer);
    var hash = window.location.hash.substr(1);// "90aab585-1641-43e9-9979-1b53d6118faa";
     
     //load the map on initial page load
     loadMap(); 
       
        
        function loadMap(e) {

            //show loading overlay
            $(".loadingOverlay").show();
     
            //bind the easy button
            eb =  L.easyButton('fa-crosshairs', function (btn, map) {
            navigator.geolocation.getCurrentPosition(showPosition, handleError);

            function handleError(error) {
                if (error.code === error.PERMISSION_DENIED) {
                    // pop up dialog asking for location
                    alert("Error: User denied to share location.");
                }
                else {
                    alert("Error: Location information is unavailable.");
                }
            }

           }).addTo(map);
          
            markers = L.markerClusterGroup({ chunkedLoading: true, spiderfyOnMaxZoom: true, maxClusterRadius: 100, showCoverageOnHover: true });

            //map.eachLayer(function (layer) {
            //    map.removeLayer(layer);
            //});
            L.Map.include({
                'clearLayers': function () {
                    this.eachLayer(function (layer) {
                        this.removeLayer(layer);
                    }, this);
                }
            });
        
        //clear markers and remove all layers
            markers.clearLayers();
            var h = $(window).height();
            var w = $(window).width();
        
        var atype = $('input:checkbox:checked.gp1').map(function () {
            return this.value;
        }).get(); // ["18", "55", "10"]

        var st = atype + "";
        //fetch alerts data
        $.ajax({
            type: "GET",
            url: appUrl + "/Home/map", 
            data: {'atype': st},
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded',
            success: function (data) {

                $.each(data, function (i, item) {
                    var img = (item.IconUrl).replace("~", "");
                    var dpawIcon = L.icon({ iconUrl: appUrl + img, iconSize: [42, 42] });
                    var id;
                    var marker = L.marker(L.latLng(item.Latitude, item.Longitude), { icon: dpawIcon }, { title: item.Name });
                    var content = "<div class='infoDiv'><h3><img src='" + appUrl + img + "' width='24' />" + item.Name + "</h3><p>" + item.Title + "</p><a href='#' data-value='" + item.AlertId + "' class='btn btn-success btn-sm' data-toggle='modal' data-target='#alertDetails'>Details</a></div>";
                    id = marker._leaflet_id;
                    
                    marker.bindPopup(content);
                    markers.addLayer(marker);
                   
                  
                });
            }

        })
       .done(function () {
           $(".loadingOverlay").hide();

           //fit the map to the bounds
           
           if (hash === "" || hash === undefined) { map.fitBounds(markers.getBounds()); }
           map.addLayer(markers);
            //set initial zoom
          // map.setZoom(4);
          
           map.invalidateSize(true);
          // map.on("zoomend", function (e) { console.log("ZOOMEND", e); console.log("Current Zoom",  map.getZoom()); });
       });

        
        }

        
        //show popup for for specific alert
        $(function (e) {
           

            if (!hash) return false;

            //show loading
            $(".loadingOverlay").show();
            
            $.ajax({
                type: "GET",
                url: appUrl + "/Home/GetAlert/" + hash,
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded',
                success: function (data) {
                    if (!data.status) {
                        $('#msg').html(data.message).attr("class", "alert alert-warning");
                        $(".loadingOverlay").hide();
                        return false;
                    }
                    $.each(data.message, function (i, item) {
                        var img = (item.IconUrl).replace("~", "");
                        var dpawIcon = L.icon({ iconUrl: appUrl + img, iconSize: [42, 42] });
                        var popupLoc = new L.LatLng(item.Latitude, item.Longitude);
                        var popupContent = "<div class='infoDiv'><h3><img src='" + appUrl + img + "' width='24' />" + item.Name + "</h3><p>" + item.Title + "</p><a href='#' data-value='" + item.AlertId + "' class='btn btn-success btn-sm alertInfo' data-toggle='modal' data-target='#alertDetails'>Details</a></div>";

                        //initialize the popup;
                        var popup = new L.Popup();
                        //set latlng
                        popup.setLatLng(popupLoc);
                        //set content
                        popup.setContent(popupContent);
                        map.setView(new L.LatLng(item.Latitude, item.Longitude), 8);
                        
                        //display popup
                        map.addLayer(popup);

                    });

                }

            })
        .done(function () {
            $(".loadingOverlay").hide();
           
            map.invalidateSize(true);
        });

          
            //e.stopPropagation();
            //e.stopImmediatePropagation();
            //e.preventDefault();

        });

       //get current user location
        function showPosition(position) {
            var latlon = L.latLng(position.coords.latitude, position.coords.longitude);
            map.setView(latlon, 10, { animation: true });
            return false;
        }


        $('#gmap').on('shown.bs.tab', function (e) {
            //clear map first
            clearMap();
            //resize the map
            map.invalidateSize(true);
            //load the map once all layers cleared
            loadMap();
        });

        $('#listView').on('shown.bs.tab', function (e) {
            $(".loadingOverlay").show();
            //fetch fresh data
            var atype = $('input:checkbox:checked.gp1').map(function () {
                return this.value;
            }).get(); // ["18", "55", "10"]

            var st = atype + "";
            $("#updatePanel").load(appUrl + '/Home/ListView/' + "?atype=" + st,
                function () {
                    //bind the data table
                    $("#DPaWDataTable").dataTable({
                        // "bSort" : false,
                        "bJQueryUI": true,
                        "sPaginationType": "full_numbers",
                        "aaSorting": [[5, "desc"]],
                        "aoColumnDefs": [
                            { "type": "date", "targets": 5, "bSortable": true, "aTargets": [6] }
                        ]

                    });
                    $(".loadingOverlay").hide();
                });
            
        });

        $("#btnSearch").click(function (e) {

            //clear all layers first
            clearMap();

            //remove the hash value if any
            parent.location.hash = '';
            $('#msg').html('').attr("class", '');
            var atype = $('input:checkbox:checked.gp1').map(function () {
                return this.value;
            }).get(); // ["18", "55", "10"]

            var st = atype + "";

            //resize the map
            // map.invalidateSize(true);
            loadMap();
            map.invalidateSize(true);
            $("#updatePanel").load(appUrl + '/Home/ListView/' + "?atype=" + st, function () { 
            $("#DPaWDataTable").dataTable({
                    // "bSort" : false,
                    "bJQueryUI": true,
                    "sPaginationType": "full_numbers",
                    "aaSorting": [[0, "asc"]],
                    "aoColumnDefs": [
                        { "type": "date", "targets": 5, "bSortable": false, "aTargets": [6] }
                    ]

                });
            });
       
        //toggle the sidebar
        $('#sidebarToggle').click();
        e.stopPropagation();
        e.stopImmediatePropagation();
        e.preventDefault();
    });

    $("#btnReset").click(function (e) {
        //clear map first
        clearMap();
        //reset the hash value
        parent.location.hash = '';
        $('#msg').html('').attr("class", '');
        // alert("I am working");

       // $('#dfes').prop('checked', true);
        $('input:checkbox.gp1').prop('checked', true);
        //set the style
        $('input:checkbox.gp1').parent().parent().attr("class", "list-group-item");

        //fetch fresh data
        var atype = $('input:checkbox:checked.gp1').map(function () {
            return this.value;
        }).get(); // ["18", "55", "10"]

        var st = atype + "";

        $("#updatePanel").load(appUrl + '/Home/ListView/' + "?atype=" + st);
        //re-bind the datatable
       // dt.ajax.reload(null, false);
        
        //resize the map
        map.invalidateSize(true);
        //load the map once all layers cleared
        loadMap();

        //toggle the sidebar
        $('#sidebarToggle').click();
        e.stopPropagation();
        e.stopImmediatePropagation();
        e.preventDefault();

    });

    $('input:checkbox.gp1').click(function () {
        if ($(this).is(':checked')) {
            $(this).parent().parent().attr("class", "list-group-item");
        }
        else
        {
            $(this).parent().parent().attr("class", "list-group-item  active");
        }
    });

    $('input:checkbox#calert').click(function () {
       // alert("I am working!");
        if ($(this).is(':checked'))
        {
            $('input:checkbox.gp1').prop('checked', true);
        }
        else {
            $('input:checkbox.gp1').prop('checked', false);
        }
    });

    function clearMap()
    {
        map.removeLayer(markers);
        map.removeControl(lcontrol);
        map.removeControl(eb);
        
    }
});