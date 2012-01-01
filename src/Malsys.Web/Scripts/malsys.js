/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

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
		return (' '+code).replace(/(\s)(as|component|configuration|connect|consider|container|default|fun|interpret|let|lsystem|nothing|or|process|return|rewrite|set|this|to|typeof|use|weight|with|where)(?=\s)/g, '$1<span class="keyword">$2</span>').trim();
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
		var newHtml = highlightLsystemKeywords(lsystemCode);
		newHtml = highlightComments(newHtml);
		newHtml = replaceTabs(newHtml);


		if ($(this).hasClass("collapsable")) {
			var colapseHtml = '<div class="collapseSwitch"><span class="u">Show</span><span class="c">Hide</span></div>';
			$(this).append(colapseHtml);
		}

		$(this).append(newHtml);

		// clean tags from comments
		$(this).find("span.comment").each(function (i) {
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

		var newHtml = $(this).text();
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

