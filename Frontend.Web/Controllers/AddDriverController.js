taxiHeavenControllers.controller('addDriverController', ['$scope', 'basementService', 'userService', '$location', '$timeout',
    function ($scope, basementService, userService) {
        $scope.model = {};
        
        $scope.basements = [];
        basementService.get()
            .then(function (basements) {
                $scope.$apply(function() {
                    $scope.basements = basements;
                    if (basements.length > 0)
                        $scope.model.BasementId = basements[0].Id;
                });
            });
        $scope.add = function() {
            userService.addDriver($scope.model)
                .then(function(result) {
                    toastr.success(result);
                    location.href = '#/panel-administracyjny';
                })
                .fail(function(result) {
                    toastr.error(result.responseJSON.Message);
                });
        }
        $scope.cancel = function() {
            location.href = "#/panel-administracyjny";
        }
    }]);