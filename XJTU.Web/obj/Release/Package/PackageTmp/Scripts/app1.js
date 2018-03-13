var myApp1 = angular.module("myApp1", ['ui.router', 'ui.bootstrap', 'ngAnimate']);
myApp1.config(function ($stateProvider, $httpProvider, $urlRouterProvider) {
    //$urlRouterProvider.when("", "/PageTab");
    $urlRouterProvider.otherwise('/HomeIndex');
    $stateProvider.state("HomeIndex", {
             url: "/HomeIndex",
             controller: 'HomeIndexController',
             templateUrl: "HomeIndexContent.html"
    }).state("Login", {
        url: "/Login",
        controller: 'LoginController',
        templateUrl: "LoginView.html"
    });
    $httpProvider.interceptors.push('myInterceptor');
});
//loading  
myApp1.factory('myInterceptor', ["$rootScope", function ($rootScope) {
    var timestampMarker = {
        request: function (config) {
            $rootScope.loading = true;
            return config;
        },
        requestError: function (response) {
            $rootScope.loading = false;
            return response;
        },
        response: function (response) {
            $rootScope.loading = false;
            return response;
        },
        responseError: function (response) {
            $rootScope.loading = false;
            return response;
        }

    };
    return timestampMarker;
}]);