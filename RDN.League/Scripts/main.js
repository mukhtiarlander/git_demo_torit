$(document).ready(function () {
    $.ajaxSetup({ cache: false });
});


var leagueHost = "https://league.rdnation.com/";
var rdnationHost = "https://rdnation.com/";
var simpleId = 0;
var simpleIdTwo = 0;
//ko configuration
ko.validation.configure({
    registerExtenders: true,
    messagesOnModified: true,
    insertMessages: true,
    parseInputAttributes: true,
    messageTemplate: null
});
jQuery.fn.reverse = [].reverse;

jQuery.fn.center = function () {
    this.css("position", "absolute");
    this.css("top", Math.max(0, (($(window).height() - this.outerHeight()) / 2) + $(window).scrollTop()) + "px");
    this.css("left", Math.max(0, (($(window).width() - this.outerWidth()) / 2) + $(window).scrollLeft()) + "px");
    return this;
}

jQuery.fn.dataTableExt.oSort['natural-asc'] = function (a, b) {
    return naturalSort(a, b);
};

jQuery.fn.dataTableExt.oSort['natural-desc'] = function (a, b) {
    return naturalSort(a, b) * -1;
};

jQuery.validator.addMethod("greaterThanEqual",
function (value, element, params) {
    if (!/Invalid|NaN/.test(new Date(value))) {
        return new Date(value) >= new Date($(params).val());
    }

    return isNaN(value) && isNaN($(params).val())
        || (Number(value) >= Number($(params).val()));
}, 'Must be greater than {0}.');

jQuery.validator.addMethod("lessThanEqual",
function (value, element, params) {
    if (!/Invalid|NaN/.test(new Date(value))) {
        return new Date(value) <= new Date($(params).val());
    }
    return isNaN(value) && isNaN($(params).val())
        || (Number(value) <= Number($(params).val()));
}, 'Must be less than {0}.');

$.validator.addMethod('minStrict', function (value, el, param) {
    return value > param;
}, 'Must be greater than {0}.');

function changeMoveTopicForum(dropdown) {
    $("#loadingCategories").toggleClass("displayNone");
    var fId = $("#ForumId").val();
    var category = $("#ChosenCategory");
    $.getJSON("/Utilities/SearchLeagueCategories", { fId: fId, gId: dropdown.value }, function (result) {
        $("#loadingCategories").toggleClass("displayNone");
        if (result.names.length > 0) {
            $(category).empty();
            $(result.names).each(function () {
                $("<option />", {
                    val: this.CategoryId,
                    text: this.CategoryName
                }).appendTo(category);
            });
        }
    });
}

function MarkForumTopicAsRead(img, topicId) {
    var forumId = $("#ForumId").val();
    $.getJSON("/forum/MarkAsRead", { forumId: forumId, topicId: topicId }, function (result) {
        if (result.isSuccess === true) {
        }
    });
    $(img).parent().parent().toggleClass("forumIsRead", true);
    $(img).remove();
}



function ForumPostToggleWatch(span) {
    var topicId = $("#TopicId").val();
    var forumId = $("#ForumId").val();
    $.getJSON("/forum/WatchTopic", { forumId: forumId, topicId: topicId }, function (result) {
        if($(span).html() == '<i class="fa fa-binoculars"></i> Watch')
        {
            $(span).html('<i class="fa fa-binoculars"></i> Stop Watching');
        }
        else
        {
            $(span).html('<i class="fa fa-binoculars"></i> Watch');
        }
    });
}

function EnableForumTopicEdit(span, enable) {
    if (enable === "true") {
        $("#topicEditTitle").toggleClass("display-none", false);
        $("#updateEditTitle").toggleClass("display-none", false);
        $("#cancelEditTitle").toggleClass("display-none", false);
        $(span).toggleClass("display-none", true);
        $("#topicTitle").toggleClass("display-none", true);
    } else {
        $("#editTitle").toggleClass("display-none", false);
        $("#topicEditTitle").toggleClass("display-none", true);
        $("#updateEditTitle").toggleClass("display-none", true);
        $("#cancelEditTitle").toggleClass("display-none", true);
        $("#topicTitle").toggleClass("display-none", false);
    }
}

function ForumTopicEditSub(button) {
    var title = $("#topicTitleEdit");
    $("#topicTitle").html(title.val());
    title.attr("placeholder", title.val());
    $("#editTitle").toggleClass("display-none", false);
    $("#topicEditTitle").toggleClass("display-none", true);
    $("#updateEditTitle").toggleClass("display-none", true);
    $("#cancelEditTitle").toggleClass("display-none", true);
    $("#topicTitle").toggleClass("display-none", false);
    if (title.val().length > 0) {
        title.toggleClass("error", false);
        $.getJSON("/forum/UpdateTopicName", { fId: $("#ForumId").val(), tId: $("#TopicId").val(), n: title.val() }, function (result) {
            if (result.isSuccess === true) {
                
            }
        });
        title.val("");
    }
    else
        title.toggleClass("error", true);
}

function PublishGameOnline(button, publish) {
    var g = $("#GameId").val();
    var pk = $("#PrivateKeyForGame").val();
    $("#loading").toggleClass("displayNone", false);
    $.getJSON("/game/PublishGame", { gId: g, pId: pk, isPub: publish }, function (result) {
        if (result.isSuccess === true) {
            if (publish === true) {
                $("#publishedGameContainer").toggleClass("displayNone", false);
                $(button).val("UnPublish").attr("onClick", "PublishGameOnline(this, false)");;

            }
            else {
                $(button).val("Publish").attr("onClick", "PublishGameOnline(this, true)");;
                $("#publishedGameContainer").toggleClass("displayNone", true);
            }
        } else {
        }
        $("#loading").toggleClass("displayNone", true);

    }).error(function () {
    });
}

function CarrierChange(list) {
    var t = $(list).find("option:selected").text();
    if (t.indexOf('MMS') > 0)
        $("#carrierWarning").text("If MMS doesn't work, please try SMS for that carrier.");
    else if (t.indexOf('Straight Talk') > 0)
        $("#carrierWarning").text("If one Straight Talk doesn't work, try the next one.");
    else
        $("#carrierWarning").text("");
}

function VerifySMSCarrier(button) {
    var num = $("#PhoneNumber").val();
    if (num == "" || num == null)
        alert("Please Enter a Number");
    num = num.replace(/\+/g, "").replace(/\./g, "");

    $("#loading").toggleClass("displayNone", false);
    $.getJSON("/member/verifysms", { number: num }, function (result) {
        if (result.isSuccess === true) {
            $("#verifyButton").toggleClass("displayNone", true);
            $("#codeHtml").toggleClass("displayNone", false);

        } else {
        }
        $("#loading").toggleClass("displayNone", true);

    }).error(function () {
    });
}

function EnterCarrierCode(button) {
    var num = $("#PhoneNumber").val();
    num = num.replace(/\+/g, "").replace(/\./g, "");
    var code = $("#code").val();
    $("#loading2").toggleClass("displayNone", false);
    $.getJSON("/member/verifysmscode", { number: num, code: code }, function (result) {
        if (result.isSuccess === true) {
            $('.bottom-right').notify({
                message: { text: 'Verification number is saved! ' },
                fadeOut: { enabled: true, delay: 4000 }
            }).show();
        } else {
        }
        $("#loading2").toggleClass("displayNone", true);
    }).error(function () {
    });
}

function SetLeagueOwnership(button, memId, ownerType) {
    $.getJSON("/league/AddOwnerType", { memberId: memId, ownerType: ownerType }, function (result) {
        if (result.isSuccess === true) {
        } else {
        }
    }).error(function () {
    });
}

function joinCode() {
    /// /league/members/view/all/refresh
    var currentUrl = $(location).attr('href');
    var url = window.location.origin + "/league/members/view/all/refresh";
    // alert(url);
    //  location.replace("http://localhost:1094/league/members/view/all/refresh");
    $(location).attr('href', url);
    /*
    $.ajax({
            url: "http://google.com"
        });
        */
}

function FullTextDocumentSearchLeague() {
    var gId = getParameterByName('g');
    var fId = getParameterByName('f');
    var owner = $("#OwnerId");
    var box = $("#textSearchBox");
    var tableBody = $("#documentsBody");
    $("#loading").toggleClass("displayNone", false);
    $.getJSON("/document/FullTextSearchLeague", { leagueId: owner.val(), text: box.val(), folderId: fId, groupId: gId }, function (result) {
        $("#loading").toggleClass("displayNone", true);
        if (result.isSuccess === true) {
            tableBody.html("");
            $.each(result.results, function (i, item) {
                DisplayDocumentRow(result, tableBody, item);
            });
        } else {
        }
    }).error(function () {
        $("#loading").toggleClass("displayNone", true);
    });
}
function DisplayDocumentRow(result, tableBody, item) {

    var row = $(document.createElement('tr'));

    var checkColumn = $(document.createElement('td'));

    row.append(checkColumn);

    var firstColumn = $(document.createElement('td'));
    if (item.Folder != null)
        firstColumn.append(item.Folder.FolderName);
    row.append(firstColumn);

    var secondColumn = $(document.createElement('td'));
    //RDN.Library.Classes.Document.Enums
    if (item.MimeType === 3)
        secondColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/excel.png")" />');
    else if (item.MimeType === 2)
        secondColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/word.png")" />');
    else if (item.MimeType === 5)
        secondColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/pic.png")" />');
    else if (item.MimeType === 6)
        secondColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs.png")" />');
    else if (item.MimeType === 1)
        secondColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/pdf.png")" />');
    else if (item.MimeType === 4)
        secondColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/zip.png")" />');
    else if (item.MimeType === 7)
        secondColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/xml.png")" />');
    else if (item.MimeType === 9)
        secondColumn.append('<img class="docIcon" src="' + leagueHost + '/Content/images/icons/docs/ppt.png")" />');
    row.append(secondColumn);

    var thirdColumn = $(document.createElement('td'));
    var memberLink = $(document.createElement('a'));
    memberLink.attr({ href: leagueHost + "document/download/" + item.DocumentId.replace(/-/g, "") });
    memberLink.html(item.DocumentName);
    thirdColumn.append(memberLink);
    row.append(thirdColumn);

    var fourthColumn = $(document.createElement('td'));
    fourthColumn.append('<div class="spanIconsDoc"><a title="Comments (' + item.CommentCount + ')" href="' + leagueHost + 'league/document/comments/' + item.DocumentId.replace(/-/g, "") + '/' + item.OwnerDocId + '"><img src="' + leagueHost + '/Content/images/icons/comment.png"  /><span class="docCount">' + item.CommentCount + '</span></a></div>');
    row.append(fourthColumn);

    var fifthColumn = $(document.createElement('td'));
    fifthColumn.append();
    row.append(fifthColumn);
    tableBody.append(row);

    var sixColumn = $(document.createElement('td'));
    sixColumn.append(item.UploadedHuman);
    row.append(sixColumn);

    var sevenColumn = $(document.createElement('td'));
    sevenColumn.append("<b>" + item.SearchMatches + "</b> Matches");
    row.append(sevenColumn);

    var eightColumn = $(document.createElement('td'));
    row.append(eightColumn);
}








function AddCommentToDocument(id, ownerId) {
    var com = $("#comment");
    $.getJSON("/document/AddCommentToLeagueDocument", { docId: id, docOwnerId: ownerId, comment: com.val() }, function (result) {
        if (result.isSuccess === true) {
            $("#folderBody div:first").before('<div class="panel panel-default margin-top-10"><div class="panel-heading"><b>You</b><button type="button" class="btn btn-xs btn-danger pull-right" title="Delete"  onclick="DeleteCommentFromDocument(this,\'' + id + '\',\'' + ownerId + '\')"><i class="fa fa-trash"></i></button><br /><i class="fa fa-quote-left" style="color:silver"></i> <span style="font-size:18px">' + com.val() + '</span> <i class="fa fa-quote-right" style="color:silver"></i><br /><span class="text-muted small">Just Now</span></div></div>');
        } else {
        }
        com.val("");
    }).error(function () {
        com.val("");
    });
}
function DeleteCommentFromDocument(span, docOwnerId, commentId) {
    var folder = $("#folderName");
    $.getJSON("/document/DeleteCommentForDocument", { commentId: commentId, docOwnerId: docOwnerId }, function (result) {
        if (result.isSuccess === true) {
            $(span).parent().parent().remove();
        } else {
        }
    }).error(function () {
    });
    
}
function AddFolderToDocumentRepo(leagueId) {
    var folder = $("#folderName");
    $.getJSON("/document/AddFolderToLeagueDocuments", { leagueId: leagueId, folderName: folder.val() }, function (result) {
        if (result.isSuccess === true) {
            //add to table
            $("#folderBody").append("<tr><td>" + folder.val() + "</td><td class='center'>0</td><td class='center'>0</td><td class='center'><span class='spanLink' onclick=\"DeleteFolderFromDocumentRepo(this,'" + leagueId + "', '" + result.folderId + "')\">Delete Folder</span></td></tr>");
        } else {
        }
        folder.val("");
    }).error(function () {
        folder.val("");
    });
}

function DeleteFolderFromDocumentRepo(span, leagueId, folderId) {
    var folder = $("#folderName");
    $.getJSON("/document/DeleteFolderFromLeagueDocuments", { leagueId: leagueId, folderId: folderId }, function (result) {
        if (result.isSuccess === true) {

        } else {
        }
    }).error(function () {
    });
    $(span).parent().parent().remove();
}

function CreateNewColor() {
    var nameOfColor = $("#colorName");
    var colorSelected = $("#colorSelected");
    var dropDown = $("#ColorTempSelected");
    if (nameOfColor.val() === "") {
        nameOfColor.toggleClass("error", true);
        return;
    }
    nameOfColor.toggleClass("error", false);
    $.getJSON("/utilities/AddColor", { nameOfColor: nameOfColor.val(), hexOfColor: colorSelected.text() }, function (result) {
        if (result.isSuccess === true) {
            dropDown.append($('<option></option>').val(colorSelected.text()).html(nameOfColor.val()));
            LoadDropDownBackgroundColors();
            $('#ColorTempSelected option:last-child').attr("selected", "selected");

            dropDown.css('background-color', colorSelected.text());
        }
        nameOfColor.val("");
    }).error(function () {
        nameOfColor.val("");
    });

}
function LoadDropDownBackgroundColors() {
    $("#ColorTempSelected option").each(function () {
        if ($(this).val() != '') {
            $(this).css('background-color', $(this).val());
        }
    });
}
function RemoveSelectedColor(span, color) {
    $(span).parent().remove();
    var colors = $("#ColorsSelected");
    colors.val(colors.val().replace(color + ";", ""));
}
function AddSelectedColor() {
    var selected = $("#ColorTempSelected option:selected");
    var colors = $("#ColorsSelected");
    colors.val(colors.val() + selected.val() + ";");
    $("#colorsToAdd").append("<div class='selectedColorCon' title=" + selected.text() + " data-toggle='tooltip' data-placement='top'><div class='selectedColor' style='background-color:" + selected.val() + "'></div><span onclick='RemoveSelectedColor(this,\"" + selected.val() + "\")'><i class='fa fa-times fa-lg'></i></span></div>");

}
function GenerateSelectedColors() {
    var selected = $("#ColorTempSelected option:selected");
    var colors = $("#ColorsSelected");
    colors.val(colors.val() + selected.val() + ";");
    $("#colorsToAdd").append("<div class='selectedColorCon'><div class='selectedColor' style='background-color:" + selected.val() + ";'>" + selected.html() + "</div><span class='spanLink' onclick=\"RemoveSelectedColor(this, '" + selected.val() + "')\">Remove</span></div>");
    $('[data-toggle="tooltip"]').tooltip();
}
function ColorSelectorChanged() {
    var selected = $("#ColorTempSelected option:selected").val();
    if ($("#ColorTempSelected").val() != '') {
        $("#ColorTempSelected").css('background-color', selected);
    }
    else
        $("#ColorTempSelected").css('background-color', "#FFFFFF");
}

function changeMemberSettingCalView(changeTo) {
    $.getJSON("/member/ChangeMemberSettingCalView", { newId: changeTo }, function (result) {
        if (result.isSuccess) {
            $('.bottom-right').notify({
                message: { text: 'Saved! ' },
                fadeOut: { enabled: true, delay: 3000 }
            }).show();

        }
    }).error(function () {

    });

}

function deleteChatMessage(element, group, mem) {
    if (confirm('Really Delete Message?')) {
        $.getJSON("/message/deletechatmessage", { groupId: group, memId: mem }, function (result) {

        });
        $(element).parent().parent().remove();
    }
}

function AddHashToUrl(a) {
    $(a).attr('href', function () {
        return this.href + window.location.hash;
    });
}

function moveInvoiceToNextStatus(spanLink, invoiceId, status) {
    $.getJSON("/store/MoveInvoiceToNextStatus", { invoiceId: invoiceId, status: status }, function (result) {
        if (result.isSuccess === true) {
            $("#invoiceStatus").html(result.status);
        }
    });
    $(spanLink).parent().html("");
}

function removeMemberContact(contactId) {
    if (confirm('Really Remove Contact?')) {
        var memId = $("#MemberId").val();
        $.getJSON("/member/removecontact", { memberId: memId, contactId: contactId }, function (result) {
            if (result.isSuccess === true) {

            }
        });
        $("#" + contactId + "-div").remove();
    }
}

function AddContactToMember() {
   
    var firstName = $("#FirstName");
    var lastName = $("#LastName");
    var contactType = $("#ContactType option:selected").val();
    var email = $("#Email");
    var phoneNumber = $("#PhoneNumber");
    var address = $("#Address1");
    var address2 = $("#Address2");
    var city = $("#City");
    var state = $("#State");
    var zipCode = $("#ZipCode");
    var country = $("#Country option:selected").val();
    var memId = $("#MemberId").val();

    $.getJSON("/member/addcontact", { memberId: memId, first: firstName.val(), last: lastName.val(), type: contactType, email: email.val(), phone: phoneNumber.val(), address1: address.val(), address2: address2.val(), city: city.val(), state: state.val(), zip: zipCode.val(), country: country }, function (result) {
        if (result.isSuccess === true) {
            firstName.val("");
            lastName.val("");
            email.val("");
            phoneNumber.val("");
            address.val("");
            address2.val("");
            city.val("");
            state.val("");
            zipCode.val("");
        }
    });
    var cont = $("#Contacts");
    var currentCont = cont.html();
    var newContact = '<div class="col-xs-12 col-sm-6 col-md-4 newContact" style="display:none">';
    newContact += '<div class="panel panel-default" style="min-height:260px"><div class="panel-heading">';
    newContact += '<h4 class="no-margin"><i class="fa fa-user"></i>&nbsp;' + firstName.val() + ' ' + lastName.val() + '</h4></div>';
    newContact += '<div class="panel-body no-padding-bottom"><table><tr><td class="b"><i class="fa fa-suitcase"></i>&nbsp;</td><td>';
    newContact += contactType.replace(/_/g, " ") + '</td></tr><tr><td class="b"><i class="fa fa-envelope"></i>&nbsp;</td><td>';
    newContact += email.val();
    newContact += '</td></tr><tr>';
    newContact += '<td class="b"><i class="fa fa-phone"></i>&nbsp;</td><td>';
    newContact += phoneNumber.val() + '</a></td></tr><tr><td class="b" style="vertical-align:top">';
    newContact += '<span><i class="fa fa-map-marker"></i>&nbsp;</span></td><td>';
    newContact += address.val() + "<br/>";
    newContact += address2.val() + "<br />";
    newContact += city.val() + "<br/>";
    newContact += state.val() + "<br/>";
    newContact += zipCode.val() + "<br/>";
    newContact += $("#Country option:selected").text();
    newContact += "</td></tr>";
    newContact += "</table>";
    newContact += "</div></div></div>";
    $('html,body').animate({ 'scrollTop': $('#add-contact-form').position().top - 400 }, 500, function () {
        cont.html(currentCont + newContact);
        $("#Contacts .newContact:last-child").fadeIn();
    });
}

function HideShowCCInfo(hideShow) {
    if (hideShow === 'show') {
        $("#expirationDateTR").toggleClass('displayNone', false);
        $("#securityCodeTR").toggleClass('displayNone', false);
        $("#cardNumberTR").toggleClass('displayNone', false);
        toggleSubscriptionValidationRules(true);
    }
    else if (hideShow === 'hide') {
        $("#expirationDateTR").toggleClass('displayNone', true);
        $("#securityCodeTR").toggleClass('displayNone', true);
        $("#cardNumberTR").toggleClass('displayNone', true);
        toggleSubscriptionValidationRules(false);
    }
}

function RemoveGroupFromLeague(gId) {
    if (confirm('Really Remove Group?')) {
        $.getJSON("/league/RemoveGroupFromLeague", { groupId: gId }, function (result) {
            if (result.isSuccess === true) {
                window.location.href = window.location.href;
            }
        });
    }
}

function AddCategoryToForum(button) {
    var forumId = $("#ForumId").val();
    var group = $("#GroupId").val();
    var groupN = $("#GroupName").val();
    var cat = $("#NewCategory").val();
    $.getJSON("/forum/AddCategoryToForum", { forumId: forumId, categoryName: cat, groupId: group }, function (result) {
        if (result.isSuccess === true) {
        }
    });
    $('#categories > tbody:last').append('<tr><td></td><td>' + cat + '</td><td>' + groupN + '</td><td></td></tr>');
    $("#NewCategory").val("");
}
function UpdateCategoryToForum(button, categoryId) {
    var forumId = $("#ForumId").val();
    var group = $("#GroupId").val();
    var newCategory = $("#catName-" + categoryId).val();
    $.getJSON("/forum/UpdateCategoryToForum", { forumId: forumId, catagoryId: categoryId, categoryName: newCategory, groupId: group }, function (result) {
        if (result.isSuccess === true) {
        }
    });
    $(button).parent().html("Updated");
}
function DeleteCategoryToForum(button, categoryId) {
    categoryId = $.trim(categoryId);
    if (confirm('Really Remove Category?')) {
        var forumId = $("#ForumId").val();
        $.getJSON("/forum/DeleteCategoryToForum", { forumId: forumId, catagoryId: categoryId }, function (result) {
            if (result.isSuccess === true) {
            }
        });
        $(button).parent().parent().remove();
    }
}
function BroadcastMessageChange(checkBox, forumId, groupId) {
    var isChecked = $(checkBox).is(":checked");
    $.getJSON("/forum/BroadcastMessageChange", { forumId: forumId, groupId: groupId, change: isChecked }, function (result) {
        if (result.isSuccess === true) {
        }
    });
}

function RemovePostOfForumTopic(link, messageId) {
    var forumId = $("#ForumId").val();
    var topicId = $("#TopicId").val();
    $.getJSON("/forum/RemovePost", { forumId: forumId, topicId: topicId, postId: messageId }, function (result) {
        if (result.isSuccess === true) {
        }
    });
    $(link).parent().parent().parent().remove();
}
function PinForumTopic(link, topicId, pin) {
    var forumId = $("#ForumId").val();
    $.getJSON("/forum/PinForumTopic", { forumId: forumId, topicId: topicId, pin: pin }, function (result) {
        if (result.isSuccess === true) {
            if (pin == "true") {
                $(link).html("<i class='fa fa-times-circle'></i>");
                $(link).attr('onclick', "javascript:PinForumTopic(this,'" + topicId + "', 'false')");
                $(link).attr('data-original-title', "UnPin");
                $("#forum-title-" + topicId).append('<i id="pinned-icon-' + topicId + '" class="fa fa-check-circle text-muted" data-toggle="tooltip" data-placement="top" title="Topic is Pinned"></i>');
                $("#pinned-icon-" + topicId).tooltip('show');
            }
            else
            {
                $(link).html("<i class='fa fa-check-circle'></i>");
                $(link).attr('data-original-title', "Pin");
                $(link).attr('onclick', "javascript:PinForumTopic(this,'" + topicId + "', 'true')");
                $("#pinned-icon-" + +topicId).remove();
                $(".tooltip").fadeOut();
            }
        }
    });
}
function LockForumTopic(link, topicId, loc) {
    var forumId = $("#ForumId").val();
    $.getJSON("/forum/LockForumTopic", { forumId: forumId, topicId: topicId, lockTopic: loc }, function (result) {
        if (result.isSuccess === true) {
            $(link).remove();
        }
    });
}

var Forum = new function () {
    var thisViewModel = this;
    thisViewModel.isArchived = false;
    thisViewModel.groupId = 0;
    thisViewModel.category = 0;
    thisViewModel.currentPage = 0;
    thisViewModel.forumId = "";
    thisViewModel.forumType = "";
    thisViewModel.IsScrollingAllowed = true;
    thisViewModel.IsGettingScrolledContent = false;

    this.Initialize = function (forumId, forumType) {
        thisViewModel.forumId = forumId;
        thisViewModel.forumType = forumType;
    }

    this.searchForumPosts = function (searchbox) {
        var qu = $(searchbox).val();
        var group = $("#currentGroupId").val();
        if (qu.length === 1)
            return;
        $("#header-loading").show();
        var tableBody = $("#forumbody");
        $.getJSON("/forum/SearchForumPosts", { q: qu, limit: 50, groupId: group, forumId: thisViewModel.forumId, forumType: thisViewModel.forumType }, function (result) {
            tableBody.html("");
            $.each(result, function (i, item) {
                Forum.DisplayForumRow(result, tableBody, item);
            });
            $("#header-loading").hide();
            $('[data-toggle="tooltip"]').tooltip();
        });
    }
    this.DisplayForumRow = function (result, tableBody, item) {
        var row = $(document.createElement('tr'));
        if (item.IsRead !== true)
            row.addClass("forum-read");

        var firstColumn = $(document.createElement('td'));
        firstColumn.attr('id', "forum-title-" + item.TopicId);
        var forumLink = $(document.createElement('a'));
        if (result.IsRead === false)
            forumLink.addClass("b");
        forumLink.attr({ href: leagueHost + "forum/post/view/" + item.ForumId.replace(/-/g, "") + "/" + item.TopicId + "/" + item.TopicTitleForUrl });
        forumLink.html(item.TopicTitle);
        firstColumn.append(forumLink);
        firstColumn.append('<br/>');
        if (item.IsLocked === true)
            firstColumn.append('<i class="fa fa-lock text-muted" data-toggle="tooltip" data-placement="top" title="Topic is Locked"></i>');
        if (item.IsPinned === true)
            firstColumn.append(' <i id="pinned-icon-' + item.TopicId + '" class="fa fa-check-circle text-muted" data-toggle="tooltip" data-placement="top" title="Topic is Pinned"></i>');
        row.append(firstColumn);

        var catColumn = $(document.createElement('td'));

        var catLink = $(document.createElement('span'));
        catLink.attr({ onclick: "Forum.changeForumCategoryLink('" + item.GroupId + "', '" + item.CategoryId + "')" });
        catLink.html(item.Category);
        catLink.addClass("spanLink");
        catColumn.append(catLink);
        row.append(catColumn);

        var thirdColumn = $(document.createElement('td'));
        var memberLink = $(document.createElement('a'));
        if (item.ForumOwnerTypeEnum === "league")
            memberLink.attr({ href: leagueHost + "member/" + item.CreatedByMember.MemberId.replace(/-/g, "") + "/" + item.CreatedByMember.DerbyNameUrl });
        else
            memberLink.attr({ href: rdnationHost + "roller-derby-skater/" + item.CreatedByMember.DerbyNameUrl + "/" + item.CreatedByMember.MemberId.replace(/-/g, "") });
        memberLink.html(item.CreatedByMember.DerbyName);
        thirdColumn.append(memberLink);
        var ent = $(document.createElement('br'));
        thirdColumn.append(ent);
        thirdColumn.append('<span class="text-muted">' + item.CreatedHuman + '</span>');
        row.append(thirdColumn);

        var fourthColumn = $(document.createElement('td'));
        fourthColumn.append(item.Replies);
        row.append(fourthColumn);

        var fifthColumn = $(document.createElement('td'));
        fifthColumn.append(item.ViewCount);
        row.append(fifthColumn);
        tableBody.append(row);

        var sixColumn = $(document.createElement('td'));
        var memberByLink = $(document.createElement('a'));
        if (item.ForumOwnerTypeEnum === "league")
            memberLink.attr({ href: leagueHost + "member/" + item.LastPostByMember.MemberId.replace(/-/g, "") + "/" + item.LastPostByMember.DerbyNameUrl });
        else
            memberLink.attr({ href: rdnationHost + "roller-derby-skater/" + item.LastPostByMember.DerbyNameUrl + "/" + item.LastPostByMember.MemberId.replace(/-/g, "") });
        memberByLink.html(item.LastPostByMember.DerbyName);
        sixColumn.append(memberByLink);
        var entt = $(document.createElement('br'));
        sixColumn.append(entt);
        sixColumn.append('<span class="text-muted">' + item.LastPostHuman + '</span>');
        row.append(sixColumn);

        var sevenColumn = $(document.createElement('td'));
        sevenColumn.addClass('vertical-middle');
        if (item.IsManagerOfTopic) {

            sevenColumn.append('<a class="btn btn-default btn-sm" data-toggle="tooltip" data-placement="top" title="Move"  href="https://' + document.domain + '/forum/post/move/' + item.ForumId.replace(/-/g, "") + '/' + item.TopicId + '"><i class="fa fa-arrows"></i></a>');
            if (item.IsRead === false)
                sevenColumn.append('&nbsp;<button class="btn btn-default btn-sm" data-toggle="tooltip" data-placement="top"  title="Mark As Read"  onclick="javascript:MarkForumTopicAsRead(this, \'' + item.TopicId + '\')"> <i class="fa fa-check-square-o"></i></button>');

            if (item.IsPinned)
                sevenColumn.append('&nbsp;<button class="btn btn-default btn-sm" type="button" data-toggle="tooltip" data-placement="top" title="UnPin" onclick="javascript:PinForumTopic(this, \'' + item.TopicId + '\', \'' + false + '\')"><i class="fa fa-times-circle"></i></button>');
            else
                sevenColumn.append('&nbsp;<button class="btn btn-default btn-sm" type="button" data-toggle="tooltip" data-placement="top" title="Pin" onclick="javascript:PinForumTopic(this, \'' + item.TopicId + '\', \'' + true + '\')"><i class="fa fa-check-circle"></i></button>');

            if (item.IsLocked)
                sevenColumn.append('&nbsp;<button class="btn btn-default btn-sm" type="button" data-toggle="tooltip" data-placement="top" title="Unlock" onclick="javascript:LockForumTopic(this, \'' + item.TopicId + '\', \'' + false + '\')"><i class="fa fa-unlock"></i></button>');
            else
                sevenColumn.append('&nbsp;<button class="btn btn-default btn-sm" type="button" data-toggle="tooltip" data-placement="top" title="Lock" onclick="javascript:LockForumTopic(this, \'' + item.TopicId + '\', \'' + true + '\')"><i class="fa fa-lock"></i></button>');

            if (item.IsArchived)
                sevenColumn.append('&nbsp;<button class="btn btn-default btn-sm" type="button" data-toggle="tooltip" data-placement="top" title="UnArchive" onclick="javascript:Forum.ArchiveForumTopic(this, \'' + item.TopicId + '\', \'' + false + '\')"><i class="fa fa-folder-o"></i></button>');
            else
                sevenColumn.append('&nbsp;<button class="btn btn-default btn-sm" type="button" data-toggle="tooltip" data-placement="top" title="Archive" onclick="javascript:Forum.ArchiveForumTopic(this, \'' + item.TopicId + '\', \'' + true + '\')"><i class="fa fa-archive"></i></button>');

            sevenColumn.append('&nbsp;<button class="btn btn-default btn-sm" type="button" data-toggle="tooltip" data-placement="top" title="Delete" onclick="javascript:Forum.RemoveForumTopic(this, \'' + item.TopicId + '\')"><i class="fa fa-trash"></i></button>');
        }
        row.append(sevenColumn);
    }


    this.ArchiveForumTopic = function (link, topicId, loc) {
        thisViewModel.IsScrollingAllowed = true;
        $("#noMoreTopics").toggleClass("displayNone", true);
        var forumId = $("#ForumId").val();
        $.getJSON("/forum/ArchiveForumTopic", { forumId: thisViewModel.forumId, topicId: topicId, lockTopic: loc }, function (result) {
            if (result.isSuccess === true) {
                $(link).parent().parent().remove();
            }
        });
    }

    this.ScrolledToBottomOfPosts = function () {
        if (thisViewModel.IsScrollingAllowed) {
            if (thisViewModel.IsGettingScrolledContent === false) {
                thisViewModel.currentPage += 1;
                thisViewModel.IsGettingScrolledContent = true;
                $("#loadingMoreTopics").toggleClass("displayNone", false);
                $("#loading").toggleClass("displayNone", false);
                var tableBody = $("#forumbody");
                $.getJSON("/forum/GetForumPosts", { groupId: thisViewModel.groupId, forumId: thisViewModel.forumId, page: thisViewModel.currentPage, isArchived: thisViewModel.isArchived, pageCount: "20", forumType: thisViewModel.forumType }, function (result) {
                    console.log(result.Topics.length);
                    if (result.Topics.length > 0) {
                        $.each(result.Topics, function (i, item) {
                            thisViewModel.DisplayForumRow(result.Topics, tableBody, item);
                        });
                    }
                    else {
                        thisViewModel.IsScrollingAllowed = false;
                        $("#noMoreTopics").toggleClass("displayNone", false);
                    }
                    $("#loadingMoreTopics").toggleClass("displayNone", true);
                    $("#loading").toggleClass("displayNone", true);
                    thisViewModel.IsGettingScrolledContent = false;
                });
            }
        }
    }
    this.getArchivedForumGroup = function (link, gId) {
        thisViewModel.IsScrollingAllowed = true;
        $("#noMoreTopics").toggleClass("displayNone", true);
        thisViewModel.currentPage = 0;
        thisViewModel.isArchived = true;
        thisViewModel.groupId = gId;
        $("#loading").toggleClass("displayNone", false);
        isArchived = true;
        thisViewModel.pullForumMessages(gId, true);

    }
    this.pullForumMessages = function (gId, isArchived) {
        thisViewModel.groupId = gId;
        var tableBody = $("#forumbody");
        var categoriesBody = $("#forumCategories");
        $("#header-loading").show();
        $.getJSON("/forum/GetForumPosts", { groupId: thisViewModel.groupId, forumId: thisViewModel.forumId, page: thisViewModel.currentPage, isArchived: isArchived, pageCount: "100", forumType: thisViewModel.forumType }, function (result) {
            tableBody.html("");
            categoriesBody.html("");
            $("#forumCategories").append($("<option></option>").val("").html("Select Category"));

            $.each(result.Categories, function (i, item) {
                $("#forumCategories").append($("<option></option>").val(item.CategoryId).html(item.CategoryName).attr({ onclick: "Forum.changeForumCategories(this)" }));
            });
            $.each(result.Topics, function (i, item) {
                thisViewModel.DisplayForumRow(result.Topics, tableBody, item);
            });
            if (result.IsManager)
                $("#postSettings").toggleClass("displayNone", false);
            else
                $("#postSettings").toggleClass("displayNone", true);
            $("#header-loading").hide();

        });
    }
    this.changeForumGroup = function (link, gId) {

        thisViewModel.isArchived = false;
        thisViewModel.groupId = gId;
        thisViewModel.IsScrollingAllowed = true;
        $("#noMoreTopics").toggleClass("displayNone", true);
        thisViewModel.currentPage = 0;
        $(link).parent().children().each(function () {
            $(this).toggleClass("active", false);
        });
        $(link).toggleClass("active", true);
        $("#loading").toggleClass("display-none", false);
        $("#currentGroupId").val(gId);
        $("#newPost").attr("href", "/forum/new/" + $("#type").val() + "/" + thisViewModel.forumId + "/" + gId);
        $("#postSettingLink").attr("href", "/forum/settings/" + $("#ForumId").val() + "/" + gId);
        $("#archivedBtn").attr("onclick", "Forum.getArchivedForumGroup(this, '" + gId + "')");

        thisViewModel.pullForumMessages(gId, false);
    }
    this.changeForumCategory = function (gId, catId) {
        thisViewModel.groupId = gId;
        thisViewModel.category = catId;
        $("#currentGroupId").val(gId);
        var tableBody = $("#forumbody");
        thisViewModel.IsScrollingAllowed = true;
        $("#noMoreTopics").toggleClass("displayNone", true);
        thisViewModel.currentPage = 0;
        $("#header-loading").show();
        $.getJSON("/forum/GetForumPostsCat", { groupId: gId, catId: catId, forumId: thisViewModel.forumId, page: thisViewModel.currentPage, forumType: thisViewModel.forumType }, function (result) {
            tableBody.html("");
            $.each(result, function (i, item) {
                thisViewModel.DisplayForumRow(result, tableBody, item);
            });
            $("#header-loading").hide();
        });
    }
    this.changeForumCategories = function (select) {
        var catId = $(select).find(":selected").val();
        thisViewModel.changeForumCategory(thisViewModel.groupId, catId);
    }
    this.changeForumCategoryLink = function (gId, catId) {
        $("#forumCategories").children("li").each(function () {
            $(this).toggleClass("forumCatPagerSel", false);
        });
        thisViewModel.changeForumCategory(gId, catId);
    }
    this.RemoveForumTopic = function (link, topicId) {
        if (confirm('Really Remove Topic?')) {
            $.getJSON("/forum/RemoveTopic", { forumId: thisViewModel.forumId, topicId: topicId, forumType: thisViewModel.forumType }, function (result) {
                if (result.isSuccess === true) {
                }
            });
            $(link).parent().parent().remove();
        }
    }

}


function AddMemberToLeagueGroup() {
    var leagueId = $("#League_LeagueId").val();
    var mem = $("#AddMembers option:selected");
    var memId = mem.val();
    if (memId.length > 2) {
        var memName = mem.text();
        var gId = $("#Id").val();
        var memType = $('#1-memType option:selected').val();
        $.getJSON("/league/AddLeagueGroupMember", { groupId: gId, leagueId: leagueId, memberId: memId, memberType: memType }, function (result) {
            if (result.isSuccess === true) {
                $('#groupMembers > tbody > tr:first').after('<tr><td>' + memName + '</td><td>' + memType + '</td><td></td></tr>');
                mem.remove();
            }
        });
        $("#AddMembers").toggleClass("red", false);
    }
    else {
        $("#AddMembers").toggleClass("red", true);
    }

}

function UpdateMemberToLeagueGroup(button, memId, gId) {
    memId = $.trim(memId);
    gId = $.trim(gId);
    var memType = $('#' + memId + '-level option:selected').val();
    var isApartOfGroup = $("#" + memId + "-check").is(":checked");
    $.getJSON("/league/UpdateLeagueGroupMember", { groupId: gId, memberId: memId, memberType: memType, isApartOfG: isApartOfGroup }, function (result) {
        if (result.isSuccess === true) {
        }
    });
    $(button).parent().html("Updated");
}

function toggleSubscriptionValidationRules(onOff) {
    var settings = $('#PaymentForm').validate().settings;
    //on
    if (onOff === true) {
        $.extend(settings, {
            rules: {
                FirstName: "required",
                LastName: "required",
                Address: "required",
                City: "required",
                State: "required",
                ZipCode: "required",
                Country: "required",
                CCNumber: "required",
                SecurityCode: "required",
                MonthOfExpiration: "required",
                YearOfExpiration: "required",
                EmailAddress: "required",
                PhoneNumber: "required"
            },
            submitHandler: function (form) {
                // disable the submit button to prevent repeated clicks
                $('#submitButton1').toggleClass("displayNone", true);
                $('#submitButton').toggleClass("displayNone", true);
                $('#working').toggleClass("displayNone", false);
                $('#working1').toggleClass("displayNone", false);
                Stripe.createToken({
                    number: $('.card-number').val(),
                    cvc: $('.card-cvc').val(),
                    exp_month: $('.card-expiry-month').val(),
                    exp_year: $('.card-expiry-year').val(),
                    name: $('#FirstName').val() + " " + $('#LastName').val(),
                    address_line1: $('#Address').val(),
                    address_city: $('#City').val(),
                    address_state: $('#State').val(),
                    address_zip: $('#ZipCode').val(),
                    address_country: $("#Country option:selected").text()
                }, stripeResponseHandler);
            }
        });
    }
    else if (onOff === false) {
        $.extend(settings, {
            rules: {
                FirstName: "required",
                LastName: "required",
                Address: "required",
                City: "required",
                State: "required",
                ZipCode: "required",
                Country: "required",
                EmailAddress: "required",
                PhoneNumber: "required"
            },
            submitHandler: function (form) {
                // disable the submit button to prevent repeated clicks
                $('#submitButton1').toggleClass("displayNone", true);
                $('#submitButton').toggleClass("displayNone", true);
                $('#working').toggleClass("displayNone", false);
                $('#working1').toggleClass("displayNone", false);
                var form$ = $("#PaymentForm");
                // and submit
                form$.get(0).submit();
            }
        });
    }
}

function stripeResponseHandler(status, response) {
    if (response.error) {
        // show the errors on the form
        $(".paymentErrors").html(response.error.message);
        $('#submitButton1').toggleClass("displayNone", false);
        $('#submitButton').toggleClass("displayNone", false);
        $('#working').toggleClass("displayNone", true);
        $('#working1').toggleClass("displayNone", true);
    } else {
        var form$ = $("#PaymentForm");
        // token contains id, last4, and card type
        var token = response['id'];
        // insert the token into the form so it gets submitted to the server
        form$.append("<input type='hidden' name='stripeToken' value='" + token + "'/>");
        // and submit
        form$.get(0).submit();
    }
}

function selectSubOption(div, price) {
    var parentOfDiv = $(div).parent();
    $(':radio').each(function () {
        $(this).parent().toggleClass("subOptionSel", false);
    });
    $(div).toggleClass("subOptionSel");
    $('.dueAmount').text(price);
}

function GetCurrentDateTime() {
    var d = new Date();
    var curr_date = d.getDate();

    var curr_month = d.getMonth() + 1; //Months are zero based
    if (curr_month < 10) {
        curr_month = "0" + curr_month;
    }
    var curr_year = d.getFullYear();
    var curr_hour = d.getHours();
    var curr_minutes = d.getMinutes();
    if (curr_minutes < 10) {
        curr_minutes = "0" + curr_minutes;
    }
    var a_p = "";
    if (curr_hour < 12) {
        a_p = "am";
    }
    else {
        a_p = "pm";
    }
    if (curr_hour == 0) {
        curr_hour = 12;
    }
    if (curr_hour > 12) {
        curr_hour = curr_hour - 12;
    }
    return curr_month + "/" + curr_date + "/" + curr_year + " " + curr_hour + ":" + curr_minutes + " " + a_p;
}

function SendEmailInviteToRDNation(memberId, leagueId) {
    memberId = $.trim(memberId);
    leagueId = $.trim(leagueId);
    var to = $("#" + memberId + "-invite");
    var sent = $("#" + memberId + "-connected");
    $.getJSON("/league/SendEmailInviteToMemberAgain", { leagueName: leagueId, memberId: memberId }, function (result) {
        if (result.isSuccess === true) {
            sent.html("Verfication Link: " + result.emailSent);
        }
        else {
            sent.html("No Email For Member");
        }
    });
    to.remove();
}

function GetInternalMessageHistory() {
    var to = $("#OwnerUserId").val();
    var id = $("#lastMessageId").val();
    var grp = $("#GroupMessageId").val();
    $.getJSON("/message/GetConversationUpdates", { groupId: grp, ownerUserId: to, lastMessageId: id }, function (result) {
        if (result.isSuccess === true) {
            $.each(result.message, function (i, field) {
                if (to != field.FromId) {
                    var message = field.MessageText.replace(/\n\r?/g, '<br />');
                    var newMess = "";
                    if (field.FromId != lastMessageNameId) {
                        newMess += '<span class="messName">' + field.FromName + '</span>';
                        newMess += '<span class="messCreated">' + GetCurrentDateTime() + '</span>';
                    }
                    newMess += '<div class="messText clear">' + message + '</div>';
                    if (field.FromId != lastMessageNameId) {
                        newMess += "<br/>";
                    }
                    $("#messagesAdd").append(newMess);
                    var objDiv = document.getElementById("messageView");
                    objDiv.scrollTop = objDiv.scrollHeight;
                }

                lastMessageNameId = field.FromId;
                id = field.MessageId;
            });
            $("#lastMessageId").val(id);
        }
    });
}

var lastMessageNameId;
function PostInternalMessage() {
    var message = $("#inputNewMessage").val();
    var fromName = $("#FromName").val();
    var to = $("#OwnerUserId").val();
    var grp = $("#GroupMessageId").val();
    if (message.length > 0) {
        $.getJSON("/message/PostMessage", { groupId: grp, ownerUserId: to, mess: message });
        $("#inputNewMessage").val("");
    }
    $("#inputNewMessage").focus();
    message = message.replace(/\n\r?/g, '<br />');
    var newMess = "";
    if (lastMessageNameId != to) {
        newMess += '<span class="messName">' + fromName + '</span>';
        newMess += '<span class="messCreated">' + GetCurrentDateTime() + '</span>';
    }
    newMess += '<span class="messText clear">' + message + '</span>';

    $("#messagesAdd").append(newMess);
    var objDiv = document.getElementById("messageView");
    objDiv.scrollTop = objDiv.scrollHeight;
    lastMessageNameId = to;
}

var dict = [];
function ChangeDictionaryItem(checkbox, id, displayName) {
    var memNames = $("# ToMemberNamesSelected");
    var memIds = $("#ToMemberIds");
    var checked = $(checkbox).is(":checked");
    if (checked) {
        var member = { name: displayName, idd: id };
        dict.push(member);
    }
    else {
        var member = { name: displayName, idd: id };
        dict = jQuery.grep(dict, function (value) {
            return value.idd != member.idd;
        });
    }
    var text = "";
    var ids = "";
    $.each(dict.reverse(), function (i, val) {
        text += '<span class="label label-primary">' + val.name + '</span> ';
        ids += val.idd + ",";
    });
    if (document.getElementById('ToMemberNamesSelected') !== null)
        document.getElementById('ToMemberNamesSelected').innerHTML = text;
    memIds.val(ids);
}

function toggleCheckedForRecipients(checkbox) {
    var memNames = $("#ToMemberNamesSelected");
    var memIds = $("#ToMemberIds");
    //var checked = $(checkbox).attr("checked");
    var isChecked = $(checkbox).is(":checked");
    dict.length = 0;
    var text = "";
    var ids = "";
    $("#checkboxes input:checkbox").each(function () {
        $(this).prop('checked', isChecked);
        if (isChecked) {
            var cbId = $(this).attr("name");
            var derbyName = $(this).attr("derbyname");
            dict.push({ name: derbyName, idd: cbId });
            text += '<span class="label label-primary">' + derbyName + '</span> ';
            ids += cbId + ",";
        }
    });
    if (document.getElementById('ToMemberNamesSelected') !== null)
        document.getElementById('ToMemberNamesSelected').innerHTML = text;
    memIds.val(ids);
}

function TypeOfStoreItemSelected(dropDown) {
    var text = $('#ItemType option:selected').text();
    $("#shirtSize").toggleClass("displayNone", true);
    $("#colors").toggleClass("displayNone", true);
    if (text === "Shirt") {
        $("#shirtSize").toggleClass("displayNone", false);
    }
    else if (text === "Decal") {
        $("#colors").toggleClass("displayNone", false);
    }

}

function EventTypeChanged(dropDown) {
    var text = $('#SelectedEventTypeId option:selected').val();
    if (text != "") {
        $("#addNewEventTypeText").toggleClass("displayNone", false);
        $("#addNewEventTypeLink").toggleClass("displayNone", true);

        $.getJSON("/Calendar/GetEventTypeValues", { eventTypeId: text }, function (result) {
            if (result.isSuccess === true) {
                $("#presentType").text(result.type.PointsForPresent);
                $("#partialType").text(result.type.PointsForPartial);
                $("#excusedType").text(result.type.PointsForExcused);
                $("#notPresentType").text(result.type.PointsForNotPresent);
                $("#tardyType").text(result.type.PointsForTardy);
                $("#ColorTempSelected").find('option[value="' + result.type.ColorTempSelected + '"]').attr('selected', 'selected');
            } else {
            }
        }).error(function () {
            $.getJSON("/Calendar/GetEventTypeValues", { eventTypeId: text }, function (result) {
                if (result.isSuccess === true) {
                    $("#presentType").text(result.type.PointsForPresent);
                    $("#partialType").text(result.type.PointsForPartial);
                    $("#excusedType").text(result.type.PointsForExcused);
                    $("#notPresentType").text(result.type.PointsForNotPresent);
                    $("#tardyType").text(result.type.PointsForTardy);
                    $("#ColorTempSelected").find('option[value="' + result.type.ColorTempSelected + '"]').attr('selected', 'selected');
                } else {
                }
            })
        });
    }
    else {
        $("#addNewEventTypeText").toggleClass("displayNone", true);
        $("#addNewEventTypeLink").toggleClass("displayNone", false);
    }
}

function PublicEventChangeCalendar(checkBox) {
    var isChecked = $(checkBox).is(":checked");

}

function ChangeSummaryOfRepeatedEvent(removeSummary) {
    var sum = "";
    var freqencyDrop = $("#RepeatsFrequencySelectedId option:selected").text();
    sum += freqencyDrop;

    if (freqencyDrop === "Daily") {
    } else if (freqencyDrop === "Weekly" || freqencyDrop === "Monthly") {
        sum += " on ";
        if (freqencyDrop === "Monthly") {
            sum += "the " + $("#MonthlyIntervalId option:selected").text() + " ";
        }
        var alreadyChecked = false;
        if ($("#IsSunday").is(':checked')) {
            alreadyChecked = true;
            sum += "Sunday";
        }
        if ($("#IsMonday").is(':checked')) {
            if (alreadyChecked)
                sum += ", ";
            alreadyChecked = true;
            sum += "Monday";
        }
        if ($("#IsTuesday").is(':checked')) {
            if (alreadyChecked)
                sum += ", ";
            alreadyChecked = true;
            sum += "Tuesday";
        }
        if ($("#IsWednesday").is(':checked')) {
            if (alreadyChecked)
                sum += ", ";
            alreadyChecked = true;
            sum += "Wednesday";
        }
        if ($("#IsThursday").is(':checked')) {
            if (alreadyChecked)
                sum += ", ";
            alreadyChecked = true;
            sum += "Thursday";
        }
        if ($("#IsFriday").is(':checked')) {
            if (alreadyChecked)
                sum += ", ";
            alreadyChecked = true;
            sum += "Friday";
        }
        if ($("#IsSaturday").is(':checked')) {
            if (alreadyChecked)
                sum += ", ";
            alreadyChecked = true;
            sum += "Saturday";
        }
        //removes the error warning.
        if (alreadyChecked)
            $("#selectDate").toggleClass("displayNone", true);
    }

    if ($("input[name='EndsWhen']:checked").val()) {
        var when = $("input[name='EndsWhen']:checked").val();
        if (when != "Never") {
            if (when === "After")
                sum += ", " + $("#EndsOccurences").val() + " times";
            else if (when === "On")
                sum += ", until " + $("#EndsDate").val();
        }
    }

    if (removeSummary === false)
        sum = "";

    $("#summaryEvent").text(sum);
    $("#summaryPopUp").text(sum);

}

function RepeatsFrequencyEventChange(dropDown) {
    var text = $('#RepeatsFrequencySelectedId option:selected').text();
    if (text === "Daily" || text === "Monthly") {
        $("#repeatsOnRow").toggleClass("displayNone", true);
        $("#repeatsOnRowSel").toggleClass("displayNone", true);
        $("#repeatsOnRowSelMonthly").toggleClass("displayNone", true);
        if (text === "Daily") {
            $("#repeatEveryText").text("Days");
        } else if (text === "Monthly") {
            $("#repeatsOnRow").toggleClass("displayNone", false);
            $("#repeatsOnRowSel").toggleClass("displayNone", false);
            $("#repeatsOnRowSelMonthly").toggleClass("displayNone", false);
            $("#repeatEveryText").text("Months");
        }
    } else if (text === "Weekly") {
        $("#repeatsOnRowSelMonthly").toggleClass("displayNone", true);
        $("#repeatsOnRow").toggleClass("displayNone", false);
        $("#repeatsOnRowSel").toggleClass("displayNone", false);
        $("#repeatEveryText").text("Weeks");
    }
    ChangeSummaryOfRepeatedEvent(true);
}

function ToggleReoccuringEventPopup(checkBox) {
    var isChecked = $(checkBox).is(':checked');

    if (isChecked) {
        $('#myModal').modal('show');
    }
    else {
        $('#myModal').modal('hide');
    }

}

function IsReoccuringEvent(checkBox) {
    var isChecked = $(checkBox).is(':checked');

    if (isChecked) {
        $("#createtrackSubmit").toggleClass("displayNone", true);
        ChangeSummaryOfRepeatedEvent(true);
    }
    else {
        $("#createtrackSubmit").toggleClass("displayNone", false);
        ChangeSummaryOfRepeatedEvent(false);
    }

}
function CloseReOccuringEventWindow(isDone) {
    //we pressed the cancel button.
    if (!isDone) {
        $("#IsReoccurring").removeAttr('checked');
        $("#createtrackSubmit").toggleClass("displayNone", false);
        ChangeSummaryOfRepeatedEvent(false);
    } else {
        $("#createtrackSubmit").toggleClass("displayNone", true);
        var freqencyDrop = $("#RepeatsFrequencySelectedId option:selected").text();
        if (freqencyDrop === "Weekly") {
            var sun = $("#IsSunday");
            var mon = $("#IsMonday");
            var tues = $("#IsTuesday");
            var wed = $("#IsWednesday");
            var thur = $("#IsThursday");
            var fri = $("#IsFriday");
            var sat = $("#IsSaturday");
            if (!sun.is(':checked') && !mon.is(':checked') && !tues.is(':checked') && !wed.is(':checked') && !thur.is(':checked') && !fri.is(':checked') && !sat.is(':checked')) {
                $("#selectDate").toggleClass("displayNone", false);
                return;
            }
        }
    }
    $("#selectDate").toggleClass("displayNone", true);
    //cheks for a real date if reoccurr ends on specified date.
    var reccurringEnds = $('input[name=EndsWhen]:checked').val();
    var endsDate = $("#EndsDate");
    if (reccurringEnds === 'On') {
        var d = new Date(endsDate.val());
        //no date was entered
        if (!isValidDate(d)) {
            endsDate.addClass("error");
            return;
        }
    }
    endsDate.toggleClass("error", false);
    $("#eventRepeatsPopUp").fadeOut("fast");
}

function isValidDate(d) {
    if (Object.prototype.toString.call(d) !== "[object Date]")
        return false;
    return !isNaN(d.getTime());
}

function DeleteStorePhoto(photoId, storeItemId, mId) {
    $.getJSON("/Store/DeleteStoreItemPicture", { pictureId: photoId, storeItemId: storeItemId, mId: mId }, function (result) {
        if (result.isSuccess === true) {
        } else {
        }
    }).error(function () {
        $.getJSON("/Store/DeleteStoreItemPicture", { pictureId: photoId, storeItemId: storeItemId, mId: mId }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        })
    });
    var parent = $("#image-" + photoId).parent();
    parent.html('<input type="file" name="file-' + photoId + '" id="file-' + photoId + '" class="fileUpload" onchange="ValidatePhoto(this)" />');
    $("#image-" + photoId).remove();
    $("#button-" + photoId).remove();
}

function RemoveDuesPayment(idOfCollection, memberId, duesId, duesItemId) {
    $.getJSON("/Dues/RemoveDuesPayment", { duesItemId: duesItemId, duesManagementId: duesId, memId: memberId, duesCollectedId: idOfCollection }, function (result) {
        if (result.isSuccess === true) {
        } else {
        }
    }).error(function () {
        $.getJSON("/Dues/RemoveDuesPayment", { duesItemId: duesItemId, duesManagementId: duesId, memId: memberId, duesCollectedId: idOfCollection }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        })
    });
    $("#" + idOfCollection + "-row").toggleClass("displayNone");
}
function SendReminderToPayDuesWithstanding(button, memId) {
    memId = $.trim(memId);
    var duesManagementId = $("#DuesId").val();
    var leagueIdd = $("#LeagueOwnerId").val();
    $.getJSON("/Dues/SendEmailReminderWithstanding", { duesManagementId: duesManagementId, memId: memId, leagueId: leagueIdd }, function (result) {
        if (result.isSuccess === true) {
        } else {
        }
    }).error(function () {
        $.getJSON("/Dues/SendEmailReminderWithstanding", { duesManagementId: duesManagementId, memId: memId, leagueId: leagueIdd }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        })
    });
    $(button).parent().text("Sent Email");
}

function SendReminderToPayDues(button, memId) {
    memId = $.trim(memId);
    var duesManagementId = $("#DuesId").val();
    var DuesItemId = $("#DuesItemId").val();
    var leagueIdd = $("#LeagueOwnerId").val();
    $.getJSON("/Dues/SendEmailReminder", { duesItemId: DuesItemId, duesManagementId: duesManagementId, memId: memId, leagueId: leagueIdd }, function (result) {
        if (result.isSuccess === true) {
        } else {
        }
    }).error(function () {
        $.getJSON("/Dues/SendEmailReminder", { duesItemId: DuesItemId, duesManagementId: duesManagementId, memId: memId, leagueId: leagueIdd }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        })
    });
    $(button).parent().text("Sent Email");
}
function SendReminderToPayDuesAll(button) {
    var duesManagementId = $("#DuesId").val();
    var DuesItemId = $("#DuesItemId").val();
    $.getJSON("/Dues/SendEmailReminderAll", { duesItemId: DuesItemId, duesManagementId: duesManagementId }, function (result) {
        if (result.isSuccess === true) {
        } else {
        }
    }).error(function () {
        $.getJSON("/Dues/SendEmailReminderAll", { duesItemId: DuesItemId, duesManagementId: duesManagementId }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        })
    });
    $(button).parent().text("Sent Emails");
}

function WaiveDues(button, memId) {
    memId = $.trim(memId);
    var amountButton = $("#" + memId + "-Dues-Amount");
    var sendReminderButton = $("#" + memId + "-button-email");
    var duesManagementId = $("#DuesId").val();
    var DuesItemId = $("#DuesItemId").val();
    var due = $("#" + memId + "-Due");
    var collected = $("#" + memId + "-Collected");
    var paidButton = $("#" + memId + "-button");
    var amount = amountButton.val();

    $.getJSON("/Dues/WaiveDuesAmount", { duesId: DuesItemId, duesManagementId: duesManagementId, memberId: memId, note: "" }, function (result) {
        if (result.isSuccess === true) {
        } else {
        }
    }).error(function () {
        $.getJSON("/Dues/WaiveDuesAmount", { duesId: DuesItemId, duesManagementId: duesManagementId, memberId: memId, note: "" }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        })
    });

    amountButton.remove();
    $(button).remove();
    sendReminderButton.remove();
    paidButton.remove();

    collected.text("Waived");
    due.text(parseFloat('0.00').toFixed(2));
}

function PayDues(button, memId) {
    memId = $.trim(memId);
    var amountButton = $("#" + memId + "-Dues-Amount");
    var sendReminderButton = $("#" + memId + "-button-email");
    var duesManagementId = $("#DuesId").val();
    var DuesItemId = $("#DuesItemId").val();
    var due = $("#" + memId + "-Due");
    var collected = $("#" + memId + "-Collected");
    var waiveButton = $("#" + memId + "-waive-button");
    var amount = amountButton.val();
    if (parseFloat(amount) > parseFloat(0)) {
        $.getJSON("/Dues/PayDuesAmount", { duesId: DuesItemId, duesManagementId: duesManagementId, amountPaid: amount, memberId: memId, note: "" }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        }).error(function () {
            $.getJSON("/Dues/PayDuesAmount", { duesId: DuesItemId, duesManagementId: duesManagementId, amountPaid: amount, memberId: memId, note: "" }, function (result) {
                if (result.isSuccess === true) {
                } else {
                }
            });
        });

        if ((parseFloat(due.text().replace(/,/g, '')) - parseFloat(amount)) <= parseFloat(0)) {
            amountButton.remove();
            $(button).remove();
            waiveButton.remove();
            sendReminderButton.remove();
        }
        else {
            amountButton.val((parseFloat(due.text().replace(/,/g, '')) - parseFloat(amount)).toFixed(2));
        }
        collected.text((parseFloat(collected.text().replace(/,/g, '')) + parseFloat(amount)).toFixed(2));
        due.text((parseFloat(due.text().replace(/,/g, '')) - parseFloat(amount)).toFixed(2));
    }
}
var Dues = new function () {
    var thisViewModel = this;
    this.RemoveWaivedFee = function (span, memId) {
        memId = $.trim(memId);
        var collected = $("#" + memId + "-Collected");
        var duesManagementId = $("#DuesId").val();
        var DuesItemId = $("#DuesItemId").val();
        var forumId = $("#ForumId").val();
        $.getJSON("/Dues/RemoveDuesWaived", { duesId: DuesItemId, duesManagementId: duesManagementId, memberId: memId }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        }).error(function () {
            $.getJSON("/Dues/RemoveDuesWaived", { duesId: DuesItemId, duesManagementId: duesManagementId, memberId: memId }, function (result) {
                if (result.isSuccess === true) {
                } else {
                }
            })
        });
        collected.text("");
        $(span).remove();
    }
}

function UpdateDuesDueForMem(button, memId) {
    memId = $.trim(memId);
    var amountButton = $("#due-" + memId);
    var duesManagementId = $("#DuesId").val();
    var DuesItemId = $("#DuesItemId").val();
    var amount = amountButton.val();
    if (parseFloat(amount) >= parseFloat(0)) {
        $.getJSON("/Dues/SetDuesAmount", { duesId: DuesItemId, duesManagementId: duesManagementId, amountDue: amount, memberId: memId }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        }).error(function () {

        });
        $(button).parent().append('<img  src="' + leagueHost + '/Content/images/greenCheck.png")" />');
        $(button).remove();
    }
}
function updateEditMemberDuesCost(btn, memberId, duesId, duesItemId) {
    $(btn).html('<i class="fa fa-refresh fa-spin"></i> Update');
    memberId = $.trim(memberId);
    var amountButton = $("#amountDue");
    var amount = amountButton.val();
    if (parseFloat(amount) >= parseFloat(0)) {
        $.getJSON("/Dues/SetDuesAmount", { duesId: duesItemId, duesManagementId: duesId, amountDue: amount, memberId: memberId }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }   
        }).error(function () {

        });
        $(btn).html("<i class='fa fa-check'></i> Updated");
        amountButton.toggleClass("isTextBoxValid", false);
    }
    else
        amountButton.toggleClass("isTextBoxValid", true);

}

function changeDuesClassification(radio, memId, classId) {
    memId = $.trim(memId);
    classId = $.trim(classId);
    var duesId = $("#DuesId").val();
    $.getJSON("/Dues/ChangeDuesClassification", { duesId: duesId, memberId: memId, classification: classId }, function (result) {
        if (result.isSuccess === true) {

        }
        else {

        }
    });

}

function classificationDoesNotPayDues(cb) {
    if ($(cb).is(":checked")) {
        $("#FeeRequired").prop('disabled', true);
        $("#FeeRequired").val("0");
    }
    else {
        $("#FeeRequired").prop('disabled', false);
    }
}
//list of countries
var countries = ko.observableArray();
var Country = function (name, id) {
    this.name = name;
    this.countryId = id;
};

var jamCount = 0;
function onReturnOfJamsInAddGame() {
    //focuses on textbox for the next written data item.
    $("#ScoresTeam1_" + jamCount + "__Points").focus();
    jamCount += 1;
    $('html, body').animate({ scrollTop: $(document).height() }, 'slow');

}

function onReturnOfSkaterAddedToTeam() {
    document.getElementById('hiddenSubmitForPenalties').click();
}

var calendarId;
var eventId;
var addEventPopup = false;
function checkIntoEvent(idOfPopUp, calId, evenId, name) {

    calendarId = calId;
    eventId = evenId;
    $(".popover").hide().remove();
    $('button[data-toggle="popover"]').popover('destroy');
    var popup = $('#checkInPopUp').clone();
    addEventPopup = true;
    $("#" + eventId).popover({ content: popup.html() });
    $("#" + eventId).popover('show');
    
}
function setAvailForEvent(calId, evenId) {
    calendarId = calId;
    eventId = evenId;
    $(".popover").hide().remove();
    $('button[data-toggle="popover"]').popover('destroy');
    var popup = $('#availablePopUp').clone();
    addEventPopup = true;
    $("#" + eventId + "-setAvail").popover({ content: popup.html() });
    $("#" + eventId + "-setAvail").popover('show');
}

function CloseAddedRow() {
    if (addEventPopup === true) {
        $("#calendar-" + eventId).remove();
        $(".popover").hide().remove();
        addEventPopup = false;
        $("#" + eventId + "-setAvail").popover('destroy');
    }
}

function checkInMemberToEvent() {
    var noted = $(".popover-content #notes").val();
    var selectedItem = $(".popover-content #checkInSelection option:selected").val();
    if (selectedItem === "") {
        $('.bottom-right').notify({
            message: { text: 'Please Select Check In Type. ' },
            fadeOut: { enabled: true, delay: 4000 },
            type: "warning"
        }).show();
        return;
    }
    var isTardy = $(".popover-content #IsTardy").is(':checked');

    CloseAddedRow();
    $.getJSON("/Calendar/CheckSelfIntoEvent", { calendarId: calendarId, eventId: eventId, note: noted, eventTypePoints: selectedItem, isTardy: isTardy }, function (result) {
        if (result.isSuccess === true) {
            $("#" + eventId).children('i').toggleClass("fa-square-o", false);
            $("#" + eventId).children('i').toggleClass("fa-check-square", true);
            $('.bottom-right').notify({
                message: { text: 'Checked In! ' },
                fadeOut: { enabled: true, delay: 4000 }
            }).show();
        } else {
            $('.bottom-right').notify({
                message: { text: 'Something Happened, Try again later. ' },
                fadeOut: { enabled: true, delay: 4000 },
                type: "danger"
            }).show();
        }
    }).error(function () {
        $('.bottom-right').notify({
            message: { text: 'Something Happened, Try again later. ' },
            fadeOut: { enabled: true, delay: 4000 },
            type: "danger"
        }).show();
    });
}

function setAvailabilityMemberToEvent() {
    var noted = $(".popover-content #availableNotes").val();
    var selectedItem = $(".popover-content #availableSelection option:selected");
    if (selectedItem.val() === "") {
        $('.bottom-right').notify({
            message: { text: 'Please Select an Available Type. ' },
            fadeOut: { enabled: true, delay: 4000 },
            type: "warning"
        }).show();
        return;
    }
    CloseAddedRow();
    $.getJSON("/Calendar/SetAvailabilityForEvent", { calendarId: calendarId, eventId: eventId, note: noted, eventTypePoints: selectedItem.val() }, function (result) {
        if (result.isSuccess === true) {
            $("#" + eventId + "-setAvail").html("<i class='fa fa-calendar fa-lg'></i>");
            $('.bottom-right').notify({
                message: { text: 'RSVPed! ' },
                fadeOut: { enabled: true, delay: 4000 }
            }).show();
        } else {
            $('.bottom-right').notify({
                message: { text: 'Something Happened, Try again later. ' },
                fadeOut: { enabled: true, delay: 4000 },
                type: "danger"
            }).show();
        }
    }).error(function () {
        $("#" + eventId + "-setAvail").html("<i class='fa fa-calendar-o fa-3x cursor'></i>");
        $('.bottom-right').notify({
            message: { text: 'Something Happened, Try again later. ' },
            fadeOut: { enabled: true, delay: 4000 },
            type: "danger"
        }).show();
    });
}

function SearchLeagueNames(control) {
    $("#loadingLeagueName").toggleClass("displayNone");
    $.getJSON("/Utilities/SearchForLeagueName", { name: control.value }, function (result) {
        $("#loadingLeagueName").toggleClass("displayNone");
        if (result.names.length > 0) {
            $("#signUpFindLeagueNameHeader").html("<div class='green'>Click the Link if this is Your League!</div>");
            $("#signUpFindLeagueName").html("");
            $.each(result.names, function (i, field) {
                var memId = field.LeagueId.replace(/\-/g, '');
                $("#signUpFindLeagueName").append("<div><a href='http://" + window.location.host + "/league/setup/" + memId + "/" + field.Name + "'>" + field.Name + "</a> - " + field.City + ", " + field.Country + "</div>");
            });
        }
    });
}

function AddPageViewToCount(f, t) {
    $.getJSON("/Utilities/AddPostViewToCount", { forumId: f, topicId: t });
}

function validateXmlGameFile() {
    str = document.getElementById('file').value.toUpperCase();
    suffix = ".XML";
    if (!(str.indexOf(suffix, str.length - suffix.length) !== -1)) {
        alert('File type not allowed,\nAllowed file: *.xml');
        document.getElementById('file').value = '';
    }
}

var _validPhotoExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
function ValidatePhoto(handle) {
    var oInput = handle;
    if (oInput.type == "file") {
        var sFileName = oInput.value;
        if (sFileName.length > 0) {
            var blnValid = false;
            for (var j = 0; j < _validPhotoExtensions.length; j++) {
                var sCurExtension = _validPhotoExtensions[j];
                if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                    blnValid = true;
                    break;
                }
            }
            if (!blnValid) {
                alert("Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validPhotoExtensions.join(", "));
                oInput.value = '';
                return false;
            }
        }
    }
    return true;
}

function closePopUp(popUpName) {
    $("#" + popUpName).fadeOut("fast");
}
var Polls = new function () {
    var thisViewModel = this;
    thisViewModel.PollId = "";
    this.Initialize = function (pollId) {
        thisViewModel.PollId = pollId;
    }

    this.AddQuestionToPoll = function () {
        var isMultipleAnswers = $("#multipleOptionsInput").is(":checked");
        var questions = $("#HiddenQuestions");
        var answers = $("#HiddenAnswers");

        var panel = $(document.createElement('div'));
        panel.addClass('panel panel-default');

        var panelHeader = $(document.createElement('div'));
        panelHeader.addClass('panel-heading no-padding padding-5');
        if (isMultipleAnswers === true)
            panelHeader.append("Question - <small class='text-muted'>Multiple Selections</small>")
        else
            panelHeader.append("Question ")
        panelHeader.append("<button class='btn btn-xs btn-default pull-right' onclick='DeleteQuestionForPoll(this)'><i class='fa fa-times '></i></button>");
        panel.append(panelHeader);

        var questionAndAnswersPanel = $(document.createElement('div'));
        questionAndAnswersPanel.addClass('panel-body no-padding padding-top-10 padding-bottom-10');

        var question = $(document.createElement('div'));
        question.addClass("b col-xs-12 margin-bottom-10");
        var questionHtml = "<input type='textbox' class='form-control' value='" + $("#questionInput").val() + "' name='question" + simpleId + "'/>";
        if (isMultipleAnswers === true)
            questionHtml += "<input  type='hidden' value='Multiple' name='questiontype" + simpleId + "' />";
        else
            questionHtml += "<input type='hidden' value='Single' name='questiontype" + simpleId + "' />";
        question.html(questionHtml);
        questionAndAnswersPanel.append(question);

        for (var i = 1; i < 100; i++) {
            var answerItem = $("#answer" + i + "Input");
            if (answerItem.val() !== null && answerItem.val() !== undefined && answerItem.val().length > 0) {
                var answer = $(document.createElement('div'));
                answer.addClass("col-sm-12 col-md-6 margin-bottom-5");
                var inputType = "<table style='width:100%'><tr><td>";
                if (isMultipleAnswers === true)
                    inputType += " <input type='checkbox' disabled /></td>";
                else
                    inputType += " <input type='radio' disabled /></td>";

                answer.html(inputType + "<td><input class='form-control input-sm' type='textbox' value='" + answerItem.val() + "' name='answer-" + i + "-" + simpleId + "'/></td></tr></table>");
                questionAndAnswersPanel.append(answer);
                answerItem.val("");
            }
            else { break; }
        }
        $("#questionInput").val("");
        


        panel.append(questionAndAnswersPanel);

        $('#addQuestionRow').append(panel);
        $("#createPollPopup1").modal('hide');
        simpleId += 1;
    }


    this.SaveResortedPoll = function (sortedIds) {
        console.log(sortedIds);
        $.getJSON("/vote/SaveResortedOrderOfQuestions", { pId: thisViewModel.PollId, newIds: sortedIds }, function (result) {
            if (result.isSuccess === true) {

            }
            else {

            }
        });
    }
}


function SendEmailPollNotification(leId, pollId) {
    pollId = $.trim(pollId);
    leId = $.trim(leId);
    var sent = $("#emailPollLink");
    $.getJSON("/vote/SendEmailReminderAboutPoll", { lId: leId, pId: pollId }, function (result) {
        if (result.isSuccess === true) {
            sent.html("Email Reminders Sent");
        }
        else {

        }
    });
    to.remove();
}
function AddAnotherAnswerToPoll() {
    simpleIdTwo += 1;

    var div = $(document.createElement('div'));
    div.addClass('col-xs-12');

    var lbl = $(document.createElement('label'));
    lbl.addClass('form-label');
    lbl.addClass('margin-top-10');
    lbl.html("Answer");

    var innerdiv = $(document.createElement('div'));
    var input = $(document.createElement('input'));
    input.attr("type", "text");
    input.addClass('form-control');
    input.attr("id", "answer" + simpleIdTwo + "Input");

    div.append(lbl).append(input);
    $("#addAnswerToAnswersList").append(div);
    input.focus();
}
function RemoveAnswerInPoll(answerId) {
    if (confirm("Really Remove Answer?")) {
        $.getJSON("/vote/RemoveAnswerFromPoll", { answerId: answerId }, function (result) {
            if (result.isSuccess === true) {
                $("#btnEditAnswerTd-" + answerId).html("<span >Removed</span>");
            }
            else {
            }
        });
    }
}
function EditAnswerInPoll(answerText, answerId) {
    $("#labelAnswerTd-" + answerId).hide();
    $("#btnEditAnswerTd-" + answerId).hide();
    $("#updateAnswerTd-" + answerId).show();
    $("#btnUpdateAnswerTd-" + answerId).show();
    $("#textAnswerId-" + answerId).focus();
    $("#textAnswerId-" + answerId).select();
}
function CancelEditAnswerInPoll(answerId) {
    $("#labelAnswerTd-" + answerId).show();
    $("#btnEditAnswerTd-" + answerId).show();
    $("#updateAnswerTd-" + answerId).hide();
    $("#btnUpdateAnswerTd-" + answerId).hide();
}
function UpdateEditAnswerInPoll(answerId) {
    $.getJSON("/vote/UpdateAnswerToPoll", { answerId: answerId, text: $('#textAnswerId-' + answerId).val() }, function (result) {
        if (result.isSuccess === true) {
            $("#labelAnswerTd-" + answerId).show();
            $("#btnEditAnswerTd-" + answerId).show();
            $("#updateAnswerTd-" + answerId).hide();
            $("#btnUpdateAnswerTd-" + answerId).hide();
            $("#labelAnswerTd-" + answerId).html("<b>" + $('#textAnswerId-' + answerId).val() + "</b>");
        }
        else {
        }
    });
}
function EditQuestionInPoll(questionText, questionId) {
    $("#editQuestionId-" + questionId).hide();
    $("#lblDivQuestionId-" + questionId).hide();
    $("#updateQuestionId-" + questionId).show();
    $("#updateBtnQuestionId-" + questionId).show();
    $("#textQuestionId-" + questionId).focus();
    $("#textQuestionId-" + questionId).select();
}
function CancelEditQuestionInPoll(questionId) {
    $("#editQuestionId-" + questionId).show();
    $("#lblDivQuestionId-" + questionId).show();
    $("#updateQuestionId-" + questionId).hide();
    $("#updateBtnQuestionId-" + questionId).hide();
}
function UpdateEditQuestionInPoll(questionId) {
    $.getJSON("/vote/UpdateQuestionToPoll", { questionId: questionId, text: $('#textQuestionId-' + questionId).val() }, function (result) {
        if (result.isSuccess === true) {
            $("#lblQuestionId-" + questionId).html('<i class="fa fa-question-circle"></i> ' + $('#textQuestionId-' + questionId).val());
            $("#editQuestionId-" + questionId).show();
            $("#lblDivQuestionId-" + questionId).show();
            $("#updateQuestionId-" + questionId).hide();
            $("#updateBtnQuestionId-" + questionId).hide();
        }
        else {
        }
    });
}

function ShowVotes(questionId)
{
    $("#votes-" + questionId).slideDown("fast");
}

function AddNewAnswerToQuestionPoll(questionId) {
    $.getJSON("/vote/AddAnswerToQuestionToPoll", { questionId: questionId, text: $('#newAnswerForQuestionInput-' + questionId).val() }, function (result) {
        if (result.isSuccess === true) {
            var row = ' <div class="row padding-bottom-10" style="border-bottom:1px solid #eee"><div class="col-xs-12 col-sm-3 col-md-2 margin-top-10" ><label class="label label-primary padding-top-5 padding-bottom-5" >0 vote(s) 0%</label></div><div class="col-xs-12 col-sm-6 col-md-8 margin-top-10"><b>' + $('#newAnswerForQuestionInput-' + questionId).val() + '</b></div></div>';
            $("#newAnswerForQuestionTd-" + questionId).append(row);
            $('#newAnswerForQuestionInput-' + questionId).val("");
        }
        else {
        }
    });
}

function getParameterByName(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

function changeEmailNotification(cb, groupLeague, id) {
    var isChecked = $(cb).is(":checked");
    $.getJSON("/member/ChangeEmailNotificationSetting", { groupLeague: groupLeague, id: id, checkedUnCheck: isChecked }, function (result) {
        if (result.isSuccess === true) {
        }
        else {
        }
    });
}

function AddPollAnswerCreate() {
    //$("#createPollPopup").fadeIn("fast");
    //$("#createPollPopup").center();
    //$("#questionInput").focus();
    $("#createPollPopup1").modal('show');
}


function DeleteQuestionForPoll(span) {
    $(span).parent().parent().remove();
}

var Calendar = new function () {
    var thisViewModel = this;
    var groupsSelectedIds = [];

    this.checkInRemoveSmall = function (button, memId, derbyName) {
        var calId = $("#CalendarId").val();
        var evnId = $("#CalendarItemId").val();
        var memberRow = $("#" + memId + "-row");
        var memberRow2 = $("#" + memId + "-row2");

        $.getJSON("/Calendar/CheckInMemberRemove", { calendarId: calId, eventId: evnId, memberId: memId }, function (result) {
        }).error(function () {
            $.getJSON("/Calendar/CheckInMemberRemove", { calendarId: calId, eventId: evnId, memberId: memId }, function (result) {
            });
        });

        memberRow.remove();
        memberRow2.remove();
    }


    this.checkInMemberSmall = function () {
        var dropDown = $("#SelectedMemberId");
        var selectedItem = dropDown.val();
        var selectedMember = $("#SelectedMemberId option:selected").text();
        var noteCheckIn = $("#noteCheckIn");
        var calId = $("#CalendarId").val();
        var evnId = $("#CalendarItemId").val();
        var present = $("#Present").is(':checked');
        var partial = $("#Partial").is(':checked');
        var notPresent = $("#Not_Present").is(':checked');
        var excused = $("#Excused").is(':checked');
        var isTardy = $("#isTardy").is(':checked');
        var additionalPoints = $("#additionalPoints").val();

        if (selectedItem === "") {
            $("#warning").text("Please Select Member");
            return;
        }
        else {
            $("#warning").text("");
        }

        var pontType;
        var points;
        if (present) {
            pontType = "Present";
            points = Number($("#Present").attr("points"));
        }
        else if (partial) {
            pontType = "Partial";
            points = Number($("#Partial").attr("points"));
        }
        else if (notPresent) {
            pontType = "Not_Present";
            points = Number($("#Not_Present").attr("points"));
        }
        else if (excused) {
            pontType = "Excused";
            points = Number($("#Excused").attr("points"));
        }
        if (isTardy)
            points += Number($("#isTardy").attr("points"));

        $.getJSON("/Calendar/CheckMemberIntoEvent", { calendarId: calId, eventId: evnId, memberId: selectedItem, pointType: pontType, note: noteCheckIn.val(), isTardy: isTardy, addPoints: additionalPoints }, function (result) {
            if (result.isSuccess === true) {
                $("#warning").text("");
                $('.bottom-right').notify({
                    message: { text: 'Checked In! ' },
                    fadeOut: { enabled: true, delay: 4000 }
                }).show();
            } else {
            }
        }).error(function () {
            $.getJSON("/Calendar/CheckMemberIntoEvent", { calendarId: calId, eventId: evnId, memberId: selectedItem, pointType: pontType, note: noteCheckIn.val(), isTardy: isTardy, addPoints: additionalPoints }, function (result) {
                if (result.isSuccess === true) {
                    $("#warning").text("");
                    $('.bottom-right').notify({
                        message: { text: 'Checked In! ' },
                        fadeOut: { enabled: true, delay: 4000 }
                    }).show();
                } else {
                }
            });
        });

        if (isTardy)
            pontType += " - Tardy";
        var rowCount = $('#checkInMembers tr').length + 1;
        $("#checkInMembers tbody").prepend('<tr class="trBorderB" id="' + selectedItem + '-row"><td></td><td></td><td>' + selectedMember + '</td><td>' + pontType + '</td><td>' + (+points + +additionalPoints) + '</td><td><button  class="btn btn-danger" onclick="javascript:Calendar.checkInRemoveSmall(this,\'' + selectedItem + '\', \'' + selectedMember + '\')"><i class="fa fa-trash"></i></button></td></tr>');
        $("#SelectedMemberId option[value='" + selectedItem + "']").remove();
        dropDown.val("");
        noteCheckIn.val("");
    }

    this.checkInMemberLarge = function (button, memId, derbyName) {
        memId = $.trim(memId);
        derbyName = $.trim(derbyName);
        var calId = $("#CalendarId").val();
        var evnId = $("#CalendarItemId").val();
        var note = $("#" + memId + "-Note");
        var notecell = $("#" + memId + "-Note-cell");
        var present = $("#Present").is(':checked');
        var partial = $("#Partial").is(':checked');
        var notPresent = $("#Not_Present").is(':checked');
        var excused = $("#Excused").is(':checked');
        var isTardy = $("#" + memId + "-tardy").is(':checked');
        var additionalPoints = $("#additionalPoints").val();

        var pontType;
        var points;
        if (present) {
            pontType = "Present";
            points = Number($("#Present").attr("points"));
        }
        else if (partial) {
            pontType = "Partial";
            points = Number($("#Partial").attr("points"));
        }
        else if (notPresent) {
            pontType = "Not_Present";
            points = Number($("#Not_Present").attr("points"));
        }
        else if (excused) {
            pontType = "Excused";
            points = Number($("#Excused").attr("points"));
        }
        if (isTardy)
            points += Number($("#TardyPoints").val());


        $.getJSON("/Calendar/CheckMemberIntoEvent", { calendarId: calId, eventId: evnId, memberId: memId, pointType: pontType, note: note.val(), isTardy: isTardy, addPoints: additionalPoints }, function (result) {
            if (result.isSuccess === true) {
                $('.bottom-right').notify({
                    message: { text: 'Checked In! ' },
                    fadeOut: { enabled: true, delay: 4000 }
                }).show();
            } else {
            }
        }).error(function () {
            $.getJSON("/Calendar/CheckMemberIntoEvent", { calendarId: calId, eventId: evnId, memberId: memId, pointType: pontType, note: note.val(), isTardy: isTardy, addPoints: additionalPoints }, function (result) {
                if (result.isSuccess === true) {
                    $('.bottom-right').notify({
                        message: { text: 'Checked In! ' },
                        fadeOut: { enabled: true, delay: 4000 }
                    }).show();
                } else {
                }
            });
        });
        $(button).parent().html("<button type='button' class='btn btn-warning' onclick='javascript:Calendar.checkInRemoveLarge(this,\"" + memId + "\", \"" + derbyName + "\")'>Remove</button>");

        if (isTardy)
            pontType += " - Tardy";
        $("#" + memId).text(pontType);
        notecell.html('<span class="i">' + note.val() + '</span>');
        $("#" + memId + "-tardy-cell").text("");
        $("#" + memId + "-points").text(+points + +additionalPoints);
    }


    this.checkInRemoveLarge = function (button, memId, derbyName) {
        memId = $.trim(memId);
        derbyName = $.trim(derbyName);
        var calId = $("#CalendarId").val();
        var evnId = $("#CalendarItemId").val();
        var tardyCell = $("#" + memId + "-tardy-cell");
        var noteCell = $("#" + memId + "-Note-cell");
        var pointCell = $("#" + memId + "-points");
        var pointTypeCell = $("#" + memId);
        var tardyPoints = $("#TardyPoints").val();


        $.getJSON("/Calendar/CheckInMemberRemove", { calendarId: calId, eventId: evnId, memberId: memId }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        }).error(function () {
            $.getJSON("/Calendar/CheckInMemberRemove", { calendarId: calId, eventId: evnId, memberId: memId }, function (result) {
                if (result.isSuccess === true) {
                } else {
                }
            });
        });

        $(button).parent().html('<input type="button" class="btn btn-primary" value="Check In" onclick="javascript:Calendar.checkInMemberLarge(this,\'' + memId + '\', \'' + derbyName + '\')" />');
        tardyCell.html('<input type="checkbox" id="' + memId + '-tardy" points="' + tardyPoints + '"  title="Is Tardy" /><span>Tardy?</span>');
        noteCell.html('<input type="text" id="' + memId + '-Note" class="form-control" />');
        pointCell.text("");
        pointTypeCell.text("None");
    }

    this.ExpandGroupMemberList = function () {
        $("#groupMemberLists").slideToggle();
    };
    this.SetAvailForEventHomePage = function (calId, evenId) {
        calendarId = calId;
        eventId = evenId;
        if (addEventPopup === false) {
            var popup = $('#availablePopUp').clone();
            $("<tr id='calendar-" + evenId + "'><td colspan='3'>" + popup.html() + "</td></tr>").hide().insertAfter($("#" + evenId + "-calendarEventRow")).slideDown('fast');
            addEventPopup = true;
        }
    }
    this.InitializeNewEvent = function () {
        var gList = $("#groupList");
        var currentIds = $("#ToGroupIds").val();
        $.getJSON("/league/GetGroupsOfCurrentMember", {}, function (result) {
            if (result.isSuccess === true) {
                $(result.groups).each(function () {
                    if (currentIds.indexOf(this[1]) === -1)
                        gList.append("<li><label><input groupName='" + this[0] + "' id='" + this[1] + "' name='" + this[1] + "' onchange='Calendar.ChangeGroupDictionaryItem(this)' type='checkbox' >" + this[0] + "</label></li>");
                    else { // if the group already is in the event.
                        gList.append("<li><label><input checked='checked' groupName='" + this[0] + "' id='" + this[1] + "' name='" + this[1] + "' onchange='Calendar.ChangeGroupDictionaryItem(this)' type='checkbox' >" + this[0] + "</label></li>");
                        var group = { name: this[0], idd: this[1] };
                        groupsSelectedIds.push(group);
                    }
                });
            }
        }).error(function () {
        });
    };
    this.InitializeNewReport = function () {
        var gList = $("#groupList");
        $.getJSON("/league/GetGroupsOfCurrentMember", {}, function (result) {
            if (result.isSuccess === true) {
                gList.append("<li><label><input groupName='Entire League' id='0' name='groupToReportOn' onchange='Calendar.ChangeGroupDictionaryItemReport(this)' type='radio' >Entire League</label></li>");
                $(result.groups).each(function () {
                    gList.append("<li><label><input groupName='" + this[0] + "' id='" + this[1] + "' name='groupToReportOn' onchange='Calendar.ChangeGroupDictionaryItemReport(this)' type='radio' >" + this[0] + "</label></li>");
                });
            }
        }).error(function () {
        });
    };
    this.ChangeGroupDictionaryItem = function (checkbox) {
        var memNames = $("#ToMemberNamesSelected");
        var memIds = $("#ToGroupIds");
        var box = $(checkbox);
        var checked = box.is(":checked");
        if (checked) {
            var group = { name: box.attr("groupName"), idd: box.attr("id") };
            groupsSelectedIds.push(group);
        }
        else {
            var group = { name: box.attr("groupName"), idd: box.attr("id") };
            groupsSelectedIds = jQuery.grep(groupsSelectedIds, function (value) {
                return value.idd != group.idd;
            });
        }
        var text = "";
        var ids = "";
        if (groupsSelectedIds.length > 0) {
            $.each(groupsSelectedIds.reverse(), function (i, val) {
                text += '<span class="label label-primary">' + val.name + ',</span> ';
                ids += val.idd + ",";
            });
        } else {
            text += '<span class="label label-primary">Entire League</span> ';
        }

        if (document.getElementById('ToMemberNamesSelected') !== null)
            document.getElementById('ToMemberNamesSelected').innerHTML = text;
        memIds.val(ids);
    }
    this.ChangeGroupDictionaryItemReport = function (checkbox) {
        var memNames = $("#ToMemberNamesSelected");
        var memIds = $("#ToGroupIds");
        var box = $(checkbox);
        var checked = box.is(":checked");
        groupsSelectedIds = [];
        if (checked) {
            var group = { name: box.attr("groupName"), idd: box.attr("id") };
            groupsSelectedIds.push(group);
        }

        var text = "";
        var ids = "";
        if (groupsSelectedIds.length > 0) {
            $.each(groupsSelectedIds.reverse(), function (i, val) {
                text += '<span class="label label-primary">' + val.name + '</span> ';
                ids += val.idd + ",";
            });
            $("#pullGroupEvents").toggleClass("displayNone", false);
        } else {
            text += '<span class="label label-primary">Entire League</span> ';
            $("#pullGroupEvents").toggleClass("displayNone", true);
        }

        if (document.getElementById('ToMemberNamesSelected') !== null)
            document.getElementById('ToMemberNamesSelected').innerHTML = text;
        memIds.val(ids);
    }
}

var Messages = new function () {
    var thisViewModel = this;
    var groupsSelectedIds = [];
    var membersSelectedIds = [];
    this.ExpandGroupMemberList = function () {
        $("#groupMemberLists").slideToggle();
    };
    this.InitializeNewMessages = function () {
        var gList = $("#groupList");
        $.getJSON("/league/GetGroupsOfCurrentMember", {}, function (result) {
            if (result.isSuccess === true) {
                $(result.groups).each(function () {
                    gList.append("<li><label><input groupName='" + this[0] + "' id='" + this[1] + "' name='" + this[1] + "' onchange='Messages.ChangeGroupDictionaryItem(this)' type='checkbox' >" + this[0] + "</label></li>");
                });
            }
        }).error(function () {
        });
    };
    this.ChangeGroupDictionaryItem = function (checkbox) {
        var memNames = $("#ToMemberNamesSelected");
        var memIds = $("#ToGroupIds");
        var box = $(checkbox);
        var checked = box.is(":checked");
        if (checked) {
            var group = { name: box.attr("groupName"), idd: box.attr("id") };
            groupsSelectedIds.push(group);
        }
        else {
            var group = { name: box.attr("groupName"), idd: box.attr("id") };
            groupsSelectedIds = jQuery.grep(groupsSelectedIds, function (value) {
                return value.idd != group.idd;
            });
        }
        var text = "";
        var ids = "";
        $.each(groupsSelectedIds.reverse(), function (i, val) {
            text += '<span class="label label-primary">' + val.name + '</span> ';
            ids += val.idd + ",";
        });
        $.each(membersSelectedIds.reverse(), function (i, val) {
            text += '<span class="label label-primary">' + val.name + '</span> ';
        });
        if (document.getElementById('ToMemberNamesSelected') !== null)
            document.getElementById('ToMemberNamesSelected').innerHTML = text;
        memIds.val(ids);
    }
    this.ChangeDictionaryItem = function (checkbox, id, displayName) {
        var memNames = $("#ToMemberNamesSelected");
        var memIds = $("#ToMemberIds");
        var checked = $(checkbox).is(":checked");
        if (checked) {
            var member = { name: displayName, idd: id };
            membersSelectedIds.push(member);
        }
        else {
            var member = { name: displayName, idd: id };
            membersSelectedIds = jQuery.grep(membersSelectedIds, function (value) {
                return value.idd != member.idd;
            });
        }
        var text = "";
        var ids = "";
        $.each(groupsSelectedIds.reverse(), function (i, val) {
            text += '<span class="label label-primary">' + val.name + '</span> ';
        });
        $.each(membersSelectedIds.reverse(), function (i, val) {
            text += '<span class="label label-primary">' + val.name + '</span> ';
            ids += val.idd + ",";
        });

        if (document.getElementById('ToMemberNamesSelected') !== null)
            document.getElementById('ToMemberNamesSelected').innerHTML = text;
        memIds.val(ids);
    }
    this.toggleCheckedForRecipients = function (checkbox) {
        var memNames = $("#ToMemberNamesSelected");
        var memIds = $("#ToMemberIds");
        //var checked = $(checkbox).attr("checked");
        var isChecked = $(checkbox).is(":checked");
        membersSelectedIds.length = 0;
        var text = "";
        var ids = "";
        $.each(groupsSelectedIds.reverse(), function (i, val) {
            text += '<span class="label label-primary">' + val.name + '</span> ';
        });
        $("#checkboxes input:checkbox").each(function () {
            $(this).prop('checked', isChecked);
            if (isChecked) {
                var cbId = $(this).attr("name");
                var derbyName = $(this).attr("derbyname");
                membersSelectedIds.push({ name: derbyName, idd: cbId });
                text += '<span class="label label-primary">' + derbyName + '</span> ';
                ids += cbId + ",";
            }
        });
        if (document.getElementById('ToMemberNamesSelected') !== null)
            document.getElementById('ToMemberNamesSelected').innerHTML = text;
        memIds.val(ids);
    }
}

var Member = new function () {
    var thisViewModel = this;

    this.RemoveSelfFromLeague = function (span, memberId, leagueId) {
        if (confirm("Remove Your Self Entirely From the League?")) {
            $.getJSON("/member/removememberfromleague", { memberId: memberId, leagueId: leagueId }, function (result) {
                if (result.isSuccess === true) {
                    $(span).parent().html("Removed From League");
                }
            }).error(function () {
            });
        }
    };
    this.BuildLeagueReport = function (link, leagueId) {

        $.getJSON("/member/removememberfromleague", { memberId: memberId, leagueId: leagueId }, function (result) {
            if (result.isSuccess === true) {
                $(span).parent().html("Removed From League");
            }
        }).error(function () {
        });
    };
    this.SetPrivacySetting = function (checkbox, privacySetting) {

        $.getJSON("/member/setprivacysetting", { setting: privacySetting }, function (result) {
            if (result.isSuccess === true) {

            }
        }).error(function () {
        });
    };
};

function MoveSelectionRight() {
    var selectedColumns = $("#selectedColumns");
    var availableColumns = $("#availableColumns :selected");
    availableColumns.each(function () {
        selectedColumns.append(new Option($(this).text(), $(this).val()));
        $(this).remove();
    });

    $("#SelectedColumnsHidden").val("");
    $("#selectedColumns option").each(function () {

        $("#SelectedColumnsHidden").val($("#SelectedColumnsHidden").val() + $(this).val() + ",");
    });
}
function MoveSelectionLeft() {
    var availableColumns = $("#availableColumns");
    var selectedColumns = $("#selectedColumns :selected");

    selectedColumns.each(function () {
        availableColumns.append(new Option($(this).text(), $(this).val()));
        $(this).remove();
    });
    var my_options = $("#availableColumns option");

    my_options.sort(function (a, b) {
        if (a.text > b.text) return 1;
        else if (a.text < b.text) return -1;
        else return 0;
    })

    $("#availableColumns").empty().append(my_options);
    $("#SelectedColumnsHidden").val("");
    $("#selectedColumns option").each(function () {
        $("#SelectedColumnsHidden").append($(this).val() + ",");
    });
}

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
    }
    this.SetUpDocumentsSection = function () {
        //moveScroller("folder-anchor", "folder-scroller");
        moveScroller("header-anchor", "header-scroller");
        $("#documents tbody tr").on('click', function (event) {
            var row = $(this);
            var box = $(event.target).closest('input[type="checkbox"]');
            //if the user clicked on the actual checkbox.
            if (box.length > 0) {
                if (box.is(":checked")) {
                    $("#documents tbody tr").removeClass('rowSelected').find(":checkbox").prop('checked', false);
                    thisViewModel.documentId = parseInt(row.attr("id").replace(/[\D]/g, ""), 10);
                    alert(thisViewModel.documentId);
                    row.addClass('rowSelected').find(":checkbox").prop('checked', true);
                    $("#itemOptions").toggleClass("displayNone", false);
                    $("#img-MoveItem").toggleClass("displayNone", true);
                }
                else {
                    row.removeClass('rowSelected').find(":checkbox").prop('checked', false);
                    $("#itemOptions").toggleClass("displayNone", true);
                    $("#img-MoveItem").toggleClass("displayNone", true);
                }
            } else {
                //if the user clicked on the row.
                if (row.find(":checkbox").is(":checked")) {
                    row.removeClass('rowSelected').find(":checkbox").prop('checked', false);
                    $("#itemOptions").toggleClass("displayNone", true);
                    $("#img-MoveItem").toggleClass("displayNone", true);
                }
                else {
                    $("#documents tbody tr").removeClass('rowSelected').find(":checkbox").prop('checked', false);
                    thisViewModel.documentId = parseInt(row.attr("id").replace(/[\D]/g, ""), 10);
                    row.addClass('rowSelected').find(":checkbox").prop('checked', true);
                    $("#itemOptions").toggleClass("displayNone", false);
                    $("#img-MoveItem").toggleClass("displayNone", true);
                }
            }
        });

    };
    this.DeleteDocument = function (span) {
        var owner = $("#OwnerId");
        $.getJSON("/document/DeleteDocument", { ownerId: owner.val(), doc: thisViewModel.documentId }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        }).error(function () {
        });
        $("#docRow-" + thisViewModel.documentId).remove();
    }
    this.SetDocumentId = function (docId) {
        documentId = docId;
    };
    this.RenameDocument = function (span) {
        var docName = $("#docRow-" + thisViewModel.documentId).attr("name");
        var output = docName;
        $("#rn-" + thisViewModel.documentId).html("<input type='text' id='rns-" + thisViewModel.documentId + "' value='" + output + "' /> <span id='rnl-" + thisViewModel.documentId + "' class='spanLink' onclick='League.RenameDocumentSubmit(this,\"" + thisViewModel.documentId + "\")'>Rename</span>");
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
            } else {
            }
        }).error(function () {
        });
        text.remove();
        spans.remove();
    }
    this.MoveDocumentToFolder = function (dropDown) {
        var owner = $("#OwnerId");
        var folder = $("#folderName");
        var selectedFolder = $(dropDown).find("option:selected");
        $.getJSON("/document/MoveFileTo", { ownerId: owner.val(), moveTo: selectedFolder.val(), moveToName: selectedFolder.text(), doc: thisViewModel.documentId }, function (result) {
            if (result.isSuccess === true) {
                $("#fn-" + thisViewModel.documentId).html(selectedFolder.text());
            } else {
            }
        }).error(function () {
        });
    }
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

function moveScroller(anchorId, scrollerId) {
    var move = function () {
        var st = $(window).scrollTop();
        var ot = $("#" + anchorId).offset().top;
        var s = $("#" + scrollerId);
        if (st > ot) {
            s.css({ position: "fixed", top: "0px" });
        } else {
            if (st <= ot) {
                s.css({ position: "relative", top: "" });
            }
        }
    };
    $(window).scroll(move);
    move();
}

var Locations = new function () {
    this.RemoveLocation = function (img, locId) {
        if (confirm("Are You Sure?")) {
            $.getJSON("/location/DeleteLocation", { lId: locId }, function (result) {
                if (result.isSuccess === true) {
                    $(img).parent().parent().remove();
                }
            }).error(function () {
            });
        }
    }
}

var LeagueMembersReportBuilder = new function () {

    this.ExportReport = function (formName) {
        if ($("#selectedColumns").find("option").length <= 0) {
            $("#warning").text("No Columns Selected");
            return;
        }
        $("#warning").text("");
        document.getElementById('MembersReport').submit();
    }

}


var Tournament = new function () {
    var thisViewModel = this;
    var teamCountAdding = 0;
    var tournamentId;
    thisViewModel.teams = ko.observableArray();
    this.GetTeamsOfGame = function (dropDown, pairingId) {
        var id = $(dropDown).find("option:selected").val();
        var team1 = $("#TeamId-0-" + pairingId);
        var team2 = $("#TeamId-1-" + pairingId);
        $.getJSON("/game/GetTeamsOfGame", { gameId: id }, function (result) {
            if (result.isSuccess === true) {
                team1.empty();
                team2.empty();
                $(result.teams).each(function () {
                    $("<option />", {
                        val: this.TeamId,
                        text: this.Name
                    }).appendTo(team1);

                    $("<option />", {
                        val: this.TeamId,
                        text: this.Name
                    }).appendTo(team2);
                });
                var score1 = $("#ScoreId-0-" + pairingId);
                var score2 = $("#ScoreId-1-" + pairingId);
                score1.val(result.teams[0].Score);
                score2.val(result.teams[0].Score);
            }
        }).error(function () {
        });
    }
    this.GetScoreOfTeamForGame = function (dropDown, pairingId, teamNumber) {
        var score = $("#ScoreId-" + teamNumber + "-" + pairingId);
        var gameId = $("#GameId-" + pairingId).find("option:selected").val();
        var id = $(dropDown).find("option:selected").val();
        $.getJSON("/game/GetTeamScoreOfGame", { gameId: gameId, teamId: id }, function (result) {
            if (result.isSuccess === true) {
                score.val(result.score);

            }
        }).error(function () {
        });
    }
    this.SavePairingOfTournament = function (link, pId) {
        $("#savePairingCheck-" + pId).toggleClass("displayNone", true);
        $("#savePairing-" + pId).toggleClass("displayNone", false);
        var tn = $("#trackNumber-" + pId);
        var tt = $("#trackTime-" + pId);
        var s1 = $("#ScoreId-0-" + pId);
        var s2 = $("#ScoreId-1-" + pId);
        var gId = $("#GameId-" + pId).find("option:selected");
        var t1 = $("#TeamId-0-" + pId).find("option:selected");
        var t2 = $("#TeamId-1-" + pId).find("option:selected");
        $.getJSON("/tournament/SavePairingOfTournament", { tournId: tournamentId, pairingId: pId, gameId: gId.val(), team1Id: t1.val(), team2Id: t2.val(), team1Score: s1.val(), team2Score: s2.val(), trackNumber: tn.val(), trackTime: tt.val() }, function (result) {
            if (result.isSuccess === true) {
                $("#savePairingCheck-" + pId).toggleClass("displayNone", false);
                $("#savePairing-" + pId).toggleClass("displayNone", true);
            }
        }).error(function () {
            $("#savePairing-" + pId).toggleClass("displayNone", true);
        });

    }
    this.PublishBrackets = function (button) {
        $.getJSON("/tournament/PublishTournamentBrackets", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                $("#publishTournament").toggleClass("displayNone", false);
                $(button).remove();
            }
        }).error(function () {
            $("#publishTournament").toggleClass("displayNone", true);
        });
    }
    this.PublishTournament = function (button) {
        $.getJSON("/tournament/PublishTournament", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                $("#publishTournament").toggleClass("displayNone", false);
                $(button).remove();
            }
        }).error(function () {
            $("#publishTournament").toggleClass("displayNone", true);
        });
    }
    this.StartNextRound = function (button) {
        $("#loadingRounds").toggleClass("displayNone", false);
        $.getJSON("/tournament/StartNextRound", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                window.location = window.location;
                $("#loadingRounds").toggleClass("displayNone", true);
            }
        }).error(function () {
            $("#loadingRounds").toggleClass("displayNone", true);
        });
    }
    this.RollBackRound = function (button) {
        $("#loadingRounds").toggleClass("displayNone", false);
        $.getJSON("/tournament/RollBackRound", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                window.location = window.location;
                $("#loadingRounds").toggleClass("displayNone", true);
            }
        }).error(function () {
            $("#loadingRounds").toggleClass("displayNone", true);
        });
    }
    this.StartNextPerformanceRound = function (button) {
        $("#loadingPerformanceRounds").toggleClass("displayNone", false);
        $.getJSON("/tournament/StartNextPerformanceRound", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                window.location = window.location;
                $("#loadingPerformanceRounds").toggleClass("displayNone", true);
            }
        }).error(function () {
            $("#loadingPerformanceRounds").toggleClass("displayNone", true);
        });
    }
    this.RollBackPerformanceRound = function (button) {
        $("#loadingPerformanceRounds").toggleClass("displayNone", false);
        $.getJSON("/tournament/RollBackPerformanceRound", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                window.location = window.location;
                $("#loadingPerformanceRounds").toggleClass("displayNone", true);
            }
        }).error(function () {
            $("#loadingPerformanceRounds").toggleClass("displayNone", true);
        });
    }

    this.SetTournamentId = function (id) {
        tournamentId = id;
    }

    this.AddTeamName = function (span) {
        var teamName = $("#teamName0");
        var seedRating = $("#seedRating0");
        var poolNumber = $("#pool0");
        if (teamName.val().length === 0) {
            teamName.toggleClass("error", true);
            return;
        }
        else
            teamName.toggleClass("error", false);
        if (seedRating.val().length === 0) {
            seedRating.toggleClass("error", true);
            return;
        }
        else
            seedRating.toggleClass("error", false);
        if (parseInt(seedRating.val()) > 100) {
            seedRating.val("100");
            $("#seedWarning").text("Must be <= 100");
            return;
        }
        else { $("#seedWarning").text(""); }
        $("#loadingTeams").toggleClass("displayNone", false);
        $("#successTeams").toggleClass("displayNone", true);
        $.getJSON("/tournament/SaveTeamToTournament", { tournId: tournamentId, team: teamName.val(), rating: seedRating.val(), pool: poolNumber.val() }, function (result) {
            if (result.isSuccess === true) {
                $("#TeamsTable tbody").append("<tr><td class='bracketTeamTd center'><span id='teamName" + teamCountAdding + "' name='teamName" + teamCountAdding + "'  >" + teamName.val() + "</span></td><td class='center'>" + seedRating.val() + "</td><td class='center'>" + poolNumber.val() + "</td><td> <span onclick=\"Tournament.RemoveTeamName(this, '" + result.id + "')\" class='spanLink'>Remove</span></td></tr>");
                $("#loadingTeams").toggleClass("displayNone", true);
                $("#successTeams").toggleClass("displayNone", false);
                teamName.val("");
                seedRating.val("100");
                teamName.focus();
            }
        }).error(function () {
        });
    }
    this.RemoveTeamName = function (span, teamId) {
        $(span).parent().parent().remove();
        $.getJSON("/tournament/RemoveTeamFromTournament", { tournId: tournamentId, teamId: teamId }, function (result) {
            if (result.isSuccess === true) {

            }
        }).error(function () {
        });
    }
    this.SaveBracketTeams = function () {
        $("#loadingTeams").toggleClass("displayNone", false);
        $("#successTeams").toggleClass("displayNone", true);
        thisViewModel.teams.push($("#teamName0").val());
        $.getJSON("/tournament/SaveTeamsToTournament", { tournId: tournamentId, teams: ko.toJSON(thisViewModel.teams) }, function (result) {
            if (result.isSuccess === true) {
                $("#loadingTeams").toggleClass("displayNone", true);
                $("#successTeams").toggleClass("displayNone", false);
                window.location = window.location;
            }
        }).error(function () {
        });

    }
}

function createmarkerforMembers(lon, lat, data, index) {
    var mem = data.split("|");
    if (lon != "0" && lat != "0") {
        var popupinfo = "<table class='popuptable'><tr><td class='popuptd'><div class='popupmemname'><a style='text-decoration: none;color:#BA2B3C;' target='_blank' href='#' >" + mem[0] + "</a></div></br><div class='popupaddress'>" + mem[1] + "</br>" + mem[2] + " " + mem[3] + " " + mem[4] + "</div></br></td><td style='width:50px;padding-top:5px;padding-right:5px'>";
        if (mem[5] != null)
            popupinfo += "<img src='" + mem[5] + "' width='100px'/>";
        else if (mem[6] != null)
            popupinfo += "<img src='" + mem[6] + "' width='100px'/>";
        popupinfo += "</td></tr></table>";
        var lonLat = new OpenLayers.LonLat(lon, lat)
              .transform(
                new OpenLayers.Projection(PROJECTIONFROM, PROJECTIONTO), // transform from WGS 1984
                map.getProjectionObject() // to Spherical Mercator Projection
              );
        var myLocation = new OpenLayers.Geometry.Point(lon, lat)
            .transform(PROJECTIONFROM, PROJECTIONTO);
        var zoom = 0;
        var markers = new OpenLayers.Layer.Markers("Markers");
        map.addLayer(markers);
        var size = new OpenLayers.Size(21, 25);
        var offset = new OpenLayers.Pixel(-(size.w / 2), -size.h);
        var icon1 = new OpenLayers.Icon('../../../Content/images/marker3.png', size, offset);
        markers.addMarker(new OpenLayers.Marker(lonLat, icon1));

        markers.events.register('mousedown', markers, function (evt) {
            $(".olPopup").hide();
            var popup = new OpenLayers.Popup.FramedCloud("Popup",
            myLocation.getBounds().getCenterLonLat(), null,
            popupinfo, null,
            true // <-- true if we want a close (X) button, false otherwise
        );

            map.addPopup(popup);
            OpenLayers.Event.stop(evt);
            $(".olPopup").css("z-index", 10000);
        });

        bounds.extend(new OpenLayers.LonLat(lon, lat).transform(new OpenLayers.Projection(PROJECTIONFROM), new OpenLayers.Projection(PROJECTIONTO)));
    }
}

var jquerySortableHelper = function (e, tr) {
    var $originals = tr.children();
    var $helper = tr.clone();
    $helper.children().each(function (index) {
        // Set helper cell sizes to match the original sizes
        $(this).width($originals.eq(index).width());
    });
    return $helper;
}

var MsgLike = function (messageId, memberId, forumId, topicId) {
    //alert("Called");
    var paramValue = JSON.stringify({ messageId: messageId, memberId: memberId, forumId: forumId, topicId: topicId });
    $.ajax({

        url: '/forum/message/like', //This will call the function in controller
        type: 'POST',
        dataType: 'json',
        data: paramValue,

        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            //  alert(data.MessageId + ":" + data.MessageLike);
            if (data.MessageLike2 == -1) {
                alert("You already liked this.");
            }
            else {
                $('#' + "L-" + data.MessageId).text(data.MessageLike2 + " " + "Likes");
            }
        }
    });
}

var MsgIAgree = function (messageId, memberId, forumId, topicId) {
    //alert("Called");
    var paramValue = JSON.stringify({ messageId: messageId, memberId: memberId, forumId: forumId, topicId: topicId });
    $.ajax({

        url: '/forum/message/agree', //This will call the function in controller
        type: 'POST',
        dataType: 'json',
        data: paramValue,

        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            //  alert(data.MessageId + ":" + data.MessageLike);
            if (data.MessageIAgree2 == -1) {
                alert("You already Agreed this.");
            }
            else {
                $('#' + "A-" + data.MessageId).text(data.MessageIAgree2 + " " + "in Agreement");
            }
        }
    });
}

