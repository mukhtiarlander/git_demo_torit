angular.module('blogAdmin').controller('PostsController', ["$rootScope", "$scope", "$location", "$http", "$filter", "dataService", function ($rootScope, $scope, $location, $http, $filter, dataService) {
    $scope.items = [];
    $scope.filter = ($location.search()).fltr;
    $scope.canPublishPosts = canPublishPosts();
    $scope.Message = {};


    $scope.load = function () {
        spinOn();
        var url = '/api/posts';
        var p = { take: 20, skip: 0 }
        dataService.getItems(url, p)
        .success(function (data) {
            angular.copy(data.Posts, $scope.items);

            gridInit($scope, $filter, data.TotalPostCount);

            if ($scope.filter) {
                $scope.setFilter($scope.filter);
            }
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPosts);
            spinOff();
        });
    }
    $scope.loadPublished = function () {
        spinOn();
        var url = '/api/posts';
        var p = { take: 20, skip: 0, pub: true }
        dataService.getItems(url, p)
        .success(function (data) {
            angular.copy(data.Posts, $scope.items);
            
            if ($scope.filter) {
                $scope.setFilter($scope.filter);
            }
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPosts);
            spinOff();
        });
    }
    $scope.loadUnPublished = function () {
        spinOn();
        var url = '/api/posts';
        var p = { take: 20, skip: 0, pub: false }
        dataService.getItems(url, p)
        .success(function (data) {
            angular.copy(data.Posts, $scope.items);

            if ($scope.filter) {
                $scope.setFilter($scope.filter);
            }
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPosts);
            spinOff();
        });
    }
    $scope.load();

    $scope.getPage = function (page) {

        spinOn();
        var url = '/api/posts';
        var p = { take: 20, skip: (page * 20) }
        dataService.getItems(url, p)
        .success(function (data) {
            angular.copy(data.Posts, $scope.items);
            $scope.currentPage = page;
            if ($scope.filter) {
                $scope.setFilter($scope.filter);
            }
            spinOff();
        })
            .error(function () {
                toastr.error($rootScope.lbl.errorLoadingPosts);
                spinOff();
            });
    }
    $scope.searchPage = function (s) {

        spinOn();
        var url = '/api/posts';
        var p = { take: 20, skip: 0, filter: s }
        dataService.getItems(url, p)
        .success(function (data) {
            angular.copy(data.Posts, $scope.items);
            
            if ($scope.filter) {
                $scope.setFilter($scope.filter);
            }
            spinOff();
        })
            .error(function () {
                toastr.error($rootScope.lbl.errorLoadingPosts);
                spinOff();
            });

    }


    $scope.processChecked = function (action) {
        spinOn();
        var i = $scope.items.length;
        var checked = [];
        while (i--) {
            var item = $scope.items[i];
            if (item.IsChecked === true) {
                checked.push(item);
            }
        }
        if (checked.length < 1) {
            spinOff();
            return false;
        }
        dataService.processChecked("/api/posts/processchecked/" + action, $scope.items)
        .success(function (data) {
            $scope.load();
            gridInit($scope, $filter);
            toastr.success($rootScope.lbl.completed);
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            spinOff();
        });
    }

    $scope.setFilter = function (filter) {
        if ($scope.filter === 'pub') {
            $scope.gridFilter('IsPublished', true, 'pub');
        }
        if ($scope.filter === 'dft') {
            $scope.gridFilter('IsPublished', false, 'dft');
        }
    }

}]);