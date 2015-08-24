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

        var zeroColumn = $(document.createElement('td'));
        zeroColumn.addClass("text-center vertical-middle");
        if (item.IsRead !== true) {
            var btn = $(document).createElement('button');
            btn.addClass('btn btn-default btn-sm');
            btn.attr({ 'data-toggle': 'tooltip', 'data-placement': 'top', 'data-original-title': 'Mark As Read', onclick: "javascript:MarkForumTopicAsRead(this, '" + item.TopicId + "')" });
            btn.html('<i class="fa fa-envelope"></i>');
            zeroColumn.append(btn);
        }
        else {
            zeroColumn.append('<i class="fa fa-check-circle"></i>');
        }
        row.append(zeroColumn);

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
            firstColumn.append(' <i id="pinned-icon-' + item.TopicId + '" class="fa fa-thumb-tack text-muted" data-toggle="tooltip" data-placement="top" title="Topic is Pinned"></i>');
        row.append(firstColumn);

        var catColumn = $(document.createElement('td'));


        var catLink = $(document.createElement('span'));
        catLink.attr({ onclick: "Forum.changeForumCategoryLink('" + item.GroupId + "', '" + item.CategoryId + "')" });

        catLink.html(item.Category);
        catLink.addClass("hyperlink");

        catColumn.append(catLink);
        row.append(catColumn);

        var thirdColumn = $(document.createElement('td'));
        var memberLink = $(document.createElement('a'));
        if (item.ForumOwnerTypeEnum === "league")
            memberLink.attr({ href: leagueHost + "member/" + item.CreatedByMember.MemberId.replace(/-/g, "") + "/" + item.CreatedByMember.DerbyNameUrl });
        else
            memberLink.attr({ href: Host + SportNamePlusMemberNameForUrl + "/" + item.CreatedByMember.DerbyNameUrl + "/" + item.CreatedByMember.MemberId.replace(/-/g, "") });
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
            memberLink.attr({ href: Host + SportNamePlusMemberNameForUrl + "/" + item.LastPostByMember.DerbyNameUrl + "/" + item.LastPostByMember.MemberId.replace(/-/g, "") });
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
            //if (item.IsRead === false)
            //    sevenColumn.append('&nbsp;<button class="btn btn-default btn-sm" data-toggle="tooltip" data-placement="top"  title="Mark As Read"  onclick="javascript:MarkForumTopicAsRead(this, \'' + item.TopicId + '\')"> <i class="fa fa-check-square-o"></i></button>');

            if (item.IsPinned)
                sevenColumn.append('&nbsp;<button class="btn btn-default btn-sm" type="button" data-toggle="tooltip" data-placement="top" title="UnPin" onclick="javascript:PinForumTopic(this, \'' + item.TopicId + '\', \'' + false + '\')"><i class="fa fa-thumb-tack fa-rotate-90"></i></button>');
            else
                sevenColumn.append('&nbsp;<button class="btn btn-default btn-sm" type="button" data-toggle="tooltip" data-placement="top" title="Pin" onclick="javascript:PinForumTopic(this, \'' + item.TopicId + '\', \'' + true + '\')"><i class="fa fa-thumb-tack"></i></button>');

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
        $('[data-toggle="tooltip"]').tooltip();
    }


    this.ArchiveForumTopic = function (link, topicId, loc) {
        thisViewModel.IsScrollingAllowed = true;
        $("#noMoreTopics").toggleClass("inline-block", true);
        var forumId = $("#ForumId").val();
        $.getJSON("/forum/ArchiveForumTopic", { forumId: thisViewModel.forumId, topicId: topicId, lockTopic: loc }, function (result) {
            if (result.isSuccess === true) {
                $(link).parent().parent().remove();
            }
        });
    }

    this.CheckIsForumPostExists = function (forumId, topicId, messageId) {
        $.getJSON("/forum/CheckIsForumPostExists", { forumId: forumId, topicId: topicId, messageId: messageId }, function (result) {
            if (result.isSuccess === true) {
                if (result.isExists === true)
                    window.location.href = "/forum/post/quote/" + forumId + "/" + topicId + "/" + messageId;
            } else {
                $('.bottom-right').notify({
                    message: { text: 'This post has been removed from the thread. This post has been removed from the thread' },
                    fadeOut: { enabled: true, delay: 4000 },
                    type: "danger"
                }).show();
            }
        });
        return false;
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
                    console.dir(result.Topics);
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
    this.MarkHomeForumTopicAsRead = function (btn, topicId, forumId) {
        $(btn).html("<i class='fa fa-refresh fa-spin'></i>");
        $.getJSON("/forum/MarkAsRead", { forumId: forumId, topicId: topicId }, function (result) {
            if (result.result === true) {
                $(btn).parent().parent().remove();
            }
        });
    }


    this.GetMentionedMembers = function () {
        var members = [];
        $("#wmd-input_ifr").contents().find(".mentioned_name").each(function () {
            $("#mentioned_members_ids").val($("#mentioned_members_ids").val() + $(this).attr('id') + ",");
            members.push({
                id: $(this).attr('id'),
                name: $(this).html()
            });
        });

    }

    this.SetupPost = function (uploadType, forumId, forumType, topicId, groupId) {
        tinymce.init({
            mode: "textareas",
            elements: "wmd-input",
            theme: "modern",
            content_css: '/content/tinymce/tinymce.content.css',
            plugins: "mention,layer,table,preview,media,contextmenu,directionality,fullscreen,noneditable,visualchars,nonbreaking,template",
            language: "en",
            relative_urls: false,
            uploadType: uploadType,
            ForumId: forumId,
            ForumType: forumType,
            TopicId: topicId,
            GroupId: groupId,
            UploadFileUrl: "/forum/postimageupload",
            mentions: {
                source: function (query, process, delimiter) {
                    if (delimiter === '@') {
                        $.getJSON('/Utilities/SearchNamesForLeague?q=' + query + "&groupId=" + groupId, function (data) {
                            process(data)
                        });
                    }
                },
                render: function (item) {
                    var markup = '<li><a href="#">';
                    if (item.picture != '')
                        markup += '<img src="' + item.picture + '" class="w20 round-corners"/> ';
                    else
                        markup += '<i class="fa fa-user fa-lg text-muted"></i> ';
                    if (item.name && item.realname)
                        markup += item.name + " [" + item.realname + "]";
                    else if (item.name)
                        markup += item.name;
                    else if (item.realname)
                        markup += item.realname;
                    markup += '</a></li>';

                    return markup;
                },
                highlighter: function (text) {
                    //make matched block italic
                    return text.replace(new RegExp('(' + this.query + ')', 'ig'), function ($1, match) {
                        return '<b>' + match + '</b>';
                    });
                },
                insert: function (item) {
                    var html = '';
                    if (item.name && item.realname)
                        html = '<span class="mentioned_name" id="' + item.id + '">' + item.name + ' [' + item.realname + ']</span>';
                    else if (item.name)
                        html = '<span class="mentioned_name" id="' + item.id + '">' + item.name + '</span>';
                    else if (item.realname)
                        html = '<span class="mentioned_name" id="' + item.id + '">' + item.realname + '</span>';

                    return html;
                }
            }
        });


    }
}