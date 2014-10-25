(function () {
    var app = angular.module("blogEditor", ['ngRoute', 'ngAnimate', 'ngSanitize']);

    var config = ["$httpProvider", function ($httpProvider) {
        $httpProvider.defaults.headers.post['x-blog-instance'] = SiteVars.BlogInstanceId;
    }];
    app.config(config);

    var run = ["$rootScope", "$log", function ($rootScope, $log) {
        $rootScope.lbl = BlogAdmin.i18n;
        $rootScope.SiteVars = SiteVars;
        toastr.options.positionClass = 'toast-bottom-right';
        toastr.options.backgroundpositionClass = 'toast-bottom-right';
    }];
    app.run(run);

})();