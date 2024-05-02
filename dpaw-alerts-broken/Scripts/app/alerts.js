//Author: Mohammed Tajuddin,  mohammed.tajuddin@dpaw.wa.gov.au
// DPaW Alerts system

var appUrl = document.location.origin;
 
$(function () {
    //fix jquery date validation issue
    $.validator.methods.date = function (value, element) {
        return this.optional(element) || $.datepicker.parsedate('dd/mm/yy', value);
    }

    $.datetimepicker.setDateFormatter({
        parseDate: function (date, format) {
            var d = moment(date, format);
            return d.isValid() ? d.toDate() : false;
        },

        formatDate: function (date, format) {
            return moment(date).format(format);
        }
    });

    $('#StartDate').datetimepicker({
        format: 'DD/MM/YYYY hh:mm a',
        formatTime: 'hh:mm a',
        formatDate: 'DD/MM/YYYY',
        minDate: '-1970/01/02'
    });
    $('#EndDate').datetimepicker({
        format: 'DD/MM/YYYY hh:mm a',
        formatTime: 'hh:mm a',
        formatDate: 'DD/MM/YYYY',
        minDate: '-1970/01/02'//,yesterday is minimum date(for today use 0 or -1970/01/01)
       // maxDate: '+1970/01/02'//tomorrow is maximum date calendar
    });

    $("#parkId").change(function () {
      var prkId=  $("#parkId").val();

      alert(prkId);
    });
  //change the location to fix styling issue
    $("input[name=PubImmediately][type=hidden]").detach().appendTo('div.check-control')

    $('#PubImmediately[type=checkbox]').click(function () {
           
        if ($("#PubImmediately").is(':checked'))
                $("#StartDate").attr("readonly",true);  // checked
            else
                $("#StartDate").attr("readonly", false);  // unchecked

           
    });

    //$('#StartDate').datepicker();

    $('#Description').summernote({
        height: 300,                 // set editor height
        minHeight: null,             // set minimum height of editor
        maxHeight: null,             // set maximum height of editor
        focus: true                  // set focus to editable area after initializing summernote);
    });


    /* affix the navbar after scroll below header */
    $('.sticky').affix({
        offset: {
            top: $('#top-nav').height() - $('.sticky').height(), right: 0
        }
    });

});


$(function() {
    

    $("#btnAddLocation").click(function () {
        $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: '@(Url.Action("AddLocation", "Locations"))', // Controller/View
            data: { //Passing data
                parkId: $("#parkId").val(), //Reading text box values using Jquery
                AlertId: $("#AlertId").val()

            }, success: function (result) {
                //alert("Data inserted");
                $("#parklist").load('@(Url.Action("Index","AlertParks",new { id = Model.AlertId }))');
                $("#msg").html(result.message);
                // window.location.reload();
            }
        });

       

    });

     


    $(document).on('change', '#RPrkId', function () {
        var prkId = $('.modal-body #RPrkId option:selected').val();
        //alert(prkId);
        $(".loadingOverlay").removeClass("hide");
        $.ajax(
                {
                    type: "GET", //HTTP POST Method,
                    dataType: 'json',
                    // contentType: 'application/json; charset=utf-8',
                    url: appUrl + '/Locations/GetPark/', // Controller/View
                    data: { //Passing data
                        id: prkId
                    },
                    success: function (data) {
                        //alert("Data inserted");
                        $.each(data, function (index, element) {
                            $('.modal-body #Name').val(element.name);
                            $('.modal-body #Latitude').val(element.latitude);
                            $('.modal-body #Longitude').val(element.longitude);
                            $('.modal-body #Contact').val(element.contact);
                            $('.modal-body #Email').val(element.email);
                            //hide the overlay
                           $(".loadingOverlay").addClass("hide");
                        });

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                        $("#msgmodal").html("<div class='alert alert-danger'> Error: " + thrownError + "</div>");
                    }
                });
    });

    $(document).on('click', '.modal-body #Name', function () {
        $('.modal-body #ParkId').val("");
        $('.modal-body #Name').val("");
        $('.modal-body #Latitude').val("");
        $('.modal-body #Longitude').val("");
        $('.modal-body #Contact').val("");
        $('.modal-body #Email').val("");

    });

    
       

    $(document).on('shown.bs.modal', function () {

      //  loadMap();
        
       // map.invalidateSize();
       //set chosen
        $(".chzn-select").chosen();

    });

    // $('#map_collapse').click(function () {
    //    // map.invalidateSize();
    //     loadMap();
    //    });
    

    //function loadMap()
    // {
    //    //setup the map
    //    var latlng = L.latLng(-30.81881, 116.16596);
    //    var map = L.map('lmap', { center: latlng, zoom: 6 });
    //    var lcontrol = new L.control.layers();

    //    var roadMutant = L.gridLayer.googleMutant({
    //        type: 'roadmap' // valid values are 'roadmap', 'satellite', 'terrain' and 'hybrid'
    //    }).addTo(map);
    //    var satMutant = L.gridLayer.googleMutant({
    //        maxZoom: 24,
    //        type: 'satellite'
    //    });

    //    var terrainMutant = L.gridLayer.googleMutant({
    //        maxZoom: 24,
    //        type: 'terrain'
    //    });

    //    var hybridMutant = L.gridLayer.googleMutant({
    //        maxZoom: 24,
    //        type: 'hybrid'
    //    });

    //    //add the control on the map

    //    lcontrol = L.control.layers({
    //        Roadmap: roadMutant,
    //        Aerial: satMutant,
    //        Terrain: terrainMutant,
    //        Hybrid: hybridMutant //,Styles: styleMutant

    //    }, {}, {
    //        collapsed: false
    //    }).addTo(map);

    //    var popup = L.popup();

    //    function onMapClick(e) {
    //        popup
    //       .setLatLng(e.latlng)
    //       .setContent("You clicked the map at " + e.latlng.toString())
    //       .openOn(map);
    //       // map.panTo(new L.LatLng(-30.81881, 116.16596));

    //        $('.modal-body #Latitude').val(e.latlng.lat);
    //        $('.modal-body #Longitude').val(e.latlng.lng);
    //    }

    //    map.on('click', onMapClick);

    //}
    

    //$("#btnPublish").click(function () {

    //    $.ajax(
    //    {
    //        type: "POST", //HTTP POST Method
    //        url: appUrl+ '/Alerts/Publish', // '@(Url.Action("Publish", "Alerts"))', // Controller/View
    //        data: { id: $("#AlertId").val() },
    //        success: function (result) {
    //            $("#msg").html(result.message);
    //        },
    //        error: function (xhr, ajaxOptions, thrownError) {
    //            //alert(xhr.status);
    //            // alert(thrownError);
    //             var err = "Error -" + xhr.status + "(" + thrownError + ")";
    //            $("#msg").html(err);
    //        }
    //    });

    //});


});




function SubmitInfo(formContainer) {
    var frm = $('#editform');

    $.ajax({
        url: frm.attr('action'),
        type: 'post',
        data: formContainer.serialize(),
        success: function (data) {
            if (data.IsSuccess) {
                // Clear the input tags
                formContainer.find("input[type='text']").each(function (i, element) {
                    $(this).val('');
                });
            }

            alert(data.Message);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });
}

$(function () {
    $.ajaxSetup({ cache: false });

    $("a[data-modal]").on("click", function (e) {
        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');

        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                /*backdrop: 'static',*/
                keyboard: true
            }, 'show');
            bindForm(this);
        });
        return false;
    });

});

$(function () {
    $.ajaxSetup({ cache: false });

    $("a[file-data-modal]").on("click", function (e) {
        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');

        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                /*backdrop: 'static',*/
                keyboard: true
            }, 'show');
            bindFileForm(this);
        });
        return false;
    });

});

function bindForm(dialog) {
    $('form', dialog).submit(function () {
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    $('#myModal').modal('hide');
                    $("#locations").load(result.url); //  Load data from the server and place the returned HTML into the matched element
                    $("#msg").html(result.message);
                } else {
                    $('#myModalContent').html(result);
                    bindForm(dialog);
                }
            }
        });
        return false;
    });
}

function bindFileForm(dialog) {
    $('#frmCreateFile', dialog).submit(function (e) {
        e.preventDefault();
        //show the loading panel
        $(".loadingOverlay").removeClass("hide");
        var fd = new FormData(document.querySelector("form"));
        //fd.append("ImageFile", "This is some extra data");
        fd.append("file", $('#file')[0].files[0]);
        var other_data = $(this).serialize();
       // alert(other_data);

        $.ajax({
            url: this.action + '?' + other_data,
            type: this.method,
            //  contentType: this.enctype,
            data: fd,
            processData: false,  // tell jQuery not to process the data
            contentType: false,
            success: function (result) {
                if (result.success) {
                    //hide the loading panel
                    $(".loadingOverlay").addClass("hide");
                    $('#myModal').modal('hide');
                    
                    $("#fileslist").load(result.url); //  Load data from the server and place the returned HTML into the matched element
                    $("#msg").html(result.message);
                } else {
                    $('#myModalContent').html(result);
                    bindFileForm(dialog);
                }
            },
            error: function (jqXHR, textStatus, errorMessage) {
                var msg = "Error: " + jqXHR.responseText + textStatus + errorMessage;
                alert(msg); // Optional
            }
        });
        return false;
    });
}