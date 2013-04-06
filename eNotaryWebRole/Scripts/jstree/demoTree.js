function loadJstree(treeModel, arrayTest, filterModels, selUM, DevicesOpen, urlGetData, urlSearchData, isPlant, selectedVariablesString) {
   
    $("#demoTree").jstree({
        "themes": {
            "theme": "classic"
        },
        "animation": 0,
        "json_data": {
            "data": treeModel,
            "ajax": {
                "type": 'POST',
                "data": function (n) {
                    return { id: n.attr ? n.attr("id") : 0 };
                },
                "url": function (node) {
                    // model filter
                    var filter = "";
                    $.each(filterModels, function (index, value) {
                        if (index == 0)
                            filter = value;
                        else
                            filter = filter + ',' + value;
                    });

                    //UM filter
                    var filterUM = "";
                    $.each(selUM, function (index, value) {
                        if (index == 0)
                            filterUM = value;
                        else
                            filterUM = filterUM + ',' + value;
                    });

                    if (typeof $("#" + node[0].id).attr('description') === "undefined" || $("#" + node[0].id).attr('description').indexOf('dv') < 0) {
                        return urlGetData + '?id=' + node[0].id + '&modelID=' + filter + '&umID=' + filterUM + '&openedDeviceIDs=' + '' + '&isPlant=' + isPlant;
                    } else {
                        DevicesOpen = DevicesOpen + "," + node[0].id;
                        return urlGetData + '?id=' + node[0].id + '&modelID=' + filter + '&umID=' + filterUM + '&openedDeviceIDs=' + DevicesOpen + '&isPlant=' + isPlant;
                    }

                },
                "success": function (new_data) {
                    return new_data;
                }
            },
            "progressive_render": true
        },
        "core": {
            "initially_open": arrayTest
        }
        ,
        "rules": {
            "multiple": false
        },
        "search": {
            "case_insensitive": true
                 ,
            "show_only_matches": true,
            "ajax": {
                "type": 'POST',
                "url": function (search) {

                    return urlSearchData + search;
                },
                "success": function (n) {
                    return n;
                },
                "error": function (message) {
                    return message;
                }
            }
        },
        "checkbox": {
            "override_ui": true,
            "real_checkboxes": true,
            "real_checkboxes_names": function (n) {
               
                        return [("check_" + (n[0].id || Math.ceil(Math.random() * 10000))), n[0].id];
                
            }
        },

        plugins: ["themes", "json_data", "ui", "sort", "search", "adv_search", "checkbox", "crrm"]
    })
    .delegate("a", "click", function (event, data) {

        
            // reload the grid when new element is checked or 
            // unchecked in tree
            var checkedNodes = $("#demoTree").jstree("get_checked", null, true);


      

        $(event.target).find('.jstree-checkbox').attr("checked", "checked");

        if (event.target.className == "jstree-checkbox") {
            $("#demoTree").jstree("open_all", "#" + event.target.parentElement.parentElement.id);
        }
        else {
            $("#demoTree").jstree("open_all", "#" + event.target.parentElement.id);

        }

        
    })
    .bind("before.jstree", function (e, data) {
        var url = document.URL;
        

            if (data.func === "check_node") {

                $.jstree._reference('#demoTree').uncheck_all();


            }
    })
    .bind('open_node.jstree', function (e, data) {
        
    })
    .bind("loaded.jstree", function (e, data) {})
    .bind('after_open.jstree', function (e, data) {

        if (typeof path === "undefined") {
            // ???
        } else {
            $('#demoTree').jstree("open_all", "#" + path[0]);

            if (typeof selectedVariables == "undefined") { var selectedVariables; }

            $.jstree._reference('#demoTree').check_node("#" + selectedVariablesString);
        }

        var description = data.args[0][0].attributes[1].value;
        var getVariables = [];

       
        

    });
}