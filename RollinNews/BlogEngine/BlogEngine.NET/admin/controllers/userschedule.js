angular.module('blogAdmin').controller('UserScheduleController', ["$rootScope", "$scope", "$filter", "dataService", function ($rootScope, $scope, $filter, dataService) {
    $scope.items = [];
    $scope.rights = [];
    $scope.editItem = {};
    $scope.newItem = false;
    $scope.lookups = [];
    $scope.schedule = UserTask;
    $scope.selectedAuthor = "";
    $scope.TaskFrequency = "";
    $scope.rights = [];

    $scope.load = function () {
        spinOn();
        dataService.getItems('/api/lookups')
  .success(function (data) {
      angular.copy(data, $scope.lookups);
      $scope.loadTags();

  })
  .error(function () {
      toastr.error("Error loading lookups");
  });

        dataService.getItems('/api/schedule', { take: 40, skip: 0 })
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

        spinOn();
        dataService.getItems('/api/schedule/getrights/' + id)
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
        if (!$scope.selectedAuthor) {
            return false;
        }

        if ($scope.schedule) {
            if (!$('#form').valid()) {
                return false;
            }
            $scope.schedule.TaskFrequency = $("#form input[type='radio']:checked").val();
            $scope.schedule.UserName = $scope.selectedAuthor.OptionValue;
            $scope.saveSchedule();
        }
    }

    $scope.saveSchedule = function () {
        spinOn();

        dataService.addItem("/api/schedule", $scope.schedule)
        .success(function (data) {
            toastr.success($rootScope.lbl.roleAdded);
            $scope.load();
            spinOff();
            $("#modal-edit").modal('hide');
        })
        .error(function () {
            toastr.error("Failed Adding Schedule.");
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
        dataService.processChecked("/api/schedule/processchecked/" + action, $scope.items)
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
                txtRoleName: { required: true }
            }
        });
    });
}]);

var UserTask = {
    "UserName": "Admin",
    "DateCreated": moment().format("YYYY-MM-DD HH:MM"),
}