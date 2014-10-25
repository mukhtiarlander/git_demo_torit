angular.module('blogAdmin').controller('DashboardController', ["$rootScope", "$scope", "$location", "$log", "dataService", function ($rootScope, $scope, $location, $log, dataService) {
    $scope.stats = {};
    $scope.funds = {};
    $scope.draftposts = [];
    $scope.draftpages = [];
    $scope.recentcomments = [];
    $scope.trash = [];
    $scope.packages = [];
    $scope.logItems = [];
    $scope.itemToPurge = {};

    $scope.openLogFile = function () {
        dataService.getItems('/api/logs/getlog/file')
        .success(function (data) {
            angular.copy(data, $scope.logItems);
            $("#modal-log-file").modal();
            return false;
        })
        .error(function (data) {
            toastr.error($rootScope.lbl.errorGettingLogFile);
        });
    }

    $scope.purgeLog = function () {
        dataService.updateItem('/api/logs/purgelog/file', $scope.itemToPurge)
        .success(function (data) {
            $scope.logItems = [];
            $("#modal-log-file").modal('hide');
            toastr.success($rootScope.lbl.purged);
            return false;
        })
        .error(function (data) {
            toastr.error($rootScope.lbl.errorPurging);
        });
    }

    $scope.purge = function (id) {
        if (id) {
            $scope.itemToPurge = findInArray($scope.trash.Items, "Id", id);
        }
        dataService.updateItem('/api/trash/purge/' + id, $scope.itemToPurge)
        .success(function (data) {
            $scope.loadTrash();
            toastr.success($rootScope.lbl.purged);
            return false;
        })
        .error(function (data) {
            toastr.error($rootScope.lbl.errorPurging);
        });
    }

    $scope.purgeAll = function () {
        dataService.updateItem('/api/trash/purgeall/all')
        .success(function (data) {
            $scope.loadTrash();
            toastr.success($rootScope.lbl.purged);
            return false;
        })
        .error(function (data) {
            toastr.error($rootScope.lbl.errorPurging);
        });
    }

    $scope.restore = function (id) {
        if (id) {
            $scope.itemToPurge = findInArray($scope.trash.Items, "Id", id);
        }
        dataService.updateItem('/api/trash/restore/' + id, $scope.itemToPurge)
        .success(function (data) {
            $scope.loadTrash();
            toastr.success($rootScope.lbl.restored);
            return false;
        })
        .error(function (data) {
            toastr.error($rootScope.lbl.errorRestoring);
        });
    }

    $scope.load = function () {
        spinOn();

        $scope.loadPackages();

        dataService.getItems('/api/stats')
            .success(function (data) { angular.copy(data, $scope.stats); })
            .error(function (data) { toastr.success($rootScope.lbl.errorGettingStats); });

        dataService.getItems('/api/funds')
    .success(function (data) { angular.copy(data, $scope.funds); })
    .error(function (data) { toastr.success($rootScope.lbl.errorGettingFunds); });


        dataService.getItems('/api/posts', { take: 15, skip: 0, filter: "IsPublished == false" })
            .success(function (data) { angular.copy(data, $scope.draftposts); })
            .error(function () { toastr.error($rootScope.lbl.errorLoadingDraftPosts); });

        dataService.getItems('/api/comments', { type: 5, take: 5, skip: 0, filter: "IsDeleted == false", order: "DateCreated descending" })
            .success(function (data) { angular.copy(data, $scope.recentcomments); })
            .error(function () { toastr.error($rootScope.lbl.errorLoadingRecentComments); });

        dataService.getItems('/api/logs/getlog/file')
            .success(function (data) { angular.copy(data, $scope.logItems); })
            .error(function (data) { toastr.error($rootScope.lbl.errorGettingLogFile); });

        $scope.loadTrash();
    }

    $scope.loadPackages = function () {
        loading("gal");
        dataService.getItems('/api/packages', { take: 5, skip: 0 })
        .success(function (data) {
            angular.copy(data, $scope.packages);
            loaded("gal");
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPackages);
            loaded("gal");
        });
    }

    $scope.loadTrash = function () {
        dataService.getItems('/api/trash', { type: 0, take: 5, skip: 0 })
            .success(function (data) { angular.copy(data, $scope.trash); })
            .error(function () { toastr.error($rootScope.lbl.errorLoadingTrash); });
    }

    $scope.load();
}]);