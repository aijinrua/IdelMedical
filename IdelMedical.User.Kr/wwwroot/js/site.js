var currentSubMenuIndex = -1;
var subMenuHideTimer = -1;

$(window).on('load', function () {
    initializeMenuAction();
});

function initializeMenuAction() {
    $('.menu-bar > a').hover(function () {

        if (subMenuHideTimer > 0) {
            clearTimeout(subMenuHideTimer);
        }

        var menuIndex = parseInt($(event.currentTarget).attr('menuindex'));

        if (menuIndex === currentSubMenuIndex) {
            return;
        }

        if (currentSubMenuIndex >= 0) {
            $('.sub-menu-bar > div > div:nth-of-type(' + (currentSubMenuIndex + 1) + ')').css('display', 'none');
        }

        $('.sub-menu-bar > div > div:nth-of-type(' + (menuIndex + 1) + ')').css('display', 'flex');
        $('.sub-menu-bar > div').fadeIn(300);

        currentSubMenuIndex = menuIndex;
    });

    $('.sub-menu-bar > div').mouseleave(function () {
        subMenuHideTimer = setTimeout(function () {
            $('.sub-menu-bar > div').fadeOut(300);
            $('.sub-menu-bar > div > div:nth-of-type(' + (currentSubMenuIndex + 1) + ')').css('display', 'none');
            currentSubMenuIndex = -1;
        }, 300);
    });
}