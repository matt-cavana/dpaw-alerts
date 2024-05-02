var baseUrl = document.location.origin, appUrl = baseUrl /*+ "/alerts"*/, dt = $("#DPaWDataTable");
$(function () {
    bindDataTable();
    $("#alertDetails").on("shown.bs.modal", function (b) {
        var e = $(b.relatedTarget).data("value"),
            d = appUrl + "/Home/ViewAlert/" + e;
        b.stopPropagation();
        b.preventDefault();
        $("#alertContent").load(d, function (b, d, f) {
            if ("error" == d)
                $("#error").html("Sorry but there was an error loading data from the server!" + f.status + " " + f.statusText), $(".loadingOverlay").hide();
            else {
                b = appUrl + "/Home/Index%23" + e;
                d = $("#ttl").text();
                f = $("#fb");
                var c = $("#twt");
                f.attr("href", "https://www.facebook.com/sharer/sharer.php?u=" +
                    b + "&quote=" + d);
                c.attr("href", "https://twitter.com/intent/tweet?text=" + d + "%0A" + b);
                $(".loadingOverlay").hide();
                $("div#share-div").removeAttr("class", "hidden").attr("class", "pull-left")
            }
        })
    });
    $(document).on("click", "#prnt", function (b) {
        var e = $("#alertContent").html(),
            d = window.open("", "PRINT", "height=600,width=800");
        d.document.write("<html><head><title>" + document.title + "</title>");
        d.document.write("</head><body >");
        d.document.write("<h3>" + document.title + "</h3><hr />");
        d.document.write(e);
        d.document.write("</body></html>");
        d.document.close();
        d.focus();
        d.print();
        d.close();
        b.preventDefault()
    });
    $("#siteInfo").on("shown.bs.modal", function (b) {
        var e = $(b.relatedTarget).data("value");
        e = appUrl + "/Home/Details/" + e;
        b.stopPropagation();
        b.preventDefault();
        $("#siteContent").load(e)
    });
    $("#help").on("shown.bs.modal", function (b) {
        var e = appUrl + "/Home/Help/";
        $("#helpContent").load(e);
        b.stopPropagation();
        b.preventDefault()
    })
});
$(function (b) {
    function e(a) {
        $(".loadingOverlay").show();
        f = L.easyButton("fa-crosshairs", function (c, a) {
            navigator.geolocation.getCurrentPosition(d, function (c) {
                c.code == c.PERMISSION_DENIED ? alert("Error: User denied to share location.") : alert("Error: Location information is unavailable.")
            })
        }).addTo(c);
        g = L.markerClusterGroup({
            chunkedLoading: !0,
            spiderfyOnMaxZoom: !0,
            maxClusterRadius: 100,
            showCoverageOnHover: !0
        });
        L.Map.include({
            clearLayers: function () {
                this.eachLayer(function (c) {
                    this.removeLayer(c)
                }, this)
            }
        });
        g.clearLayers();
        $(window).height();
        $(window).width();
        a = $("input:checkbox:checked.gp1").map(function () {
            return this.value
        }).get();
        $.ajax({
            type: "GET",
            url: appUrl + "/Home/map",
            data: {
                atype: a + ""
            },
            dataType: "json",
            contentType: "application/x-www-form-urlencoded",
            success: function (c) {
                $.each(c, function (c, a) {
                    var b = a.IconUrl.replace("~", ""),
                        d = L.icon({
                            iconUrl: appUrl + b,
                            iconSize: [42, 42]
                        });
                    d = L.marker(L.latLng(a.Latitude, a.Longitude), {
                        icon: d
                    }, {
                        title: a.Name
                    });
                    d.bindPopup("<div class='infoDiv'><h3><img src='" + appUrl + b +
                        "' width='24' />" + a.Name + "</h3><p>" + a.Title + "</p><a href='#' data-value='" + a.AlertId + "' class='btn btn-success btn-sm' data-toggle='modal' data-target='#alertDetails'>Details</a></div>");
                    g.addLayer(d)
                })
            }
        }).done(function () {
            $(".loadingOverlay").hide();
            c.invalidateSize(!0)
        });
        c.addLayer(g);
        c.setZoom(6)
    }

    function d(a) {
        a = L.latLng(a.coords.latitude, a.coords.longitude);
        c.setView(a, 10, {
            animation: !0
        });
        return !1
    }

    function m() {
        c.removeLayer(g);
        c.removeControl(n);
        c.removeControl(f)
    }
    var n = new L.control.layers,
        f = new L.control.layers,
        c = null,
        g = [],
        h = L.latLng(-30.81881, 116.16596);
    b = L.gridLayer.googleMutant({
        type: "roadmap"
    });
    var p = L.gridLayer.googleMutant({
        maxZoom: 24,
        type: "satellite"
    }),
        q = L.gridLayer.googleMutant({
            maxZoom: 24,
            type: "terrain"
        }),
        r = L.gridLayer.googleMutant({
            maxZoom: 24,
            type: "hybrid"
        });
    c = new L.map("lmap", {
        center: h,
        zoom: 6,
        maxZoom: 18,
        scrollwheel: !1,
        layers: [b],
        streetViewControl: !1,
        scrollWheelZoom: !1
    });
    var s = appUrl + "/media/WiilmanBilya.gpx"; // New GPX file path
    var k = appUrl + "/media/Bibbulmun_Track.gpx",
        l = appUrl + "/media/Cape_to_Cape_Track.gpx";
    h = (new L.GPX(appUrl + "/media/Munda_Biddi_Trail.gpx", {
        async: !0,
        marker_options: {
            startIconUrl: null,
            endIconUrl: null,
            shadowUrl: null
        },
        polyline_options: {
            color: "#0170FF"
        }
    })).on("loaded", function (a) {
        c.fitBounds(a.target.getBounds())
    });
    k = (new L.GPX(k, {
        async: !0,
        marker_options: {
            startIconUrl: null,
            endIconUrl: null,
            shadowUrl: "images/pin-shadow.png"
        },
        polyline_options: {
            color: "#FF771C"
        }
    })).on("loaded", function (a) {
        c.fitBounds(a.target.getBounds())
    });
    l = (new L.GPX(l, {
        async: !0,
        marker_options: {
            startIconUrl: null,
            endIconUrl: null,
            shadowUrl: null
        },
        polyline_options: {
            color: "#F58025"
        }
    })).on("loaded", function (a) {
        c.fitBounds(a.target.getBounds())
    });
    s = (new L.GPX(s, { // Create the new GPX layer
        async: true,
        marker_options: {
            startIconUrl: null,
            endIconUrl: null,
            shadowUrl: null
        },
        polyline_options: {
            color: "#FF771C" // Color for the new GPX file
        }
    })).on("loaded", function (a) {
        c.fitBounds(a.target.getBounds());
    });
    c.addLayer(h);
    c.addLayer(k);
    c.addLayer(l);
    c.addLayer(s); // Add the new layer to the map
    c.on("focus", function () {
        c.scrollWheelZoom.enable()
    });
    c.on("blur", function () {
        c.scrollWheelZoom.disable()
    });
    c.addControl(new L.Control.Layers({
        Streets: b,
        Satellite: p,
        Terrain: q,
        Hybrid: r
    }, {
        "Munda Biddi": h,
        "Cape to Cape": l,
        "Bibbulmun": k,
        "Wiilman Bilya": s // Add new GPX file to layer control
    }, {
        collapsed: !0
    }));
    e();
    $(function (a) {
        var b = window.location.hash.substr(1);
        if (!b) return !1;
        $(".loadingOverlay").show();
        $.ajax({
            type: "GET",
            url: appUrl + "/Home/GetAlert/" + b,
            dataType: "json",
            contentType: "application/x-www-form-urlencoded",
            success: function (a) {
                if (!a.status) return $("#msg").html(a.message).attr("class", "alert alert-warning"), $(".loadingOverlay").hide(), !1;
                $.each(a.message, function (a, b) {
                    var d = b.IconUrl.replace("~", "");
                    L.icon({
                        iconUrl: appUrl + d,
                        iconSize: [42, 42]
                    });
                    var e = new L.LatLng(b.Latitude, b.Longitude);
                    d = "<div class='infoDiv'><h3><img src='" + appUrl + d + "' width='24' />" + b.Name + "</h3><p>" + b.Title + "</p><a href='#' data-value='" + b.AlertId + "' class='btn btn-success btn-sm alertInfo' data-toggle='modal' data-target='#alertDetails'>Details</a></div>";
                    var f = new L.Popup;
                    f.setLatLng(e);
                    f.setContent(d);
                    c.setView(new L.LatLng(b.Latitude, b.Longitude), 8);
                    c.addLayer(f)
                })
            }
        }).done(function () {
            $(".loadingOverlay").hide();
            c.invalidateSize(!0)
        });
        a.stopPropagation();
        a.stopImmediatePropagation();
        a.preventDefault()
    });
    $("#gmap").on("shown.bs.tab", function (a) {
        m();
        c.invalidateSize(!0);
        e()
    });
    $("#btnSearch").click(function (a) {
        m();
        parent.location.hash = "";
        $("#msg").html("").attr("class", "");
        var b = $("input:checkbox:checked.gp1").map(function () {
            return this.value
        }).get() +
            "";
        e();
        c.invalidateSize(!0);
        $("#updatePanel").load(appUrl + "/Home/ListView/?atype=" + b);
        dt.fnDraw();
        $("#sidebarToggle").click();
        a.stopPropagation();
        a.stopImmediatePropagation();
        a.preventDefault()
    });
    $("#btnReset").click(function (a) {
        m();
        parent.location.hash = "";
        $("#msg").html("").attr("class", "");
        $("input:checkbox.gp1").prop("checked", !0);
        $("input:checkbox.gp1").parent().parent().attr("class", "list-group-item");
        var b = $("input:checkbox:checked.gp1").map(function () {
            return this.value
        }).get() + "";
        $("#updatePanel").load(appUrl +
            "/Home/ListView/?atype=" + b);
        c.invalidateSize(!0);
        e();
        $("#sidebarToggle").click();
        a.stopPropagation();
        a.stopImmediatePropagation();
        a.preventDefault()
    });
    $("input:checkbox.gp1").click(function () {
        $(this).is(":checked") ? $(this).parent().parent().attr("class", "list-group-item") : $(this).parent().parent().attr("class", "list-group-item  active")
    });
    $("input:checkbox#calert").click(function () {
        $(this).is(":checked") ? $("input:checkbox.gp1").prop("checked", !0) : $("input:checkbox.gp1").prop("checked", !1)
    })
});
function bindDataTable() {
    dt.dataTable({
        bJQueryUI: !0,
        sPaginationType: "full_numbers",
        aaSorting: [[0, "asc"]],
        aoColumnDefs: [{
            type: "date",
            targets: 5,
            bSortable: !1,
            aTargets: [6]
        }]
    })
};
