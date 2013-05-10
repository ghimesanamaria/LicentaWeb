function addEvents() {

    $("#areaContainerUploadToggle").click(function (e) {


        e.preventDefault();

        // get the full details about the person selected       

            var iconElem = $(this).children('i'),
                  currentClassAttr = iconElem.attr('class');

            if (currentClassAttr == 'icon-chevron-left') {
                iconElem.attr('class', 'icon-chevron-right');
                $("#areaToUpload").hide();
            } else {
                iconElem.attr('class', 'icon-chevron-left');
                $("#areaToUpload").show();

            }
        


       
    });
}