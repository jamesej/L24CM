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
    });
})(jQuery);