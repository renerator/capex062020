(function($){
    $.fn.filetree = function(method){
        var settings = { 
            animationSpeed      : 'fast',            
            collapsed           : false,
            console             : false
        }
        var methods = {
            init : function(options){
                // Get standard settings and merge with passed in values
                var options = $.extend(settings, options); 
                // Do this for every file tree found in the document
                return this.each(function(){
                    var $fileList = $(this);
                    $fileList
                    .addClass('file-list')
                    .find('li')
                    .has('ul')
                    .addClass('folder-root open')
                    .on('click', 'a[href="#"]', function(e){ // Add a click override for the folder root links
                        e.preventDefault();
                        $(this).parent().toggleClass('closed').toggleClass('open');                                
                        return false;
                    });
                });
            }
        }

        if (typeof method === 'object' || !method){
            return methods.init.apply(this, arguments);
        } else {
            $.on( "error", function(){
                console.log(method + " Fallo en componente.");
            } );
        }  
    }
    
}(jQuery));