$(document).ready(function () {
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
function LoadDropDownBackgroundColors() {
    $("#ColorTempSelected option").each(function () {
        if ($(this).val() != '') {
            $(this).css('background-color', $(this).val());
        }
    });
}
function ColorSelectorChanged() {
    var selected = $("#ColorTempSelected option:selected").val();
    if ($("#ColorTempSelected").val() != '') {
        $("#item-color-chosen").css('background-color', selected);
    }
    else
        $("#item-color-chosen").css('background-color', "transparent");
}

function searchStoreItems(searchbox) {
    var qu = $(searchbox).val();
    if (qu.length != 1) {
        $.getJSON("/Utilities/SearchStoreItem", { q: qu, limit: 50 }, function (result) {
            var html = "<div class='shop-grid'>";
            $.each(result, function (i, item) {
                html += '<div class="shop-grid-item"> <div class="thumbnail full-width bg-lighter">';
                html += '<a href="/'+SportNameForUrl+'-item/';
                html += item.Name;
                html += '/'
                html += item.StoreItemId;
                html += '">';
                if (item.PhotoUrl != null) {
                    html += '<img data-holder-rendered="true" src="';
                    html += item.PhotoUrl;
                    html += '"  style="width: 100%; display: block;" alt="';
                    html += item.Name;
                    html += '" />';
                }
                html += '</a>';
                html += '<div class="caption"><p class="margin-bottom-5">';
                html += '<a class="font15 b" href="/' + SportNameForUrl + '-item/';
                html += item.Name;
                html += '/'
                html += item.StoreItemId;
                html += '">';
                html += item.NameTrimmed;
                html += '</a></p><p class="margin-bottom-5 b font16 text-muted">';
                html += '$' + item.Price;
                html += ' <span class="font11">';
                html += item.Currency;
                html += '</span></p>';
                html += '<a class="font12" href="/' + SportNameForUrl + '-shop/';
                html += item.ShopMerchantId;
                html += '/'
                html += item.ShopName;
                html += '">';
                html += item.ShopNameTrimmed;
                html += '</a></div></div></div>';
            });
            html += '</div>';
            $("#mainList").html(html);
            $("#mainList").highlight(qu);
            $('.shop-grid').imagesLoaded().progress(function () {
                $('.shop-grid').masonry();
            });
        });
    }
}

function UpdateListingViewCount(itemId) {
    $.getJSON("/Utilities/UpdateViewsCount", { storeItemId: itemId });
}

function DeleteItemFromCart(storeItemCartId, merchantId, cartId) {
    $.getJSON("/Cart/DeleteItemFromCart", { storeItemCartId: storeItemCartId, merchantId: merchantId, cartId: cartId }, function (result) {
        $("#" + storeItemCartId + "-row").remove();
        $("#" + merchantId + "-subTotal").text(result.subtotal);
        $("#" + merchantId + "-shippingTotal").text(result.shipping);
        $("#" + merchantId + "-orderTotal").text(result.afterShipping);
    });
}
function ToggleShipment(span, storeItemCartId, merchantId, cartId) {
    $.getJSON("/Cart/ToggleShipment", { storeItemCartId: storeItemCartId, merchantId: merchantId, cartId: cartId }, function (result) {
        $("#" + merchantId + "-subTotal").text(result.subtotal);
        $("#" + merchantId + "-shippingTotal").text(result.shipping);
        $("#" + merchantId + "-orderTotal").text(result.afterShipping);
        $("#" + storeItemCartId + "-price").html(result.itemPrice);
        var sp = $(span);
        if (sp.text().indexOf("pick up locally") >= 0)
            sp.text("ship item");
        else
            sp.text("pick up locally");
    });
}


function BillingAddressSameAsShipping(checkBox) {
    if (checkBox.checked === true) {
        $("#shippingAddress").toggleClass("displayNone", false);
        $("#ShippingAddress_LastName").val($("#BillingAddress_LastName").val());
        $("#ShippingAddress_FirstName").val($("#BillingAddress_FirstName").val());
        $("#ShippingAddress_Street").val($("#BillingAddress_Street").val());
        $("#ShippingAddress_Street2").val($("#BillingAddress_Street2").val());
        $("#ShippingAddress_City").val($("#BillingAddress_City").val());
        $("#ShippingAddress_State").val($("#BillingAddress_State").val());
        $("#ShippingAddress_Zip").val($("#BillingAddress_Zip").val());
        $("#ShippingAddress_Country").val($("#BillingAddress_Country").val());
    } else {
        $("#shippingAddress").toggleClass("displayNone", true);
        $("#ShippingAddress_LastName").val("");
        $("#ShippingAddress_FirstName").val("");
        $("#ShippingAddress_Street").val("");
        $("#ShippingAddress_Street2").val("");
        $("#ShippingAddress_City").val("");
        $("#ShippingAddress_State").val("");
        $("#ShippingAddress_Zip").val("");
        $("#ShippingAddress_Country").val("");
        $("#ShippingAddress_Email").val("");
    }
}

function showCartQuantityUpdate(itemId) {
    $("#" + itemId + "-saved").toggleClass("displayNone", true);
    var oldVal = $("#" + itemId + "-oldVal").val();
    var newVal = $("#" + itemId + "-item").val();
    if (oldVal != newVal) {
        $("#" + itemId + "-update").toggleClass("displayNone", false);
        $("#" + itemId + "-oldVal").val(newVal);
    }
}

function UpdateCartItemQuantity(storeCartItemId, mercId, cartId) {
    var quan = $("#" + storeCartItemId + "-item").val();
    if (quan.length > 0) {
        $.getJSON("/Cart/UpdateStoreQuantityItem", { storeCartItemId: storeCartItemId, merchantId: mercId, cartId: cartId, quantity: quan }, function (result) {
            $("#" + storeCartItemId + "-saved").toggleClass("displayNone", false);
            $("#" + storeCartItemId + "-update").toggleClass("displayNone", true);
            $("#" + mercId + "-subTotal").text(result.subtotal);
            $("#" + mercId + "-shippingTotal").text(result.shipping);
            $("#" + mercId + "-orderTotal").text(result.afterShipping);
            $("#" + storeCartItemId + "-price").text(result.itemPrice);
        });
    }
    else if (quan === 0 || quan.length === 0) {
        DeleteItemFromCart(storeCartItemId, mercId, cartId);
    }
}

function MouseOverPhotoStore(imgUrl) {
    $("#mainPhoto").attr("src", imgUrl);
}

function AddSiteMapNode(url, modified) {
    $.getJSON("/Utilities/AddNodeToSiteMap", { url: url, modified: modified });
}

function HideShowCCInfo(hideShow) {
    if (hideShow === 'show') {
        $("#CCTable").toggleClass('displayNone', false);
        $("#ccSplitter").toggleClass('displayNone', false);
        toggleSubscriptionValidationRules(true);
    }
    else if (hideShow === 'hide') {
        $("#CCTable").toggleClass('displayNone', true);
        $("#ccSplitter").toggleClass('displayNone', true);
        toggleSubscriptionValidationRules(false);
    }
}

function toggleSubscriptionValidationRules(onOff) {
    var settings = $('#PaymentForm').validate().settings;
    //on
    if (onOff === true) {
        $.extend(settings, {
            rules: {
                BillingAddress_FirstName: "required",
                BillingAddress_LastName: "required",
                BillingAddress_Street: "required",
                BillingAddress_City: "required",
                BillingAddress_State: "required",
                BillingAddress_Zip: "required",
                BillingAddress_Country: "required",
                BillingAddress_Email: "required",
                CCNumber: "required",
                SecurityCode: "required",
                MonthOfExpiration: "required",
                YearOfExpiration: "required"
            },
            submitHandler: function (form) {
                // disable the submit button to prevent repeated clicks
                $('#submitButton').toggleClass("displayNone", true);
                $('#working').toggleClass("displayNone", false);
                Stripe.createToken({
                    number: $('.card-number').val(),
                    cvc: $('.card-cvc').val(),
                    exp_month: $('.card-expiry-month').val(),
                    exp_year: $('.card-expiry-year').val(),
                    name: $('#BillingAddress_FirstName').val() + " " + $('#BillingAddress_LastName').val(),
                    address_line1: $('#BillingAddress_Street').val(),
                    address_city: $('#BillingAddress_City').val(),
                    address_state: $('#BillingAddress_State').val(),
                    address_zip: $('#BillingAddress_Zip').val(),
                    address_country: $("#BillingAddress_Country option:selected").text()
                }, stripeResponseHandler);
            }
        });
    }
    else if (onOff === false) {
        $.extend(settings, {
            rules: {
                BillingAddress_FirstName: "required",
                BillingAddress_LastName: "required",
                BillingAddress_Street: "required",
                BillingAddress_City: "required",
                BillingAddress_State: "required",
                BillingAddress_Zip: "required",
                BillingAddress_Country: "required",
                BillingAddress_Email: "required",
            },
            submitHandler: function (form) {
                // disable the submit button to prevent repeated clicks
                $('#submitButton').toggleClass("displayNone", true);
                $('#working').toggleClass("displayNone", false);
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
        $('#submitButton').toggleClass("displayNone", false);
        $('#working').toggleClass("displayNone", true);
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
