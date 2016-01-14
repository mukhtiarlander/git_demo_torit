
var Roster = new function() {

    this.InitializeAddNewRoster = function () {
       
        $('#GameDate').val("");
        $('#GameDate').datepicker({ dateFormat: 'mm-dd-yy' });
        InitializeSortable();
    };
    
    this.InitializeEditRoster = function () {
     
        InitializeSortable();
    };
    function InitializeSortable() {
        var group = $("ol.simple_with_animation").sortable({
            group: 'simple_with_animation',
            pullPlaceholder: false,
            onDrop: function ($item, container, _super) {
                var $clonedItem = $('<li/>').css({ height: 0 });
                $item.before($clonedItem);
                //$clonedItem.animate({ 'height': $item.height() });
                $clonedItem.detach();
                _super($item, container);
                var arrMembers = [];
                $("#rosterMembers").children().each(function (i) {
                    var li = $(this);
                    var id = li.attr("data-id");
                    if (id != "" && id != "@Guid.Empty") {
                        arrMembers.push(id);
                    }
                });
                $("#RosterMemberIds").val(arrMembers.join(","));
            },
            onDragStart: function ($item, container, _super) {
                var offset = $item.offset(),
                    pointer = container.rootGroup.pointer;
                adjustment = {
                    left: pointer.left - offset.left,
                    top: pointer.top - offset.top
                };
                _super($item, container);
            },
            onDrag: function ($item, position) {
                $item.css({
                    left: position.left - adjustment.left,
                    top: position.top - adjustment.top
                });
            }
        });
    }
}