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
		code = code.replace(/(\/\*(.|[\r\n])*?\*\/)/g, '<span class="comment">$1</span>');
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
		return code.replace(/(as|component|configuration|connect|consider|container|default|fun|interpret|let|lsystem|nothing|or|process|return|rewrite|set|this|to|typeof|use|weight|with|where)([ \r\n\t])/g, '<span class="keyword">$1</span>$2');
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

	function urlEncodeLsystem(code) {

		code = urlEncode(code);
		code = code.replace(/\t/g, "++++");
		code = code.replace(/ /g, "+");
		code = code.replace(/\r\n|\n|\r/g, "%0A");
		//code = encodeURI(code);
		return code;
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
		var encodedLsystem = urlEncodeLsystem(lsystemCode);
		var tryMeHtml = '<a href="/Process?lsystem=' + encodedLsystem + '" class="lsysTryMe">Try me!</a>';
		$(this).append(tryMeHtml);

		// format code
		var newHtml = highlightLsystemKeywords(lsystemCode);
		newHtml = highlightComments(newHtml);
		newHtml = replaceTabs(newHtml);


		if ($(this).hasClass("collapsable")) {
			var colapseHtml = '<div class="collapseSwitch"><span class="u">Show</span><span class="c">Hide</span></div>';
			$(this).append(colapseHtml);
		}

		// clean tags from comments
		$(this).find("span.comment").each(function (i) {
			$(this).text($(this).text());
		});

		$(this).append(newHtml);

	});

	$("code.malsys").each(function (i) {

		var newHtml = $(this).text();
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

