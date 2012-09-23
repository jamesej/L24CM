﻿<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%; width: 100%" >
<head runat="server">
        <title>Editor</title>
    <!--<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.5.0/jquery.min.js" type="text/javascript"></script>-->
    <link href="/L24CM/Embedded/Content/jquery.jstreelist.css" rel="stylesheet" type="text/css" />
    <link href="/L24CM/Embedded/Content/jquery.layout.css" rel="stylesheet" type="text/css" />
    <link href="/L24CM/Embedded/Content/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="/L24CM/Embedded/Content/L24Main.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/jquery-ui.js"></script>
    <script src="/L24CM/Embedded/Scripts/jquery.tmpl.js" type="text/javascript"></script>
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/jquery.layout.js"></script>
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/jquery.simplemodal.js"></script>
    <script type="text/javascript" src="/Areas/L24CM/scripts/tiny_mce/jquery.tinymce-applier.js"></script>
    <script src="/L24CM/Embedded/Scripts/fileuploader.js" type="text/javascript"></script>
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/jquery.jstree.js"></script>
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/jquery.jstreelist.js"></script>
    <script type="text/javascript" src="/L24CM/Embedded/Scripts/jquery.contextMenu.js"></script>
    <script>
        $(document).ready(function() {
            var firstReload = true;
            $('#container').layout();
            $('.ui-layout-east').load(function() {
                if (!firstReload)
                    $('.ui-layout-center')[0].contentDocument.location.reload(true);
                firstReload = false;
            });

            $('#_L24FileMgrContainer').jstreelist({ rootPath: '<%= ViewData["FileManagerRoot"] %>' });
            //$('#outer').layout();
        });
        // RTE

        function positionTool(selTool) {
            var $smCont = $('#simplemodal-container');
            var scTop = parseInt($smCont.css('top'), 10);
            var scPadTop = parseInt($smCont.css('padding-top'), 10);
            var scLeft = parseInt($smCont.css('left'), 10)
            var scPadLeft = parseInt($smCont.css('padding-left'), 10);
            $(selTool).css({ top: scTop + scPadTop, left: scLeft + scPadLeft });
            $('.simplemodal-close').css({ top: scTop + scPadTop - 16, left: scLeft + $smCont.width() + scPadLeft });
        }
        $.modal.impl.setPositionBase = $.modal.impl.setPosition;
        $.modal.getContainer = function() { return this.impl.d.container; }
        $.modal.impl.setPosition = function() {
            var s = this;
            s.setPositionBase();
            s.d.container.trigger('move.modal');
        }
        function showHtml(contents, updateHtml) {
            var $rte = $('#_L24RTEContainer').css('display', 'block').find('#_L24RTE_tbl');
            $("<div id='modalPlaceholder' style='background-color: Blue;'></div>")
                .width($rte.width()).height($rte.height())
                .modal({
                    overlayClose: true,
                    onClose: function(dialog) {
                        $('#_L24RTEContainer').css('display', 'none');
                        updateHtml($('textarea#_L24RTE').html());
                        $.modal.getContainer().unbind('move.modal');
                        $.modal.close();
                    }
                });

            $('#_L24RTEContainer').css({ 'z-index': '1010', position: 'fixed' });
            $('.simplemodal-close').css({
                'z-index': '1003', position: 'fixed', display: 'block',
                'background-image': 'url(/l24cm/embedded/Content/Images/close-white.png)',
                width: '16px', height: '16px'});
            positionTool('#_L24RTEContainer');
            $.modal.getContainer().bind('move.modal', function() { positionTool('#_L24RTEContainer'); });
            
            $('textarea#_L24RTE').html(contents);
        }

        // FileMgr

        function getFile(current, updateFile) {
            var $fm = $('#_L24FileMgrContainer').css('display', 'block');
            $fm.find('#outer').layout();
            $fm.css({ 'z-index': '1010', position: 'fixed' });
            $("<div id='modalPlaceholder' style='background-color: White;'></div>")
                .width($fm.width()).height($fm.height())
                .modal({
                    overlayClose: true,
                    onClose: function(dialog) {
                        var msg = updateFile($('#filename').val());
                        if (msg) {
                            alert(msg);
                            this.bindEvents();
                            this.occb = false;
                        } else {
                            $('#_L24FileMgrContainer').css('display', 'none');
                            $.modal.getContainer().unbind('move.modal');
                            $.modal.close();
                        }
                    }
                });

            $('.simplemodal-close').css({
                'z-index': '1003', position: 'fixed', display: 'block',
                'background-image': 'url(/l24cm/embedded/Content/Images/close-white.png)',
                width: '16px', height: '16px'
            });
            positionTool('#_L24FileMgrContainer');
            $.modal.getContainer().bind('move.modal', function() { positionTool('#_L24FileMgrContainer'); });
            
        }
    </script>
    <script id="fileListTemplate" type="text/x-jquery-tmpl">
        <table style='width:300px'>
        <tr><th></th><th>Name</th><th>Size</th></tr>
        {{each dirs}}<tr title='${dir}${name}/'><td class='dir jstree-default'><ins style='width:16px;height:16px;display:inline-block' class='ext ext_dir'/></td><td><span>${name}</span></td><td></td></tr>{{/each}}
        {{each files}}<tr title='${dir}${name}'><td><ins style='width:16px;height:16px;display:inline-block' class='ext ext_${ext}'/></td><td><span>${name}</span></td><td>${size}</td></tr>{{/each}}
        </table>
    </script>
</head>
<body style="height: 100%; width: 100%">
<div id='container' style="height: 100%; width: 100%; position:relative;">
<iframe class="ui-layout-center" src="<%= ViewData["Path"] %>?-mode=view<%= ViewData["AddToQuery"] %>"></iframe>
<iframe class="ui-layout-east" id="editor" src="<%= ViewData["Path"] %>?-action=edit"></iframe>
</div>
<div id='_L24RTEContainer' style='display:none'><textarea id='_L24RTE'>abcdef</textarea></div>
<div id='_L24FileMgrContainer' style='display:none'>
    <div id='outer'>
        <div id='treeContainer' class='treeContainer ui-layout-west'></div>
        <div id='listContainer' class='listContainer ui-layout-center'></div>
    </div>
    <div id='filenameBox'>
        <input id='filename' class='filename' type='text' />
        <div id='fileDetails' class='fileDetails'></div>
    </div>
</div>

</body>
</html>
