/// <reference path="typings/cordova/cordova.d.ts" />
/// <reference path="typings/cordova/plugins/Camera.d.ts" />
var RDNCordovaApp;
(function (RDNCordovaApp) {
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
            if (navigator && navigator.splashscreen)
                navigator.splashscreen.hide();
        }

        function onPause() {
        }

        function onResume() {
        }
    })(RDNCordovaApp.Application || (RDNCordovaApp.Application = {}));
    var Application = RDNCordovaApp.Application;

    window.onload = function () {
        Application.initialize();
    };
})(RDNCordovaApp || (RDNCordovaApp = {}));
//# sourceMappingURL=app.js.map
