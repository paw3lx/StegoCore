initializeUploader = function(){
$(function() {
  Dropzone.options.uploaddropzone = {
  paramName: "file", 
  maxFilesize: 10, 
  maxFiles : 1,
  previewTemplate: document.getElementById('preview-template').innerHTML,
  dictDefaultMessage: "Drop your file image here (1mb max)",
  acceptedFiles: "image/*",
  accept: function(file, done) {
    done();
  },
  init: function() {
    this.on("addedfile", function() {
      if (this.files[1]!=null){
        this.removeFile(this.files[0]);
      }
    });
  },
  success: function(file, response) {
    document.location = "/Home/ShowImage";
  },
  error: function (file, response) {
    var html = '<div class="alert alert-danger alert-dismissible" role="alert">'
   + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>'
   + '<strong>Error! </strong>'+ response +'</div>';
   $("#uploaddropzone").after(html);
   this.removeFile(file);
  }
  };
});

}

initializeUploader();