(function($) {
	$('.l24-bimodal-autocomplete').each(function () {
		$(this).autocomplete({
				source: _autocompleteSources[$(this).attr('id')],
				delay: 0,
				minLength: 0
			}); 
		});
	$('body').delegate('.l24-bimodal-autocomplete-button', 'click', function () {
		var $ac = $(this).closest('.l24-bimodal-autocomplete-container').find('.l24-bimodal-autocomplete');
		$ac.autocomplete("search", "");
		$ac.focus();
	})
})(jQuery);