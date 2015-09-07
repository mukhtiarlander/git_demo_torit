function DeleteConfirm(obj) {
    var id = $(obj).attr('id').split('_')[1];

    var isDelete = confirm('Are you sure you want to delete this item?');

    if (isDelete) {
        location.href = "/Configurations/Delete/" + id;
    }
    return;
}

function Edit(obj) {
    var id = $(obj).attr('id').split('_')[1];
    $("#txtKey_" + id).show();
    $("#txtValue_" + id).show();
    $("#txtDescription_" + id).show();
    $("#SaveCancel_" + id).show();
    $(".label_" + id).css("display", "none");
    $("#EditDelete_" + id).hide();
}

function Save(obj) {
    var id = $(obj).attr('id').split('_')[1];
    $.ajax({
        url: '/Configurations/Edit', //This will call the function in controller
        type: 'POST',
        data: { id: id, key: $("#txtKey_" + id).val(), value: $("#txtValue_" + id).val(), description: $("#txtDescription_" + id).val() },
        success: function (data) {
            if (data) {
                location.href = "/Configurations/ViewConfigurations";
                return;
            }
        }
    });
}

function Cancel(obj) {
    var id = $(obj).attr('id').split('_')[1];
    $("#txtKey_" + id).hide();
    $("#txtValue_" + id).hide();
    $("#txtDescription_" + id).hide();
    $("#SaveCancel_" + id).hide();
    $(".label_" + id).show();
    $("#EditDelete_" + id).show();
}