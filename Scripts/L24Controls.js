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
	}).delegate('.l24-styled-dd select', 'change', function () {
		$(this).next().text($(this).find('option:selected').text());
	});
	// set span on styled selects to show selected value if not already a default;
	$('.l24-styled-dd option:selected').each(function () {
		var $span = $(this).parent().next();
		if ($span.text() == "&#160")
			$span.text($(this).text())
		else
			$(this).parent().val(null);
	});
})(jQuery);