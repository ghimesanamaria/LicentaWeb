function loadJstree(treeModel, arrayTest, filters, selUM, DevicesOpen, urlGetData, urlSearchData, isPlant, selectedVariablesString, urlDisplayImage,urlServerImage,role) {
   
    $("#demoTree").jstree({
        "themes": {
            "theme": "classic",
            "dots" : false
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
                    //  filters
                    var filter = "";
                    $.each(filters, function (index, value) {
                        if (index == 0)
                            filter = value;
                        else
                            filter = filter + ',' + value;
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

                   
                        return urlGetData + '?id=' + node[0].id + '&filter=' + filter +'&typeAct='+typeAct+'&role='+role;
                  

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
                return [("check_" + t.value + (n[0].id || Math.ceil(Math.random() * 10000))), n[0].id];
            }
        },

        plugins: ["themes", "json_data", "ui", "sort", "search", "adv_search", "checkbox", "crrm"]
    })
    .delegate("a", "click", function (event, data) {

       



        $(event.target).find('.jstree-checkbox').attr("checked", "checked");

        if (event.target.className == "jstree-checkbox") {
            $("#demoTree").jstree("open_all", "#" + event.target.parentElement.parentElement.id);
        }
        else {
            $("#demoTree").jstree("open_all", "#" + event.target.parentElement.id);

        }

    })
    .bind("before.jstree", function (e, data) {
      

            if (data.func === "check_node") {

                $.jstree._reference('#demoTree').uncheck_all();
                console.log(e);


            }
    })
    .bind('open_node.jstree', function (e, data) {
        
        
    })
    .bind("check_node.jstree", function (e, data) {
        console.log('aici');
       

        // because the access on a blob it's private make an ajax request to load the image on server temporarily
        var param = new Object();
        if (role != "utilizator") {
            param.id = data.args[0].parentNode.id;
            param.parentID = data.args[0].parentNode.parentNode.parentNode.parentNode.parentNode.id;
        }
       else {
          param.id = data.args[0].parentNode.parentNode.id;
           param.parentID = data.args[0].parentNode.parentNode.parentNode.parentNode.id;
        }
        console.log($("#" + param.id).attr('description'));
        if ($("#" + param.id).attr('description') != 'act') { }
        else {
            var parameter = JSON.stringify(param);
            $.ajax({
                type: "POST",
                url: urlDisplayImage,
                dataType: 'json',
                data: parameter,

                contentType: 'application/json',
                success: function (data) {
                    console.log(data.message);

                    $("#iframeDoc").show();
                    $("#iframeDoc").attr('src', "Content/"+data.nameFile);
                    //$("#iframeDocA").attr('href', urlServerImage + data.nameFile );


                    // complete the forms with dates 
                    $("#sdPersonFirstName").val(data.person.FirstName);
                    $("#sdPersonMiddleName").val(data.person.MiddleName);
                    $("#sdPersonLastName").val(data.person.LastName);
                    $("#sdExtraDetails").val(data.person.ExtraDetails);
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
                    $("#sdCreationDate").val(creationdate.getDay() + "/" + creationdate.getMonth() + 1 + "/" + creationdate.getFullYear());
                    $("#sdReason").val(data.act.Reason);
                    $("#sdState").val(data.act.State);
                    $("#sdReasonState").val(data.act.ReasonState);

                    console.log(data.act.SentToClient);
                    if (data.act.SentToClient == 1)
                        $("#sdSentToClient").attr('checked', 'checked');



                    return true;
                },
                error: function () {
                    console.log('eroare');

                }
            });
        }
       
        })
    .bind("loaded.jstree", function (e, data) {

        $("#demoTree a .jstree-icon").height('32px');
        $("#demoTree a .jstree-icon").width('32px');

        var line = "#0" ;
        var bar = "#-1" ;
        var pie = "#-2" ;
        $(line + " a").first().css('height', '32px');
        $(bar + " a").first().css('height', '32px');
        $(pie + " a").first().css('height', '32px');

        $(line + " a").first().css('line-height', '32px');
        $(bar + " a").first().css('line-height', '32px');
        $(pie + " a").first().css('line-height', '32px');

        $(line).find('ins.jstree-checkbox').hide();
        $(bar).find('ins.jstree-checkbox').hide();
        $(pie).find('ins.jstree-checkbox').hide();
        $(line).find('ins.jstree-icon').eq(1).height('32px');




        $(bar).find('ins.jstree-icon').eq(1).height('32px');
        $(pie).find('ins.jstree-icon').eq(1).height('32px');
        $(line).find('ins.jstree-icon').eq(1).width('32px');
        $(bar).find('ins.jstree-icon').eq(1).width('32px');
        $(pie).find('ins.jstree-icon').eq(1).width('32px');


        $(line).find('.jstree-icon').eq(0).hide();
        $(bar).find('.jstree-icon').eq(0).hide();
        $(pie).find('.jstree-icon').eq(0).hide();
       

    })
    .bind('after_open.jstree', function (e, data) {
        $("#demoTree a .jstree-icon").height('32px');
        $("#demoTree a .jstree-icon").width('32px');
        $("#demoTree a ").height('32px');
        $("#demoTree a ").width('32px');

        
    });
}