'use strict';

var app = angular.module('publicWebTesterApplication', []);

function MainGridController($http, $scope) {
    $http({ method: 'GET', url: '/api/PublicWeb/2013-7' }).
          success(function (data, status, headers, config) {
            $scope.returns = data;
        }).
          error(function (data, status, headers, config) {
              // called asynchronously if an error occurs
              // or server returns response with an error status.
          });
}