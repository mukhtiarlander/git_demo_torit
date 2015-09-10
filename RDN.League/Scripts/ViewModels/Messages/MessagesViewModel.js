var Messages = new function () {

    var thisViewModel = this;
    var recipientsSelected = [];
    this.ExpandGroupMemberList = function () {
        $("#groupMemberLists").slideToggle();
    };
    this.SelectRecipient = function (checkbox, id, displayName) {
        var member = { id: id, name: displayName };

        var checked = $(checkbox).is(":checked");
        if (checked) {
            $("#ToMemberNames").tokenInput("add", member);
            recipientsSelected.push(member);
        }
        else {
            $("#ToMemberNames").tokenInput("remove", member);
            recipientsSelected = jQuery.grep(recipientsSelected, function (value) {
                return value.id != member.id;
            });
        }
        this.SaveRecipientListToForm();
    };
    this.ToggleCheckedAllForRecipients = function (checkbox) {
        var isChecked = $(checkbox).is(":checked");
        $("#checkboxesForMembers input:checkbox").each(function () {
            $(this).prop('checked', isChecked);
            var cbId = $(this).attr("name");
            var derbyName = $(this).attr("derbyname");
            var member = { id: cbId, name: derbyName };
            if (isChecked) {
                recipientsSelected.push(member);
                $("#ToMemberNames").tokenInput("add", member);
            }
            else {
                var members = $("#ToMemberNames");
                if ($.inArray(member, members) != -1)
                    $("#ToMemberNames").tokenInput("remove", member);
                recipientsSelected = jQuery.grep(recipientsSelected, function (value) {
                    return value.id != member.id;
                });
            }
        });

        this.SaveRecipientListToForm();
    };
    this.ToggleCheckedAllForGroups = function (checkbox) {
        var isChecked = $(checkbox).is(":checked");
        $("#groupCheckBoxes input:checkbox").each(function () {
            $(this).prop('checked', isChecked);
            var cbId = $(this).attr("name");
            var derbyName = $(this).attr("derbyname");
            var member = { id: cbId, name: derbyName };
            if (isChecked) {
                recipientsSelected.push(member);
                $("#ToMemberNames").tokenInput("add", member);
            }
            else {
                $("#ToMemberNames").tokenInput("remove", member);
                recipientsSelected = jQuery.grep(recipientsSelected, function (value) {
                    return value.id != member.id;
                });
            }
        });

        this.SaveRecipientListToForm();
    };
    this.SaveRecipientListToForm = function () {
        var memIds = $("#ToMemberIds");
        var ids = "";
        $.each(recipientsSelected.reverse(), function (i, val) {
            ids += val.id + ",";
        });
        memIds.val(ids);
    };
    this.ShowNewPrivateMessageMembersPopup = function () {
        $("#NewPrivateMessageMembersPopup").modal('show');
    };
    this.ShowNewTextMessageMembersPopup = function () {
        $("#NewTextMessageMembersPopup").modal('show');
    };
    this.ShowAddMemberPopup = function (group_id, url) {
        $("#btn_add_member").on('shown.bs.popover', function () {
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
            $("#add_member_ddl_area").css("visibility", "initial");
        }).popover(
          {
              html: true,
              content: '<select id="ddlMembersList" class="js-data-example-ajax w200" multiple="multiple"></select>',
              template: '<div class="popover"><div class="arrow"></div><h3 class="popover-title"></h3><div id="add_member_ddl_area" class="popover-content" style="height:53px;overflow:hidden;visibility:hidden"></div><div class="popover-footer"><button type="button" class="btn btn-success" onclick="Messages.SaveMembersToMessage(' + group_id + ')"><i class="fa fa-save"></i> Save</button> <button type="button" class="btn btn-default" onclick="Messages.HideAddMemberPopup()">Close</button></div></div>',
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
                $("#btn_add_member").popover('hide');

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
    this.HideAddMemberPopup = function() {
        $("#panel-members").popover('hide');
        $("#btn_add_member").popover('hide');
    };
    this.SetupNewMessage = function (memberId, type) {
        if (memberId != 0) {
            $.getJSON("/Message/GetConnectedRecepients", { memberId: memberId, type: type }, function (result) {
                if (result.success) {
                    if (result.Recipients == null || result.Recipients == 'undefined') return;
                    var members = JSON.parse(result.Recipients);
                    $.each(members, function (index, item) {
                        $('#ToMemberNames').tokenInput('add', { id: item.Key, name: item.Value });
                    });
                }
            });
        }

        $("#ToMemberNames").tokenInput('/Utilities/SearchNamesForLeague', {
            theme: "facebook",
            preventDuplicates: true,
            hintText: "Begin typing Name of the person.",
            noResultsText: "No results",
            searchingText: "Searching...",
            resultsFormatter: function (item) {
                var markup = '<li>';
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
                markup += '</li>';

                return markup;
            },
            tokenFormatter: function (item) {
                var markup = '<li>';
                if (item.name && item.realname)
                    markup += item.name + " [" + item.realname + "]";
                else if (item.name)
                    markup += item.name;
                else if (item.realname)
                    markup += item.realname;
                markup += '</li>';

                return markup;
            },
            onAdd: function (item) {
                var member = { id: item.id, name: item.name };
                recipientsSelected.push(member);
            },
            onDelete: function (item) {
                var member = { id: item.id, name: item.name };
                recipientsSelected = jQuery.grep(recipientsSelected, function (value) {
                    return value.id != member.id;
                });
            }
        });
    }
}