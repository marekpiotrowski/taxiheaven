taxiHeavenControllers.controller('addEmployeeController', ['$scope', 'userService', '$location', '$timeout',
    function ($scope, userService) {
        function guid() {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                  .toString(16)
                  .substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
              s4() + '-' + s4() + s4() + s4();
        }

        $scope.guid = guid();

        $scope.adminChecked = false;
        $scope.employeeChecked = true;

        $scope.add = function() {
            userService.addEmployee($scope.adminChecked || false, $scope.employeeChecked || false, $scope.guid)
                .then(function(result) {
                    toastr.success(result);
                    location.href = "#/panel-administracyjny";
                }).fail(function (result) {
                    if (result && result.responseJSON)
                        toastr.error(result.responseJSON.Message);
                    else if(result.status == 401)
                        toastr.error("Nie masz wystarczających uprawnień.");
                });
        }

        $scope.cancel = function() {
            location.href = "#/panel-administracyjny";
        }
    }]);