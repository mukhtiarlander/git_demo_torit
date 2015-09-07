var League = new function () {
    var thisViewModel = this;
    var documentId;
    var Archived = false;
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
                $("#doc-archive-btn").toggleClass("display-none", true);
                $("#doc-move-ddl").toggleClass("display-none", true);
            }
            else if ($("#documents tbody tr").find(":checkbox:checked").length == 1) {
                $("#doc-archive-btn").toggleClass("display-none", false);
                $("#doc-rename-btn").toggleClass("display-none", false);
                $("#doc-delete-btn").toggleClass("display-none", false);
                $("#doc-move-ddl").toggleClass("display-none", false);
            }
            else {
                $("#doc-archive-btn").toggleClass("display-none", false);
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
    this.ArchiveDocument = function () {
        var docs = thisViewModel.documentId.split(',');
        var msg = docs.length > 1 ? 'Are you sure you want to archive ' + docs.length + ' documents?' : 'Are you sure you want to archive this document?';
        if (confirm(msg)) {
            var owner = $("#OwnerId");
            $.getJSON("/document/ArchiveDocument", { ownerId: owner.val(), doc: thisViewModel.documentId, isArchived: Archived }, function (result) {
                if (result.isSuccess === true) {
                    $('.bottom-right').notify({
                        message: { text: 'Archived Successfully' },
                        fadeOut: { enabled: true, delay: 4000 },
                        type: "success"
                    }).show();
                } else {
                    $('.bottom-right').notify({
                        message: { text: 'Something wrong Happened, Try again later. ' },
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
    };

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
        $("#doc-archive-btn").toggleClass("display-none", true);
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
    var delay = (function () {
        var timer = 0;
        return function (callback, ms) {
            clearTimeout(timer);
            timer = setTimeout(callback, ms);
        };
    })();

    var lastSearch = "";
    var lastArchiveState = false;
    this.FullTextDocumentSearchLeague = function (IsOnKeyPress, isArchived) {
        var isDeepSearch = $("#cbDeepSearch").prop('checked');
        var box = $("#textSearchBox");
        if (IsOnKeyPress && isDeepSearch) {
            return;
        }
        if (typeof isArchived === "undefined" || isArchived === null) {
            isArchived = Archived;
        }
        if (lastSearch == box.val().trim() && lastArchiveState == isArchived) {
            return;
        }
        var waitTime = 0;
        if (IsOnKeyPress)
            waitTime = 2000;
        delay(function () {
            Archived = isArchived;
            lastArchiveState = Archived;
            var gId = getParameterByName('g');
            var fId = getParameterByName('f');
            var owner = $("#OwnerId");
            ;
            var tableBody = $("#documentsBody");
            $("#loading").toggleClass("displayNone", false);
            var url = "";
            if (!isDeepSearch)
                url = "/document/SeachByDocumentName";
            else
                url = "/document/FullTextSearchLeague";
            $.getJSON(url, { leagueId: owner.val(), text: box.val().trim(), folderId: fId, groupId: gId, isArchived: Archived }, function (result) {
                $("#loading").toggleClass("displayNone", true);
                if (result.isSuccess === true) {
                    tableBody.html("");
                    $.each(result.results, function (i, item) {
                        League.DisplayDocumentRow(result, tableBody, item);
                    });
                    League.SetUpDocumentsSection();
                    lastSearch = box.val().trim();
                } else {
                }
            }).error(function () {
                $("#loading").toggleClass("displayNone", true);
            });
        }, waitTime);

    };

    this.DisplayArchivedDocuments = function (button) {
        Archived = $("#doc-view-archive-btn").hasClass("non-archive");
        League.FullTextDocumentSearchLeague(false, Archived);
        if (Archived) {
            $(button).removeClass("non-archive");
            $(button).attr("title", "View Non-Archived Documents");
            $(button).addClass("archive");
        } else {
            $(button).removeClass("archive");
            $(button).addClass("non-archive");
            $(button).attr("title", "View Archived Documents");
        }
        if (Archived) {
            $("#doc-archive-btn").attr("title", "Remove Archive");
        } else {
            $("#doc-archive-btn").attr("title", "Archive");
        }
    };

    this.DisplayDocumentRow = function (result, tableBody, item) {

        var row = $(document.createElement('tr'));
        row.attr("id", "docRow-" + item.OwnerDocId);
        row.attr("name", item.DocumentName);
        var checkColumn = $(document.createElement('td'));
        var cbId = "cb-" + item.OwnerDocId;
        checkColumn.append('<input type="checkbox" id="' + cbId + '"   />');
        row.append(checkColumn);

        var firstColumn = $(document.createElement('td'));
        firstColumn.css("vertical-align", "middle");
        //RDN.Library.Classes.Document.Enums
        if (item.MimeType === 3 || item.MimeType === 28 || item.MimeType === 32)
            firstColumn.append('<i class="fa fa-file-excel-o fa-lg"></i>');
        else if (item.MimeType === 2 || item.MimeType === 11 || item.MimeType === 30)
            firstColumn.append(' <i class="fa fa-file-excel-o fa-lg"></i>');
        else if (item.MimeType === 33)
            firstColumn.append('<i class="fa fa-file-excel-o fa-lg"></i>');
        else if (item.MimeType === 35)
            firstColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/odf.png")" />');
        else if (item.MimeType === 36)
            firstColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/ods.png")" />');
        else if (item.MimeType === 19 || item.MimeType === 13 || item.MimeType === 18 || item.MimeType === 20 || item.MimeType === 26 || item.MimeType === 24)
            firstColumn.append('<i class="fa fa-file-image-o fa-lg"></i>');
        else if (item.MimeType === 6)
            firstColumn.append('<i class="fa fa-file-text-o fa-lg"></i>');
        else if (item.MimeType === 25)
            firstColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/ai.png")" />');
        else if (item.MimeType === 1)
            firstColumn.append('<i class="fa fa-file-pdf-o fa-lg"></i>');
        else if (item.MimeType === 4)
            firstColumn.append('  <i class="fa fa-file-zip-o fa-lg"></i>');
        else if (item.MimeType === 7)
            firstColumn.append('<i class="fa fa-file-code-o fa-lg"></i>');
        else if (item.MimeType === 7)
            firstColumn.append('<i class="fa fa-file-code-o fa-lg"></i>');
        else if (item.MimeType === 9 || item.MimeType === 34)
            firstColumn.append('<i class="fa fa-file-powerpoint-o fa-lg"></i>');
        else if (item.MimeType === 23)
            firstColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/svg.png")" />');
        else if (item.MimeType === 27)
            firstColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/html.png")" />');
        else if (item.MimeType === 29)
            firstColumn.append('<i class="fa fa-newspaper-o fa-lg"></i>');
        else if (item.MimeType === 6)
            firstColumn.append('<i class="fa fa-file-text-o fa-lg"></i>');
        else if (item.MimeType === 37)
            firstColumn.append('<i class="fa fa-file-audio-o fa-lg"></i>');
        else if (item.MimeType === 38)
            firstColumn.append(' <i class="fa fa-film fa-lg"></i>');
        else if (item.MimeType === 39)
            firstColumn.append(' <img class="docIcon" src="@Url.Content("~/Content/images/icons/docs/ps.png")" />');
        else if (item.MimeType === 40)
            firstColumn.append(' <img class="docIcon" src="@Url.Content("~/Content/images/icons/docs/wps.png")" />');
        else if (item.MimeType === 41)
            firstColumn.append('<i class="fa fa-file-excel-o fa-lg"></i>');
        else if (item.MimeType === 42)
            firstColumn.append('<span class="fa-stack"><i class="fa fa-file-o fa-stack-2x font19 width-initial"></i><i class="fa fa-google fa-stack-1x font11 margin-left-3 width-initial"></i></span>');
        row.append(firstColumn);

        var secondColumn = $(document.createElement('td'));
        secondColumn.attr("id", "rn-" + item.OwnerDocId);
        var memberLink = $(document.createElement('a'));
        memberLink.attr({ href: leagueHost + "document/download/" + item.DocumentId.replace(/-/g, "") });
        memberLink.html(item.DocumentName);
        secondColumn.append(memberLink);
        row.append(secondColumn);


        var thirdColumn = $(document.createElement('td'));
        if (item.Folder != null)
            thirdColumn.append(item.Folder.FolderName);
        row.append(thirdColumn);


        var fourthColumn = $(document.createElement('td'));
        fourthColumn.css("vertical-align", "middle");
        var count = item.CommentCount.toString().trim();
        if (count === "0") {
            count = "";
        }
        fourthColumn.append('<div class="spanIconsDoc"><a class="btn btn-xs btn-primary" title="Comments (' + item.CommentCount + ')" href="' + leagueHost + 'league/document/comments/' + item.DocumentId.replace(/-/g, "") + '/' + item.OwnerDocId + '"><i class="fa fa-comments"></i><span class="docCount"> ' + count + '</span></a></div>');
        row.append(fourthColumn);

        var fifthColumn = $(document.createElement('td'));
        fifthColumn.append("<span>" + GetFileSize(item.SizeOfDocument, true) + "</span>");
        row.append(fifthColumn);

        var sixColumn = $(document.createElement('td'));
        sixColumn.append(item.UploadedHuman);
        row.append(sixColumn);

        if ($("#cbDeepSearch").prop('checked')) {
            var sevenColumn = $(document.createElement('td'));
            sevenColumn.append("<b>" + item.SearchMatches + "</b> Matches");
            row.append(sevenColumn);
        }

        var eightColumn = $(document.createElement('td'));
        row.append(eightColumn);

        tableBody.append(row);
    };

}