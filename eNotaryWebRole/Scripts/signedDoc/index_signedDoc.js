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

$("#btnViewFullProfile").click(function (e) {
    e.preventDefault();

    // get the full details about the person selected 

    var checked_variables = $("#demoTree")
            .jstree("get_checked", null, false);
    if (checked_variables.length > 0) {

        var iconElem = $(this).children('i'),
              currentClassAttr = iconElem.attr('class');

        if (currentClassAttr == 'icon-chevron-up') {
            iconElem.attr('class', 'icon-chevron-down');
            $("#divFullProfile").hide();
        } else {
            iconElem.attr('class', 'icon-chevron-up');


            var param = new Object();

            // verify if the selected variable has description act
            var id = checked_variables[0].id;
            var ok = 0;
            while ($("#" + id).attr('description') != 'act' || ($("#" + id).attr('description') == 'act' && $("#" + id).hasClass('jstree-unchecked'))) {
                id = $("#" + id).find('li')[0].id;

            }
            param.parentID = $("#" + id).parent().parent().parent().parent()[0].id;
            id = id.split('_')[0];
            param.id = id;

            var parameter = JSON.stringify(param);
            $.ajax({
                type: "POST",
                url: '@Url.Content("~/SignDocs/PersonDetails")',
                dataType: 'json',
                data: parameter,

                contentType: 'application/json',
                success: function (data) {
                    console.log(data);
                    birthday = new Date(parseFloat(data[0].Birthday.split('(')[1].split(')')[0]));
                    $("#divFullProfile").html(
                        "<input type=\"text\" id =\"sdNationality\" class=\"inputSignDocs\" placeholder =\"" + data[0].Nationality + "\"/>" +
                        "<input type=\"text\" id =\"sdAddress\" style=\"margin-left:10px\" class=\"inputSignDocs\" placeholder =\"" + data[0].Address1 + "\"/>" +
                         "<input type=\"text\" id =\"sdEducationLevel\" style=\"margin-left:10px\" class=\"inputSignDocs\" placeholder =\"" + data[0].EducationLevel1 + "\"/>" +
                        "<input type=\"text\" id =\"sdNJobName\"  class=\"inputSignDocs\" placeholder =\"" + data[0].JobName + "\"/>" +
                        "<input type=\"text\" id =\"sdJobPlace\" style=\"margin-left:10px\" class=\"inputSignDocs\" placeholder =\"" + data[0].JobPlace + "\"/>" +
                        "<input type=\"text\" id =\"sdStreet_1\" style=\"margin-left:10px\" class=\"inputSignDocs\" placeholder =\"" + data[0].Street_1 + "\"/>" +
                        "<input type=\"text\" id =\"sdStreet_2\"  class=\"inputSignDocs\" placeholder =\"" + data[0].Street_2 + "\"/>" +
                        "<input type=\"text\" id =\"sdStreet_3\" style=\"margin-left:10px\" class=\"inputSignDocs\" placeholder =\"" + data[0].Street_3 + "\"/>" +
                        "<input type=\"text\" id =\"sdCity\" style=\"margin-left:10px\" class=\"inputSignDocs\" placeholder =\"" + data[0].City + "\"/>" +
                        "<input type=\"text\" id =\"sdCountry\"  class=\"inputSignDocs\" placeholder =\"" + data[0].Country + "\"/>" +
                        "<input type=\"text\" id =\"sdMobilePhoneNumber\" style=\"margin-left:10px\" class=\"inputSignDocs\" placeholder =\"" + data[0].MobilePhoneNumber + "\"/>" +
                        "<input type=\"text\" id =\"sdHomePhoneNumber\" style=\"margin-left:10px\" class=\"inputSignDocs\" placeholder =\"" + data[0].HomePhoneNumber + "\"/>" +
                        "<input type=\"text\" id =\"sdEmail\"  class=\"inputSignDocs\" placeholder =\"" + data[0].Email + "\"/>" +
                        "<input type=\"text\" id =\"sdFacebookID\" style=\"margin-left:10px\" class=\"inputSignDocs\" placeholder =\"" + data[0].FacebookID + "\"/>"


                        );
                    $("#divFullProfile").show();

                    return true;
                },
                error: function () {
                    console.log('eroare');

                }
            });
        }
    }


});

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