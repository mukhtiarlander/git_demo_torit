angular.module('blogAdmin').controller('RSSFeedController', ["$rootScope", "$scope", "$location", "$http", "$filter", "dataService", function ($rootScope, $scope, $location, $http, $filter, dataService) {
    $scope.items = [];
    $scope.lookups = [];
    $scope.rss = {};
    $scope.id = ($location.search()).id;
    $scope.EditPostCategories = EditPostCategories();
    $scope.lookups = [];
    $scope.allTags = [];

    $scope.images = [];
    $scope.SearchImages = [];

    $scope.uploadFile = function (action, files) {
        var fd = new FormData();
        fd.append("file", files[0]);

        dataService.uploadFile("/api/upload?action=" + action, fd)
        .success(function (data) {
            toastr.success($rootScope.lbl.uploaded);
            if (action === "main") {
                $scope.rss.MainImageUrl = SiteVars.AbsoluteWebRoot.substring(0, SiteVars.AbsoluteWebRoot.length - 1) + data.replace(/\"/g, '');
                $("#MainImageUrl").val(SiteVars.AbsoluteWebRoot.substring(0, SiteVars.AbsoluteWebRoot.length - 1) + data.replace(/\"/g, ''));
            }
            if (action === "initial") {
                $scope.rss.InitialImageUrl = SiteVars.AbsoluteWebRoot.substring(0, SiteVars.AbsoluteWebRoot.length - 1) + data.replace(/\"/g, '');
                $("#InitialImageUrl").val(SiteVars.AbsoluteWebRoot.substring(0, SiteVars.AbsoluteWebRoot.length - 1) + data.replace(/\"/g, ''));
            }
        })
        .error(function () { toastr.error($rootScope.lbl.importFailed); });
    }

    $scope.setUrlImageFromClick = function (img) {
        var src = $(img).attr("src");
        $scope.rss.MainImageUrl = SiteVars.AbsoluteWebRoot.substring(0, SiteVars.AbsoluteWebRoot.length - 1) + src;
        $("#MainImageUrl").val(SiteVars.AbsoluteWebRoot.substring(0, SiteVars.AbsoluteWebRoot.length - 1) + src);
    }
    $scope.setUrlImageFromClick1 = function (img) {
        var src = $(img).attr("src");
        $scope.rss.InitialImageUrl = SiteVars.AbsoluteWebRoot.substring(0, SiteVars.AbsoluteWebRoot.length - 1) + src;
        $("#InitialImageUrl").val(SiteVars.AbsoluteWebRoot.substring(0, SiteVars.AbsoluteWebRoot.length - 1) + src);
    }

    $scope.loadImages = function () {
        var p = { search: "" };
        dataService.getItems('/api/files', p)
.success(function (data) {
    angular.copy(data, $scope.images);
});
    }
    $scope.searchImageChanged = function (input) {
        var p = { search: $(input).val() };
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


    $scope.editRss = function (id) {
        $scope.id = id;
        $scope.loadFeed();
    }

    $scope.loadFeed = function () {

        dataService.getItems('/api/rssfeed', { Id: $scope.id })
            .success(function (data) {
                angular.copy(data, $scope.rss);
                if ($scope.rss.Categories != null) {
                    for (var i = 0; i < $scope.rss.Categories.length; i++) {
                        for (var j = 0; j < $scope.lookups.CategoryList.length; j++) {
                            if ($scope.rss.Categories[i].CategoryRNId === $scope.lookups.CategoryList[j].OptionValue) {
                                $scope.lookups.CategoryList[j].IsSelected = true;
                            }
                        }
                    }
                }
                $scope.selectedAuthor = selectedOption($scope.lookups.AuthorList, $scope.rss.AuthorUserName);
                var existingTags = [];
                if ($scope.rss.Tags != null) {
                    for (var i = 0; i < $scope.rss.Tags.length; i++) {
                        existingTags[i] = ($scope.rss.Tags[i].TagName);
                    }
                }

                load_tags(existingTags, $scope.allTags);

            })
            .error(function () { toastr.error($rootScope.lbl.errorLoadingCategory); });
        $("#modal-add-cat").modal();

    }

    $scope.addNew = function () {
        $scope.clear();
        $("#modal-add-cat").modal();
    }

    $scope.loadTags = function () {
        var tagsUrl = '/api/tags';
        var p = { take: 0, skip: 0 };
        dataService.getItems(tagsUrl, p)
        .success(function (data) {
            $scope.allTags = [];
            for (var i = 0; i < data.length; i++) {
                $scope.allTags[i] = (data[i].TagName);
            }
            if ($scope.id) {
                $scope.loadFeed();
            }
            else {
                load_tags([], $scope.allTags);

            }
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingTags);
        });
    }


    function load_tags(postTags, allTags) {
        $('#postTags')
        .textext({
            plugins: 'tags autocomplete',
            tagsItems: postTags
        })
        .bind('getSuggestions', function (e, data) {
            var list = allTags,
                textext = $(e.target).textext()[0],
                query = (data ? data.query : '') || '';

            $(this).trigger(
                'setSuggestions',
                { result: textext.itemManager().filter(list, query) }
            );
        });
    }

    function get_tags() {
        var tags = [];
        var tagList = [];
        $('.post-tags-selector .text-tags .text-label').each(function () { tags.push($(this).text()) });
        for (var i = 0; i < tags.length; i++) {
            tagList[i] = { TagName: tags[i] };
        }
        return tagList;
    }

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
        dataService.getItems('/api/rssfeed')
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
        if ($scope.selectedAuthor != undefined)
            $scope.rss.AuthorUserName = $scope.selectedAuthor.OptionValue;
        if (!$('#form').valid()) {
            return false;
        }

        $scope.rss.Categories = [];
        if ($scope.lookups.CategoryList != null) {
            for (var i = 0; i < $scope.lookups.CategoryList.length; i++) {
                var cat = $scope.lookups.CategoryList[i];
                if (cat.IsSelected) {
                    var catAdd = { "CategoryRNId": cat.OptionValue, "Title": cat.OptionName };
                    $scope.rss.Categories.push(catAdd);
                }
            }
        }
        $scope.rss.Tags = get_tags();

        if ($scope.rss.FeedId) {
            dataService.updateItem("/api/rssfeed/update/" + $scope.rss.FeedId, $scope.rss)
		   .success(function (data) {
		       toastr.success("Updated");
		       $scope.load();
		   })
		   .error(function () { toastr.error($rootScope.lbl.updateFailed); });
        }
        else {

            dataService.addItem("/api/rssfeed", $scope.rss)
		   .success(function (data) {
		       toastr.success("Rss Item Added");
		       if (data.Id) {
		           angular.copy(data, $scope.rss);
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
        dataService.processChecked("/api/rssfeed/processchecked/" + action, $scope.items)
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
        $scope.category = { "IsChecked": false, "FeedId": null, "Url": null, "LastChecked": "", "Created": "", "TotalPostsPulled": 0 };
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