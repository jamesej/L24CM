String.prototype.left = function(n) {
    if (n <= 0) return "";
    else if (n > this.length) return this;
    else return this.substring(0, n);
}

String.prototype.right = function(n) {
    if (n <= 0) return "";
    else if (n > this.length) return this;
    else {
        var len = this.length;
        return this.substring(len, len - n);
    }
};

String.prototype.upTo = function(s) {
    var pos = this.indexOf(s);
    if (pos < 0) return this;
    return this.substring(0, pos);
}

String.prototype.upToLast = function(s) {
    var pos = this.lastIndexOf(s);
    if (pos < 0) return this;
    return this.substring(0, pos);
}

String.prototype.after = function(s){
    var pos = this.indexOf(s);
    if (pos < 0) return "";
    return this.substring(pos + s.length);
}

String.prototype.afterLast = function(s) {
    var pos = this.lastIndexOf(s);
    if (pos < 0) return "";
    return this.substring(pos + s.length);
}

var showControlPanel;

(function($) {
    showControlPanel = function(role) {
        $('html').css('height', '100%');
        $('body').css('height', '100%').wrapInner("<div id='outer-div' style='height:100%;width:100%'><div id='body-div'></div></div>");
        $("<div id='l24-div'><iframe src='" + location.href.upTo('?') + "?op=edit' style='width:300px;height:100%'/></div>").css({'height': '100%' }).appendTo($('#outer-div'));
        $('#outer-div').split({ orientation: 'vertical', limit: 50 });
    }

    $(document).ready(function() {
        //alert('hello');
        $('._L24Text')
            .attr('contentEditable', 'true')
            .focus(function() {
                $(this).data('border', $(this).css('border') == undefined ? 'none' : $(this).css('border'));
                //alert($(this).data('border'));
                $(this).css('border', '1px solid #404040');
            })
            .blur(function() {
                //alert($(this).data('border'));
                $(this).css('border', $(this).data('border'));
            });
    });

})(jQuery);