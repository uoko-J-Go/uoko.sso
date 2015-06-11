define(['angular'], function (angular) {

    var moduleName = 'uoko.sso';

    // 仅仅是为了组装各个部件用.
    var baseModule = angular.module(moduleName, []).value('requireConfig', requireConfig);

    // make http request as ajax [can be recognise by server]
    baseModule.config([
        '$httpProvider', function($httpProvider) {
            $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
        }
    ]);

    return {
        appName: moduleName,
        app: baseModule
    };
});
