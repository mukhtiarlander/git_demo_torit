angular.module('blogAdmin').controller('CommentsController', ["$rootScope", "$scope", "$location", "$filter", "$log", "dataService", function ($rootScope, $scope, $location, $filter, $log, dataService) {
    $scope.items = [];
    $scope.item = {};
    $scope.id = ($location.search()).id;
    $scope.filter = ($location.search()).fltr;
    $scope.commentsPage = true;

    if ($scope.id) {
        $("#modal-add-item").modal();
    }

    $scope.load = function () {
        spinOn();
        var p = { type: 5, take: 0, skip: 0, filter: "", order: "" };
        dataService.getItems('/api/comments', p)
        .success(function (data) {
            angular.copy(data, $scope.items);
            gridInit($scope, $filter);
            if ($scope.filter) {
                $scope.setFilter();
            }
            spinOff();
        })
        .error(function (data) {
            toastr.error($rootScope.lbl.errorGettingTags);
            spinOff();
        });
    }

    $scope.setFilter = function () {
        if ($scope.filter === 'pnd') {
            $scope.gridFilter('IsPending', true, 'pnd');
        }
        if ($scope.filter === 'apr') {
            $scope.gridFilter('IsApproved', true, 'apr');
        }
        if ($scope.filter === 'spm') {
            $scope.gridFilter('IsSpam', true, 'spm');
        }
    }

    $scope.load();
	
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
	        return false;
	    }
        dataService.processChecked("/api/comments/processchecked/" + action, checked)
        .success(function (data) {
            $scope.load();
            toastr.success($rootScope.lbl.completed);
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            spinOff();
        });
	}

	$scope.deleteAll = function () {
	    if ($scope.filter) {
	        spinOn();
	        var url = "/api/comments/DeleteAll/spam";

	        if ($scope.filter === "pnd") {
	            url = "/api/comments/DeleteAll/pending";
	        }
	        dataService.updateItem(url, { item: $scope.item })
            .success(function (data) {
                toastr.success($rootScope.lbl.commentsDeleted);
                $scope.load();
                spinOff();
            })
            .error(function () { toastr.error($rootScope.lbl.failed); spinOff(); });
	    }
	}

	$scope.showPage = function (n, current, total) {
	    if (!current) {
	        current = 0;
	    }
	    if (n === 0 || n - 1 === current || n - 2 === current || n - 3 === current) {
	        return true;
	    }
	    if (n === current || n + 1 === current || n + 2 === current || n + 3 === current) {
	        return true;
	    }
	    if (n + 1 === total) {
	        return true;
	    }
	    return false;
	}

    $scope.save = function () {
        if ($scope.tag) {
            dataService.updateItem("/api/comments", { item: $scope.item })
           .success(function (data) {
               toastr.success($rootScope.lbl.commentUpdated);
               $scope.load();
           })
           .error(function () { toastr.error($rootScope.lbl.updateFailed); });
        }
        $("#modal-add-item").modal('hide');
    }
}]);