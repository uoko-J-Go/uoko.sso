define(['commonModule', 'lodash', 'jquery'], function (commonModule, _, $) {

    commonModule.app.controller('AppListController', [
        '$scope', 'requireConfig', function($scope, requireConfig) {

            var pageOptions = requireConfig.pageOptions || {};
            var appList = pageOptions.appList || [];

            $scope.appList = appList;
        }
    ]);
});
