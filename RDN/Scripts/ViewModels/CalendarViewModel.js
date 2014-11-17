var CalendarViewModel = new function () {
    var thisViewModel = this;
    this.Events = ko.observableArray([]);
    this.maxId = ko.observable();
    this.pendingRequest = ko.observable(false);
    this.page = ko.observable();
    this.IsFinishedScrolling = ko.observable(false);
    this.SearchText = ko.observable();
    this.ListCountPull = ko.observable();

    

    function CreateMarkerForEvent(lon, lat, data, index) {
        if (lon != "0" && lat != "0") {
            //console.log(data[6]);
            var popupinfo = "<table class='popuptable'><tr><td class='popuptd'><div class='popupleagename'><a style='text-decoration: none;color:#BA2B3C;' target='_blank' href='" + data.NameUrl + "' >" + data.Name + "</a></div></br><div class='popupleagemem'>" + data.DateTimeHuman + "</div></br><div class='popupaddress'>" + data.Address + "</div></br><div class='popupbold'></div></td><td style='width:50px;padding-top:5px;padding-right:5px'>";
            if (data[6] != null)
                popupinfo += "<img src='" + data[6] + "' width='100px'/>";
            popupinfo += "</td></tr></table>";
            var lonLat = new OpenLayers.LonLat(lon, lat)
                  .transform(
                    new OpenLayers.Projection(PROJECTIONFROM, PROJECTIONTO), // transform from WGS 1984
                    map.getProjectionObject() // to Spherical Mercator Projection
                  );
            var myLocation = new OpenLayers.Geometry.Point(lon, lat)
                .transform(PROJECTIONFROM, PROJECTIONTO);
            var zoom = 0;
            var markers = new OpenLayers.Layer.Markers("Markers");
            markers.setZIndex(100);
            map.addLayer(markers);
            var size = new OpenLayers.Size(21, 25);
            var offset = new OpenLayers.Pixel(-(size.w / 2), -size.h);
            var icon = new OpenLayers.Icon('../Content/images/marker3.png', size, offset);
            markers.addMarker(new OpenLayers.Marker(lonLat, icon));

            markers.events.register('mousedown', markers, function (evt) {
                $(".olPopup").hide();
                var popup = new OpenLayers.Popup.FramedCloud("Popup",
    myLocation.getBounds().getCenterLonLat(), null,
    popupinfo, null,
    true // <-- true if we want a close (X) button, false otherwise
    );

                map.addPopup(popup);

                OpenLayers.Event.stop(evt);
                $(".olPopup").css("z-index", 10000);
            });
            map.setCenter(lonLat);
            map.zoomToMaxExtent();

        }
    }

    this.LoadEventMap = function (count) {

        $.ajax({
            type: "POST",
            url: apiUrl + "Calendar/SearchEventsAllByLL",
            data: { p: thisViewModel.page(), c: thisViewModel.ListCountPull(), lat: 0.0, lon: 0.0 },
            dataType: "json",
            success: function (data) {

            }
        });
    }

    this.LoadEventsList = function (count) {
        thisViewModel.ListCountPull(count);
        thisViewModel.page(0);
        getItems(thisViewModel.ListCountPull(), false, true);
        $(window).scroll(function () {
            if ($(window).scrollTop() >= $(document).height() - $(window).height() - 400) {
                getItems(thisViewModel.ListCountPull(), false, true);
            }
        });
    }

    this.SearchEvents = function (input) {
        thisViewModel.SearchText($(input).val());
        thisViewModel.IsFinishedScrolling(false);
        thisViewModel.pendingRequest(false);
        thisViewModel.page(0);
        getItems(thisViewModel.ListCountPull(), true, true);
    }

    function getItems(cnt, isSearch, addMarkers) {
        if (!thisViewModel.pendingRequest() && !thisViewModel.IsFinishedScrolling()) {
            thisViewModel.pendingRequest(true);
            $.ajax({
                type: "POST",
                url: apiUrl + "Calendar/SearchEventsAll",
                data: { p: thisViewModel.page(), c: cnt, s: thisViewModel.SearchText() },
                dataType: "json",
                success: function (data) {
                    if (data.Events.length > 0) {
                        if (!isSearch) {
                            ko.utils.arrayForEach(data.Events, function (entry) {
                                thisViewModel.Events.push(entry);
                            });
                        }
                        else {
                            thisViewModel.Events.removeAll();
                            thisViewModel.Events(data);
                        }
                        if (addMarkers) {
                            var i = 0;
                            ko.utils.arrayForEach(thisViewModel.Events(), function (entry) {
                                CreateMarkerForEvent(entry.Latitude, entry.Longitude, entry, i + 1);
                            });
                        }
                        thisViewModel.page(thisViewModel.page() + 1);
                        thisViewModel.pendingRequest(false);
                       
                    }
                    else
                        thisViewModel.IsFinishedScrolling(true);
                }
            });

        }
    }

}