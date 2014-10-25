angular.module('blogAdmin').controller('FundsController', ["$rootScope", "$scope", "$location", "$log", "$http", "dataService", function ($rootScope, $scope, $location, $log, $http, dataService) {
    $scope.funds = {};
    $scope.canSetMonthlyEarnings = canSetMonthlyEarnings();

    $scope.load = function () {
        spinOn();
        $scope.loadSettings();
    }

    $scope.loadSettings = function () {
        dataService.getItems('/api/funds')
        .success(function (data) {
            angular.copy(data, $scope.funds);
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingSettings);
            spinOff();
        });
    }

    $scope.save = function () {
        if (!$('#form').valid()) {
            return false;
        }
        spinOn();
        dataService.updateItem("/api/funds?action=save", $scope.funds)
        .success(function (data) {
            toastr.success($rootScope.lbl.settingsUpdated);
            $scope.load();
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.updateFailed);
            spinOff();
        });
    }
    $scope.submitEarnings = function (btn) {

        if (!$('#form').valid()) {
            return false;
        }
        spinOn();
        $(btn).toggleClass("btn-success", false).prop('disabled', true);
        dataService.updateItem("/api/funds?action=submitEarnings", $scope.funds)
        .success(function (data) {
            toastr.success("Earnings Added");
            $scope.load();
            $(btn).prop('disabled', false);
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.updateFailed);
            spinOff();
        });
    }

    $scope.withdrawBitcoin = function () {
        if (!$('#form').valid()) {
            return false;
        }
        spinOn();
        if ($scope.funds.AmountToWithdraw <= $scope.funds.ActiveInUserAccount) {
                        dataService.updateItem("/api/funds?action=withdrawBc", $scope.funds)
            .success(function (data) {
                toastr.success($rootScope.lbl.settingsUpdated);
                $scope.load();
                spinOff();
            })
            .error(function () {
                toastr.error($rootScope.lbl.updateFailed);
                spinOff();
            });
        } else {
            toastr.error("Amount Is More Than In The Account");
            spinOff();
        }
    }
    $scope.withdrawPaypal = function () {
        if (!$('#form').valid()) {
            return false;
        }
        spinOn();
        if ($scope.funds.AmountToWithdraw <= $scope.funds.ActiveInUserAccount) {
            dataService.updateItem("/api/funds?action=withdrawPP", $scope.funds)
            .success(function (data) {
                toastr.success("Funds Will be Sent Within 24 Hours");
                $scope.load();
                spinOff();
            })
            .error(function () {
                toastr.error($rootScope.lbl.updateFailed);
                spinOff();
            });
        } else {
            toastr.error("Amount Is More Than In The Account");
            spinOff();
        }
    }


    $scope.load();
}]);