taxiHeavenControllers.controller('placeAnOrderController', ['$scope', '$uibModal',  'orderService', '$routeParams', 'userService', '$location', '$timeout',
    function ($scope, $uibModal, orderService, $routeParams, userService) {
        if ($routeParams.orderId) {
            orderService.get($routeParams.orderId)
                .then(function (result) {
                    $scope.$apply(function() {
                        $scope.model = result;
                        $scope.requiresSpecialCarDisplay = result.RequiresSpecialCar ? 'Tak' : 'Nie';
                    });
                });
        }
        $scope.register = false;
        $scope.user = {};

        $scope.model = {
            Start: null,
            Destination: null,
            StartLatitude: null,
            StartLongitude: null,
            DestinationLatitude: null,
            DestinationLongitude: null,
            FirstName: null,
            LastName: null,
            PhoneNumber: null,
            Email: null,
            RequiresSpecialCar: false
        }

        $scope.chooseStart = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'Views/MapModal.html',
                controller: 'mapModalController',
                size:'lg',
                resolve: {
                    initialAddress: function () {
                        return $scope.model.Start;
                    },
                    interactive: function () {
                        return true;
                    }
                }
            });

            modalInstance.result.then(function (location) {
                $scope.model.Start = location.address;
                $scope.model.StartLatitude = location.lat;
                $scope.model.StartLongitude = location.lng;
            }, function () {

            });
        };
        $scope.chooseDestination = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'Views/MapModal.html',
                controller: 'mapModalController',
                size: 'lg',
                resolve: {
                    initialAddress: function () {
                        return $scope.model.Destination;
                    },
                    interactive: function () {
                        return true;
                    }
                }
            });

            modalInstance.result.then(function (location) {
                $scope.model.Destination = location.address;
                $scope.model.DestinationLatitude = location.lat;
                $scope.model.DestinationLongitude = location.lng;
            }, function () {

            });
        };

        $scope.placeAnOrder = function () {
            orderService.placeAnOrder($scope.model)
                .then(function(result) {
                    window.location.href = "#/nowe-zamowienie/" + result.Id;
                }).fail(function(result) {
                    toastr.error(result.responseJSON.Message);
                });
        }

        $scope.confirm = function() {
            orderService.confirm($scope.model.Id)
                .then(function () {
                    toastr.success("Potwierdzono zamówienie - proszę czekać na maila z potwierdzeniem.")
                    window.location.href = "#/nowe-zamowienie";
                });
        }

        $scope.cancel = function () {
            orderService.cancel($scope.model.Id)
                .then(function () {
                    toastr.info("Zamówienie anulowano.")
                    window.location.href = "#/nowe-zamowienie";
                });
        }

        $scope.show = function (location) {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'Views/MapModal.html',
                controller: 'mapModalController',
                size: 'lg',
                resolve: {
                    initialAddress: function () {
                        return location;
                    },
                    interactive: function() {
                        return false;
                    }
                }
            });
        }

        $scope.signIn = function() {
            userService.signIn($scope.login, $scope.password)
                .then(function(person) {
                    sessionStorage.setItem("login", person.Email);
                    sessionStorage.setItem("password", person.Password);
                    sessionStorage.setItem("roles", person.UserRoles);
                    location.href = '#/panel-administracyjny';
                }).fail(function(result) {
                    toastr.error(result.responseJSON.Message);
                });
        }

        $scope.signUp = function() {
            userService.signUp($scope.user)
                .then(function(result) {
                    toastr.success(result);
                    $scope.$apply(function() {
                        $scope.register = false;
                    });
                    $scope.user = {};
                }).fail(function (result) {
                    toastr.error(result.responseJSON.Message);
                });
        }

        $scope.switchToRegister = function() {
            $scope.register = true;
        }

        $scope.switchToSignIn = function () {
            $scope.register = false;
        }
        $scope.showRegister = function() {
            if (sessionStorage.getItem('login'))
                return false;
            return $scope.register;
        }
        $scope.showLogin = function () {
            if (sessionStorage.getItem('login'))
                return false;
            return !$scope.register;
        }
    }]);