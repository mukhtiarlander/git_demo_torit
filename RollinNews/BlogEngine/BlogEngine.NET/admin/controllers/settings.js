angular.module('blogAdmin').controller('SettingsController', ["$rootScope", "$scope", "$location", "$log", "$http", "dataService", function ($rootScope, $scope, $location, $log, $http, dataService) {
    $scope.settings = {};
    $scope.lookups = {};
    $scope.feedOptions = [
        { "OptionName": "RSS 2.0", "OptionValue": "Rss", "IsSelected": false },
        { "OptionName": "Atom 1.0", "OptionValue": "Atom", "IsSelected": false }
    ];
    $scope.closeDaysOptions = [
        { "OptionName": "Never", "OptionValue": "0", "IsSelected": false },
        { "OptionName": "1", "OptionValue": "1", "IsSelected": false },
        { "OptionName": "2", "OptionValue": "2", "IsSelected": false },
        { "OptionName": "3", "OptionValue": "3", "IsSelected": false },
        { "OptionName": "7", "OptionValue": "7", "IsSelected": false },
        { "OptionName": "10", "OptionValue": "10", "IsSelected": false },
        { "OptionName": "14", "OptionValue": "14", "IsSelected": false },
        { "OptionName": "21", "OptionValue": "21", "IsSelected": false },
        { "OptionName": "30", "OptionValue": "30", "IsSelected": false },
        { "OptionName": "60", "OptionValue": "60", "IsSelected": false },
        { "OptionName": "90", "OptionValue": "90", "IsSelected": false },
        { "OptionName": "180", "OptionValue": "180", "IsSelected": false },
        { "OptionName": "365", "OptionValue": "365", "IsSelected": false }
    ];
    $scope.commentsPerPageOptions = [
        { "OptionName": "5", "OptionValue": "5", "IsSelected": false },
        { "OptionName": "10", "OptionValue": "10", "IsSelected": false },
        { "OptionName": "15", "OptionValue": "15", "IsSelected": false },
        { "OptionName": "20", "OptionValue": "20", "IsSelected": false },
        { "OptionName": "50", "OptionValue": "50", "IsSelected": false }
    ];

    $scope.load = function () {
        spinOn();
        dataService.getItems('/api/lookups')
        .success(function (data) {
            angular.copy(data, $scope.lookups);
            $scope.loadSettings();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingSettings);
            spinOff();
        });
    }

    $scope.loadSettings = function () {
        dataService.getItems('/api/settings')
        .success(function (data) {
            angular.copy(data, $scope.settings);
            $scope.selectedLanguage = selectedOption($scope.lookups.Cultures, $scope.settings.Culture);
            $scope.selectedDeskTheme = selectedOption($scope.lookups.InstalledThemes, $scope.settings.DesktopTheme);
            $scope.selectedMobileTheme = selectedOption($scope.lookups.InstalledThemes, $scope.settings.MobileTheme);
            $scope.selfRegistrationInitialRole = selectedOption($scope.lookups.SelfRegisterRoles, $scope.settings.SelfRegistrationInitialRole);
            $scope.selFeedFormat = selectedOption($scope.feedOptions, $scope.settings.SyndicationFormat);
            $scope.selCloseDays = selectedOption($scope.closeDaysOptions, $scope.settings.DaysCommentsAreEnabled);
            $scope.selCommentsPerPage = selectedOption($scope.commentsPerPageOptions, $scope.settings.CommentsPerPage);
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
        $scope.settings.DesktopTheme = $scope.selectedDeskTheme.OptionValue;
        $scope.settings.MobileTheme = $scope.selectedMobileTheme.OptionValue;
        $scope.settings.Culture = $scope.selectedLanguage.OptionValue;
        if ($scope.selfRegistrationInitialRole) {
            $scope.settings.SelfRegistrationInitialRole = $scope.selfRegistrationInitialRole.OptionValue;
        }
        $scope.settings.SyndicationFormat = $scope.selFeedFormat.OptionValue;
        $scope.settings.DaysCommentsAreEnabled = $scope.selCloseDays.OptionValue;
        $scope.settings.CommentsPerPage = $scope.selCommentsPerPage.OptionValue;
        
        dataService.updateItem("/api/settings", $scope.settings)
        .success(function (data) {
            toastr.success($rootScope.lbl.settingsUpdated);
            $scope.load();
        })
        .error(function () { toastr.error($rootScope.lbl.updateFailed); });
    }

    $scope.exportToXml = function() {
        location.href = SiteVars.ApplicationRelativeWebRoot + 'blogml.axd';
    }

    $scope.importClickOnce = function () {
        var url = 'http://dotnetblogengine.net/clickonce/blogimporter/blog.importer.application?url=';
        url += SiteVars.AbsoluteWebRoot + '&username=' + SiteVars.UserName;
        location.href = url;
    }

    $(document).ready(function () {
        $('#form').validate({
            rules: {
                txtName: { required: true },
                txtTimeOffset: { required: true, number: true },
                txtPostsPerPage: { required: true, number: true },
                txtDescriptionCharacters: { required: true, number: true },
                txtDescriptionCharactersForPosts: { required: true, number: true },
                txtRemoteFileDownloadTimeout: { required: true, number: true },
                txtRemoteMaxFileSize: { required: true, number: true },
                txtFeedAuthor: { email: true },
                txtEndorsement: { url: true },
                txtAlternateFeedUrl: { url: true },
                txtpostsPerFeed: { number: true },
                txtEmail: { email: true },
                txtSmtpServerPort: { number: true },
                txtNumberOfRecentPosts: { number: true },
                txtNumberOfRecentComments: { number: true }
            }
        });
    });

    $scope.uploadFile = function(files) {
        var fd = new FormData();
        fd.append("file", files[0]);

        dataService.uploadFile("/api/upload?action=import", fd)
        .success(function (data) {
            toastr.success($rootScope.lbl.importedFromBlogML);
        })
        .error(function () { toastr.error($rootScope.lbl.importFailed); });
    }

    $scope.testEmail = function () {
        dataService.updateItem("/api/settings?action=testEmail", $scope.settings)
        .success(function (data) {
            if (data) {
                toastr.error(data);
            }
            else {
                toastr.success($rootScope.lbl.completed);
            }
        })
        .error(function () { toastr.error($rootScope.lbl.failed); });
    }

    $scope.load();
}]);