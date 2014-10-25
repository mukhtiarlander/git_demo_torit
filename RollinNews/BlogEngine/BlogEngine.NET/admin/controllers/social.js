angular.module('blogAdmin').controller('SocialController', ["$rootScope", "$scope", "$location", "$log", "$http", "dataService", function ($rootScope, $scope, $location, $log, $http, dataService) {
    $scope.social = newSocial;
    $scope.images = [];
    //$scope.social.IsLoggedIntoFacebook = false;
    $scope.canSetMonthlyEarnings = canSetMonthlyEarnings();

    $scope.setUrlImageFromClick = function (img) {
        var src = $(img).attr("src");
        $scope.social.PictureUrl = SiteVars.AbsoluteWebRoot.substring(0, SiteVars.AbsoluteWebRoot.length - 1) + src;
        $("#pictureUrl1").val(SiteVars.AbsoluteWebRoot.substring(0, SiteVars.AbsoluteWebRoot.length - 1) + src);
        $("#pictureUrl").val(SiteVars.AbsoluteWebRoot.substring(0, SiteVars.AbsoluteWebRoot.length - 1) + src);
    }

    $scope.loadImages = function () {
        var p = { search: "" };
        dataService.getItems('/api/files', p)
.success(function (data) {
    angular.copy(data, $scope.images);
});
    }
    $scope.searchImageChanged = function () {
        var p = { search: $scope.social.SearchImages };
        dataService.getItems('/api/files', p)
.success(function (data) {
    angular.copy(data, $scope.images);
});

    }
    $scope.openImageGalleryForInitial = function (span) {
        if ($scope.images.length === 0)
            $scope.loadImages();
        var imageHolder = $("#galleryForInitialImage");
        imageHolder.toggleClass("displayNone");
        $("#postImageInitialSearch").toggleClass("displayNone");
        $("#postImageInitialSearch").focus();
    }
    $scope.openImageGalleryForInitial1 = function (span) {
        if ($scope.images.length === 0)
            $scope.loadImages();
        var imageHolder = $("#galleryForInitialImage1");
        imageHolder.toggleClass("displayNone");
        $("#postImageInitialSearch1").toggleClass("displayNone");
        $("#postImageInitialSearch1").focus();
    }

    $scope.load = function () {
        window.fbAsyncInit = function () {
            FB.init({
                appId: '648592428540709', // App ID
                status: true, // check login status
                cookie: true, // enable cookies to allow the server to access the session
                xfbml: true,  // parse XFBML
                scope: 'manage_pages'
            });

            FB.Event.subscribe('auth.authResponseChange', function (response) {
                if (response.status === 'connected') {
                    // the user is logged in and has authenticated your
                    // app, and response.authResponse supplies
                    // the user's ID, a valid access token, a signed
                    // request, and the time the access token 
                    // and signed request each expire
                    var uid = response.authResponse.userID;
                    $scope.social.UserAccessToken = response.authResponse.accessToken;
                    $scope.social.IsLoggedIntoFacebook = true;
                    $scope.$apply();
                    // TODO: Handle the access token
                    $scope.updateLongTermAccessToken();
                } else if (response.status === 'not_authorized') {
                    // the user is logged in to Facebook, 
                    // but has not authenticated your app
                } else {
                    // the user isn't logged in to Facebook.
                }
            });
        };

        // Load the SDK Asynchronously
        (function (d) {
            var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
            if (d.getElementById(id)) { return; }
            js = d.createElement('script'); js.id = id; js.async = true;
            js.src = "//connect.facebook.net/en_US/all.js";
            ref.parentNode.insertBefore(js, ref);
        }(document));
    }

    $scope.loadSettings = function () {

    }

    $scope.updateLongTermAccessToken= function () {
        
        dataService.updateItem("/api/social?action=updateLongTermToken", $scope.social)
        .success(function (data) {
            toastr.success("Facebook Access Updated");
            
        })
        .error(function () { toastr.error($rootScope.lbl.updateFailed); });

    }

    $scope.postToFacebook = function () {
        if (!$('#form').valid()) {
            return false;
        }
        dataService.updateItem("/api/social?action=postToFacebook", $scope.social)
        .success(function (data) {
            toastr.success("Posted");
            $scope.load();
        })
        .error(function () { toastr.error($rootScope.lbl.updateFailed); });

    }
    $scope.saveAndPostToFacebook = function () {
        if (!$('#form').valid()) {
            return false;
        }
        dataService.updateItem("/api/social?action=saveAndPostToFacebook", $scope.social)
            .success(function (data) {
                toastr.success("Saved and Posted");
                $scope.load();
            })
            .error(function () { toastr.error($rootScope.lbl.updateFailed); });

    }
    $scope.postScoreToFacebook = function () {
        if (!$('#form').valid()) {
            return false;
        }
        dataService.updateItem("/api/social?action=postScoreToFacebook", $scope.social)
        .success(function (data) {
            toastr.success("Score Posted");
            $scope.load();
        })
        .error(function () { toastr.error($rootScope.lbl.updateFailed); });

    }


    $scope.load();
}]);

var newSocial = {
    "Message": "",
    "Link": "",
    "PictureUrl": "",
    "Name": "",
    "Caption": "",
    "Team1Name": "",
    "Team1Score": "",
    "Team2Name": "",
    "Team2Score": "",
    "DateOfGame": moment().format("YYYY-MM-DD HH:MM"),
    "DateForMessage": moment().format("YYYY-MM-DD HH:MM"),
    "UserAccessToken": "",
    "IsLoggedIntoFacebook": false,
    "SearchImages": ""
}