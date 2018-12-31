taxiHeavenControllers.controller('mapModalController', ['$scope', '$uibModalInstance', 'initialAddress', 'interactive', '$location', '$timeout',
    function ($scope, $uibModalInstance, initialAddress, interactive) {
        var address, lat, lng;
        $scope.ok = function () {
            $uibModalInstance.close({
                lat: lat,
                lng: lng,
                address: address
            });
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        $uibModalInstance.rendered.then(function () {
            var map;
            function initMap() {
                map = new google.maps.Map(document.getElementById('map'), {
                    center: { lat: 51.1127489, lng: 17.0276665 },
                    zoom: 13
                });

                // Create the search box and link it to the UI element.
                var input = document.getElementById('pac-input');
                var searchBox = new google.maps.places.SearchBox(input);
                map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);


                function refresh(formattedAddress) {
                    var input = angular.element("#pac-input")[0];
                    input.value = formattedAddress;
                    searchBox.setValues([formattedAddress]);

                    google.maps.event.trigger(input, 'focus');

                    google.maps.event.trigger(input, 'keydown', {
                        keyCode: 13
                    });
                    if (!interactive)
                        input.disabled = true;
                    google.maps.event.trigger(map, 'resize');
                }


                // Bias the SearchBox results towards current map's viewport.
                map.addListener('bounds_changed', function () {
                    searchBox.setBounds(map.getBounds());
                });

                if (interactive) {
                    map.addListener('click',
                        function(e) {
                            var place = { lat: e.latLng.lat(), lng: e.latLng.lng() };
                            var geocoder = new google.maps.Geocoder;
                            geocoder.geocode({ 'location': place },
                                function(results, status) {
                                    if (!results)
                                        return;
                                    refresh(results[0].formatted_address);
                                });
                        });
                }
                var markers = [];
                // Listen for the event fired when the user selects a prediction and retrieve
                // more details for that place.
                searchBox.addListener('places_changed', function () {
                    var places = searchBox.getPlaces();

                    if (places.length == 0) {
                        return;
                    }
                    places = [places[0]];
                    address = places[0].formatted_address;
                    lat = places[0].geometry.location.lat();
                    lng = places[0].geometry.location.lng();
                    // Clear out the old markers.
                    markers.forEach(function (marker) {
                        marker.setMap(null);
                    });
                    markers = [];

                    // For each place, get the icon, name and location.
                    var bounds = new google.maps.LatLngBounds();
                    places.forEach(function (place) {
                        var icon = {
                            url: place.icon,
                            size: new google.maps.Size(71, 71),
                            origin: new google.maps.Point(0, 0),
                            anchor: new google.maps.Point(17, 34),
                            scaledSize: new google.maps.Size(25, 25)
                        };

                        // Create a marker for each place.
                        markers.push(new google.maps.Marker({
                            map: map,
                            icon: icon,
                            title: place.name,
                            position: place.geometry.location
                        }));

                        if (place.geometry.viewport) {
                            // Only geocodes have viewport.
                            bounds.union(place.geometry.viewport);
                        } else {
                            bounds.extend(place.geometry.location);
                        }
                    });
                    map.fitBounds(bounds);
                    map.setZoom(16);
                });

                if (initialAddress) {
                    if (!interactive)
                        setTimeout(function() {
                                refresh(initialAddress);
                            },
                            500);
                    else {
                        refresh(initialAddress);
                    }
                }
            };
            initMap();
        });
    }]);