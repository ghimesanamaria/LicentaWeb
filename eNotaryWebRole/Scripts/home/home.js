function addEvents() {

    $("#areaContainerUploadToggle").click(function (e) {


        e.preventDefault();

        // get the full details about the person selected       

            var iconElem = $(this).children('i'),
                  currentClassAttr = iconElem.attr('class');

            if (currentClassAttr == 'icon-chevron-up') {
                iconElem.attr('class', 'icon-chevron-down');
                $("#areaToUpload").hide();
            } else {
                iconElem.attr('class', 'icon-chevron-up');
                $("#areaToUpload").show();

            }
        


       
    });

    $("#submitHandler").click(function (e) {
        $("#areaToUpload").hide();
        $("#file-details").show();
    });

    $("#details-Toggle").click(function (e) {


        e.preventDefault();

        // get the full details about the person selected       

        var iconElem = $(this).children('i'),
              currentClassAttr = iconElem.attr('class');

        if (currentClassAttr == 'icon-chevron-down') {
            iconElem.attr('class', 'icon-chevron-up');
            $("#file-details").show();
           
        } else {
            iconElem.attr('class', 'icon-chevron-down');
            $("#file-details").hide();
         

        }




    });


    

}