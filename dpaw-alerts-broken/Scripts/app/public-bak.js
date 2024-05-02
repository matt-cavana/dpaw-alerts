/*
 * Parks Alerts System
 * Version 3.7
 * https://parks.dpaw.wa.gov.au/
 *
 * Interactive leaflet map for public interface
 *
 * Copyright (c) 2012 Department of Parks and Wildlife
 * Author: Mohammed Tajuddin, mohammed.tajuddin@dpaw.wa.gov.au
 * Parks and Visitor Services
 *
 */
var baseUrl = document.location.origin;
var appUrl = baseUrl + "/alerts";
var map;
$(function () {

        $('#DPaWDataTable').dataTable({
            // "bSort" : false,
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",
            "aaSorting": [[0, "asc"]],
            "aoColumnDefs": [
                            { "type": "date", "targets": 5, "bSortable": false, "aTargets": [6] }
                            ]

        });

  
    $('#alertDetails').on('shown.bs.modal', function (e) {
        var element = $(e.relatedTarget);
        var data = element.data("value");
        var url = appUrl + "/Home/ViewAlert/" + data;
        //fix the safari issue
        e.stopPropagation();
        e.preventDefault();
        $("#alertContent").load(url);
       
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

    $("#dfestab").click(function () {
       
        $(".loadingOverlay").show();
        var url = appUrl + "/Home/ERSS";
        $("#dfesdata").load(url,
            function (response, status, xhr) {
                if (status == "error") {
                    var msg = "Sorry but there was an error loading data from emergency website ";
                    $("#error").html(msg + xhr.status + " " + xhr.statusText);
                    $(".loadingOverlay").hide();
                }
                else {
                    $(".loadingOverlay").hide();
                }
            });
    });

});


$(function(e){

        var latlng = L.latLng(-30.81881, 116.16596);
        var map = L.map('lmap', {
            center: latlng, zoom: 6, scrollwheel: false,
            streetViewControl: false
        });
        var lcontrol = new L.control.layers();
        var eb = new L.control.layers();
        
       
        //clear the map first
        clearMap();
        //load the map again
        loadMap();
       
        
        //reset the map size on dom ready
        map.invalidateSize(true);

        function loadMap(e) {

            //show loading overlay
            $(".loadingOverlay").show();

            var roadMutant = L.gridLayer.googleMutant({type: 'roadmap' }).addTo(map);
            var satMutant = L.gridLayer.googleMutant({ maxZoom: 24, type: 'satellite' });
            var terrainMutant = L.gridLayer.googleMutant({ maxZoom: 24, type: 'terrain' });
            var hybridMutant = L.gridLayer.googleMutant({maxZoom: 24, type: 'hybrid' });

           //add the control on the map          
           lcontrol= L.control.layers({
                Roadmap: roadMutant,
                Aerial: satMutant,
                Terrain: terrainMutant,
                Hybrid: hybridMutant //,Styles: styleMutant

            }, {}, {
                collapsed: false
            }).addTo(map);

            //bind the easy button
            eb =  L.easyButton('fa-crosshairs', function (btn, map) {
            navigator.geolocation.getCurrentPosition(showPosition, handleError);

            function handleError(error) {
                if (error.code == error.PERMISSION_DENIED) {
                    // pop up dialog asking for location
                    alert("Error: User denied to share location.");
                }
                else {
                    alert("Error: Location information is unavailable.");
                }
            }

           }).addTo(map);
          
        var markers = L.markerClusterGroup({chunkedLoading: true, spiderfyOnMaxZoom: true, maxClusterRadius: 80, showCoverageOnHover: true });

        
        //clear markers and remove all layers
        markers.clearLayers();
        
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
           map.invalidateSize(true);
       });

        var dfesIcon = L.icon({
            iconUrl: appUrl + '/img/emergency-wa-alert.svg',
            iconSize: [42, 42],
            iconAnchor: [16, 37],
            popupAnchor: [0, -28]
        });

        //load dfes maps only when dfes checkbox is checked
        if ($('#dfes').prop('checked')) {
          
            $.ajax({
                type: "GET",
                url: appUrl + "/Home/jsondfes", //'@Url.Action("jsondfes", "Alerts")',
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded',
                success: function (data) {

                    $.each(data, function (i, item) {

                        var marker = L.marker(L.latLng(item.latitude, item.longitude), { icon: dfesIcon }, { title: item.title });
                                   
                        var div_popup = L.DomUtil.create('div', 'abcpopup');
            
                        div_popup.innerHTML = '<p>Hello I am a popup!</p> <a class="removeme">Remove Marker</a>';
                       
                       
                        div_popup.innerHTML = "<div class='infoDiv'><h4><img src='" + appUrl + "/img/emergency-wa-alert.svg' width='24' /> " + item.description + "</h4><p>" + item.title + "</p><a href='" + item.link + "' target='_blank' class='btn btn-success btn-sm myLink'>Read More</a></div>";
                        //marker.bindPopup(content);
                        marker.bindPopup(div_popup);
                        markers.addLayer(marker);
                       
                    });

                }

            })
            .done(function () {
                $(".loadingOverlay").hide();
                map.invalidateSize(true);
            });
        }
       map.addLayer(markers);
       //set initial zoom
       map.setZoom(6);

     //click event does not work in iPhone safari
       map.on('popupopen', function () {
           $('a.myLink').click(function (e) {
             var url=  $(this).attr('href');
             alert(url);
             url.attr("target", "_blank");
             window.open(url.attr("href"));

             e.preventDefault();
             return false;
           });
       });

       //map.on('click', function (e) {
       //    var marker = L.marker(e.latlng).addTo(map);

       //    var div_popup = L.DomUtil.create('div', 'abcpopup');

       //    div_popup.innerHTML = '<p>Hello I am a popup!</p> <a class="removeme">Remove Marker</a>';
       //    $('a.removeme', div_popup).on('click', function () {
       //        map.closePopup();
       //        map.removeLayer(marker);
       //    });

       //    marker.bindPopup(div_popup);
       //});

        }

        //show popup for for specific alert
        $(function (e) {
            var hash = window.location.hash.substr(1);// "90aab585-1641-43e9-9979-1b53d6118faa";

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

            e.stopPropagation();
            e.stopImmediatePropagation();
            e.preventDefault();

        });

       //get current user location
        function showPosition(position) {
            var latlon = L.latLng(position.coords.latitude, position.coords.longitude);
            map.setView(latlon, 10, { animation: true });
            return false;
        }


         //stop propagation
        $('.testlink').on('click', function (e) {
          // e.preventDefault();
           //$('.map_popup').click(function(){
           alert('Try to open Accordion ' + $(e.target).attr('href'));
           //console.log('Try to open Accordion');
       });

    $('#gmap').on('shown.bs.tab', function (e) {
        //clear map first
        clearMap();
        //resize the map
       map.invalidateSize(true);
       //load the map once all layers cleared
       loadMap();
    })

    $("#btnSearch").click(function (e) {
        //remove the hash value if any
        parent.location.hash = '';
        $('#msg').html('').attr("class", '');
        var atype = $('input:checkbox:checked.gp1').map(function () {
            return this.value;
        }).get(); // ["18", "55", "10"]

        var st = atype + "";

        //reload mapview
        clearMap();
        //resize the map
        map.invalidateSize(true);
        loadMap();
        
        $("#updatePanel").load(appUrl + '/Home/ListView/' + "?atype=" + st);

        //toggle the sidebar
        $('#sidebarToggle').click();
        e.stopPropagation();
        e.stopImmediatePropagation();
        e.preventDefault();
    });

    $("#btnReset").click(function (e) {

        //reset the hash value
        parent.location.hash = '';
        $('#msg').html('').attr("class", '');
        // alert("I am working");

       // $('#dfes').prop('checked', true);
        $('input:checkbox').prop('checked', true);
        //set the style
        $('input:checkbox').parent().parent().attr("class", "list-group-item");

        //fetch fresh data
        var atype = $('input:checkbox:checked.gp1').map(function () {
            return this.value;
        }).get(); // ["18", "55", "10"]

        var st = atype + "";

        $("#updatePanel").load(appUrl + '/Home/ListView/' + "?atype=" + st);

        //clear map first
        clearMap();
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

    $('input:checkbox').click(function () {
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
            $('input:checkbox').prop('checked', true);
        }
        else {
            $('input:checkbox').prop('checked', false);
        }
    });

    function clearMap()
    {
        // clear all layers before it reloads;
        map.eachLayer(function (layer) {
            map.removeLayer(layer);
        });
        map.removeControl(lcontrol);
        map.removeControl(eb);

    }
   

});