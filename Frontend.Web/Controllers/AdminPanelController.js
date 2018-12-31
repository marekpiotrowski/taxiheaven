taxiHeavenControllers.controller('adminPanelController', ['$scope', '$location', '$timeout',
    function ($scope) {
        if (!sessionStorage.getItem('login')) {
            location.href = '#/nowe-zamowienie';
            toastr.error("Nie masz dostępu do panelu administracyjnego.");
        }

        $scope.addEmployee = function() {
            location.href = "#/dodaj-pracownika";
        }

        $scope.addDriver = function() {
            location.href = '#/dodaj-kierowce';
        }

        $scope.list = function() {
            location.href = '#/lista-kierowcow';
        }

        $scope.addBasement = function() {
            location.href = '#/dodaj-baze';
        }
    }]);