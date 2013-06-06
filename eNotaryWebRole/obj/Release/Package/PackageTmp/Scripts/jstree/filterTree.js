function filter_tree(myID,Model,url) {
    var index  = 0;
    var treeModel = Model;
    var openNodeID = 0;
   
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

                    return url;
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
               
                    return [("check_filter_" + (n[0].id)), n[0].id]
               


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
        
    });
}