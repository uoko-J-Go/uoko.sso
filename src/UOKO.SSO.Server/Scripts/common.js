$(function() {
    $(".sso-user-dropdown").mouseover(function() {
        $(".sso-user-action").removeClass("sso-user-action-hidden").addClass("sso-user-action-visible");
    }).mouseout(function() {
        setTimeout(function() {
            if ($('.sso-user-action').attr('focus') === "false") {
                $(".sso-user-action").removeClass("sso-user-action-visible").addClass("sso-user-action-hidden");
            }
        }, 500);
    });
    $(".sso-user-action").mouseenter(function() {
        $(this).attr('focus', 'true');
    }).mouseleave(function() {
        $(this).removeClass("sso-user-action-visible").addClass("sso-user-action-hidden");
        $(this).attr('focus', 'false');
    });

    $(".main-form-input-text").focus(function () {
        $(".main-form-ctrl-user").removeClass("main-form-ctrl-blur").addClass("main-form-ctrl-focus");
        $(".user-icon").removeClass("user-icon-blur").addClass("user-icon-focus");
    }).blur(function () {
        $(".main-form-ctrl-user").removeClass("main-form-ctrl-focus").addClass("main-form-ctrl-blur");
        $(".user-icon").removeClass("user-icon-focus").addClass("user-icon-blur");
    });
    $(".main-form-input-pwd").focus(function () {
        $(".main-form-ctrl-pwd").removeClass("main-form-ctrl-blur").addClass("main-form-ctrl-focus");
        $(".pwd-icon").removeClass("pwd-icon-blur").addClass("pwd-icon-focus");
    }).blur(function () {
        $(".main-form-ctrl-pwd").removeClass("main-form-ctrl-focus").addClass("main-form-ctrl-blur");
        $(".pwd-icon").removeClass("pwd-icon-focus").addClass("pwd-icon-blur");
    });
})
