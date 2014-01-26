'use strict';

var app = angular.module('publicWebTesterApplication', []);

app.directive('eatClick', function () {
    return function (scope, element, attrs) {
        angular.element(element).bind('click', function (event) {
            event.preventDefault();
        });
    };
});

function MainGridController($http, $scope, $log, $rootScope) {

    $scope.selectedMonth = "2013-10";

    var updateData = function () {

        var url = '/api/PublicWeb/' + $scope.selectedMonth;

        $http({ method: 'GET', url: url }).
              success(function (data, status, headers, config) {
                  $scope.returns = data;
              });
    };

    $scope.monthChanged = function() {
        updateData();
    };

    $scope.calcSelected = function(ret, column) {
        $rootScope.$broadcast("CalcSelected", {
            ret: ret,
            column: column
        });
    };

    updateData();
}

function ShowWorkController($rootScope, $scope, $log) {
    $rootScope.$on("CalcSelected", function (event, args) {
        var returnObj = args.ret[args.column];

        $scope.calculation = "Type: " + args.column + "\n";
        $scope.calculation += "Calculation: " + returnObj.Calculation + "\n";
        $scope.calculation += "Has Error: " + returnObj.HasError + "\n";
        $scope.calculation += "Error Message: " + returnObj.ErrorMessage + "\n";
    });
}