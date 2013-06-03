function multiUploader(config){
  
	this.config = config;
	this.items = "";
    this.all = [];
	var self = this;
    var index = 1;
	
	multiUploader.prototype._init = function(){
		if (window.File && 
			window.FileReader && 
			window.FileList && 
			window.Blob) {		
			 var inputId = $("#"+this.config.form).find("input[type='file']").eq(0).attr("id");
			 document.getElementById(inputId).addEventListener("change", this._read, false);
			 document.getElementById(this.config.dragArea).addEventListener("dragover", function(e){ e.stopPropagation(); e.preventDefault(); }, false);
			 document.getElementById(this.config.dragArea).addEventListener("drop", this._dropFiles, false);
			 document.getElementById(this.config.form).addEventListener("submit", this._submit, false);
		} else
			console.log("Browser supports failed");
	}
	
	multiUploader.prototype._submit = function(e){
		e.stopPropagation(); e.preventDefault();
		self._startUpload();
	}
	
	multiUploader.prototype._preview = function(data){
		this.items = data;
		if(this.items.length > 0){
			var html = "";		
			var uId = "";
 			for(var i = 0; i<this.items.length; i++){
				uId = this.items[i].name._unique();
				var sampleIcon = '<img src="images/pdf-grey.png" style="height:30px; width:30px; margin-right:10px;" />';
				var errorClass = "";
				if(typeof this.items[i] != undefined){
					if(self._validate(this.items[i].type) < 0) { // if the first type is the looking one it will return 0
						sampleIcon = '<img src="images/unknown.png" />';
						errorClass = " invalid";
						$("#errorMessage").append('Fisierul '+this.items[i].name+' nu respecta formatul impus. Fisierul trebuie sa fie pdf.');
						$("#errorMessage").addClass('alert alert-error');
					} 
					html += '<div class="dfiles' + errorClass + '" rel="' + uId + '"><h5>' + sampleIcon + this.items[i].name + '</h5><div id="' + uId + '" class="progress" style="display:none;"><img src="images/ajax-loader.gif" /></div  ><div  ><img id="' + uId + ':trash" onclick=" myFunction(this); "  src="images/trash.png"/></div></div>';
					
				}
			}
			$("#dragAndDropFiles").append(html);
		}
	}

	

	multiUploader.prototype._read = function(evt){
		if(evt.target.files){
			self._preview(evt.target.files);
			self.all.push(evt.target.files);
		} else 
			console.log("Failed file reading");
	}
	
	multiUploader.prototype._validate = function(format){
		var arr = this.config.support.split(",");
		return arr.indexOf(format);
	}
	
	multiUploader.prototype._dropFiles = function(e){
		e.stopPropagation(); e.preventDefault();
		self._preview(e.dataTransfer.files);
		self.all.push(e.dataTransfer.files);
	}
	
	multiUploader.prototype._uploader = function(file,f){
		if(typeof file[f] != undefined && self._validate(file[f].type) >= 0){
		    var data = new FormData();
		    
			var ids = file[f].name._unique();
			data.append('file',file[f]);
			data.append('index',ids);
			$(".dfiles[rel='" + ids + "']").find(".progress").show();
			
			$.ajax({
				type:"POST",
				url:this.config.uploadUrl,
				data: data,
				
				cache: false,
				contentType: false,
				dataType: false,
				processData: false,
				success: function (rponse) {
				   
					$("#"+ids).hide();
					var obj = $(".dfiles").get();
					$.each(obj,function(k,fle){
						if($(fle).attr("rel") == rponse){
							$(fle).slideUp("normal", function(){ $(this).remove(); });
						}
					});
					if (f+1 < file.length) {
						self._uploader(file,f+1);
					}


					console.log(rponse);
					//$("#areaToUpload").hide();
				    //$("#showAreaToUpload").show();
					get_preview_images(rponse.png_list);
					
				}
				,
				error: function(error) {
				    console.log(error);
				}
			});
		} else
			console.log("Invalid file format - "+file[f].name);
	}
	
	multiUploader.prototype._startUpload = function(){
	    if (this.all.length > 0) {
	       
			for(var k=0; k<this.all.length; k++){
			    var file = this.all[k];
			    
			    this._uploader(file, 0);
			    
			
				
			}
		}
	}
	
	String.prototype._unique = function(){
		return this.replace(/[a-zA-Z]/g, function(c){
     	   return String.fromCharCode((c <= "Z" ? 90 : 122) >= (c = c.charCodeAt(0) + 13) ? c : c - 26);
    	});
	}

	this._init();
}

function initMultiUploader(){
    new multiUploader(config);


}
$(function () {
    var W3CDOM = (document.createElement && document.getElementsByTagName);

    function initFileUploads() {
        if (!W3CDOM) return;
        var fakeFileUpload = document.createElement('div');
        fakeFileUpload.className = 'fakefile';
        fakeFileUpload.appendChild(document.createElement('input'));
        var image = document.createElement('img');
        image.src = 'pix/button_select.gif';
        fakeFileUpload.appendChild(image);
        var x = document.getElementsByTagName('input');
        for (var i = 0; i < x.length; i++) {
            if (x[i].type != 'file') continue;
            if (x[i].parentNode.className != 'fileinputs') continue;
            x[i].className = 'file hidden';
            var clone = fakeFileUpload.cloneNode(true);
            x[i].parentNode.appendChild(clone);
            x[i].relatedElement = clone.getElementsByTagName('input')[0];
            x[i].onchange = x[i].onmouseout = function () {
                this.relatedElement.value = this.value;
            }
        }
    }
});