$(document).ready(function () {
    $.ajaxSetup({ cache: false });

    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('.scrollup').fadeIn();
        } else {
            $('.scrollup').fadeOut();
        }
    });
    $('.scrollup').click(function () {
        $("html, body").animate({ scrollTop: 0 }, 600);
        return false;
    });
    $('[data-toggle="tooltip"]').tooltip();
    $("#feedback_tab").on("mouseenter", function () {
        feedback_tab_hovered = true;
        $("#feedback_tab").css('opacity', 1).animate({ "right": "0px" }, 300);
        setTimeout(function () { feedback_tab_hovered = false; }, 1000);
    });
    $("#feedback_tab").on("mouseleave", function () {
        if (feedback_tab_hovered == false) {
            setTimeout(function () {
                $("#feedback_tab").animate({ "right": "-60px" }, 300, function () { $("#feedback_tab").animate({ "opacity": 0.7 }, 100); });
            }, 1000);
        }
    });
});

function ToggleSideMenu() {
    if ($("#mainNavbar").hasClass("slideInLeft")) {
        $("#mainNavbar").removeClass("slideInLeft");
        $("#mainNavbar").addClass("slideOutLeft");
    }
    else {
        $("#mainNavbar").removeClass("slideOutLeft");
        $("#mainNavbar").addClass("slideInLeft").show();
    }
}

var leagueHost = "https://league.rdnation.com/";
var Host = "https://rdnation.com/";
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

function MarkForumTopicAsRead(btn, topicId) {
    var forumId = $("#ForumId").val();
    $.getJSON("/forum/MarkAsRead", { forumId: forumId, topicId: topicId }, function (result) {
        if (result.isSuccess === true) {
        }
    });
    $(btn).parent().html("<i class='fa fa-check-circle'></i>");
    $('#forum-title-' + topicId + ' a').removeClass('b');
    $(btn).remove();
}



function ForumPostToggleWatch(span) {
    var topicId = $("#TopicId").val();
    var forumId = $("#ForumId").val();
    $('button[name="watch"]').attr('disabled', true);
    $('button[name="stopwatch"]').attr('disabled', true);
    $('button[name="watch"]').html('<i class="fa fa-refresh fa-spin"></i> Please wait..');
    $('button[name="stopwatch"]').html('<i class="fa fa-refresh fa-spin"></i> Please wait..');
    $.getJSON("/forum/WatchTopic", { forumId: forumId, topicId: topicId }, function (result) {
        if (result.result == true) {
            $('button[name="watch"]').attr('disabled', false);
            $('button[name="stopwatch"]').attr('disabled', false);
            if ($(span).prop('name') == 'watch') {
                $('button[name="watch"]').html('<i class="fa fa-binoculars"></i> Stop Watching');
                $('button[name="watch"]').attr('name', 'stopwatch');
            }
            else {
                $('button[name="stopwatch"]').html('<i class="fa fa-binoculars"></i> Watch');
                $('button[name="stopwatch"]').attr('name', 'watch');
            }
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


    $(button).attr('disabled', true).html("<i class='fa fa-refresh fa-spin'></i> Sending..");


    $.getJSON("/member/verifysms", { number: num }, function (result) {
        if (result.isSuccess === true) {

            $("#codeHtml").toggleClass("display-none", false);
        } else {

        }
        $(button).attr('disabled', false).html("Resend");
    }).error(function () {
    });
}

function EnterCarrierCode(button) {
    $(button).attr('disabled', true).html("<i class='fa fa-refresh fa-spin'></i> Verifying..");
    var num = $("#PhoneNumber").val();
    num = num.replace(/\+/g, "").replace(/\./g, "");
    var code = $("#code").val();
    $("#loading2").toggleClass("display-none", false);
    $.getJSON("/member/verifysmscode", { number: num, code: code }, function (result) {
        if (result.isSuccess === true) {
            $('.bottom-right').notify({
                message: { text: 'Verification number is saved!' },
                fadeOut: { enabled: true, delay: 4000 }
            }).show();
            $(button).attr('disabled', false).html("<i class='fa fa-check-circle'></i> Verify");
        } else {
            $(button).attr('disabled', false).html("<i class='fa fa-exclamation-circle'></i> Try Again");
        }

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
    var isDeepSearch = $("#cbDeepSearch").prop('checked');
    var url = "";
    if (isDeepSearch)
        url = "/document/SeachByDocumentName";
    else
        url = "/document/FullTextSearchLeague";
    alert(url);
    $.getJSON(url, { leagueId: owner.val(), text: box.val(), folderId: fId, groupId: gId }, function (result) {
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

        }
        if ($("#folderBody").html().trim() == "") {
            $("#folderBody").before('<div class="panel panel-default margin-top-10"><div class="panel-heading"><b>You</b><button type="button" class="btn btn-xs btn-danger pull-right" title="Delete"  onclick="DeleteCommentFromDocument(this,\'' + ownerId + '\',\'' + id + '\')"><i class="fa fa-trash"></i></button><br /><i class="fa fa-quote-left" style="color:silver"></i> <span style="font-size:18px">' + com.val() + '</span> <i class="fa fa-quote-right" style="color:silver"></i><br /><span class="text-muted small">Just Now</span></div></div>');
        }
        else {
            $("#folderBody div:first").before('<div class="panel panel-default margin-top-10"><div class="panel-heading"><b>You</b><button type="button" class="btn btn-xs btn-danger pull-right" title="Delete"  onclick="DeleteCommentFromDocument(this,\'' + ownerId + '\',\'' + id + '\')"><i class="fa fa-trash"></i></button><br /><i class="fa fa-quote-left" style="color:silver"></i> <span style="font-size:18px">' + com.val() + '</span> <i class="fa fa-quote-right" style="color:silver"></i><br /><span class="text-muted small">Just Now</span></div></div>');
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

function HideShowCCInfo(div, hideShow) {
    $(".subscribtion-payment-type").removeClass('panel-primary');
    $(".subscribtion-payment-type").addClass('panel-default');
    $(div).removeClass('panel-default');
    $(div).addClass('panel-primary');

    if (hideShow === 'show') {
        $("#CCTable").toggleClass('displayNone', false);
        toggleSubscriptionValidationRules(true);
    }
    else if (hideShow === 'hide') {
        $("#CCTable").toggleClass('displayNone', true);
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
        $(".group-row-" + gId).fadeOut(function () {
            $(".group-row-" + gId).remove();
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
    $('#categories > tbody:last').append('<tr><td><div class="padding-5 b">' + cat + '</div></td><td>' + groupN + '</td></tr>');
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
    $("#msg-" + messageId).fadeOut(function () {
        $("#msg-" + messageId).remove();
    });
}
function PinForumTopic(link, topicId, pin) {
    var forumId = $("#ForumId").val();
    $.getJSON("/forum/PinForumTopic", { forumId: forumId, topicId: topicId, pin: pin }, function (result) {
        if (result.isSuccess === true) {
            if (pin == "true") {
                $(link).html("<i class='fa fa-thumb-tack fa-rotate-90'></i>");
                $(link).attr('onclick', "javascript:PinForumTopic(this,'" + topicId + "', 'false')");
                $(link).attr('data-original-title', "UnPin");
                $("#forum-title-" + topicId).append('<i id="pinned-icon-' + topicId + '" class="fa fa-thumb-tack text-muted" data-toggle="tooltip" data-placement="top" title="Topic is Pinned"></i>');
                $("#pinned-icon-" + topicId).tooltip('show');
            }
            else {
                $(link).html("<i class='fa fa-thumb-tack'></i>");
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
            if (loc == "true") {
                $(link).html("<i class='fa fa-unlock'></i>");
                $(link).attr('onclick', "javascript:LockForumTopic(this,'" + topicId + "', 'false')");
                $(link).attr('data-original-title', "Unlock");
                $("#forum-title-" + topicId).append('<i id="locked-icon-' + topicId + '" class="fa fa-lock text-muted" data-toggle="tooltip" data-placement="top" title="Topic is Locked"></i>');
                $("#locked-icon-" + topicId).tooltip('show');
            }
            else {
                $(link).html("<i class='fa fa-lock'></i>");
                $(link).attr('data-original-title', "Lock");
                $(link).attr('onclick', "javascript:LockForumTopic(this,'" + topicId + "', 'true')");
                $("#locked-icon-" + +topicId).remove();
                $(".tooltip").fadeOut();
            }
        }
    });
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
    $(".subscribtion-type").removeClass('panel-primary');
    $(".subscribtion-type").addClass('panel-default');
    $(".billing-choose-action-div").html('<button type="button" class="btn btn-default">Choose</button>');
    $(div).children('.panel-body').children('.billing-choose-action-div').html('<i class="fa fa-check-circle text-warning font32"></i>');
    $(div).removeClass('panel-default');
    $(div).addClass('panel-primary');
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

function SendEmailInvite(memberId, leagueId) {
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
        $("#inputNewMessage").val("");
        var paramValue = JSON.stringify({ groupId: grp, ownerUserId: to, mess: message });
        $.ajax({
            url: '/message/PostMessage', //This will call the function in controller
            type: 'POST',
            dataType: 'json',
            data: paramValue,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                if (data.result) {

                } else {
                    $("#inputNewMessage").val(message);
                    alert("Something happened.  Try again later.");
                }
            }
        });
    }
    $("#inputNewMessage").focus();
    message = message.replace(/\n\r?/g, '<br />');
    var newMess = "";
    if (lastMessageNameId != to) {
        newMess += '<div class=" margin-top-10 margin-bottom-10"><span class="label label-default">' + GetCurrentDateTime() + '</span></div>'
        newMess += '<div class="b margin-top-10 margin-bottom-5">' + fromName + '</div>';
        newMess += '<div style="border-top:1px solid #eee; height:5px;"></div>';
    }
    newMess += '<div class="text-muted">' + message + '</div>';

    $("#messagesAdd").append(newMess);
    var objDiv = document.getElementById("messagesAdd");
    objDiv.scrollTop = objDiv.scrollHeight;
    lastMessageNameId = to;
}

var dict = [];
function ChangeDictionaryItem(checkbox, id, displayName) {
    var memNames = $("#ToMemberNamesSelected");
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
        $(button).parent().append('<i class="fa fa-check"></i>');
        $(button).remove();
    }
}
function updateEditMemberDuesCost(btn, memberId, duesId, duesItemId) {
    $(btn).html('<i class="fa fa-spinner fa-spin"></i> Update');
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
            if (selectedItem.val().toString() == "Going") {
                $("#" + eventId + "-setAvail").removeClass("btn-warning").removeClass("btn-danger").removeClass("padding-3").removeClass("padding-left-10").removeClass("padding-right-10").addClass('btn-success');
                $("#" + eventId + "-setAvail").html('<i class="fa fa-car fa-lg"></i>');
            }
            else if (selectedItem.val().toString() == "Maybe_Going") {
                $("#" + eventId + "-setAvail").removeClass("btn-success").removeClass("btn-danger").removeClass("padding-3").removeClass("padding-left-10").removeClass("padding-right-10").addClass('btn-warning');
                $("#" + eventId + "-setAvail").html('<i class="fa fa-car fa-lg"></i>');
            }
            else if (selectedItem.val().toString() == "Not_Going") {
                $("#" + eventId + "-setAvail").removeClass("btn-success").addClass("padding-3").addClass("padding-left-10").addClass("padding-right-10").addClass('btn-danger');
                $("#" + eventId + "-setAvail").html('<i class="fa fa-home font18"></i>');
            }

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



function SendEmailPollNotification(leId, pollId) {
    pollId = $.trim(pollId);
    leId = $.trim(leId);
    var sent = $("#emailPollLink");
    $.getJSON("/vote/SendEmailReminderAboutPoll", { lId: leId, pId: pollId }, function (result) {
        if (result.isSuccess === true) {
                $('.bottom-right').notify({
                    message: { text: 'Notifications Sent. ' },
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

function ShowVotes(questionId) {
    $("#votes-" + questionId).slideDown("fast");
}

function AddNewAnswerToQuestionPoll(questionId) {
    $.getJSON("/vote/AddAnswerToQuestionToPoll", { questionId: questionId, text: $('#newAnswerForQuestionInput-' + questionId).val() }, function (result) {
        if (result.isSuccess === true) {
            var row = ' <div class="row padding-bottom-10" style="border-bottom:1px solid #eee"><div class="col-xs-12 col-sm-3 col-md-2 margin-top-10" ><button class="btn btn-default btn-xs disabled" >0 vote(s) 0%</label></div><div class="col-xs-12 col-sm-6 col-md-8 margin-top-10"><b>' + $('#newAnswerForQuestionInput-' + questionId).val() + '</b></div></div>';
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
            $('.bottom-right').notify({
                message: { text: 'No Column Selected!' },
                fadeOut: { enabled: true, delay: 10000 },
                type: "danger"
            }).show();
            return;
        }
        $("#warning").text("");
        document.getElementById('MembersReport').submit();
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

function MsgLike(link, messageId, memberid, forumId, topicId) {
    var paramValue = JSON.stringify({ messageId: messageId, forumId: forumId, topicId: topicId });
    $.ajax({
        url: '/forum/message/like', //This will call the function in controller
        type: 'POST',
        dataType: 'json',
        data: paramValue,
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data.success) {
                $(link).text(data.message.MessageLikeCount + " Likes");
            }
            else {
                alert("Something happened.  Try again later.");
            }
        }
    });
}

function MsgIAgree(link, messageId, memberid, forumId, topicId) {
    var paramValue = JSON.stringify({ messageId: messageId, forumId: forumId, topicId: topicId });
    $.ajax({
        url: '/forum/message/agree', //This will call the function in controller
        type: 'POST',
        dataType: 'json',
        data: paramValue,
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data.success) {
                $(link).text(data.message.MessageAgreeCount + " Agree");
            } else {
                alert("Something happened.  Try again later.");
            }
        }
    });
}

