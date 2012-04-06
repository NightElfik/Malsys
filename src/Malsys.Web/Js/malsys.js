/// <reference path="~/Js/jquery.js" />

(function ($) {

	function addRefLinks(code) {
		return code.replace(/(.)([_a-z]+)/gi, '$1<a href="#grammar-$2">$2</a>');
	}

	function highlightTerminals(code) {

		code = code.replace(/('.[^']*')/g, '<span class="grTerminal">$1</span>');
		code = code.replace(/\[(.)\-(.)\]/g, '[<span class="grTerminal">$1</span>-<span class="grTerminal">$2</span>]');
		code = code.replace(/\[([^'\-<\]]+)\]/g, '[<span class="grTerminal">$1</span>]');
		// quantifiers needs to be after terminals because of quantifiers after terminal
		code = code.replace(/([^'])([?+*])/g, '$1<span class="grQuantifier">$2</span>');
		return code;
	}

	function highlightComments(code) {
		code = code.replace(/(\/\*(.|\s)*?\*\/)/g, '<span class="comment">$1</span>');
		code = code.replace(/(\/\/.*)/g, '<span class="comment">$1</span>');
		return code;
	}

	function replaceTabs(code) {
		return code.replace(/\t/g, "&nbsp;&nbsp;&nbsp;&nbsp;");
	}

	function replaceSpaces(code) {
		return code.replace(/ /g, "&nbsp;");
	}

	function highlightLsystemKeywords(code) {
		return (' ' + code + ' ').replace(/(\s)(all|as|component|configuration|connect|consider|container|default|fun|interpret|let|lsystem|nothing|or|process|return|rewrite|set|symbols|this|to|typeof|use|virtual|weight|with|where)(?=\s)/g, '$1<span class="keyword">$2</span>').trim();
	}

	function urlEncode(str) {
		str = str.replace(/[%]/g, "%25");  // % has to be first
		str = str.replace(/["]/g, "%22");
		str = str.replace(/[#]/g, "%23");
		str = str.replace(/[$]/g, "%24");
		str = str.replace(/[&]/g, "%26");
		str = str.replace(/[+]/g, "%2B");
		str = str.replace(/[,]/g, "%2C");
		str = str.replace(/\//g, "%2F");
		str = str.replace(/[:]/g, "%3A");
		str = str.replace(/[=]/g, "%3D");
		str = str.replace(/[?]/g, "%3F");
		str = str.replace(/[@]/g, "%40");
		return str;
	}

	function htmlEncode(str) {
		str = str.replace(/[&]/g, "&amp;");
		str = str.replace(/["]/g, "&quot;");
		str = str.replace(/[']/g, "&#39;"); // &apos; does not work in IE
		str = str.replace(/[<]/g, "&lt;");
		str = str.replace(/[>]/g, "&gt;");
		return str;
	}


	var toc = $("#toc");
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



	$("pre.grammar").each(function (i) {

		$(this).html(addRefLinks($(this).html()));

		// clean links with invalid href
		$(this).find("a").each(function (i) {
			if ($($(this).attr("href")).length == 0) {
				$(this).replaceWith($(this).text());
			}
		});

		var newHtml = highlightTerminals($(this).html());
		newHtml = replaceTabs(newHtml);
		$(this).html(newHtml);
	});

	$("code.grammar").each(function (i) {

		var newHtml = $(this).text();
		newHtml = highlightTerminals(newHtml);
		newHtml = replaceTabs(newHtml);

		$(this).text("");
		$(this).append(newHtml);

	});

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
		code = replaceTabs(code);

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
		newHtml = replaceTabs(newHtml);

		$(this).text("");
		$(this).append(newHtml);
	});

	$("p.malsysMsg").each(function (i) {

		var newHtml = $(this).text().replace(/</g, "&lt;").replace(/>/g, "&gt;");

		newHtml = newHtml.replace(/`(([a-zA-Z0-9\+]+\.)+([a-zA-Z0-9\+]+))`/g, '`<abbr title="$1">$3</abbr>`');
		newHtml = newHtml.replace(/`(.+?)`/g, '`<span class="quoted">$1</span>`');


		$(this).text("");
		$(this).append(newHtml);
	});

	$(".collapseSwitch").click(function () {
		$(this).toggleClass("collapsed").parent().toggleClass("collapsed");
	});

	$(".collapseSwitch").trigger('click');



} (jQuery));

