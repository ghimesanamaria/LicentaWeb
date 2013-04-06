function filter_tree(isReportTree,myID,Model,url,urlForData,selUM,urlGetModelID) {
    var index  = 0;
    var treeModel = Model;
    var openNodeID = 0;
    console.log(Model);
    $("#" + myID).jstree({

        "themes": {
            "theme": "classic"
        },
        "animation": 0,
        "json_data": {
            "ajax": {
                "type": 'POST',
                "data": function (n) {
                    return { id: n.attr ? n.attr("id") : 0 };
                },
                "url": function () {

                    //UM filter
                    var filterUM = "";
                    $.each(selUM, function (index, value) {
                        if (index == 0)
                            filterUM = value;
                        else
                            filterUM = filterUM + ',' + value;
                    });

                    index++;
                    if (isReportTree == 1) {

                        if (index - 1 > 0) {

                            return urlForData + '' + '?modelID=' + openNodeID + '&umID=' + filterUM;
                        }
                        else {
                            return url + '?modelID=0' + '&umID=' + filterUM;
                        }

                    }
                    else {
                        return url + '?umID=' + filterUM;
                    }
                }

            },
            "progressive_render": true


        }
        ,
        "search": {
            "case_insensitive": true
             ,
            "show_only_matches": true
        },


        "checkbox": {
            "override_ui": true,
            "real_checkboxes": true,
            "real_checkboxes_names": function (n) {
                if (treeModel.toLowerCase().indexOf("models") >= 0) {
                    if (isReportTree == 1)
                        return [("check_mreport_" + (n[0].id)), n[0].id]
                    return [("check_model_" + (n[0].id)), n[0].id]
                }
                if (treeModel.toLowerCase().indexOf("units") >= 0) {
                    return [("check_unit_" + (n[0].id)), n[0].id]
                }


            }
        },

        plugins: ["themes", "json_data", "ui", "sort", "search", "adv_search", "checkbox"]
    }).delegate("a", "click", function (event, data) {
        $(event.target).find('.jstree-checkbox').attr("checked", "checked");
        
        if (event.target.className == "jstree-checkbox") {
            $("#" + myID).jstree("open_all", "#" + event.target.parentElement.parentElement.id);
        }
        else {
            $("#" + myID).jstree("open_all", "#" + event.target.parentElement.id);
        }


    }).bind("loaded.jstree", function (event, data) {

        if ($("#isReportModel").val() == 'True') {
            var param = new Object();
            param.selectedVariables = $("#Variables").val();;
            //console.log('@Model.SelectedVariables');

            var parameter = JSON.stringify(param);

            $.ajax({

                type: "POST",
                url: urlGetModelID,
                dataType: 'json',
                data: parameter,

                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    console.log(data);
                    $.each(data, function (index, value) {
                        console.log(value);
                        //    $.jstree._reference('#modelReportTree').check_node("#" + value);
                        $("#modelReportTree").jstree("open_all", "#" + value);

                    });

                    return true;
                },
                error: function () {
                    console.log('eroare');

                }
            });
        }

    }).bind("before.jstree", function (e, data) {
        if (data.func === "open_node") {
            if (typeof data.args[0][0] !== 'undefined')
                openNodeID = data.args[0][0].id;
            else
                openNodeID = data.args[0].id;

        }

    })

        .bind('change_state.jstree', function (e, data) {




        })
    .bind('after_open.jstree', function (e, data) {
        temp = $("#Variables").val().split(',');
        $.each(temp, function (index, value) {
            $.jstree._reference('#modelReportTree').check_node("#" + value);
        });
    });
}