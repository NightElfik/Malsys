


(function ($) {

	function addRefLinks(code) {
		return code.replace(/ ([_a-z]+)/gi, " <a href=\"#grammar-$1\">$1</a>");
	}

	function highlightTerminals(code) {
		var code = code.replace(/('.[^']*')/gi, "<span class=\"gr-terminal\">$1</span>");
		var code = code.replace(/([?+*])/gi, "<span class=\"gr-quantifier\">$1</span>");
		return code.replace(/(\[[^'\]]*\])/gi, "<span class=\"gr-terminal\">$1</span>");
	}

	$("pre.grammar").each(function (i) {

		$(this).html(addRefLinks($(this).html()));

		// clean links with invalid href
		$(this).find("a").each(function (i) {
			if ($($(this).attr("href")).length == 0) {
				$(this).replaceWith($(this).text());
			}
		});

		$(this).html(highlightTerminals($(this).html()));

	});

} (jQuery));

