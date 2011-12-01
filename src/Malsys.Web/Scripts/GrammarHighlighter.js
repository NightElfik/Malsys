


(function ($) {

	function addRefLinks(code) {
		return code.replace(/(.)([_a-z]+)/gi, "$1<a href=\"#grammar-$2\">$2</a>");
	}

	function highlightTerminals(code) {

		code = code.replace(/('.[^']*')/g, "<span class=\"gr-terminal\">$1</span>");
		code = code.replace(/\[(.)\-(.)\]/g, "[<span class=\"gr-terminal\">$1</span>-<span class=\"gr-terminal\">$2</span>]");
		code = code.replace(/\[([^'\-<\]]+)\]/g, "[<span class=\"gr-terminal\">$1</span>]");
		// quantifiers needs to be after terminals because of quatificator after terminal
		code = code.replace(/([^'])([?+*])/g, "$1<span class=\"gr-quantifier\">$2</span>");
		return code;
	}

	function replaceTabs(code) {
		return code.replace(/\t/g, "&nbsp;&nbsp;&nbsp;&nbsp;");
	}

	function replaceSpaces(code) {
		return code.replace(/ /g, "&nbsp;");
	}

	function highlightLsystemKeywords(code) {
		return code.replace(/(lsystem|rewrite|to|let|set)/g, "<span class=\"lsys_kw\">$1</span>");
	}

	function compressLsystem(code, wsChar, nlChar) {

		var wsTab = wsChar + wsChar + wsChar + wsChar;
		code = code.replace(/\t/g, wsTab);
		code = code.replace(/ /g, wsChar);
		code = code.replace(/\r\n|\n|\r/g, nlChar);
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

		var lsystemCode = $(this).text();
		$(this).text("");

		// add try-me link
		var compressedLsystem = compressLsystem(lsystemCode, "+", "~");
		var tryMeHtml = '<a href="/ProcessLsystem/Text?nl=~&lsystem=' + compressedLsystem + '" class="lsysTryMe">Try me!</a>';
		$(this).append(tryMeHtml);

		// format code
		var newHtml = highlightLsystemKeywords(lsystemCode);
		newHtml = replaceTabs(newHtml);
		$(this).append(newHtml);
	});

	$("code.malsys").each(function (i) {

		var newHtml = $(this).text();
		newHtml = highlightLsystemKeywords(newHtml);
		newHtml = replaceTabs(newHtml);

		$(this).text("");
		$(this).append(newHtml);
	});

} (jQuery));

