$.fn.selectRange = function (start, end) {
    if (!end) end = start;
    return this.each(function () {
        if (this.setSelectionRange) {
            this.focus();
            this.setSelectionRange(start, end);
        } else if (this.createTextRange) {
            var range = this.createTextRange();
            range.collapse(true);
            range.moveEnd('character', end);
            range.moveStart('character', start);
            range.select();
            this.focus();
        }
    });
};

function load_tags(postTags, allTags) {
    $('#postTags')
    .textext({
        plugins: 'tags autocomplete',
        tagsItems: postTags
    })
    .bind('getSuggestions', function (e, data) {
        var list = allTags,
            textext = $(e.target).textext()[0],
            query = (data ? data.query : '') || '';

        $(this).trigger(
            'setSuggestions',
            { result: textext.itemManager().filter(list, query) }
        );
    });
}

function get_tags() {
    var tags = [];
    var tagList = [];
    $('.post-tags-selector .text-tags .text-label').each(function () { tags.push($(this).text()) });
    for (var i = 0; i < tags.length; i++) {
        tagList[i] = { TagCount: 0, TagName: tags[i] };
    }
    return tagList;
}

$(function () {
    function initToolbarBootstrapBindings() {
        var fonts = ['Serif', 'Sans', 'Arial', 'Arial Black', 'Courier',
              'Courier New', 'Comic Sans MS', 'Helvetica', 'Impact', 'Lucida Grande', 'Lucida Sans', 'Tahoma', 'Times',
              'Times New Roman', 'Verdana'],
              fontTarget = $('[title=' + BlogAdmin.i18n.font + ']').siblings('.dropdown-menu');
        $.each(fonts, function (idx, fontName) {
            fontTarget.append($('<li><a data-edit="fontName ' + fontName + '" style="font-family:\'' + fontName + '\'">' + fontName + '</a></li>'));
        });
        $('a[title]').tooltip({ container: 'body' });
        $('.dropdown-menu input').click(function () { return false; })
            .change(function () { $(this).parent('.dropdown-menu').siblings('.dropdown-toggle').dropdown('toggle'); })
            .keydown('esc', function () {
            this.value = ''; $(this).change();
        });

        $('[data-role=magic-overlay]').each(function () {
            var overlay = $(this), target = $(overlay.data('target'));
            overlay.css('opacity', 0).css('position', 'absolute').offset(target.offset()).width(target.outerWidth()).height(target.outerHeight());
        });
        if ("onwebkitspeechchange" in document.createElement("input")) {
            var editorOffset = $('#editor').offset();
            $('#voiceBtn').css('position', 'absolute').offset({ top: editorOffset.top, left: editorOffset.left + $('#editor').innerWidth() - 35 });
        } else {
            $('#voiceBtn').hide();
        }
    };
    function showErrorAlert(reason, detail) {
        var msg = '';
        if (reason === 'unsupported-file-type') { msg = "Unsupported format " + detail; }
        else {
            console.log("error uploading file", reason, detail);
        }
        $('<div class="alert"> <button type="button" class="close" data-dismiss="alert">&times;</button>' +
         '<strong>File upload error</strong> ' + msg + ' </div>').prependTo('#alerts');
    };
    $("#beVersion").html(SiteVars.Version);
    initToolbarBootstrapBindings();
    $('#editor').wysiwyg({ fileUploadError: showErrorAlert });
});

function spinOn() {
    $("#spinner").removeClass("loaded");
    $("#spinner").addClass("loading");
}

function spinOff() {
    $("#spinner").removeClass("loading");
    $("#spinner").addClass("loaded");
}

function selectedOption(arr, val) {
    for (var i = 0; i < arr.length; i++) {
        if (arr[i].OptionValue.toLowerCase() === val.toLowerCase()) return arr[i];
    }
}

function stripVowelAccent(str)
{
var s=str;

var rExps=[ /[\u00C0-\u00C5]/g, /[\u00E0-\u00E5]/g,
/[\u00C8-\u00CB]/g, /[\u00E8-\u00EB]/g,
/[\u00CC-\u00CE]/g, /[\u00EC-\u00EF]/g,
/[\u00D2-\u00D6]/g, /[\u00DD]/g, /[\u00F2-\u00F6]/g,
/[\u00DA-\u00DC]/g, /[\u00F9-\u00FC]/g, /[\u00FD]/g,
/[\u010C]/g, /[\u010D]/g, /[\u010E]/g, /[\u010C]/g, /[\u011A]/g, /[\u011B]/g,
/[\u0147]/g, /[\u010C]/g, /[\u0158]/g, /[\u0159]/g, /[\u0160]/g, /[\u0161]/g,
/[\u0164]/g, /[\u0165]/g, /[\u016E]/g, /[\u016F]/g, /[\u017D]/g, /[\u017E]/g];

var repChar=['A','a','E','e','I','i','O','Y','o','U','u','y','C','c','D','d','E','e','N','n','R','r','S','s','T','t','U','u','Z','z'];

for(var i=0; i<rExps.length; i++)
s=s.replace(rExps[i],repChar[i]);

return s;
}

function toSlug(title) {
    title = stripVowelAccent(title)
    return title
    .toLowerCase()
    .replace(/[^\w ]+/g, '')
    .replace(/ +/g, '-');
}

function webRoot(url) {
    var result = SiteVars.ApplicationRelativeWebRoot;
    if (url.substring(0, 1) === "/") {
        return result + url.substring(1);
    }
    else {
        return result + url;
    }
}

$(document).ready(function () {
    $("#txtTitle").focus();
});
