angular.module('blogAdmin').controller('TagsController', ["$rootScope", "$scope", "$location", "$filter", "$log", "dataService", function ($rootScope, $scope, $location, $filter, $log, dataService) {
    $scope.data = dataService;
    $scope.items = [];
    $scope.id = {};
    $scope.tag = {};
    $scope.EditPostTags = EditPostTags();

    $scope.loadEditForm = function (id) {
        $scope.id = id;
        $scope.tag = id;
        $("#modal-add-tag").modal();
    }

    $scope.load = function () {
        spinOn();
        var p = { take: 0, skip: 0, postId: "" };
        dataService.getItems('/api/tags', p)
            .success(function (data) {
                angular.copy(data, $scope.items);
                gridInit($scope, $filter);
                spinOff();
            })
            .error(function (data) {
                toastr.success($rootScope.lbl.errorGettingTags);
                spinOff();
            });
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
	        spinOff();
	        return false;
	    }
        dataService.processChecked("/api/tags/processchecked/" + action, $scope.items)
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

    $scope.save = function () {
        if ($scope.tag) {
            dataService.updateItem("/api/tags", { OldTag: $scope.id, NewTag: $scope.tag })
           .success(function (data) {
               toastr.success($rootScope.lbl.tagUpdated);
               $scope.load();
               gridInit($scope, $filter);
           })
           .error(function () { toastr.error($rootScope.lbl.updateFailed); });
        }
        $("#modal-add-tag").modal('hide');
    }
}]);