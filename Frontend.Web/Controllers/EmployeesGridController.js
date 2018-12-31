taxiHeavenControllers.controller('employeesGridController', ['$scope', 'userService', '$location', '$timeout',
    function ($scope, userService) {
        $scope.drivers = [];
        userService.get()
            .then(function(data) {
                var drivers = [];
                data.forEach(function(driver, i) {
                    drivers.push({
                        Id: driver.Id,
                        'Imię': driver.FirstName,
                        'Nazwisko': driver.LastName,
                        'Status': driver.Status,
                        'Baza': driver.Basement
                    });
                });
                $scope.$apply(function() {
                    $scope.drivers = drivers;
                });
            });
    }]);