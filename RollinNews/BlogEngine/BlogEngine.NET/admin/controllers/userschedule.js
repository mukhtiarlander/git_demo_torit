angular.module('blogAdmin').controller('UserScheduleController', ["$rootScope", "$scope", "$filter", "dataService", function ($rootScope, $scope, $filter, dataService) {
    $scope.items = [];
    $scope.rights = [];
    $scope.editItem = {};
    $scope.newItem = false;

    $scope.load = function () {
        spinOn();
        dataService.getItems('/api/schedule', { take: 0, skip: 0 })
            .success(function (data) {
                angular.copy(data, $scope.items);
                gridInit($scope, $filter);
                spinOff();
            })
            .error(function () {
                toastr.error($rootScope.lbl.errorLoadingRoles);
                spinOff();
            });
    }

    $scope.loadRightsForm = function (id) {
        if (!id) {
            id = "Anonymous";
            $scope.editItem = {};
            $scope.newItem = true;
        }
        else {
            $scope.newItem = false;
            $scope.loadCurrentRole(id);
        }
        spinOn();
        dataService.getItems('/api/roles/getrights/' + id)
        .success(function (data) {
            angular.copy(data, $scope.rights);
            $("#modal-edit").modal();
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingRights);
            spinOff();
        });
    }

    $scope.loadCurrentRole = function (id) {
        spinOn();
        dataService.getItems('/api/roles/get/' + id)
        .success(function (data) {
            angular.copy(data, $scope.editItem);
            $("#modal-edit").modal();
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingRole);
            spinOff();
        });
    }

    $scope.save = function () {
        if ($scope.newItem) {
            if (!$('#form').valid()) {
                return false;
            }
            $scope.saveRole();
            $scope.saveRights();
        }
        else {
            $scope.saveRights();
        }
    }

    $scope.saveRole = function () {
        spinOn();
        dataService.addItem("/api/roles", $scope.editItem)
        .success(function (data) {
            toastr.success($rootScope.lbl.roleAdded);
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

    $scope.saveRights = function () {
        spinOn();
        dataService.updateItem("/api/roles/saverights/" + $scope.editItem.RoleName, $scope.rights)
        .success(function (data) {
            toastr.success($rootScope.lbl.rightsSaved);
            $scope.load();
            spinOff();
            $("#modal-edit").modal('hide');
        })
        .error(function () {
            toastr.error($rootScope.lbl.failedToSaveRights);
            spinOff();
            $("#modal-edit").modal('hide');
        });
    }

    $scope.processChecked = function (action) {
        spinOn();
        dataService.processChecked("/api/roles/processchecked/" + action, $scope.items)
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

    $scope.delete = function (id) {
        spinOn();
        dataService.deleteById("/api/roles", id)
        .success(function (data) {
            toastr.success($rootScope.lbl.rolesItem + id + $rootScope.lbl.rolesItemDeleted);
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.couldNotDeleteItem + id);
            spinOff();
        });
    }

    $scope.load();

    $(document).ready(function () {
        $('#form').validate({
            rules: {
                txtRoleName: { required: true }
            }
        });
    });
}]);