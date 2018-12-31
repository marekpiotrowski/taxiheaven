taxiHeavenControllers.controller('addBasementController', ['$scope', 'basementService', 'userService', '$uibModal', '$location', '$timeout',
    function ($scope, basementService, userService, $uibModal) {
        $scope.model = {};

        $scope.place = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'Views/MapModal.html',
                controller: 'mapModalController',
                size: 'lg',
                resolve: {
                    initialAddress: function () {
                        return $scope.model.Place;
                    },
                    interactive: function () {
                        return true;
                    }
                }
            });

            modalInstance.result.then(function (location) {
                $scope.model.Place = location.address;
                $scope.model.Latitude = location.lat;
                $scope.model.Longitude = location.lng;
            }, function () {

            });
        };

        $scope.add = function() {
            basementService.add({
                Latitude: $scope.model.Latitude,
                Longitude: $scope.model.Longitude,
                Name: $scope.model.Name
            }).then(function(result) {
                toastr.success(result);
                location.href = '#/panel-administracyjny';
            }).fail(function(result) {
                toastr.error(result.responseJSON.Message);
            });
        }
        $scope.cancel = function() {
            location.href = "#/panel-administracyjny";
        }
    }]);