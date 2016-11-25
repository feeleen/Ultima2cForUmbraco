
var map;
var infowindow;
var geocoder;
	var marker;
	
var mapOptions = { center: new google.maps.LatLng(0.0, 0.0), zoom: 2,
mapTypeId: google.maps.MapTypeId.ROADMAP };

function initialize() {
    var myOptions = {
        center: new google.maps.LatLng(55.755826, 37.6173 ),
        zoom: 11,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    geocoder = new google.maps.Geocoder();
    var map = new google.maps.Map(document.getElementById("map_canvas"),
    myOptions);
    
    var myLatlng = {lat: 0, lng: 0};

  marker = new google.maps.Marker({
    position: myLatlng,
    map: map,
    title: 'Click to zoom',
    customInfo: -1
  });
    
    google.maps.event.addListener(map, 'click', function(event) {
        placeMarker(marker, event.latLng);
    });

	var defaultAddress = false;
	$(".addresses li").each(function() {
		var li = $(this);
		var myLatlng = {lat: li.data("lat"), lng: li.data("lng")};
		
		
		var item = new google.maps.Marker({
	    position: myLatlng,
	    map: map,
	    title: 'Click to zoom',
	    customInfo: li.data("id")
	  });
	    
	    item.addListener('click', function() {
	    placeMarker(item, item.getPosition())
	    map.setCenter(item.getPosition());
		attachSecretMessage(item, li.html())
	  });
	    
	    if (li.data("id") == $("#addressId").val())
		{
			defaultAddress = true;
			document.getElementById('lat').value=li.data("lat");
		    document.getElementById('lng').value=li.data("lng");
		    document.getElementById('address').value=li.html();
		    document.getElementById('addressId').value=li.data("id");
		    $("#selected_address").html($("#lat").val() + ", " + $("#lat").val() + "<br />" + $("#address").val());
		}
	});
	
	
		
	marker.addListener('click', function() {
	     //placeMarker(marker, marker.getPosition())
	    map.setCenter(marker.getPosition());
	//	attachSecretMessage(marker, document.getElementById('address').value )
	  });

}
			
function attachSecretMessage(marker, secretMessage) {
	if (infowindow)
	{
		infowindow.close();
	}
	

  infowindow = new google.maps.InfoWindow({
    content: $('#buble-cont').html().replace(/#address/, secretMessage)
  });
	infowindow.open(marker.get('map_canvas'), marker);
}

function placeMarker(themarker,location) {
			
    if(themarker){ 
    	
        themarker.setPosition(location);
    }else{
        themarker = new google.maps.Marker({
            position: location, 
            map: map,
            	 title: 'Click to zoom'
        });
    }
    document.getElementById('lat').value=location.lat();
    document.getElementById('lng').value=location.lng();
    
    document.getElementById('addressId').value=themarker.customInfo;
    
    if (themarker.customInfo != -1)
    {
    	getAddress(location);
	}
	else
	{
		getAddress(location, themarker);
	//	
	}
}

function getAddress(latLng, themarker = null) {
geocoder.geocode( {'latLng': latLng},
  function(results, status) {
    if(status == google.maps.GeocoderStatus.OK) {
      if(results[0]) {
        document.getElementById("address").value = results[0].formatted_address;
      }
      else {
        document.getElementById("address").value = "No results";
      }
    }
    else {
      document.getElementById("address").value = status;
    }
    
    $("#selected_address").html($("#lat").val() + ", " + $("#lat").val() + "<br />" + $("#address").val());
    if (themarker)
    {
    	attachSecretMessage(themarker, document.getElementById('address').value);
    }
  });
}

if (document.getElementById("map_canvas"))
{
	initialize();

}


