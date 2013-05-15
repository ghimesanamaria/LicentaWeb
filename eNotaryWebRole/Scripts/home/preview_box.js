$(function () {



    window.get_preview_images = function (list_of_preview_images) {

        // dinamycally add the png files 

        $.each(list_of_preview_images, function (index, value) {
            $("#area-container-preview").html("<div class='div-preview'><img src='Content/pdf_preview/" + value + "'  id=" + index + "  class='png-preview'> </div>");

        });


        $(".png-preview").click(function (event) {
            var id = event.target.id;
            $("#file-details").show();
            $("#areaToUpload").hide();
        });


      

    }

   
    


});

