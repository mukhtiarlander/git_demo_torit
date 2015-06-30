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
                    gList.append("<li class='group-icon' style='list-style: none;'><label style='cursor: pointer !important'><i  class='fa fa-group'></i>&nbsp;<input style='position: absolute;left: 0;margin-left: 0;opacity: 0;z-index: 2;cursor: pointer !important;height: 1em;width: 1em;top: 0;' groupName='" + this[0] + "' id='" + this[1] + "' name='" + this[1] + "' onchange='Messages.ChangeGroupDictionaryItem(this)' type='checkbox' >" + this[0] + "</label></li>");
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
    };
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
    };
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
    };
    this.ShowNewPrivateMessageMembersPopup = function () {
        $("#NewPrivateMessageMembersPopup").modal('show');
    };
    this.ShowNewTextMessageMembersPopup = function () {
        $("#NewTextMessageMembersPopup").modal('show');
    };
    this.ShowAddMemberPopup = function (group_id, url) {
        $("#panel-members").on('shown.bs.popover', function () {
            $(".js-data-example-ajax").select2({
                ajax: {
                    url: url,
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            q: params.term, // search term
                            messageGroupId: group_id
                        };
                    },
                    processResults: function (data, page) {
                        // parse the results into the format expected by Select2.
                        // since we are using custom formatting functions we do not need to
                        // alter the remote JSON data
                        return {
                            results: data
                        };
                    },
                    cache: true
                },
                placeholder: "Search Members..",
                escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                minimumInputLength: 1,
                templateResult: formatRepo, // omitted for brevity, see the source of this page
                templateSelection: formatRepoSelection // omitted for brevity, see the source of this page
            }).select2("open"); 
        }).popover(
          {
              html: true,
              content: '<select id="ddlMembersList" class="js-data-example-ajax w200" multiple="multiple"></select>',
              template: '<div class="popover"><div class="arrow"></div><h3 class="popover-title"></h3><div class="popover-content"></div><div class="popover-footer"><button type="button" class="btn btn-success" onclick="Messages.SaveMembersToMessage(' + group_id + ')"><i class="fa fa-save"></i> Save</button> <button type="button" class="btn btn-default" onclick="Messages.HideAddMemberPopup()">Close</button></div></div>',
              title: "<i class='fa fa-plus-circle'></i> Add Member"
          });
    };
    function formatRepo(repo) {

        if (repo.loading) return repo.text;

        var markup;
        if (repo.picture != '')
            markup = '<img src="' + repo.picture + '" class="w30 round-corners"/> ' + repo.name;
        else
            markup = '<i class="fa fa-user fa-lg text-muted"></i> ' + repo.name;
        return markup;
    };

    function formatRepoSelection(repo) {
        return repo.name;
    };
    this.SaveMembersToMessage = function (GroupMessageId) {
        console.dir($("#ddlMembersList").select2('data'));
        $.ajax({
            url: '/Message/SaveMembersToMessage',
            type: 'POST',
            data: 'memberids=' + $("#ddlMembersList").val() + '&groupId=' + GroupMessageId,
            success: function (result) {
                $("#panel-members").popover('hide');
                if (result == true) {
                    var selected_members = $("#ddlMembersList").select2('data');
                    for (var i = 0; i < selected_members.length; i++) {
                        $("#message-member-list").append('<div class="margin-bottom-10"><i class="fa fa-envelope-o"></i>&nbsp;&nbsp;<a href="' + selected_members[i].link + '">' + selected_members[i].name + '</a></div>')
                    }

                }
                else {

                }
            },
            error: function () {

            }
        });

    };
    this.HideAddMemberPopup = function () {
        $("#panel-members").popover('hide');
    }
}