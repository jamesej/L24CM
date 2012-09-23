(function ($) {
    $(document).ready(function () {
        $(window).scrollTop($('#formState').val().afterLast(';'));

        function addItem($addButton, param, postAdd) {
            var postUrl = $('#editPanel form').attr('action').upTo('?');
            var prop = $addButton.attr('id').after('-');
            var depth = $addButton.attr('class').after('depth-').upTo(' ');
            var $collection = $addButton.closest('.collection');
            $collection.removeClass('closed');
            $.get(postUrl + '?-action=PropertyItemHtml&propertyPath=' + prop + '&depth=' + depth)
				.success(function (html) {
				    var $add = $(html).find('.collection');
				    var $lastInput = $collection.find("input[name*=']']:last");
				    var n = $lastInput.length ? (parseInt($lastInput.attr('name').afterLast('[').upTo(']')) + 1) : 0;
				    $add.find("[id*=']']").each(function () {
				        var id = $(this).attr('id');
				        $(this).attr('id', id.upToLast('[') + '[' + n + ']' + id.afterLast(']'));
				    });
				    $add.find("[name*=']']").each(function () {
				        var name = $(this).attr('name');
				        $(this).attr('name', name.upToLast('[') + '[' + n + ']' + name.afterLast(']'));
				    });
				    var $added = $add.contents().appendTo($collection);
				    if (postAdd) postAdd($added, param);
				});
        }

        function setFilename($input, fname) {
            $input.val(fname);
            $input.closest('table').find('.l24-image-content-cell')
				.empty()
				.append($("<img class='file-image-thumb' src='" + fname + "'/>"));
        }

        $('#editPanel').delegate('.action-button', 'click', function () {
            $fs = $('#formState');
            $fs.val($fs.val() + $(window).scrollTop());
            $('#editPanel form').append($("<input type='hidden' name='_l24action'/>").val($(this).attr('id'))).submit();
        }).delegate('.editor-label.parent', 'click', function () {
            var $collection = $(this).next('.editor-field').find('.collection');
            if ($collection.length == 0) return;
            $(this).toggleClass('child-closed').toggleClass('child-open');
            $collection.toggleClass('closed');
            var formState = $('#formState').val();
            if ($collection.hasClass('closed'))
                $('#formState').val(formState.replace($collection.attr('id') + ";", ""));
            else
                $('#formState').val(formState + $collection.attr('id') + ";");

        }).delegate('.add-button', 'click', function () {
            addItem($(this));
        }).delegate('.l24-image-load, .l24-media-load', 'click', function () {
            var $this = $(this);
            var $fname = $this.siblings('input');
            top.getFile($fname.val(), function (fname) {
                var files = fname.split(',');
                if ($this.hasClass("l24-image-load")) {
                    for (var i = 0; i < files.Length; i++) {
                        var suffix = fname.afterLast('.').toLowerCase();
                        if (suffix && suffix.length && "png|jpg|gif".indexOf(suffix) < 0)
                            return "Please only image files";
                    }
                }
                if (files.length == 1) {
                    setFilename($fname, fname);
                } else {
                    if (confirm("You have selected " + files.length + " files, do you want to add them all?")) {
                        var $addButton = $this.closest('.collection').nextAll('.add-button');
                        setFilename($fname, $.trim(files[0]));
                        for (var i = 1; i < files.length; i++) {
                            addItem($addButton, i, function ($added, idx) {
                                setFilename($added.find('.l24-file-url'), $.trim(files[idx]));
                            });
                        }
                    } else
                        return "Please select your files";
                }
                return null;
            });
            return false;
        }).delegate('._L24Html', 'click', function () {
            var $this = $(this);
            top.showHtml($this.html(), function (h) {
                $this.html(h);
                $this.siblings('input').val(h);
            });
        });

        $('.collection').closest('.editor-field').prev('.editor-label')
			.addClass('parent').addClass('child-closed').css('cursor', 'pointer');

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