var taxiHeaven = angular.module('taxiHeaven', ['ngRoute', 'taxiHeavenControllers', 'ui.bootstrap']);

var taxiHeavenControllers = angular.module('taxiHeavenControllers', []);

taxiHeaven.config(function($routeProvider) {
        $routeProvider.when('/nowe-zamowienie',
            {
                controller: 'placeAnOrderController',
                templateUrl: 'Views/PlaceAnOrder.html'
            })
            .when('/nowe-zamowienie/:orderId',
            {
                controller: 'placeAnOrderController',
                templateUrl: 'Views/ConfirmOrder.html'
            })
            .when('/panel-administracyjny/',
            {
                controller: 'adminPanelController',
                templateUrl: 'Views/AdminPanel.html'
            })
            .when('/dodaj-pracownika/',
            {
                controller: 'addEmployeeController',
                templateUrl: 'Views/AddEmployee.html'
            })
            .when('/dodaj-kierowce/',
            {
                controller: 'addDriverController',
                templateUrl: 'Views/AddDriver.html'
            })
            .when('/lista-kierowcow/',
            {
                controller: 'employeesGridController',
                templateUrl: 'Views/EmployeesGrid.html'
            })
            .when('/dodaj-baze/',
            {
                controller: 'addBasementController',
                templateUrl: 'Views/AddBasement.html'
            })
            .otherwise({
                redirectTo: '/'
            });
    })
    .config(function($httpProvider) {

    });

taxiHeaven.run(function($rootScope) {
    $rootScope.$on("$locationChangeSuccess", function (event, next, current) {
        $(document)
            .ready(function () {
                if (location.hash.indexOf('nowe-zamowienie') >= 0) {
                    $('.nav #nowe').addClass('active');
                    $('.nav #admin').removeClass('active');
                } else {
                    $('.nav #nowe').removeClass('active');
                    $('.nav #admin').addClass('active');
                }
                if (!sessionStorage.getItem('login'))
                    $('.nav #logOut').hide();
                else
                    $('.nav #logOut').show();
                $('.nav #logOut a')
                    .click(function () {
                        sessionStorage.clear();
                        location.href = '#/nowe-zamowienie';
                        location.reload();
                    });


            });

    });
});