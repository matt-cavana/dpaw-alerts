var baseUrl = document.location.origin, appUrl = baseUrl, dt = $("#DPaWDataTable");
$(function() {
  bindDataTable();
  $("#alertDetails").on("shown.bs.modal", function(a) {
    var d = $(a.relatedTarget).data("value"), c = appUrl + "/Home/ViewAlert/" + d;
    a.stopPropagation();
    a.preventDefault();
    $("#alertContent").load(c, function(a, c, e) {
      if ("error" == c) {
        $("#error").html("Sorry but there was an error loading data from the server!" + e.status + " " + e.statusText), $(".loadingOverlay").hide();
      } else {
        a = appUrl + "/Home/Index%23" + d;
        c = $("#ttl").text();
        e = $("#fb");
        var b = $("#twt");
        e.attr("href", "https://www.facebook.com/sharer/sharer.php?u=" + a + "&quote=" + c);
        b.attr("href", "https://twitter.com/intent/tweet?text=" + c + "%0A" + a);
        $(".loadingOverlay").hide();
        $("div#share-div").removeAttr("class", "hidden").attr("class", "pull-left");
      }
    });
  });
  $(document).on("click", "#prnt", function(a) {
    var d = $("#alertContent").html(), c = window.open("", "PRINT", "height=600,width=800");
    c.document.write("<html><head><title>" + document.title + "</title>");
    c.document.write("</head><body >");
    c.document.write("<h3>" + document.title + "</h3><hr />");
    c.document.write(d);
    c.document.write("</body></html>");
    c.document.close();
    c.focus();
    c.print();
    c.close();
    a.preventDefault();
  });
  $("#siteInfo").on("shown.bs.modal", function(a) {
    var d = $(a.relatedTarget).data("value");
    d = appUrl + "/Home/Details/" + d;
    a.stopPropagation();
    a.preventDefault();
    $("#siteContent").load(d);
  });
  $("#help").on("shown.bs.modal", function(a) {
    var d = appUrl + "/Home/Help/";
    $("#helpContent").load(d);
    a.stopPropagation();
    a.preventDefault();
  });
});
$(function(a) {
  function d(a) {
    $(".loadingOverlay").show();
    e = L.easyButton("fa-crosshairs", function(b, a) {
      navigator.geolocation.getCurrentPosition(c, function(b) {
        b.code == b.PERMISSION_DENIED ? alert("Error: User denied to share location.") : alert("Error: Location information is unavailable.");
      });
    }).addTo(b);
    g = L.markerClusterGroup({chunkedLoading:!0, spiderfyOnMaxZoom:!0, maxClusterRadius:100, showCoverageOnHover:!0});
    L.Map.include({clearLayers:function() {
      this.eachLayer(function(b) {
        this.removeLayer(b);
      }, this);
    }});
    g.clearLayers();
    $(window).height();
    $(window).width();
    a = $("input:checkbox:checked.gp1").map(function() {
      return this.value;
    }).get();
    $.ajax({type:"GET", url:appUrl + "/Home/map", data:{atype:a + ""}, dataType:"json", contentType:"application/x-www-form-urlencoded", success:function(b) {
      $.each(b, function(b, a) {
        var c = a.IconUrl.replace("~", ""), f = L.icon({iconUrl:appUrl + c, iconSize:[42, 42]});
        f = L.marker(L.latLng(a.Latitude, a.Longitude), {icon:f}, {title:a.Name});
        f.bindPopup("<div class='infoDiv'><h3><img src='" + appUrl + c + "' width='24' />" + a.Name + "</h3><p>" + a.Title + "</p><a href='#' data-value='" + a.AlertId + "' class='btn btn-success btn-sm' data-toggle='modal' data-target='#alertDetails'>Details</a></div>");
        g.addLayer(f);
      });
    }}).done(function() {
      $(".loadingOverlay").hide();
      b.fitBounds(g.getBounds());
      b.addLayer(g);
      b.setZoom(4);
      b.invalidateSize(!0);
    });
  }
  function c(a) {
    a = L.latLng(a.coords.latitude, a.coords.longitude);
    b.setView(a, 10, {animation:!0});
    return !1;
  }
  function n() {
    b.removeLayer(g);
    b.removeControl(p);
    b.removeControl(e);
  }
  var p = new L.control.layers, e = new L.control.layers, b = null, g = [], h = L.latLng(-30.81881, 116.16596);
  a = L.gridLayer.googleMutant({type:"roadmap"});
  var q = L.gridLayer.googleMutant({maxZoom:24, type:"satellite"}), r = L.gridLayer.googleMutant({maxZoom:24, type:"terrain"}), t = L.gridLayer.googleMutant({maxZoom:24, type:"hybrid"});
  b = new L.map("lmap", {center:h, minZoom:0, maxZoom:18, scrollwheel:!1, layers:[a], streetViewControl:!1, scrollWheelZoom:!1});
  var k = appUrl + "/media/Bibbulmun_Track.gpx", l = appUrl + "/media/Cape_to_Cape_Track.gpx";
  h = (new L.GPX(appUrl + "/media/Munda_Biddi_Trail.gpx", {async:!0, marker_options:{startIconUrl:null, endIconUrl:null, shadowUrl:null}, polyline_options:{color:"#0170FF"}})).on("loaded", function(a) {
  });
  k = (new L.GPX(k, {async:!0, marker_options:{startIconUrl:null, endIconUrl:null, shadowUrl:"images/pin-shadow.png"}, polyline_options:{color:"#8C211F"}})).on("loaded", function(a) {
  });
  l = (new L.GPX(l, {async:!0, marker_options:{startIconUrl:null, endIconUrl:null, shadowUrl:null}, polyline_options:{color:"#F58025"}})).on("loaded", function(a) {
  });
  b.addLayer(h);
  b.addLayer(k);
  b.addLayer(l);
  b.on("focus", function() {
    b.scrollWheelZoom.enable();
  });
  b.on("blur", function() {
    b.scrollWheelZoom.disable();
  });
  b.addControl(new L.Control.Layers({Streets:a, Satellite:q, Terrain:r, Hybrid:t}, {"Munda Biddi":h, "Cape to Cape":l, Bibbulmun:k}, {collapsed:!0}));
  var m = window.location.hash.substr(1);
  "" != m && void 0 != m || d();
  $(function(a) {
    if (!m) {
      return !1;
    }
    $(".loadingOverlay").show();
    $.ajax({type:"GET", url:appUrl + "/Home/GetAlert/" + m, dataType:"json", contentType:"application/x-www-form-urlencoded", success:function(a) {
      if (!a.status) {
        return $("#msg").html(a.message).attr("class", "alert alert-warning"), $(".loadingOverlay").hide(), !1;
      }
      $.each(a.message, function(a, c) {
        var d = c.IconUrl.replace("~", "");
        L.icon({iconUrl:appUrl + d, iconSize:[42, 42]});
        var f = new L.LatLng(c.Latitude, c.Longitude);
        d = "<div class='infoDiv'><h3><img src='" + appUrl + d + "' width='24' />" + c.Name + "</h3><p>" + c.Title + "</p><a href='#' data-value='" + c.AlertId + "' class='btn btn-success btn-sm alertInfo' data-toggle='modal' data-target='#alertDetails'>Details</a></div>";
        var e = new L.Popup;
        e.setLatLng(f);
        e.setContent(d);
        b.setView(new L.LatLng(c.Latitude, c.Longitude), 8);
        b.addLayer(e);
      });
    }}).done(function() {
      $(".loadingOverlay").hide();
      b.invalidateSize(!0);
    });
  });
  $("#gmap").on("shown.bs.tab", function(a) {
    n();
    b.invalidateSize(!0);
    d();
  });
  $("#btnSearch").click(function(a) {
    n();
    parent.location.hash = "";
    $("#msg").html("").attr("class", "");
    var c = $("input:checkbox:checked.gp1").map(function() {
      return this.value;
    }).get() + "";
    d();
    b.invalidateSize(!0);
    $("#updatePanel").load(appUrl + "/Home/ListView/?atype=" + c);
    dt.fnDraw();
    $("#sidebarToggle").click();
    a.stopPropagation();
    a.stopImmediatePropagation();
    a.preventDefault();
  });
  $("#btnReset").click(function(a) {
    n();
    parent.location.hash = "";
    $("#msg").html("").attr("class", "");
    $("input:checkbox.gp1").prop("checked", !0);
    $("input:checkbox.gp1").parent().parent().attr("class", "list-group-item");
    var c = $("input:checkbox:checked.gp1").map(function() {
      return this.value;
    }).get() + "";
    $("#updatePanel").load(appUrl + "/Home/ListView/?atype=" + c);
    b.invalidateSize(!0);
    d();
    $("#sidebarToggle").click();
    a.stopPropagation();
    a.stopImmediatePropagation();
    a.preventDefault();
  });
  $("input:checkbox.gp1").click(function() {
    $(this).is(":checked") ? $(this).parent().parent().attr("class", "list-group-item") : $(this).parent().parent().attr("class", "list-group-item  active");
  });
  $("input:checkbox#calert").click(function() {
    $(this).is(":checked") ? $("input:checkbox.gp1").prop("checked", !0) : $("input:checkbox.gp1").prop("checked", !1);
  });
});
function bindDataTable() {
  dt.dataTable({bJQueryUI:!0, sPaginationType:"full_numbers", aaSorting:[[0, "asc"]], aoColumnDefs:[{type:"date", targets:5, bSortable:!1, aTargets:[6]}]});
}
;