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

    this.NewPollChooseMembersDone = function () {
        $("#NewPollSelectedMembersDiv").empty();
        $("#checkboxes input:checkbox:checked").each(function () {
            $("#NewPollSelectedMembersDiv").append('<span class="label label-primary pull-left padding-5 margin-top-5 margin-right-5 font12">' + $("#" + $(this).attr('name') + "-name").html() + '</span>');
        });
        $("#NewPollMembersPopup").modal('hide');
    }

    this.AddNewPoll = function () {
        if ($("#checkboxes input:checkbox:checked").length > 0) {
            document.getElementById('PollsAdd').submit();
        }
        else {
            alert('No Names have been selected');
        }
    }
}