(function($) {
    $(document).ready(function() {
        $('#newButton').click(function() {
            var createUrl = prompt("Enter url of new page");
        });
		
		var showLinkFields = function () {
			$link = $(this).closest('.l24-link');
			var isint = $link.find('.l24-link-isinternal input').attr('checked');
			$link.find('.l24-link-controller')[isint ? 'show' : 'hide']();
			$link.find('.l24-link-action')[isint ? 'show' : 'hide']();
			$link.find('.l24-link-url')[isint ? 'hide' : 'show']();
		}
		
		$('.l24-link').each(showLinkFields);
		$('.l24-link-isinternal input').click(showLinkFields);
		
		// RTE
		
		//$('#editor').load(function() {
		//$('#editor').contents().find('._L24Html').click(function () {
		$('._L24Html').click(function () {
			var $this = $(this);
			top.showHtml($this.html(), function(h) {
				$this.html(h);
				$this.siblings('input').val(h);
			});
		});
	    //});
		
		// FILE
		
		$('.l24-image-load').click(function () {
			var $this = $(this);
			var $fname = $this.siblings('input');
			top.getFile($fname.val(), function(fname) {
				alert($fname.val());
				$fname.val(fname);
			});
			return false;
		})
    });
})(jQuery);