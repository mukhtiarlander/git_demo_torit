angular.module('blogEditor').controller('PostEditorController', ["$rootScope", "$scope", "$location", "$filter", "$log", "dataService", function ($rootScope, $scope, $location, $filter, $log, dataService) {
    $scope.id = editVars.id;
    $scope.post = newPost;
    $scope.post.CanPublish = canPublishPosts();
    $scope.lookups = [];
    $scope.allTags = [];
    $scope.searchResults = [];
    $scope.selectedAuthor = {};
    $scope.images = [];
    $scope.typeHere = BlogAdmin.i18n.typeHere;
    $scope.search = editVars.id;
    $scope.searchImages = editVars.id;
    $scope.editorSelection;


    $scope.SetupEditor = function () {
        var editable = document.getElementById('editor'),
selection, range;

        var captureSelection = function (e) {
            // Don't capture selection outside editable region
            var isOrContainsAnchor = false,
                isOrContainsFocus = false,
                sel = window.getSelection(),
                parentAnchor = sel.anchorNode,
                parentFocus = sel.focusNode;

            while (parentAnchor && parentAnchor != document.documentElement) {
                if (parentAnchor == editable) {
                    isOrContainsAnchor = true;
                }
                parentAnchor = parentAnchor.parentNode;
            }

            while (parentFocus && parentFocus != document.documentElement) {
                if (parentFocus == editable) {
                    isOrContainsFocus = true;
                }
                parentFocus = parentFocus.parentNode;
            }

            if (!isOrContainsAnchor || !isOrContainsFocus) {
                return;
            }

            selection = window.getSelection();

            // Get range (standards)
            if (selection.getRangeAt !== undefined) {
                range = selection.getRangeAt(0);

                // Get range (Safari 2)
            } else if (
                document.createRange &&
                selection.anchorNode &&
                selection.anchorOffset &&
                selection.focusNode &&
                selection.focusOffset
            ) {
                range = document.createRange();
                range.setStart(selection.anchorNode, selection.anchorOffset);
                range.setEnd(selection.focusNode, selection.focusOffset);
            } else {
                // Failure here, not handled by the rest of the script.
                // Probably IE or some older browser
            }
            $scope.editorSelection = range;
        };

        editable.onkeyup = captureSelection;


    }

    $scope.load = function () {
        dataService.getItems('/api/lookups')
        .success(function (data) {
            angular.copy(data, $scope.lookups);
            $scope.loadTags();

        })
        .error(function () {
            toastr.error("Error loading lookups");
        });
        $("#searchMemberLeagueBox").hide();
        $scope.SetupEditor();
    }

    $scope.searchImageChanged = function () {
        var p = { search: $scope.post.SearchImages };
        dataService.getItems('/api/files', p)
.success(function (data) {
    angular.copy(data, $scope.images);
});

    }

    $scope.searchChanged = function () {
        var searchUrl = '/api/search';
        var p = { search: $scope.post.Search };
        dataService.getItems(searchUrl, p)
        .success(function (data) {
            $scope.searchResults = [];
            angular.copy(data, $scope.searchResults);
        })
        .error(function () {
            toastr.error("Error Searching");
        });
    }

    $scope.selectSearchedItem = function (itemClick) {
        var box = $("#searchMemberLeagueBox");
        box.html("");
        box.show();
        var item = $scope.searchResults[itemClick];
        var content = "<div class='searchItem'><span class='searchItemClose' onclick='closeSearchItem()'>X</span>";
        if (item.PhotoUrl != null)
            content += "<div><img src='" + item.PhotoUrl + "'/></div>";
        content += "<div><a href='" + item.UrlOfItem + "' target='_blank'>" + item.Title + "</a></div>";

        if (item.ItemType === "League") {
            if (item.Properties.Founded != null)
                content += "<div>Founded:" + item.Properties.Founded + "</div>";
            if (item.Properties.City != null)
                content += "<div>City:" + item.Properties.City + "</div>";
            if (item.Properties.State != null)
                content += "<div>State:" + item.Properties.State + "</div>";
            if (item.Properties.Country != null)
                content += "<div>Country:" + item.Properties.Country + "</div>";
            if (item.Properties.MemberCount != null)
                content += "<div>Members:" + item.Properties.MemberCount + "</div>";
            if (item.Properties.Latitude != null)
                content += "<div>Latitude:" + item.Properties.Latitude + "</div>";
            if (item.Properties.Longitude != null)
                content += "<div>Longitude:" + item.Properties.Longitude + "</div>";
            if (item.Properties.Twitter != null)
                content += "<div>Twitter:<a target='_blank' href='" + item.Properties.Twitter + "'>" + item.Properties.Twitter + "</a></div>";
            if (item.Properties.WebSite != null)
                content += "<div>WebSite:<a target='_blank' href='" + item.Properties.WebSite + "'>" + item.Properties.WebSite + "</a></div>";
            if (item.Properties.Instagram != null)
                content += "<div>Instagram:<a target='_blank' href='" + item.Properties.Instagram + "'>" + item.Properties.Instagram + "</a></div>";
            if (item.Properties.Facebook != null)
                content += "<div>Facebook:<a target='_blank' href='" + item.Properties.Facebook + "'>" + item.Properties.Facebook + "</a></div>";
            if (item.Properties.RuleSetsPlayed != null)
                content += "<div>RuleSetsPlayed:" + item.Properties.RuleSetsPlayed + "</div>";

        } else if (item.ItemType === "Member") {
            if (item.Properties.DerbyNumber != null)
                content += "<div>DerbyNumber:" + item.Properties.DerbyNumber + "</div>";

            if (item.Properties.Age != null)
                content += "<div>Age:" + item.Properties.Age + " - " + item.Properties.DOB + "</div>";

            if (item.Properties.Gender != null)
                content += "<div>Gender: " + item.Properties.Gender + "</div>";
            if (item.Properties.HeightFeet != null)
                content += "<div>Height: " + item.Properties.HeightFeet + " " + item.Properties.HeightInches + "</div>";
            if (item.Properties.LeagueName != null)
                content += "<div>League: <a href='" + item.Properties.LeagueUrl + "' target='_blank'>" + item.Properties.LeagueName + "</a></div>";
            if (item.Properties.GamesPlayed != null)
                content += "<div>Games:" + item.Properties.Wins + " / " + item.Properties.Losses + "</div>";
            if (item.Properties.Bio != null)
                content += "<div>Bio:" + item.Properties.Bio + "</div>";
        }

        content += "</div>";
        box.append(content);
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
                $scope.loadPost();
            }
            else {
                load_tags([], $scope.allTags);
                $scope.selectedAuthor = selectedOption($scope.lookups.AuthorList, SiteVars.UserName);
            }
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingTags);
        });
    }

    $scope.loadPost = function () {
        spinOn();
        var url = '/api/posts/' + $scope.id;
        dataService.getItems(url)
        .success(function (data) {
            angular.copy(data, $scope.post);
            $scope.post.CanPublish = canPublishPosts();
            // check post categories in the list
            if ($scope.post.Categories != null) {
                for (var i = 0; i < $scope.post.Categories.length; i++) {
                    for (var j = 0; j < $scope.lookups.CategoryList.length; j++) {
                        if ($scope.post.Categories[i].Id === $scope.lookups.CategoryList[j].OptionValue) {
                            $scope.lookups.CategoryList[j].IsSelected = true;
                        }
                    }
                }
            }
            var existingTags = [];
            if ($scope.post.Tags != null) {
                for (var i = 0; i < $scope.post.Tags.length; i++) {
                    existingTags[i] = ($scope.post.Tags[i].TagName);
                }
            }
            $scope.selectedAuthor = selectedOption($scope.lookups.AuthorList, $scope.post.Author);
            load_tags(existingTags, $scope.allTags);
            $("#editor").html($scope.post.Content);
            $("#initialImageUrl").html($scope.post.InitialImageUrl);
            $("#initialImage").html('<img width="50px" src=' + $scope.post.InitialImageUrl + ' />');
            $("#mainImageUrl").html($scope.post.MainImageUrl);
            $("#mainImage").html('<img width="50px" src=' + $scope.post.MainImageUrl + ' />');
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPosts);
            spinOff();
        });
    }

    $scope.save = function (action) {
        if (!$('#form').valid()) {
            return false;
        }

        $scope.post.InitialImageUrl = $("#initialImageUrl").html();
        $scope.post.MainImageUrl = $("#mainImageUrl").html();
        //if ($scope.post.InitialImageUrl.length < 1) {
        //    $("#initialImageUploadContainer").toggleClass("error", true);

        //    return false;
        //}
        //if ($scope.post.MainImageUrl.length < 1) {
        //    $("#mainImageUploadContainer").toggleClass("error", true);
        //    return false;
        //}

        spinOn();
        $scope.post.Content = $("#editor").html();
        $scope.post.Author = $scope.selectedAuthor.OptionValue;
        if ($scope.post.Slug.length == 0) {
            $scope.post.Slug = toSlug($scope.post.Title);
        }
        // get selected categories
        $scope.post.Categories = [];
        if ($scope.lookups.CategoryList != null) {
            for (var i = 0; i < $scope.lookups.CategoryList.length; i++) {
                var cat = $scope.lookups.CategoryList[i];
                if (cat.IsSelected) {
                    var catAdd = { "IsChecked": false, "Id": cat.OptionValue, "Title": cat.OptionName };
                    $scope.post.Categories.push(catAdd);
                }
            }
        }
        $scope.post.Tags = get_tags();

        if ($scope.post.Id && action === "publish") {
            dataService.updateItem('api/posts/update/foo?action=' + action, $scope.post)
           .success(function (data) {
               toastr.success($rootScope.lbl.postUpdated);
               $("#modal-form").modal('hide');
               spinOff();
           })
           .error(function () { toastr.error($rootScope.lbl.updateFailed); spinOff(); });
        }
        else if ($scope.post.Id) {
            dataService.updateItem('api/posts/update/foo?action=' + action, $scope.post)
           .success(function (data) {
               toastr.success($rootScope.lbl.postUpdated);
               $("#modal-form").modal('hide');
               spinOff();
           })
           .error(function () { toastr.error($rootScope.lbl.updateFailed); spinOff(); });
        }
        else {
            dataService.addItem('api/posts?action=' + action, $scope.post)
           .success(function (data) {
               toastr.success($rootScope.lbl.postAdded);
               if (data.Id) {
                   angular.copy(data, $scope.post);
                   var x = $scope.post.Id;
                   $scope.post.CanPublish = canPublishPosts();
               }
               $("#modal-form").modal('hide');
               spinOff();
           })
           .error(function () { toastr.error($rootScope.lbl.failedAddingNewPost); spinOff(); });
        }
    }

    $scope.saveSource = function () {
        $scope.post.Content = $("#editor-source").val();
        $("#editor").html($("#editor-source").val());
        $("#modal-source").modal('hide');
    }
    $scope.submitForApproval = function (doPublish) {
        $scope.save("submitForApproval");
    }

    $scope.publish = function (doPublish) {
        $scope.save("publishSelf");
    }
    $scope.unpublish = function (doPublish) {
        $scope.save("unpublishSelf");
    }

    $scope.openImageGalleryForInitial = function (span) {
        var imageHolder = $("#galleryForInitialImage");
        imageHolder.toggleClass("displayNone");

        if ($scope.images.length === 0)
            $scope.loadImages();
        $("#postImageInitialSearch").toggleClass("displayNone");
        $("#postImageInitialSearch").focus();

    }

    $scope.setInitialImageFromClick = function (img) {
        var src = $(img).attr("src");
        $("#initialImage").html('<img width="50px" src=' + src + ' />');
        $("#initialImageUrl").html(src);

    }
    $scope.setMainImageFromClick = function (img) {
        var src = $(img).attr("src");
        $("#mainImage").html('<img width="50px" src=' + src + ' />');
        $("#mainImageUrl").html(src);
    }

    function insertHtmlAfterSelection(html) {
        var range, expandedSelRange, node;
        if ($scope.editorSelection) {
            range = $scope.editorSelection;
            expandedSelRange = range.cloneRange();
            range.collapse(false);
            var el = document.createElement("div");
            el.innerHTML = html;
            var frag = document.createDocumentFragment(), node, lastNode;
            while ((node = el.firstChild)) {
                lastNode = frag.appendChild(node);
            }
            range.insertNode(frag);
        }
    }

    $scope.setEditorImageFromClick = function (img) {
        var src = $(img).attr("src");
        insertHtmlAfterSelection('<img src=' + src + ' />');
    }

    $scope.loadImages = function () {
        var p = { search: "" };
        dataService.getItems('/api/files', p)
.success(function (data) {
    angular.copy(data, $scope.images);
});
    }
    $scope.openImageGalleryForMain = function (span) {
        if ($scope.images.length === 0)
            $scope.loadImages();
        var imageHolder = $("#galleryForMainImage");
        imageHolder.toggleClass("displayNone");
        $("#postImageMainSearch").toggleClass("displayNone");
        $("#postImageMainSearch").focus();
    }
    $scope.openImageGalleryForEditor = function (btn) {
        if ($scope.images.length === 0)
            $scope.loadImages();
        var imageHolder = $("#galleryForEditorImage");
        imageHolder.toggleClass("displayNone");
        $("#postImageEditorSearch").toggleClass("displayNone");
        $("#postImageEditorSearch").focus();
    }

    $scope.uploadFile = function (action, files) {
        var fd = new FormData();
        fd.append("file", files[0]);

        dataService.uploadFile("/api/upload?action=" + action, fd)
        .success(function (data) {
            toastr.success($rootScope.lbl.uploaded);
            var editorHtml = $("#editor").html();
            if (action === "main") {
                $("#mainImage").html('<img width="50px" src=' + data + ' />');
                $("#mainImageUrl").html(data);
            }
            if (action === "initial") {
                $("#initialImage").html('<img width="50px" src=' + data + ' />');
                $("#initialImageUrl").html(data);
            }
            if (action === "image") {

                insertHtmlAfterSelection('<img src=' + data + ' />');
            }
            if (action === "video") {

                insertHtmlAfterSelection('<p>[video src=' + data + ']</p>');
            }
            if (action === "file") {
                var res = data.split("|");
                if (res.length === 2) {
                    $("#editor").html(editorHtml + '<a href="' + res[0].replace('"', '') + '">' + res[1].replace('"', '') + '</a>');
                }
            }
        })
        .error(function () { toastr.error($rootScope.lbl.importFailed); });
    }

    $scope.toggleEditor = function (e) {
        if ($scope.fullScreen) {
            $scope.compress();
            $scope.fullScreen = false;
        }
        else {
            $scope.expand();
            $scope.fullScreen = true;
        }
    }

    $scope.expand = function () {
        $('#overlay-editor').addClass('overlay-editor');
        $('#editor').addClass('full-editor');
    }

    $scope.compress = function () {
        $('#overlay-editor').removeClass('overlay-editor');
        $('#editor').removeClass('full-editor');
    }

    $scope.source = function () {
        $("#modal-source").modal();
        var html = $('#editor').html();
        $("#editor-source").val($("#editor").html());
    }

    $scope.status = function () {
        // 0 - unpublished; 1 - saved; 2 - published;

        if ($scope.post && $scope.post.Id && $scope.post.IsPublished) {
            return 2;
        }
        if ($scope.post && $scope.post.Id && !$scope.post.IsPublished) {
            return 1;
        }
        return 0;
    };

    $scope.load();

    $(document).ready(function () {
        $('#form').validate({
            rules: {
                txtTitle: { required: true }
            }
        });
    });
}]);

var newPost = {
    "Id": "",
    "Title": "",
    "Author": "Admin",
    "Content": "",
    "DateCreated": moment().format("YYYY-MM-DD HH:MM"),
    "Slug": "",
    "Categories": "",
    "Tags": "",
    "Comments": "",
    "HasCommentsEnabled": true,
    "IsPublished": false,
    "InitialImageUrl": "",
    "MainImageUrl": "",
    "IsSavedForApproval": false,
    "CanPublish": false,
    "Search": "",
    "SearchImages": "",
    "DisabledAutoPosting": false,
    "DisablePaymentsForPost": false
}
var searchItem = {
    "PhotoUrl": "",
    "IdOfItem": "",
    "Title": "",
    "UrlOfItem": "",
    "ItemType": "",
    "Properties": ""
}

function canPublishPosts() {
    return SiteVars.UserRights.indexOf("PublishOwnPosts") > -1 ? true : false;
}
function closeSearchItem() {
    $("#searchMemberLeagueBox").hide();
};