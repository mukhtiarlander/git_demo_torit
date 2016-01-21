var Store = new function () {
    var thisViewModel = this;

    this.CreateNewStoreColor = function () {
        var nameOfColor = $("#colorName");
        var colorSelected = $("#colorSelected");
        var dropDown = $("#ColorTempSelected");
        var storeId = $("#MerchantId");
        if (nameOfColor.val() === "") {
            nameOfColor.toggleClass("error", true);
            return;
        }
        nameOfColor.toggleClass("error", false);
        $.getJSON("/store/AddColor", { nameOfColor: nameOfColor.val(), hexOfColor: colorSelected.text(), storeId: storeId.val() }, function (result) {
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
}