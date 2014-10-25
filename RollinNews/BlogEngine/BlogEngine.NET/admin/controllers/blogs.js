
angular.module('blogAdmin').controller('BlogsController', ["$rootScope", "$scope", "$filter", "dataService", function ($rootScope, $scope, $filter, dataService) {
    $scope.items = [];
    $scope.editItem = {};
    $scope.newItem = {};
    $scope.modalTitle = $rootScope.lbl.addNewBlog;

    $scope.one = 1;

    $scope.modalNew = function () {
        $scope.modalTitle = $rootScope.lbl.addNewBlog;
        $scope.editItem = {};
        $("#modal-add").modal();
    }

    $scope.modalEdit = function (id) {
        $scope.modalTitle = $rootScope.lbl.editExistingBlog;
        spinOn();
        dataService.getItems('/api/blogs/' + id)
        .success(function (data) {
            angular.copy(data, $scope.editItem);
            $("#modal-edit").modal();
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingBlog);
            spinOff();
        });
    }

    $scope.load = function (callback) {
        spinOn();
        dataService.getItems('/api/blogs', { take: 0, skip: 0, filter: "1 == 1", order: "Name" })
        .success(function (data) {
            angular.copy(data, $scope.items);
            gridInit($scope, $filter);
            callback;
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingBlogs);
            spinOff();
        });
    }

    $scope.save = function () {
        spinOn();
        dataService.updateItem("/api/blogs/update/item", $scope.editItem)
        .success(function (data) {
            toastr.success($rootScope.lbl.blogSaved);
            $scope.load();
            spinOff();
            $("#modal-edit").modal('hide');
        })
        .error(function () {
            toastr.error($rootScope.lbl.failedAddingNewRole);
            spinOff();
            $("#modal-edit").modal('hide');
        });
    }

    $scope.saveNew = function () {
        if (!$('#form').valid()) {
            return false;
        }
        spinOn();
        dataService.addItem("/api/blogs", $scope.newItem)
        .success(function (data) {
            toastr.success($rootScope.lbl.blogAddedShort);
            $scope.newItem = {};
            $scope.load();
            spinOff();
            $("#modal-add").modal('hide');
        })
        .error(function () {
            toastr.error($rootScope.lbl.failedAddingNewRole);
            spinOff();
            $("#modal-add").modal('hide');
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
        dataService.processChecked("/api/blogs/processchecked/" + action, $scope.items)
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

    $scope.load();

    $(document).ready(function () {
        $('#form').validate({
            rules: {
                txtBlogName: { required: true },
                txtUserName: { required: true },
                txtEmail: { required: true, email: true },
                txtPassword: { required: true },
                txtConfirmPassword: { required: true, equalTo: '#txtPassword' }
            }
        });
    });
}]);