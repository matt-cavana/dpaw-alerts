//Author: Mohammed Tajuddin,  mohammed.tajuddin@dpaw.wa.gov.au
// DPaW Alerts system
var baseUrl = document.location.origin;
var appUrl = baseUrl; // + "/alerts";
var map;
$(function () {

    $(".delete").click(function (e) {

        if (confirm('Are you sure want to delete?')) {
            e.preventDefault();
            var parent = $(this).parent().parent().parent();

            {

                $.ajax({
                    url: appUrl+ "/Alerts/AjaxDelete", // '@Url.Action("AjaxDelete", "Alerts")',
                    type: 'POST',
                    dataType: 'json',
                    async: true,
                    data: { 'id': $(this).attr('data-value') },
                    contentType: 'application/x-www-form-urlencoded',
                    beforeSend: function () {
                        parent.animate({ 'backgroundColor': '#fb6c6c' }, 300);
                    },
                    success: function (data) {
                        if (data.success) {

                            $("#msg").html("<div class='alert alert-success'><i class='glyphicon glyphicon-info-sign'></i> The alert has been deleted from the system! </div>");

                            parent.slideUp(300, function () {
                                parent.remove();
                            });

                        }
                        else {
                            // show a message in a alert or div
                            $("#msg").html("<div class='alert alert-danger'><i class='glyphicon glyphicon-info-sign'></i>"+data.message+"! </div>");
                        }
                    }
                });
            }
        }
        else return false;
    });

    $(".unpublish").click(function (e) {

        if (confirm('Are you sure want to unpublish this record, this operation cannot be undone?')) {
            e.preventDefault();
            var parent = $(this);

            {
                $.ajax({
                    url: appUrl + "/Alerts/Unpublish", // '@Url.Action("Unpublish", "Alerts")',
                    type: 'POST',
                    dataType: 'json',
                    async: true,
                    data: { 'id': $(this).attr('data-value') },
                    contentType: 'application/x-www-form-urlencoded',
                    success: function (data) {
                        $("#msg").html(data.message);
                        parent.slideUp(300, function () {
                            parent.remove();
                        });
                    },
                    complete: function () {
                        // $(this).hide();
                        window.location.reload();
                    }
                });
            }
        }
        else return false;
    });


    //$(".site").click(function (e) {
    //    var id = $(this).attr("data-value");
    //    var url = appUrl+ "/Home/Details/" + id; //'@Url.Action("Details", "Locations")' + "/" + id
    //    // alert(id);
    //    $("#siteContent").load(url);

    //});

    $('#siteInfo').on('shown.bs.modal', function (e) {
        var element = $(e.relatedTarget);
        var data = element.data("value");
        var url = appUrl + "/Home/Details/" + data;
        $("#siteContent").load(url);
    });

    $('#alertDetails').on('shown.bs.modal', function (e) {
        var element = $(e.relatedTarget);
        var data = element.data("value");
        var url = appUrl + "/Home/ViewAlert/" + data;
        $("#alertContent").load(url);
    });

    //set the data table
    $('#dataTable').dataTable({
        "fnDrawCallback": function (settings) {
            // alert('DataTables has redrawn the table');
            $('img[data-toggle="tooltip"]').tooltip();
        },
        // "bSort" : false,
        "bJQueryUI": true,
        "sPaginationType": "full_numbers",
        "aaSorting": [[0, "asc"]],
        "aoColumnDefs": [
                            { "type": "date", "targets": 4, "bSortable": false, "aTargets": [9, 10] }
        ]

    });


    $("#dfestab").click(function () {
        $(".loadingOverlay").show();
        var url = appUrl + "/Alerts/ERSS";
        $("#dfesdata").load(url,
            function (response, status, xhr) {
                if (status == "error") {
                    var msg = "Sorry but there was an error loading data from emergency website";
                    $("#error").html(msg + xhr.status + " " + xhr.statusText);
                    $(".loadingOverlay").hide();
                }
                else {
                    $(".loadingOverlay").hide();
                }
            });
    });

});


$(function(){

    var latlng = L.latLng(-30.81881, 116.16596);
    map = L.map('lmap', { center: latlng, zoom: 6 });
    var lcontrol = new L.control.layers();
    //reset the map size on dom ready
    map.invalidateSize(true);
    function loadMap() {

        $(".loadingOverlay").show();
        
        // alert("I ma working"); http://{s}.tile.osm.org/{z}/{x}/{y}.png 
        //var tiles = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        //    maxZoom: 16,
        //    attribution: '&copy; <a href="https://osm.org/copyright">OpenStreetMap</a> contributors, Points &copy 2012 LINZ'
        //}),
        
        var roadMutant = L.gridLayer.googleMutant({
            type: 'roadmap' // valid values are 'roadmap', 'satellite', 'terrain' and 'hybrid'
        }).addTo(map);
        var satMutant = L.gridLayer.googleMutant({
            maxZoom: 24,
            type: 'satellite'
        });

        var terrainMutant = L.gridLayer.googleMutant({
            maxZoom: 24,
            type: 'terrain'
        });

        var hybridMutant = L.gridLayer.googleMutant({
            maxZoom: 24,
            type: 'hybrid'
        });

        var styleMutant = L.gridLayer.googleMutant({
            styles: [
                { elementType: 'labels', stylers: [{ visibility: 'off' }] },
                { featureType: 'water', stylers: [{ color: '#444444' }] },
                { featureType: 'landscape', stylers: [{ color: '#eeeeee' }] },
                { featureType: 'road', stylers: [{ visibility: 'off' }] },
                { featureType: 'poi', stylers: [{ visibility: 'off' }] },
                { featureType: 'transit', stylers: [{ visibility: 'off' }] },
                { featureType: 'administrative', stylers: [{ visibility: 'off' }] },
                { featureType: 'administrative.locality', stylers: [{ visibility: 'off' }] }
            ],
            maxZoom: 24,
            type: 'roadmap'
        });


        var mun = appUrl + "/media/Munda_Biddi_Trail.gpx";
        var bib = appUrl + "/media/Bibbulmun_Track.gpx";
        var cape = appUrl + "/media/Cape_to_Cape_Track.gpx";
        var mtrack = new L.GPX(mun, {
            async: true,
            marker_options: {
                startIconUrl: null,
                endIconUrl: null,
                shadowUrl: null
            },
            polyline_options: { color: '#0170FF' }
        }).on("loaded", function (e) {
            map.fitBounds(e.target.getBounds());
        });
        var btrack = new L.GPX(bib, {
            async: true,
            marker_options: { startIconUrl: null, endIconUrl: null, shadowUrl: 'images/pin-shadow.png' },
            polyline_options: { color: '#8C211F' }
        }).on("loaded", function (e) {
            map.fitBounds(e.target.getBounds());
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
            map.fitBounds(e.target.getBounds());
        });
        map.addLayer(mtrack);
        map.addLayer(btrack);
        map.addLayer(ctrack);

        //add the control on the map
        lcontrol= L.control.layers({
            Roadmap: roadMutant,
            Aerial: satMutant,
            Terrain: terrainMutant,
            Hybrid: hybridMutant //,Styles: styleMutant

        }, { 'Munda Biddi': mtrack, 'Cape to Cape': ctrack, 'Bibbulmun': btrack }, {
            collapsed: true
        }).addTo(map);

        var markers = L.markerClusterGroup({ chunkedLoading: true, spiderfyOnMaxZoom: true, maxClusterRadius: 80 });

        //var markers = L.markerClusterGroup({
        //    spiderfyOnMaxZoom: false,
        //    showCoverageOnHover: false,
        //    zoomToBoundsOnClick: false
        //});

        $.ajax({
            type: "GET",
            url: appUrl + "/Alerts/map", // '@Url.Action("map", "Alerts")',
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded',
            success: function (data) {

                $.each(data, function (i, item) {
                    var img = (item.IconUrl).replace("~", "/");
                    // create custom icon
                    var dpawIcon = L.icon({ iconUrl: appUrl + img, iconSize: [42, 42] });
                    //alert(item.IconUrl);
                    var marker = L.marker(L.latLng(item.Latitude, item.Longitude), { icon: dpawIcon }, { title: item.Name });
                    var content = "<div class='infoDiv'><h3><img src='"+ appUrl + img + "' width='24' />" + item.Name + "</h3><p>" + item.Title + "</p><a href='#' data-value='" + item.AlertId + "' class='btn btn-success btn-sm alertInfo' data-toggle='modal' data-target='#alertDetails'>Details</a></div>";
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

        $.ajax({
            type: "GET",
            url: appUrl + "/Alerts/jsondfes", //'@Url.Action("jsondfes", "Alerts")',
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded',
            success: function (data) {

                $.each(data, function (i, item) {

                    var marker = L.marker(L.latLng(item.latitude, item.longitude), { icon: dfesIcon }, { title: item.title });
                    var content = "<div class='infoDiv'><h4><img src='"+ appUrl + "/img/emergency-wa-alert.svg' width='24' /> " + item.description + "</h4><p>" + item.title + "</p><a href='" + item.link + "' target='_blank' class='btn btn-success btn-sm'>Read More</a></div>";
                    marker.bindPopup(content);
                    markers.addLayer(marker);

                });


            }

        })
        .done(function () {
            $(".loadingOverlay").hide();
            map.invalidateSize(true);
        });

        map.addLayer(markers);
    }
    $('#gmap').on('shown.bs.tab', function (e) {
        //clear map
        clearMap();
        map.invalidateSize(true);
        //load the map once all layers cleared
       loadMap();
    })

    function clearMap() {
        // clear all layers before it reloads;
        map.eachLayer(function (layer) {
            map.removeLayer(layer);
        });
        map.removeControl(lcontrol);
    }

    map.on('focus', function () { map.scrollWheelZoom.enable(); });
    map.on('blur', function () { map.scrollWheelZoom.disable(); });
});

$(function () {

    //load the alert summary on document load
    loadSummary();

    $(".refresh-widget").click(function () {
        $(".loading-overlay").show();
        loadSummary();
        //setTimeout(loadSummary, 2000);
    });
    
    function loadSummary()
    {
        
        $.ajax({
            type: "GET",
            url: appUrl + "/Alerts/AlertsSummary", // '@Url.Action("AlertsSummary", "Alerts")',
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded',
            success: function (data) {
                $.each(data, function (i, item) {
                    var lbl = "#al-" + i;
                    var lblv = "#alv-" + i;
                    $(lbl).text(item.Key);
                    $(lblv).text(item.Value);
                    // alert(lbl + item.Key);

                });
            }
        })
       .done(function () {
           $(".loading-overlay").hide();

       });
    }
});