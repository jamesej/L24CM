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
		
		// IMAGE
		
		$('.l24-image-load').click(function () {
			var $this = $(this);
			var $fname = $this.siblings('input');
			top.getFile($fname.val(), function(fname) {
				var suffix = fname.afterLast('.').toLowerCase();
				if ("png|jpg|gif".indexOf(suffix) < 0)
					return "Please select an image file only";
				$fname.val(fname);
				$this.closest('table').find('.l24-image-content-cell')
					.empty()
					.append($("<img class='file-image-thumb' src='" + fname + "'/>"));
				return null;
			});
			return false;
		})
    });
})(jQuery);