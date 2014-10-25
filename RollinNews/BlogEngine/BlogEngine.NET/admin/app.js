(function () {
    var app = angular.module("blogAdmin", ['ngRoute', 'ngAnimate', 'ngSanitize']);

    var config = ["$routeProvider", function ($routeProvider) {
        $routeProvider
        .when("/", { templateUrl: "views/dashboard.html" })
        .when("/blogs", { templateUrl: "views/blogs.html" })

        .when("/content", { templateUrl: "views/content/posts.html" })
        .when("/content/comments", { templateUrl: "views/content/comments.html" })
        .when("/content/pages", { templateUrl: "views/content/pages.html" })
        .when("/content/categories", { templateUrl: "views/content/categories.html" })
        .when("/content/tags", { templateUrl: "views/content/tags.html" })
            .when("/content/rss", { templateUrl: "views/content/rss.html" })

            .when("/funds", { templateUrl: "views/funds/withdraw.html" })
            .when("/funds/settings", { templateUrl: "views/funds/settings.html" })
            .when("/funds/admin", { templateUrl: "views/funds/admin.html" })

            .when("/social", { templateUrl: "views/social/index.html" })

            .when("/game", { templateUrl: "views/game/games.html" })

            .when("/custom", { templateUrl: "views/custom/index.html" })
        .when("/custom/themes", { templateUrl: "views/custom/themes.html" })
        .when("/custom/widgets", { templateUrl: "views/custom/widgets.html" })

        .when("/users", { templateUrl: "views/users/index.html" })
        .when("/users/roles", { templateUrl: "views/users/roles.html" })
        .when("/users/profile", { templateUrl: "views/users/profile.html" })

        .when("/settings", { templateUrl: "views/settings/basic.html" })
		.when("/settings/advanced", { templateUrl: "views/settings/advanced.html" })
		.when("/settings/feed", { templateUrl: "views/settings/feed.html" })
		.when("/settings/email", { templateUrl: "views/settings/email.html" })
		.when("/settings/controls", { templateUrl: "views/settings/controls.html" })
		.when("/settings/customecode", { templateUrl: "views/settings/customecode.html" })
		.when("/settings/comments", { templateUrl: "views/settings/comment.html" })
		.when("/settings/pingservices", { templateUrl: "views/settings/pingservices.html" })
		.when("/settings/importexport", { templateUrl: "views/settings/importexport.html" })
        .otherwise({ redirectTo: "/" });
    }];
    app.config(config);

    var run = ["$rootScope", "$log", function ($rootScope, $log) {

        $rootScope.lbl = BlogAdmin.i18n;
        $rootScope.SiteVars = SiteVars;
        $rootScope.testData = true;
        toastr.options.positionClass = 'toast-bottom-right';
        toastr.options.backgroundpositionClass = 'toast-bottom-right';
    }];

    app.run(run);
})();