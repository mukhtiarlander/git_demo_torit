﻿/// <reference path="typings/cordova/cordova.d.ts" />
/// <reference path="typings/cordova/plugins/Camera.d.ts" />
var CordovaHostedApp;
(function (CordovaHostedApp) {
    "use strict";

    (function (Application) {
        function initialize() {
            document.addEventListener('deviceready', onDeviceReady, false);
        }
        Application.initialize = initialize;

        function onDeviceReady() {
            // Handle the Cordova pause and resume events
            document.addEventListener('pause', onPause, false);
            document.addEventListener('resume', onResume, false);
            var takePictureButtons = document.getElementsByClassName('cordovaTakePicture');
            for (var i = 0; i < takePictureButtons.length; i++)
                takePictureButtons[i].addEventListener('click', takePicture);
        }

        function takePicture() {
            if (!navigator.camera) {
                alert("Camera API not supported");
                return;
            }
            var options = {
                quality: 20,
                destinationType: Camera.DestinationType.DATA_URL,
                sourceType: 1,
                encodingType: 0
            };

            navigator.camera.getPicture(function (imgData) {
                var el;
                el = document.getElementsByClassName('media-object')[0];
                var srcAttr = document.createAttribute("src");
                srcAttr.value = "data:image/jpeg;base64," + imgData;
                el.attributes.setNamedItem(srcAttr);
            }, function () {
                alert('Error taking picture');
            }, options);

            return false;
        }

        function onPause() {
        }

        function onResume() {
        }
    })(CordovaHostedApp.Application || (CordovaHostedApp.Application = {}));
    var Application = CordovaHostedApp.Application;

    window.onload = function () {
        Application.initialize();
    };
})(CordovaHostedApp || (CordovaHostedApp = {}));
//# sourceMappingURL=app.js.map
