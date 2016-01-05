var Polls = new function () {
    var thisViewModel = this;
    thisViewModel.PollId = "";
    this.InitializeCreatePoll = function () {
        $("#PollsAdd").validate({ rules: { Question: "required" } });
        $("#checkAll").attr("checked", "checked");
        toggleCheckedForRecipients($("#checkAll"));
        tinymce.init({
            mode: "textareas",
            theme: "modern",
            plugins: "layer,table,preview,media,contextmenu,directionality,fullscreen,noneditable,visualchars,nonbreaking,template,link,textcolor colorpicker",
            toolbar: "undo redo | styleselect | bold italic forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
            elements: "wmd-input",
            language: "en",
            relative_urls: false,
        });
    };
    this.Initialize = function (pollId) {
        thisViewModel.PollId = pollId;
        $("#PollsAdd").validate({ rules: { AnswerType: "required" } });
        $("#pollQuestions").sortable({
            handle: '.sortableHandle', update: function (event, ui) {
                var idsInOrder = JSON.stringify($("#pollQuestions").sortable("toArray"));
                Polls.SaveResortedPoll(idsInOrder);
            }
        }).disableSelection();
        $(function () {
            tinymce.init({
                mode: "textareas",
                elements: "elm2",
                theme: "modern",
                plugins: "layer,table,preview,media,contextmenu,directionality,fullscreen,noneditable,visualchars,nonbreaking,template,link,textcolor colorpicker",
                toolbar: "undo redo | styleselect | bold italic forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image"
            });
        });
    }
    this.InitializeViewPolls = function () {
        $("#PollsAdd").validate({
            rules: {
                AnswerType: "required"
            }
        });
    };

    this.AddQuestionToPoll = function () {
        var isMultipleAnswers = $("#multipleOptionsInput").is(":checked");
        var questions = $("#HiddenQuestions");
        var answers = $("#HiddenAnswers");

        var panel = $(document.createElement('div'));
        panel.addClass('panel panel-default');

        var panelHeader = $(document.createElement('div'));
        panelHeader.addClass('panel-heading no-padding padding-5');
        if (isMultipleAnswers === true)
            panelHeader.append("Question - <small class='text-muted'>Multiple Choice</small>")
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
        if ($('#addQuestionRow').html().trim() == "No Questions Added") {
            $('#addQuestionRow').empty();
        }
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

    this.ShowNewPollMembersPopup = function () {
        $("#NewPollMembersPopup").modal('show');
    }
    this.ShowEditPollMembersPopup = function () {
        $("#EditPollMembersPopup").modal('show');
    }

    this.NewPollChooseMembersDone = function () {
        $("#NewPollSelectedMembersDiv").empty();
        $("#checkboxes input:checkbox:checked").each(function () {
            $("#NewPollSelectedMembersDiv").append('<span class="label label-primary pull-left padding-5 margin-top-5 margin-right-5 font12">' + $("#" + $(this).attr('name') + "-name").html() + '</span>');
        });
        $("#NewPollMembersPopup").modal('hide');
    }

    this.EditPollChooseMembersDone = function () {
        $("#EditPollSelectedMembersDiv").empty().show();
        $("#checkboxes input:checkbox:checked").each(function () {
            $("#EditPollSelectedMembersDiv").append('<span class="label label-primary font12 margin-right-5 margin-bottom-5 pull-left padding-5"><i class="fa fa-plus"></i> ' + $("#" + $(this).attr('name') + "-name").html() + '</span>');
        });
        $("#EditPollMembersPopup").modal('hide');
    }

    this.AddNewPoll = function () {
        if ($("#checkboxes input:checkbox:checked").length > 0) {
            document.getElementById('PollsAdd').submit();
        }
        else {
            alert('No Names have been selected');
        }
    }
    this.formatMemberInSuggestionList = function (value) {
        if (value.loading) return value.text;
        var markup;
        if (value.picture != '')
            markup = '<img src="' + value.picture + '" class="w40 h40 round-corners"/> ' + value.name;
        else
            markup = '<i class="fa fa-user fa-lg text-muted"></i> ' + value.name;
        return markup;
    };
    this.formatMemberSelectionSuggestionList = function (value) { return value.name; };
    this.AddMembersToPoll = function (leagueID, pollID) {
        $.ajax({
            url: '/poll/addmemberstopoll',
            type: 'POST',
            data: 'memberids=' + $("#ddlMembersList").val() + '&leagueID=' + leagueID + '&pollID=' + pollID,
            success: function (result) {
                Polls.HideAddMemberPopup();
                if (result == true) {
                    var selected_members = $("#ddlMembersList").select2('data');
                    for (var i = 0; i < selected_members.length; i++) {
                        $("#message-member-list").append('<div class="margin-bottom-10"><i class="fa fa-times fa-lg"></i>&nbsp;&nbsp;<a href="' + selected_members[i].link + '">' + selected_members[i].name + '</a></div>');
                        $("#MembersListDontVoted").append('<span class="label font12 margin-right-5 margin-bottom-5 pull-left padding-5" style="background-color:#eee"><a target="_blank" href="' + selected_members[i].link + '">' + selected_members[i].name + '</a></span>');
                    }
                }
            },
            error: function () { }
        });

    };
    this.ShowAddMemberPopup = function (leagueID, pollID, searchURL) {
        $("#btn_poll_add_member").on('shown.bs.popover', function () {
            $(".js-data-example-ajax").select2({
                ajax: {
                    url: searchURL,
                    dataType: 'json',
                    type: "POST",
                    /// RDN - 5 - A request is being triggered on every key stroke but delay 250 milliseconds for send new request
                    delay: 250,
                    data: function (params) {
                        return {
                            q: params.term, // search term
                            leagueID: leagueID,
                            pollID: pollID
                        };
                    },
                    processResults: function (data, page) {
                        return {
                            results: data
                        };
                    },
                    cache: true
                },
                placeholder: "Search Members..",
                escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                minimumInputLength: 1,
                templateResult: Polls.formatMemberInSuggestionList,
                templateSelection: Polls.formatMemberSelectionSuggestionList
            }).select2("open");
            $("#add_member_ddl_area").css("visibility", "initial");
        }).popover({
            html: true,
            content: '<select id="ddlMembersList" class="js-data-example-ajax w200" multiple="multiple"></select>',
            template: '<div class="popover"><div class="arrow"></div><h3 class="popover-title"></h3><div id="add_member_ddl_area" class="popover-content" style="height:53px;overflow:hidden;visibility:hidden"></div><div class="popover-footer"><button type="button" class="btn btn-success" onclick="Polls.AddMembersToPoll(\'' + leagueID + '\',\'' + pollID + '\');"><i class="fa fa-save"></i> Save</button> <button type="button" class="btn btn-default" onclick="Polls.HideAddMemberPopup()">Close</button></div></div>',
            title: "<i class='fa fa-plus-circle'></i> Add Member"
        });
    }
    this.HideAddMemberPopup = function () {
        $("#panel-members").popover('hide');
        $("#btn_poll_add_member").popover('hide');
    };
}