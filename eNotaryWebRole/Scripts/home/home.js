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


    

}