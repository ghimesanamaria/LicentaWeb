function loadJstree(treeModel, arrayTest, filterModels, selUM, DevicesOpen, urlGetData, urlSearchData, isPlant, selectedVariablesString, urlDisplayImage,urlServerImage) {
   
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
                    var typeAct;

                    if (node[0].id == "-1" || node[0].id == "-2" || node[0].id == "-3") {
                        typeAct = 0;
                    }
                    else
                        if ($("#" + node[0].id).attr('description').indexOf('person') > 0) {
                           typeAct = 0;
                        }
                        else {
                            typeAct = $("#" + node[0].id).parent().parent()[0].id;
                        }

                    //if (typeof $("#" + node[0].id).attr('description') === "undefined" || $("#" + node[0].id).attr('description').indexOf('UnsignedUnvDocs') < 0) {
                    //    return urlGetData + '?id=' + node[0].id + '&modelID=' + filter + '&umID=' + filterUM + '&openedDeviceIDs=' + '' + '&isPlant=' + isPlant;
                    //} else {
                    //    DevicesOpen = DevicesOpen + "," + node[0].id;
                        return urlGetData + '?id=' + node[0].id + '&modelID=' + filter + '&umID=' + filterUM + '&openedDeviceIDs=' + DevicesOpen + '&isPlant=' + isPlant+'&typeAct='+typeAct;
                    //}

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
            "two_state": true,
            "override_ui": true,
            "real_checkboxes": true,
            "real_checkboxes_names": function (n) {
                var t = n[0].attributes.description;
                
                if (typeof t !== 'undefined') {

                    if (t.value.indexOf('Variable') > -1)
                        return [("check_" + (n[0].id || Math.ceil(Math.random() * 10000))), n[0].id];
                    else
                        return [("check_" + t.value + (n[0].id || Math.ceil(Math.random() * 10000))), n[0].id];
                }
                else
                    return [("check_" + (n[0].id || Math.ceil(Math.random() * 10000))), n[0].id];
            }
        },

        plugins: ["themes", "json_data", "ui", "sort", "search", "adv_search", "checkbox", "crrm"]
    })
    .delegate("a", "click", function (event, data) {

        if (window.g_forDashBoard) {
            // reload the grid when new element is checked or 
            // unchecked in tree
            var checkedNodes = $("#demoTree").jstree("get_checked", null, true);

            var siteIDs = [];

            $.each(checkedNodes, function (i, node) {
                //if ($(node).attr('description') == 'LiveVariable') {
                siteIDs.push(node.id);
                //}
            });

            if (siteIDs.length > 0) {

                //
                //selectedVariablesString = siteIDs;
                LoadLiveDashboard(dashboardUrl, siteIDs);
            }
        }

        if (window.g_forSetpoints) {
            // reload the grid when new element is checked or 
            // unchecked in tree
            var checkedNodes = $("#demoTree").jstree("get_checked", null, true);

            var setPointIDs = [];

            $.each(checkedNodes, function (i, node) {
                if ($(node).attr('description') == 'LiveVariable') {
                    setPointIDs.push(node.id);
                }
            });

            if (setPointIDs.length > 0) {
                $grid.fill(setPointIDs, function () {
                    console.log('Filled grid with setpoints: ', setPointIDs);
                });
            }
        }

        $(event.target).find('.jstree-checkbox').attr("checked", "checked");

        if (event.target.className == "jstree-checkbox") {
            $("#demoTree").jstree("open_all", "#" + event.target.parentElement.parentElement.id);
        }
        else {
            $("#demoTree").jstree("open_all", "#" + event.target.parentElement.id);

        }

        var sel = $(this).parent().attr('id');
        if (typeof selectedVariablesString === "undefined") {
            // ???
        }
        else {
            selectedVariablesString = jQuery.grep(selectedVariablesString, function (value) {
                return value != sel;
            });
        }
    })
    .bind("before.jstree", function (e, data) {
      

            if (data.func === "check_node") {

                $.jstree._reference('#demoTree').uncheck_all();
                console.log(e);


            }
    })
    .bind('open_node.jstree', function (e, data) {
        var description = data.inst._get_node(data.rslt.obj).attr("description");

        if (description != undefined) {
            if (description == 'dv') {
                var device = data.args[0][0].id;
                if (device != undefined)
                    openedNodes.push(device);
            }
            if (description == 'sv') {
                var device = data.args[0][0].id;
                if (device != undefined) {
                    openedParentNodes.push(device);
                } else {
                    //pt prima incarcare a raportului default
                    var device2 = data.args[0];
                    if (device2 != undefined) {
                        var d = device2.split('#');
                        openedParentNodes.push(d[1]);
                    }
                }

            }
        }
    })
    .bind("check_node.jstree", function (e, data) {
        console.log('aici');
       

        // because the access on a blob it's private make an ajax request to load the image on server temporarily
        var param = new Object();
        param.id = data.args[0].parentNode.parentNode.id;
        param.parentID = data.args[0].parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.id;
        var parameter = JSON.stringify(param);
        $.ajax({                  
            type: "POST",
            url: urlDisplayImage,
            dataType: 'json',
            data: parameter,

            contentType: 'application/json',
            success: function (data) {
                
                $("#iframeDoc").show();
                $("#iframeDoc").attr('src', urlServerImage + data.nameFile+"#toolbar=0");
                //$("#iframeDocA").attr('href', urlServerImage + data.nameFile );
               

                // complete the forms with dates 
                $("#sdPersonFirstName").val(data.person.FirstName);
                $("#sdPersonMiddleName").val(data.person.MiddleName);
                $("#sdPersonLastName").val(data.person.LastName);
                // calculate age
                    
                    birthday = new Date(parseFloat(data.person.Birthday.split('(')[1].split(')')[0]));
                    todayDate = new Date();
                    todayYear = todayDate.getFullYear();
                    todayMonth = todayDate.getMonth();
                    todayDay = todayDate.getDate();
                    age = todayYear - birthday.getFullYear();

                    if (todayMonth < birthday.getMonth() - 1) {
                        age--;
                    }

                    if (birthday.getMonth() - 1 == todayMonth && todayDay < birthday.getDate()) {
                        age--;
                    }
                


                $("#sdGender").val(data.person.Gender);
                $("#sdAge").val(age);
                $("#sdBirthday").val(birthday.getDate() + "/" + birthday.getMonth() + 1 + "/" + birthday.getFullYear());
                $('#ActTypeList option[value="' + data.act.ActTypeID + '"]').attr('selected', 'selected');
                
                $("#sdActName").val(data.act.Name);

                var creationdate = new Date(new Date(parseFloat(data.act.CreationDate.split('(')[1].split(')')[0])));
                $("#sdCreationDate").val(creationdate.getDay() + "/" + creationdate.getMonth()+1 + "/" + creationdate.getFullYear());
                $("#sdReason").val(data.act.Reason);
                $("#sdState").val(data.act.State);
                $("#sdReasonState").val(data.act.ReasonState);

                console.log(data.act.SentToClient);
                if(data.act.SentToClient == 1)
                $("#sdSentToClient").attr('checked','checked');
            


                return true;
            },
            error: function () {
                console.log('eroare');

            }
        });

       
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

        if (typeof selectedVariablesString === "undefined" || selectedVariablesString == "") {
            // ???
        } else {
            getVariables = selectedVariablesString.split(',');
        }

        $.each(getVariables, function (index, value) {
                if (value != '') {
                //var varNode = '\#' + value + '\'';
                var varNode = '#' + value;

                $.jstree._reference('#demoTree').check_node(varNode);
            }
        });

        if (description == 'sv') {
            $.each(uncheckedModelFilters, function (index, value) {
                hideFilteredNodesmodelFilterTree(value, openedParentNodes);
            });
        }

        if (description == 'dv') {
            $.each(uncheckedUnitsFilters, function (index, value) {
                hideFilteredNodesunitsFilterTree(value, openedNodes);
            });
        }
    });
}