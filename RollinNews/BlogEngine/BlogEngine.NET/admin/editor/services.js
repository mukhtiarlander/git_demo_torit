angular.module('blogEditor').factory("dataService", ["$http", "$q", function ($http, $q) {
    return {
        getItems: function (url, p) {
            return $http.get(webRoot(url), {
                // query string like { userId: user.id } -> ?userId=value
                params: p,
                // for sub-blogs with URL rewrite
                headers: { 'x-blog-instance': SiteVars.BlogInstanceId }
            });
        },
        addItem: function (url, item) {
            return $http({
                url: webRoot(url),
                method: 'POST',
                data: item,
                headers: { 'x-blog-instance': SiteVars.BlogInstanceId }
            });
        },
        deleteItem: function (url, item) {
            return $http({
                url: webRoot(url) + "/" + item.Id,
                method: 'DELETE',
                headers: { 'x-blog-instance': SiteVars.BlogInstanceId }
            });
        },
        updateItem: function (url, item) {
            return $http({
                url: webRoot(url),
                method: 'PUT',
                data: item,
                headers: { 'x-blog-instance': SiteVars.BlogInstanceId }
            });
        },
        uploadFile: function (url, file) {
            return $http({
                url: webRoot(url),
                method: 'POST',
                data: file,
                withCredentials: true,
                headers: { 'Content-Type': undefined, 'x-blog-instance': SiteVars.BlogInstanceId },
                transformRequest: angular.identity
            });
        }
    };
}]);