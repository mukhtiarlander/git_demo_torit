angular.module('blogEditor').controller('PageEditorController', ["$rootScope", "$scope", "$location", "$filter", "$log", "dataService", function ($rootScope, $scope, $location, $filter, $log, dataService) {
    $scope.id = editVars.id;
    $scope.page = newPage;
    $scope.lookups = [];
    $scope.selectedParent = {};
    $scope.fullScreen = false;

    $scope.load = function () {
        var lookupsUrl = '/api/lookups';
        dataService.getItems(lookupsUrl)
        .success(function (data) {
            angular.copy(data, $scope.lookups);
            if ($scope.id) {
                $scope.loadPage();
            }
        })
        .error(function () {
            toastr.error("Error loading lookups");
        });
    }

    $scope.loadPage = function () {
        spinOn();
        var url = '/api/pages/' + $scope.id;
        dataService.getItems(url)
        .success(function (data) {
            angular.copy(data, $scope.page);
            if ($scope.page.Parent != null) {
                $scope.selectedParent = selectedOption($scope.lookups.PageList, $scope.page.Parent.OptionValue);
                var x = $scope.selectedParent;
            }
            $("#editor").html($scope.page.Content);
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPage);
            spinOff();
        });
    }

    $scope.save = function () {
        if (!$('#form').valid()) {
            return false;
        }
        spinOn();
        $scope.page.Content = $("#editor").html();
        
        $scope.page.Parent = $scope.selectedParent;

        if ($scope.page.Slug.length == 0) {
            $scope.page.Slug = toSlug($scope.page.Title);
        }

        if ($scope.page.Id) {
            dataService.updateItem('/api/pages/update/foo', $scope.page)
           .success(function (data) {
               toastr.success("Page updated");
               $("#modal-form").modal('hide');
               spinOff();
           })
           .error(function () {
               toastr.error("Update failed");
               spinOff();
           });
        }
        else {
            dataService.addItem('/api/pages', $scope.page)
           .success(function (data) {
               toastr.success("Page added");
               $log.log(data);
               if (data.Id) {
                   angular.copy(data, $scope.page);
                   var x = $scope.page.Id;
               }
               $("#modal-form").modal('hide');
               spinOff();
           })
           .error(function () {
               toastr.error("Failed adding new page");
               spinOff();
           });
        }
    }

    $scope.saveSource = function () {
        $scope.page.Content = $("#editor-source").val();
        $("#editor").html($("#editor-source").val());
        $("#modal-source").modal('hide');
    }

    $scope.uploadFile = function (action, files) {
        var fd = new FormData();
        fd.append("file", files[0]);

        dataService.uploadFile("/api/upload?action=" + action, fd)
        .success(function (data) {
            toastr.success("Uploaded");
            var editorHtml = $("#editor").html();
            if (action === "image") {
                $("#editor").html(editorHtml + '<img src=' + data + ' />');
            }
            if (action === "video") {
                $("#editor").html(editorHtml + '<p>[video src=' + data + ']</p>');
            }
            if (action === "file") {
                var res = data.split("|");
                if (res.length === 2) {
                    $("#editor").html(editorHtml + '<a href="' + res[0].replace('"', '') + '">' + res[1].replace('"', '') + '</a>');
                }
            }
        })
        .error(function () { toastr.error("Import failed"); });
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

    $scope.load();

    $(document).ready(function () {
        $('#form').validate({
            rules: {
                txtTitle: { required: true }
            }
        });
    });
}]);

var newPage = {
    "Id": "",
    "Title": "",
    "Content": "",
    "DateCreated": moment().format("YYYY-MM-DD HH:MM"),
    "Slug": "",
    "ShowInList": true,
    "IsPublished": true
}