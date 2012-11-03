(function ($) {
    $(document).ready(function () {
        $(window).scrollTop($('#formState').val().afterLast(';'));

        var showLinkFields = function () {
            $link = $(this).closest('.l24-link');
            var isint = $link.find('.l24-link-isinternal input').attr('checked');
            $link.find('.l24-link-controller')[isint ? 'show' : 'hide']();
            $link.find('.l24-link-action')[isint ? 'show' : 'hide']();
            $link.find('.l24-link-url')[isint ? 'hide' : 'show']();
        }

        function setupAfterLoad($container) {
            $container.find('.collection').closest('.editor-field').prev('.editor-label')
			.addClass('parent').addClass('child-closed').css('cursor', 'pointer');

            $container.find('.l24-link').each(showLinkFields);
            $container.find('.l24-link-isinternal input').click(showLinkFields);
            $container.find('.l24-datetime').datepicker({ changeMonth: true, changeYear: true, dateFormat: 'yy-mm-dd' });
        }

        setupAfterLoad($('#editPanel'));

        function reindex($container, idx, addToExisting) {
            $container.find("[id*=']']").andSelf().filter("[id*=']']").each(function () {
                var id = $(this).attr('id');
                var newIdx = idx;
                if (addToExisting)
                    newIdx += parseInt(id.afterLast('[').upToLast(']'));
                $(this).attr('id', id.upToLast('[') + '[' + newIdx + ']' + id.afterLast(']'));
            });
            $container.find("[name*=']']").andSelf().filter("[name*=']']").each(function () {
                var name = $(this).attr('name');
                var newIdx = idx;
                if (addToExisting)
                    newIdx += parseInt(name.afterLast('[').upToLast(']'));
                $(this).attr('name', name.upToLast('[') + '[' + newIdx + ']' + name.afterLast(']'));
            });
        }

        function setFirstLast($coll) {
            var $reorders = $coll.find('.reorder');
            $reorders.removeClass('first').removeClass('last');
            $reorders.first().addClass('first');
            $reorders.last().addClass('last');
        }

        function addItem($addButton, param, postAdd) {
            var postUrl = $('#editPanel form').attr('action').upTo('?');
            var prop = $addButton.attr('id').after('-');
            var depth = $addButton.attr('class').after('depth-').upTo(' ');
            var $collection = $addButton.closest('.collection');
            $collection.removeClass('closed');
            $collection.find('.reorder.last').removeClass('last');
            $.get(postUrl + '?-action=PropertyItemHtml&propertyPath=' + prop + '&depth=' + depth)
				.success(function (html) {
				    var $add = $(html).find('.collection');
				    $add.find('.add-button').remove();
				    var $lastInput = $collection.find("input[name*=']']:last");
				    var n = $lastInput.length ? (parseInt($lastInput.attr('name').afterLast('[').upTo(']')) + 1) : 0;
				    reindex($add, n, false);
				    var indentInc = parseInt($addButton.attr('class').after('indent-').upTo(' '));
				    $add.find("[class*='indent-']").each(function () {
				        var cls = $(this).attr('class');
				        var indent = parseInt(cls.after('indent-').upTo(' ')) + indentInc;
				        $(this).attr('class', cls.upTo('indent-') + 'indent-' + indent + ' ' + cls.after('indent-').after(' '));
				    });
				    var $added = $add.contents().insertBefore($addButton);
				    setFirstLast($collection);
				    setupAfterLoad($added);
				    if (postAdd) postAdd($added, param);
				});
        }

        function setFilename($input, fname) {
            $input.val(fname);
            $input.closest('.l24-image').find('.l24-image-content')
				.empty()
				.append($("<img class='file-image-thumb' src='" + fname + "'/>"));
        }

        $('#editPanel').delegate('.action-button', 'click', function () {
            var $this = $(this);
            if ($this.attr('id') == 'save') {
                $fs = $('#formState');
                $fs.val($fs.val() + $(window).scrollTop());
                $('#editPanel form').append($("<input type='hidden' name='_l24action'/>").val($(this).attr('id'))).submit();
            }
            if ($this.hasClass('delete')) {
                reindex($this.nextAll(), -1, true);
                $this.next().next().remove();
                $this.next().remove();
                $this.remove();
            } else if ($this.hasClass('reorder-up')) {
                var $this = $this.closest('.reorder').prev();
                var $block = $this.add($this.next()).add($this.next().next());
                var $above = $this.prev().prev().prev();
                $above.before($block);
                reindex($block, -1, true);
                $block = $above.add($above.next()).add($above.next().next());
                reindex($block, 1, true);
                setFirstLast($this.closest('.collection'));
            } else if ($this.hasClass('reorder-down')) {
                var $this = $this.closest('.reorder').prev();
                var $block = $this.add($this.next()).add($this.next().next());
                var $below = $this.next().next().next();
                $below.next().next().after($block);
                reindex($block, 1, true);
                $block = $below.add($below.next()).add($below.next().next());
                reindex($block, -1, true);
                setFirstLast($this.closest('.collection'));
            }
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
                        var $addButton = $this.closest('.collection').children('.add-button');
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

    });
})(jQuery);