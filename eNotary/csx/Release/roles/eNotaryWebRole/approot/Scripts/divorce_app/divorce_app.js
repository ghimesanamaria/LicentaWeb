
// toggle icon from up to down
// when details controls collapse
$('a.accordion-toggle').click(function () {
    var iconElem = $(this).children('i'),
        currentClassAttr = iconElem.attr('class');

    if (currentClassAttr == 'icon-chevron-up') {
        iconElem.attr('class', 'icon-chevron-down');
    } else {
        iconElem.attr('class', 'icon-chevron-up');
    }
});


