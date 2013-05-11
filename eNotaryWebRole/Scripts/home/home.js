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


    var myFunction = function (element) {
        var relDiv = element.id.split(':')[0];
        if (confirm("Sunteti sigur ca doriti  sa stergeti acest fisier")) {
            $(".dfiles[rel='" + relDiv + "']").remove();
        }
    }

    var config = {
        // Valid file formats
        //support: "image/jpg,image/png,image/bmp,image/jpeg,image/gif",
        support: "application/pdf",
        form: "demoFiler", // Form ID
        dragArea: "dragAndDropFiles", // Upload Area ID
        uploadUrl: '@Url.Content("~/Home/Upload")' // Server side file url
    };
    //Initiate file uploader.
    $(document).ready(function () {
        initMultiUploader(config);

    });

}