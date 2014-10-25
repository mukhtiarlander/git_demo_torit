angular.module('blogAdmin').controller('CategoriesController', ["$rootScope", "$scope", "$location", "$http", "$filter", "dataService", function ($rootScope, $scope, $location, $http, $filter, dataService) {
	$scope.items = [];
	$scope.lookups = [];
	$scope.category = {};
	$scope.id = ($location.search()).id;
	$scope.EditPostCategories = EditPostCategories();


	if ($scope.id) {
		dataService.getItems('/api/categories', { Id: $scope.id })
			.success(function (data) { angular.copy(data, $scope.category); })
			.error(function () { toastr.error($rootScope.lbl.errorLoadingCategory); });
		$("#modal-add-cat").modal();
	}

	$scope.addNew = function () {
		$scope.clear();
		$("#modal-add-cat").modal();
	}

	$scope.load = function () {
		spinOn();
		dataService.getItems('/api/lookups')
			.success(function (data) { angular.copy(data, $scope.lookups); })
			.error(function () { toastr.error("Error loading lookups"); });

		dataService.getItems('/api/categories')
			.success(function (data) {
				angular.copy(data, $scope.items);
				gridInit($scope, $filter);
				spinOff();
			})
			.error(function () {
				toastr.error($rootScope.lbl.errorLoadingCategories);
				spinOff();
			});
	}

	$scope.load();

	$scope.save = function () {
		if (!$('#form').valid()) {
			return false;
		}
		if ($scope.category.Id) {
			dataService.updateItem("/api/categories/update/" + $scope.category.Id, $scope.category)
		   .success(function (data) {
			   toastr.success($rootScope.lbl.categoryUpdated);
			   $scope.load();
		   })
		   .error(function () { toastr.error($rootScope.lbl.updateFailed); });
		}
		else {
			dataService.addItem("/api/categories", $scope.category)
		   .success(function (data) {
			   toastr.success($rootScope.lbl.categoryAdded);
			   if (data.Id) {
				   angular.copy(data, $scope.category);
				   $scope.load();
			   }
		   })
		   .error(function () { toastr.error($rootScope.lbl.failedAddingNewCategory); });
		}
		$("#modal-add-cat").modal('hide');
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
		dataService.processChecked("/api/categories/processchecked/" + action, $scope.items)
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

	$scope.clear = function () {
		$scope.category = { "IsChecked": false, "Id": null, "Parent": null, "Title": "", "Description": "", "Count": 0 };
		$scope.id = null;
	}

	$(document).ready(function () {
		$('#form').validate({
			rules: {
				txtSlug: { required: true }
			}
		});
	});
}]);