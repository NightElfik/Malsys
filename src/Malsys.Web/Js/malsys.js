/**
* Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
* All rights reserved.
*/
/// <reference path="~/Js/jquery.js" />

(function ($) {

	function highlightComments(code) {
		code = code.replace(/(\/\*(.|\s)*?\*\/)/g, '<span class="comment">$1</span>');
		code = code.replace(/(\/\/.*)/g, '<span class="comment">$1</span>');
		return code;
	}

	function highlightLsystemKeywords(code) {
		return (' ' + code + ' ').replace(/(\s)(abstract|all|as|component|configuration|connect|consider|container|default|extends|fun|interpret|let|lsystem|nothing|or|process|return|rewrite|set|symbols|this|to|typeof|use|virtual|weight|with|where)(?=\s)/g, '$1<span class="keyword">$2</span>').trim();
	}

	function htmlEncode(str) {
		str = str.replace(/[&]/g, "&amp;");
		str = str.replace(/["]/g, "&quot;");
		str = str.replace(/[']/g, "&#39;"); // &apos; does not work in IE
		str = str.replace(/[<]/g, "&lt;");
		str = str.replace(/[>]/g, "&gt;");
		return str;
	}


	var toc = $("#toc:empty");
	if (toc !== undefined) {
		toc.append('<div class="collapseSwitch"><span class="u">Table of contents [show]</span><span class="c">Table of contents [hide]</span></div>');
		toc.append('<ul>');

		$('h3').each(function (i) {
			var text = $(this).text();
			var id = $(this).attr('id');
			if (id === undefined) {
				id = text;
				$(this).attr('id', id);
			}
			toc.append('<li><a href="#' + id + '">' + text + '</a></li>');
		});

		toc.append('</ul>');
	}


	$("pre.malsys").each(function (i) {

		var lsystemCode = $(this).text().trim();
		if (lsystemCode.length == 0) {
			return;
		}

		$(this).text("");

		// add try-me link
		var tryMeHtml = '<form action="/Process" method="post"><input type="submit" class="lsysTryMe" name="Process" value="Try me!" />'
			+ '<input name="SourceCode" type="hidden" value="' + htmlEncode(lsystemCode) + '"></form>';
		$(this).append(tryMeHtml);

		// format code
		var code = highlightLsystemKeywords(lsystemCode);
		code = highlightComments(code);

		if ($(this).attr("data-unimportant-lines")) {
			var lines = code.split(/\n/);
			var indices = $(this).attr("data-unimportant-lines").split(/[ ]+/);
			for (var i = 0; i < indices.length; i++) {
				var index = indices[i] * 1;
				if (index > 0) {
					index--;
				}
				else {
					index += lines.length;
				}
				if (index >= 0 || index < lines.length) {
					lines[index] = '<span class="unimportant">' + lines[index] + '</span>';
				}
			}
			code = lines.join("\n");
		}


		if ($(this).hasClass("collapsable")) {
			var colapseHtml = '<div class="collapseSwitch"><span class="u">Show</span><span class="c">Hide</span></div>';
			$(this).append(colapseHtml);
		}

		$(this).append(code);

		// clean tags from comments and unimportant code
		$(this).find("span.comment").each(function (i) {
			$(this).text($(this).text());
		});
		$(this).find("span.unimportant").each(function (i) {
			$(this).text($(this).text());
		});

	});

	$("code.malsys").each(function (i) {

		var newHtml = $(this).text().trim();
		newHtml = highlightLsystemKeywords(newHtml);
		//newHtml = replaceTabs(newHtml);

		$(this).text("");
		$(this).append(newHtml);
	});

	$(".collapseSwitch").click(function () {
		$(this).toggleClass("collapsed").parent().toggleClass("collapsed");
	});

	$(".collapseSwitch").trigger('click');

//	$('form').submit(function () {
//		$(':submit', this).each(function () {
//			$(this).attr("disabled", true);
//		});
//	});


} (jQuery));

