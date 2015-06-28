var League = new function () {
    var thisViewModel = this;
    var documentId;
    this.ChangeGroupOfFolderSettings = function (dropDown, folderId) {
        $("#img-" + folderId).toggleClass("displayNone", true);
        var owner = $("#OwnerId");
        var folder = $("#folderName");
        var selectedFolder = $("#folder-" + folderId + " option:selected");
        $.getJSON("/document/MoveFolderToGroup", { ownerId: owner.val(), moveType: selectedFolder.text(), moveTo: selectedFolder.val(), fold: folderId }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        }).error(function () {
        });
        $("#img-" + folderId).toggleClass("displayNone", false);
    };
    this.SetUpDocumentsSection = function () {
        $('#documents tbody tr input[type="checkbox"]').on('change', function (event) {
            thisViewModel.documentId = '';
            $.each($("#documents tbody tr"), function (index, tr) {
                if ($(tr).find(":checkbox").is(":checked")) {
                    thisViewModel.documentId += parseInt($(tr).attr("id").replace(/[\D]/g, ""), 10) + ",";
                }
            });
            thisViewModel.documentId = thisViewModel.documentId.substr(0, thisViewModel.documentId.length - 1);


            if ($("#documents tbody tr").find(":checkbox:checked").length == 0) {
                $("#doc-rename-btn").toggleClass("display-none", true);
                $("#doc-delete-btn").toggleClass("display-none", true);
                $("#doc-move-ddl").toggleClass("display-none", true);
            }
            else if ($("#documents tbody tr").find(":checkbox:checked").length == 1) {
                $("#doc-rename-btn").toggleClass("display-none", false);
                $("#doc-delete-btn").toggleClass("display-none", false);
                $("#doc-move-ddl").toggleClass("display-none", false);
            }
            else {
                $("#doc-rename-btn").toggleClass("display-none", true);
                $("#doc-delete-btn").toggleClass("display-none", false);
                $("#doc-move-ddl").toggleClass("display-none", false);
            }

            $("#img-MoveItem").toggleClass("display-none", true);

        });

    };
    this.DeleteDocument = function (span) {
        var docs = thisViewModel.documentId.split(',');
        var msg = docs.length > 1 ? 'Are you sure you want to delete ' + docs.length + ' documents?' : 'Are you sure you want to delete this document?';
        if (confirm(msg)) {
            var owner = $("#OwnerId");
            $.getJSON("/document/DeleteDocument", { ownerId: owner.val(), doc: thisViewModel.documentId }, function (result) {
                if (result.isSuccess === true) {
                    $('.bottom-right').notify({
                        message: { text: 'Files Deleted. ' },
                        fadeOut: { enabled: true, delay: 4000 },
                        type: "success"
                    }).show();
                } else {
                    $('.bottom-right').notify({
                        message: { text: 'Something Happened, Try again later. ' },
                        fadeOut: { enabled: true, delay: 4000 },
                        type: "danger"
                    }).show();
                }
                League.ResetDocumentGrid();
            }).error(function () {
            });

            for (var i = 0; i < docs.length; i++) {
                $("#docRow-" + docs[i].toString()).remove();
            }
        }
    }
    this.SetDocumentId = function (docId) {
        documentId = docId;
    };
    this.RenameDocument = function (span) {
        var docName = $("#docRow-" + thisViewModel.documentId).attr("name");
        var output = docName;
        $("#rn-" + thisViewModel.documentId).html("<div class='input-group'><input type='text' class='form-control' id='rns-" + thisViewModel.documentId + "' value='" + output + "' /><span class='input-group-btn'><button type='button' id='rnl-" + thisViewModel.documentId + "' class='btn btn-success' onclick='League.RenameDocumentSubmit(this,\"" + thisViewModel.documentId + "\")'><i class='fa fa-save'></i> Rename</button></span></div>");
        $("#rns-" + thisViewModel.documentId).focus();
    }
    this.RenameDocumentSubmit = function (span) {
        var owner = $("#OwnerId");
        var text = $("#rns-" + thisViewModel.documentId);
        var spans = $(span);
        $.getJSON("/document/RenameDocument", { ownerId: owner.val(), doc: thisViewModel.documentId, newName: text.val() }, function (result) {
            if (result.isSuccess === true) {
                var row = $("#rn-" + thisViewModel.documentId).html(result.link);
                $("#docRow-" + thisViewModel.documentId).attr("name", text.val());
                League.ResetDocumentGrid();
            }
            else {
            }
        }).error(function () {
        });
        text.remove();
        spans.remove();
    };
    this.MoveDocumentToFolder = function (dropDown) {
        var owner = $("#OwnerId");
        var folder = $("#folderName");
        var selectedFolder = $(dropDown).find("option:selected");

        $.getJSON("/document/MoveFileTo", { ownerId: owner.val(), moveTo: selectedFolder.val(), moveToName: selectedFolder.text(), doc: thisViewModel.documentId }, function (result) {
            if (result.isSuccess === true) {
                var docs = thisViewModel.documentId.split(',');
                for (var i = 0; i < docs.length; i++) {
                    $("#fn-" + docs[i]).html(selectedFolder.text());
                }

                $('.bottom-right').notify({
                    message: { text: 'Files Moved. ' },
                    fadeOut: { enabled: true, delay: 4000 },
                    type: "success"
                }).show();
            } else {
                $('.bottom-right').notify({
                    message: { text: 'Something Happened, Try again later. ' },
                    fadeOut: { enabled: true, delay: 4000 },
                    type: "danger"
                }).show();
            }
            League.ResetDocumentGrid();
        }).error(function () {

        });
    };
    this.ResetDocumentGrid = function () {

        $.each($("#documents tbody tr"), function (index, tr) {
            $(tr).find(":checkbox").prop("checked", false);
        });
        $("#doc-rename-btn").toggleClass("display-none", true);
        $("#doc-delete-btn").toggleClass("display-none", true);
        $("#doc-move-ddl").toggleClass("display-none", true);
        $("#doc-move-ddl").val('');
    };
    this.DeleteLeagueReport = function (span) {
        var reportId = $("#SelectedReport :selected");
        if (reportId.val() > 0) {
            $.getJSON("/league/RemoveLeagueReport", { reportId: reportId.val() }, function (result) {
                if (result.isSuccess === true) {
                    reportId.remove();
                }
            }).error(function () {
            });
        }
    };
}