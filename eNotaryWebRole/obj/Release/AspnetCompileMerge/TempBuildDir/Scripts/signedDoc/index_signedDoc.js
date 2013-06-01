var onBegin = function (data) {
    return true;
};
var onSuccess = function (data) {

    count = data[0];

    if (count != null) {


        if (data == "Datele nu au fost salvate!") {
            $("#ErrorMessageSignDocs").removeClass('alert alert-success');
            $("#ErrorMessageSignDocs").addClass('alert alert-error');

        } else {

            $("#ErrorMessageSignDocs").removeClass('alert alert-error');
            $("#ErrorMessageSignDocs").addClass('alert alert-success');
        }

        $("#ErrorMessageSignDocs").html(data);

    } else {

        $("#ErrorMessageSignDocs").html("Datele nu au fost salvate!");


        $("#ErrorMessageSignDocs").addClass('alert alert-error');

        $("#ErrorMessageSignDocs").html(data);
    }
};



$(function () {

    $("#ActTypeList").addClass('inputSignDocs');
    $("#ActTypeList").width($("#sdGender").width() + 14);
    // view full profile tooltip
    $("#btnViewFullProfile").tooltip({
        title: 'Vizualizati intreg profilul'
    });


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

    // iframe container width
    var parentContainer = $('#signDocsContent');
    treeContainer = $('#tree-container'),
    reportContainer = $('#documentListed'),
    docControlsContainer = $('#accordion-and-doc');
    docContainer = reportContainer.find('#documentListed')
                .first();


    reportContainer.width(docControlsContainer.width());



    $(window).resize(function () {
        reportContainer.width(docControlsContainer.width());
    });

    $('#tree-toggle').click(function () {
        $("#filters").hide();
        var $this = $(this),
            parent = treeContainer,
            icon = $this.children('i');

        docContainer = reportContainer.find('.is-doc-container')
                        .first();

        parent.addClass('closing');

        if (parent.hasClass('closed')) { // if expand

            parent.removeClass('closed');
            parent.animate({
                width: '29%'
            }, 300, function () {
                parent.removeClass('closing');
                icon.attr('class', 'icon-chevron-left');
            });
            docControlsContainer.animate({
                width: '69%'
            }, 300, function () {
                // make the report take the size of the doc controls
                reportContainer.width(docControlsContainer.width());


                // call reflow on report if one exists
                if (typeof $report !== 'undefined') {
                    $report.reflow();
                }
            });

        } else {

            parent.animate({
                width: '25px'
            }, 300, function () {
                reportContainer.width(docControlsContainer.width());


                // call reflow on report if one exists
                if (typeof $report !== 'undefined') {
                    $report.reflow();
                }

                parent.addClass('closed');
                parent.removeClass('closing');
                icon.attr('class', 'icon-chevron-right');
            });

            docControlsContainer.animate({
                width: '95%'
            }, 300);
        }
    });

});