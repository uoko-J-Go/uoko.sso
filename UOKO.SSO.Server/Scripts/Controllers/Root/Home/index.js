var requireConfig = requireConfig || {};
require([
    'angular',
    'commonModule',
    'Modules/angular/controllers/appListCtrl'
], function (angular,commonModule) {

    angular.bootstrap(document, [commonModule.appName]);
});