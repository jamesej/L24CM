
//(function($) {
$.fn.jstreelist = function(options) {
    settings = {
        treeContainerSelector: '.treeContainer',
        listContainerSelector: '.listContainer',
		filenameSelector: 'input.filename'
    };
    if (options && typeof options == "object")
        $.extend(settings, options);

    var $treeContainer = this.find(settings.treeContainerSelector);

    // set up list container
    var $listContainer = this.find(settings.listContainerSelector);
	
	var $filename = this.find(settings.filenameSelector);

    if (typeof options == "string") {
        var cmdName = options;
        var command = $listContainer.data('jstreelist_commands')[cmdName];
        if (command) {
            var comArgs = ['clientcode'];
            for (var i = 1; i < arguments.length; i++)
                comArgs[i] = arguments[i];
            command.apply($listContainer, comArgs);
        }
        return;
    }

    var commands = {
        showDir: function(invoker, path) {
            var currPath = $listContainer.data('jstreelist_path');
            path = path || currPath;
            if (invoker != 'tree' && path != currPath) {
                var currNode = $treeContainer.jstree('get_selected')[0];
                $treeContainer.jstree('open_node', currNode, function() {
                    $treeContainer.jstree('deselect_node', currNode);
                    $treeContainer.jstree('select_node', "[title='" + path + "']");
                });
            }
            $.getJSON("/L24CM/Ajax/FileTreeFiles",
                    { dir: path },
                    function(list) {
                        if (list.dir == null) return;
                        $listContainer.find('.fileList').remove();
                        $('#fileListTemplate').tmpl(list).addClass('fileList').appendTo($listContainer.find('.file-list-area'));
						$('.file-list-area tr').draggable({
							appendTo: 'body',
							scroll: false,
							opacity: 0.7,
							helper: function() {
								return $(this).find('ins').clone().attr('title', $(this).attr('title'))[0];
								},
							zindex: 999,
							cursorAt: { left: 8 }
							});
                    });
            $listContainer.data('jstreelist_path', path);
        },
        rename: function(invoker, path, newName, rlbk, done) {
            //alert('renaming from ' + invoker + ' of ' + path + ' to ' + newName);
            $.post("/L24CM/Ajax/Rename",
                    { path: path, newName: newName },
                    function(newPath) {
                        if (!newPath) {
                            if (rlbk) rlbk();
                        } else {
                            if (done) done(newPath);
                        }
                    });
        },
		move: function(invoker, path, newDir, rlbk, done) {
            //alert('moving from ' + invoker + ' of ' + path + ' to ' + newDir);
            $.post("/L24CM/Ajax/Move",
                    { path: path, newDir: newDir },
                    function(succeeds) {
                        if (!succeeds) {
                            if (rlbk) rlbk();
                        } else {
                            if (done) done();
                        }
                    });
        },
		remove: function(invoker, path, rlbk, done) {
			$.post("/L24CM/Ajax/Delete",
					{ path: path },
					function(succeeds) {
                        if (!succeeds) {
                            if (rlbk) rlbk();
                        } else {
                            if (done) done();
                        }	
					});
			
		}
    };
    $listContainer.data('jstreelist_commands', commands);
	
	var runRename = function(){
		var $renBox = $listContainer.find('.jstreelist-rename-box');
		if ($renBox.length == 0) return;
		var $span = $renBox.closest('span');
		var path = $span.closest('tr').attr('title');
		var oldText = $renBox.data('oldText');
		var newText = $renBox.val();
	    commands.rename('list', path, newText,
	        function() { $span.remove("input").text(oldText); },
	        function(newPath) {
				$span.remove("input").text(newText).closest('tr').attr('title',newPath);
				});
	}
	
	var showFilename = function (filename) {
		$filename.val(filename);
	}

    // setup list container
	if ($filename.length > 0)
		$('tr', $listContainer[0]).live('click', function() {
	        if ($(this).attr('title'))
	            showFilename($(this).attr('title'));
	    });
		
    $('tr', $listContainer[0]).live('dblclick', function() {
        if ($(this).attr('title'))
            commands.showDir('list', $(this).attr('title'));
    });
    $('span', $listContainer[0]).live('dblclick', function(event) {
        var $span = $(event.currentTarget);
        var text = $span.text();
        var path = $span.closest('tr').attr('title');
		//don't do this for directories
		if (path.charAt(path.length - 1) == '/') return;
        $span.text(" ").append($('<input/>'))
				.find("input").val(text)
				.css({ padding: '0', border: '1px solid silver' })
				.addClass('jstreelist-rename-box')
				.data('oldText', text)
				.click(function (event) {
					event.stopPropagation();
					})
				.blur(runRename)
				.keydown(function(e) { if (e.keyCode == 13) runRename(); });
        event.stopPropagation();
    });


    var uploader = new qq.FileUploader({
        element: $listContainer[0],
        action: '/L24CM/Upload/',
        template: '<div class="qq-upload-drop-area"></div>' +
                        '<div class="file-list-area"></div>' +
                        '<div class="upload-control-area">' +
                        '<div class="qq-upload-button">Upload</div>' +
                        '<ul class="qq-upload-list"></ul>' +
                        '</div>',
        fileTemplate: '<li>' +
                    '<span class="qq-upload-file"></span>' +
                    '<span class="qq-upload-spinner"></span>' +
                    '<span class="qq-upload-size"></span>' +
                    '<a class="qq-upload-cancel" href="#">Cancel</a>' +
                    '<span class="qq-upload-failed-text">Failed</span>' +
                '</li>',
        onSubmit: function(id, filename) {
            $listContainer.find('.qq-upload-list').empty();
            uploader.setParams({ dir: $listContainer.data('jstreelist_path') });
        },
        onComplete: function(id, filename, responseJSON) {
            $listContainer.jstreelist("showDir");
        }
    });

    $treeContainer.jstree({
        plugins: ["themes", "json_data", "ui", "crrm", "dnd"],
        json_data: {
            data: [{
                data: { title: settings.rootPath },
                attr: { title: settings.rootPath },
                state: "closed"
}],
                ajax: {
                    url: "/L24CM/Ajax/FileTreeFolders",
                    data: function(n) {
                        return { dir: n.attr('title') }
                    }
                }
            },
            themes: {
                dots: false
            },
            ui: {
                select_limit: 1
            }
        }).bind('select_node.jstree', function(event, data) {
            var dir = data.rslt.obj.attr('title');
            commands.showDir('tree', dir);
        }).bind("dblclick.jstree", function(event) {
            var node = $(event.target).closest("li");
            $treeContainer.jstree("rename", node);
        }).bind("rename.jstree", function(event, data) {
            var dir = data.rslt.obj.attr('title');
            commands.rename('tree', dir, data.rslt.new_name, function() { $.jstree.rollback(data.rlbk); })
        }).bind("load_node.jstree", function(event, data) {
			var $n = data.args[0];
			if ($n == -1) return;
			$n.find('a').droppable({
				hoverClass: 'jstreelist-folder-active',
				drop: function(event, ui) {
					commands.move('tree', ui.helper.attr('title'), $(this).closest('li').attr('title'),
						function() { alert(getMessage('failedMove')); },
						function() { ui.draggable.remove(); });
				}
			});
		});
		
		var getClickedNode = function($clicked) {
			if ($clicked.is('tr')) return $clicked;
			else return $clicked.parent();
		}
		
		var getMessage = function(msgCode) {
			var msgs = {
				confirmDelete: "Are you sure you want to delete %1",
				failedMove: "Move operation failed"
			};
			return msgs[msgCode];
		}
		
		$.contextMenu({
			selector: settings.listContainerSelector + ' tr,' + settings.treeContainerSelector + ' a',
			items: {
				'delete': {
					name: "Delete",
					callback: function(key, opt){
							var $this = $(this);
							var path = getClickedNode($this).attr('title');
							if (window.confirm(getMessage("confirmDelete").replace("%1", path)))
								commands.remove('menu', path, null,
									function() {
										if ($this.is('tr')) $this.remove();
										else $treeContainer.jstree("remove", $this.parent());
									});
						}
					},
				rename: {
					name: "Rename",
					callback: function(key, opt){
							var $this = $(this);
							if ($this.is('tr')) $this.find('span').dblclick();
							else $treeContainer.jstree("rename", $this.parent());
						}
					},
				properties: {
					name: "Properties",
					callback: function(key, opt){
						alert("Properties " + key);
						}
					}
			}
		});

    };
    //})(jQuery);