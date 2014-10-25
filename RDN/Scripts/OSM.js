map = new OpenLayers.Map("mapdiv");
map.addLayer(new OpenLayers.Layer.OSM());

var PROJECTIONFROM = "EPSG:4326";
var PROJECTIONTO = "EPSG:900913"

function createmarkerforleage(lon, lat, data, index) {
    if (lon != "0" && lat != "0") {
       console.log(data[5]);
        var popupinfo = "<table class='popuptable'><tr><td class='popuptd'><div class='popupleagename'><a style='text-decoration: none;color:#BA2B3C;' target='_blank' href='" + data[8] + "' >" + data[2] + "</a></div></br><div class='popupleagemem'>" + data[7] + " Members</div></br><div class='popupaddress'>" + data[3] + " " + data[4] + "</br>" + data[5] + "</div></br><div class='popupbold'></div></td><td style='width:50px;padding-top:5px;padding-right:5px'><img src='" + data[5] + "' height='100px' /></td></tr></table>"
        var lonLat = new OpenLayers.LonLat(lon, lat)
              .transform(
                new OpenLayers.Projection(PROJECTIONFROM, PROJECTIONTO), // transform from WGS 1984
                map.getProjectionObject() // to Spherical Mercator Projection
              );
        var myLocation = new OpenLayers.Geometry.Point(lon, lat)
            .transform(PROJECTIONFROM, PROJECTIONTO);
        var zoom = 0;
        var markers = new OpenLayers.Layer.Markers("Markers");
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
        }); map.setCenter(lonLat);
        map.zoomToMaxExtent();
    }
}
