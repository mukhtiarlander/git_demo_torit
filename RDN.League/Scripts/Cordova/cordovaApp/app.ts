/// <reference path="typings/cordova/cordova.d.ts" />
/// <reference path="typings/cordova/plugins/Camera.d.ts" />
module RDNCordovaApp {
    "use strict";

    export module Application {
        export function initialize() {
            document.addEventListener('deviceready', onDeviceReady, false);
        }

        function onDeviceReady() {
            // Handle the Cordova pause and resume events
            document.addEventListener('pause', onPause, false);
            document.addEventListener('resume', onResume, false);
            if (navigator && navigator.splashscreen) navigator.splashscreen.hide();
        }

        function onPause() {
            
        }

        function onResume() {
            
        }

    }

    window.onload = function () {
        Application.initialize();
    }
}