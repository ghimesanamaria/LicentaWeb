$(function () {



    window.get_preview_images = function (list_of_preview_images) {

        // dinamycally add the png files 
        console.log(list_of_preview_images);
        png_list = list_of_preview_images.replace("{", "").replace("}", "").replace(/"/g, "").split(",");
        console.log(png_list);

        $.each(png_list, function (index, value) {
            temp = value.split(":");
            console.log(temp);
            $("#area-container-preview").append("<div class='div-preview'><img src='Content/pdf_preview/" + temp[1] + "'  id=" + temp[0] + "  class='png-preview'> <i id="+temp[0]+"sign"+" class='icon-question-sign'/><label></label>"+temp[1]+"</div>");

        });


        $(".png-preview").click(function (event) {
            image_clicked = event.target.id;
            $.each($("div.div-preview").find("img.clicked"), function (index, value) {
                $(value.id).removeClass("clicked");
            });
            $("#" + image_clicked).addClass('clicked');
            $("#file-details").show();
            $("#areaToUpload").hide();
        });


      

    }

   
    


});

