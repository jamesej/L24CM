using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("L24CM")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("L24CM")]
[assembly: AssemblyCopyright("Copyright Regulus Systemt Ltd©  2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("02cdda62-5096-4a6d-bf39-518bebbf7864")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// Web Resources
[assembly: WebResource("L24CM.Scripts.L24Main.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.L24Editor.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.L24Admin.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.L24Controls.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.L24EditPanel.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.jquery.jstree.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.jquery.jstreelist.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.jquery.layout.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.jquery-ui.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.fileuploader.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.jquery.tmpl.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.jquery.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.jquery.contextMenu.js", "application/javascript")]
[assembly: WebResource("L24CM.Scripts.jquery.simplemodal.js", "application/javascript")]


[assembly: WebResource("L24CM.Content.L24Main.css", "text/css")]
[assembly: WebResource("L24CM.Content.jquery.jstreelist.css", "text/css")]
[assembly: WebResource("L24CM.Content.jquery.layout.css", "text/css")]
[assembly: WebResource("L24CM.Content.fileuploader.css", "text/css")]
[assembly: WebResource("L24CM.Scripts.themes.default.style.css", "text/css")]
[assembly: WebResource("L24CM.Content.jquery.contextMenu.css", "text/css")]
[assembly: WebResource("L24CM.Content.jquery-ui.css", "text/css")]
[assembly: WebResource("L24CM.Content.l24cm.ui.css", "text/css")]

[assembly: WebResource("L24CM.Views.TI.aspx", "text/html")]

[assembly: WebResource("L24CM.Content.Images.application.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.close-white.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.code.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.css.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.cut.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.db.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.directory.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.doc.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.door.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.dropdownarrow.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.file.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.film.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.flash.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.folder_open.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.html.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.java.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.linux.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.loading.gif", "image/gif")]
[assembly: WebResource("L24CM.Content.Images.music.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.page_white_copy.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.page_white_delete.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.page_white_edit.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.page_white_paste.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.pdf.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.php.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.picture.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ppt.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.psd.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ruby.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.script.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.spinner.gif", "image/gif")]
[assembly: WebResource("L24CM.Content.Images.txt.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-bg_diagonals-thick_18_b81900_40x40.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-bg_diagonals-thick_20_666666_40x40.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-bg_flat_10_000000_40x100.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-bg_glass_65_ffffff_1x400.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-bg_glass_100_f6f6f6_1x400.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-bg_glass_100_fdf5ce_1x400.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-bg_gloss-wave_35_f6a828_500x100.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-bg_highlight-soft_75_ffe45c_1x100.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-bg_highlight-soft_100_eeeeee_1x100.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-icons_228ef1_256x240.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-icons_222222_256x240.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-icons_ef8c08_256x240.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-icons_ffd27a_256x240.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.ui-icons_ffffff_256x240.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.xls.png", "image/png")]
[assembly: WebResource("L24CM.Content.Images.zip.png", "image/png")]
[assembly: WebResource("L24CM.Scripts.themes.default.d.png", "image/png")]
[assembly: WebResource("L24CM.Scripts.themes.default.d.gif", "image/gif")]
[assembly: WebResource("L24CM.Scripts.themes.default.throbber.gif", "image/gif")]
