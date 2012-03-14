(function($) {
    $(document).ready(function() {
		$(window).scrollTop($('#formState').val().afterLast(';'));
		
		$('#editPanel').delegate('.action-button', 'click', function () {
			$fs = $('#formState');
			$fs.val($fs.val() + $(window).scrollTop());
			$('#editPanel form').append($("<input type='hidden' name='_l24action'/>").val($(this).attr('id'))).submit();
		}).delegate('.editor-label', 'click', function () {
			var $collection = $(this).next('.editor-field').find('.collection');
			$collection.toggleClass('closed');
			var formState = $('#formState').val();
			if ($collection.hasClass('closed'))
				$('#formState').val(formState.replace($collection.attr('id') + ";", ""));
			else
				$('#formState').val(formState + $collection.attr('id') + ";");
			
		}).delegate('.add-button', 'click', function() {
			var postUrl = $('#editPanel form').attr('action').upTo('?');
			var prop = $(this).attr('id').after('-');
			var $collection = $(this).siblings('.collection');
			$collection.removeClass('closed');
			$.get(postUrl + '?-action=PropertyItemHtml&propertyPath=' + prop)
				.success(function (html) {
					var $add = $(html).find('.collection');
					var $lastInput = $collection.find("input[name*=']']:last");
					var n = $lastInput.length ? (parseInt($lastInput.attr('name').afterLast('[').upTo(']')) + 1) : 0;
					$add.find("[id*=']']").each(function () {
						var id = $(this).attr('id');
						$(this).attr('id', id.upToLast('[') + '[' + n + ']' + id.afterLast(']'));
					});
					$add.find("[name*=']']").each(function() {
						var name = $(this).attr('name');
						$(this).attr('name', name.upToLast('[') + '[' + n + ']' + name.afterLast(']'));
					});
					$collection.append($add.contents());
				});
		}).delegate('.l24-image-load', 'click', function () {
			var $this = $(this);
			var $fname = $this.siblings('input');
			top.getFile($fname.val(), function(fname) {
				var suffix = fname.afterLast('.').toLowerCase();
				if (suffix && suffix.length) {
					if ("png|jpg|gif".indexOf(suffix) < 0) 
						return "Please select an image file only";
					$fname.val(fname);
					$this.closest('table').find('.l24-image-content-cell')
						.empty()
						.append($("<img class='file-image-thumb' src='" + fname + "'/>"));
				}
				return null;
			});
			return false;
		}).delegate('._L24Html', 'click', function () {
			var $this = $(this);
			top.showHtml($this.html(), function(h) {
				$this.html(h);
				$this.siblings('input').val(h);
			});
		});
		
		$('.collection').closest('.editor-field').prev('.editor-label')
			.append($('<span>...</span>')).css('cursor', 'pointer');
			
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