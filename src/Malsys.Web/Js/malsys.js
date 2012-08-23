/// <reference path="~/Js/jquery.js" />
/**
* Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
* All rights reserved.
*/

(function ($) {

	$.fn.disable = function () {
		return $(this).each(function () {
			$(this).attr('disabled', 'disabled');
		});
	};

	$.fn.enable = function () {
		return $(this).each(function () {
			$(this).removeAttr('disabled');
		});
	};

	$.fn.extend({
		insertAtCaret: function (myValue) {
			return this.each(function (i) {
				if (document.selection) {
					this.focus();
					sel = document.selection.createRange();
					sel.text = myValue;
					this.focus();
				}
				else if (this.selectionStart || this.selectionStart == '0') {
					var startPos = this.selectionStart;
					var endPos = this.selectionEnd;
					var scrollTop = this.scrollTop;
					this.value = this.value.substring(0, startPos) + myValue + this.value.substring(endPos, this.value.length);
					this.focus();
					this.selectionStart = startPos + myValue.length;
					this.selectionEnd = startPos + myValue.length;
					this.scrollTop = scrollTop;
				} else {
					this.value += myValue;
					this.focus();
				}
			});
		}
	})


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


	var toc = $('#toc:empty');
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


	$('pre.malsys').each(function (index) {

		var lsystemCode = $(this).text().trim();
		if (lsystemCode.length == 0) {
			return;
		}

		$(this).text('');

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


		if ($(this).hasClass('collapsable')) {
			var colapseHtml = '<div class="collapseSwitch"><span class="u">Show</span><span class="c">Hide</span></div>';
			$(this).append(colapseHtml);
		}

		$(this).append(code);

		// clean tags from comments and unimportant code
		$(this).find('span.comment').each(function (i) {
			$(this).text($(this).text());
		});
		$(this).find('span.unimportant').each(function (i) {
			$(this).text($(this).text());
		});

	});

	$('code.malsys').each(function (index) {

		var newHtml = $(this).text().trim();
		newHtml = highlightLsystemKeywords(newHtml);
		//newHtml = replaceTabs(newHtml);

		$(this).text('');
		$(this).append(newHtml);
	});

	$('.collapseSwitch').click(function () {
		$(this).toggleClass('collapsed').parent().toggleClass('collapsed');
	});

	$('.collapseSwitch').trigger('click');


	$('.inlineDoc').each(function (index) {
		var container = $(this);
		var configNames = container.attr('data-config-names').split(';');
		var fetchUrl = container.attr('data-fetch-url');

		$.each(configNames, function (configNameIndex, configName) {
			var showBtn = $('<a href="#">Inline doc for <b>' + configName + '</b> <span class="note">[click to toggle]</span></a>');
			var header = $('<div class="clearfix" />').append(showBtn);
			var content = $('<div class="inlineDoc" />');

			var showed = false;
			var contentFetched = false;

			var ajaxLoader = $('<div><p>Loading ...</p></div>');

			showBtn.click(function () {
				if (showed) {
					content.hide();
				}
				else {
					content.show();
					if (!contentFetched) {
						$.ajax({
							type: 'POST',
							url: fetchUrl,
							cache: true,
							data: {
								processConfigName: configName
							},
							beforeSend: function () {
								showBtn.disable();
								container.append(ajaxLoader);
							},
							success: function (recvData) {
								//console.log(recvData);
								showInlineDocData(content, recvData)
							},
							complete: function () {
								showBtn.enable();
								ajaxLoader.remove();
							}
						});
						contentFetched = true;
					}
				}
				showed = !showed;
				return false;
			});

			container.append(header);
			container.append(content);
		});

	});

	function showInlineDocData(container, data) {

		var mainTextarea = $('#SourceCode');
		var infoDiv = $('<div class="popup" style="position:absolute; width: 350px;" />');

		var settablePropTmpl = '<p><b>${name}</b> [${component}]</p><p>Value type (constant or array): <b>${valTyle}</b></p>'
			+ '<p>Default value: <b>${defaultVal}</b></p><p>Expected value: <b>${expectedVal}</b></p><hr /><p>${doc}</p><hr />'
			+ '<p class="note">Click to insert: ${textOnClick}</p>';
		infoDiv.hide();
		var settablePropList = $('<ul><li>Settable properties: </li></ul>');

		$.each(data, function (i, componentData) {
			$.each(componentData.SettableProperties, function (j, setProp) {
				$.each(setProp.Names, function (k, name) {
					var link = $('<a href="#">' + name + '</a>')
					settablePropList.append($('<li>, </li>').prepend(link));
					var textOnClick = 'set ' + name + ' = ' + setProp.TypicalValue + ';';

					link.mouseover(function () {
						infoDiv.text('');
						infoDiv.append($.tmpl(settablePropTmpl, {
							name: name,
							component: componentData.ComponentType,
							valTyle: setProp.ValueType,
							defaultVal: setProp.DefaultValue,
							expectedVal: setProp.ExpectedValue,
							doc: setProp.Doc,
							textOnClick: textOnClick
						}));
						showPopup(infoDiv, link);
					});
					link.mouseout(function () {
						infoDiv.hide();
					});
					link.click(function () {
						if(mainTextarea){
							mainTextarea.insertAtCaret(textOnClick);
						}
						return false;
					});
				});
			});
		});

		var gettablePropTmpl = '<p><b>${name}</b> [${component}]</p><p>Value type (constant or array): <b>${valTyle}</b></p>'
			+ '<hr /><p>${doc}</p>';
		infoDiv.hide();
		var gettablePropList = $('<ul><li>Gettable properties: </li></ul>');

		$.each(data, function (i, componentData) {
			$.each(componentData.GettableProperties, function (j, getProp) {
				$.each(getProp.Names, function (k, name) {
					var link = $('<a href="#">' + name + '</a>')
					gettablePropList.append($('<li>, </li>').prepend(link));

					link.mouseover(function () {
						infoDiv.text('');
						infoDiv.append($.tmpl(gettablePropTmpl, {
							name: name,
							component: componentData.ComponentType,
							valTyle: getProp.ValueType,
							doc: getProp.Doc
						}));
						showPopup(infoDiv, link);
					});
					link.mouseout(function () {
						infoDiv.hide();
					});
					link.click(function () {
						mainTextarea.insertAtCaret(name);
						return false;
					});
				});
			});
		});

		container.append(settablePropList);
		container.append(gettablePropList);
		container.append(infoDiv);
	}

	function showPopup(element, parent) {
		var offset = parent.offset();
		var width = element.outerWidth();
		var height = element.outerHeight();
		var x = offset.left - width / 2 + parent.outerWidth() / 2;
		var minX = 8;
		var maxX = $(window).width() - width - 8;
		element.css({ 'top': Math.max(10, offset.top - height), 'left': Math.min(Math.max(minX, x), maxX) });
		element.show();
	}

	$('.discusAddMsg').each(function (i) {

		var container = $(this);
		container.text('');  // erase warning for non-JS users

		var addMsgUrl = container.attr('data-url');
		var threadId = parseInt(container.attr('data-thread-id'));
		var loggedUser = container.attr('data-user') == 'true';

		if (!addMsgUrl || !(threadId >= 0)) {  // '>=' operator to catch NaNs
			return;
		}

		var addCommentButton = $('<input type="submit" value="Add comment" />');
		container.append(addCommentButton);

		addCommentButton.click(function () {
			$(this).hide();

			var textArea = $('<textarea class="deleteMe" cols="60" rows="4" />');
			var tbName = $('<input id="userName" name="userName" type="text" />');
			var submitButton = $('<input type="submit" name="submit" value="Submit comment" />');
			var messageArea = $('<p style="white-space: pre-line;"></p>');

			submitButton.click(function () {

				var name = tbName.val();
				var msg = textArea.val();
				if (!msg || (!loggedUser && !name)) {
					messageArea.text('Please fill all fields.');
					return;
				}

				$.ajax({
					type: 'POST',
					url: addMsgUrl,
					data: {
						ThreadId: threadId,
						Message: msg,
						AuthorNameNonRegistered: name
					},
					beforeSend: function () {
						submitButton.disable();
						messageArea.text('');
					},
					success: function (recvData) {
						//console.log(recvData);
						if (recvData.error) {
							// show error message
							messageArea.text('Failed to add the comment.\n' + recvData.message);
						}
						else {
							$('.deleteMe').remove();
							messageArea.text('Your comment was successfully added (refresh the page if you want to see it).');
							location.reload();
						}
					},
					error: function (jqXHR, textStatus, errorThrown) {
						messageArea.text(errorThrown);
					},
					complete: function () {
						submitButton.enable();
					}
				});
			});

			container.append(messageArea);
			if (!loggedUser) {
				container.append($('<p class="deleteMe" />').html($('<label for="userName">Name: </label>').append(tbName)));
			}
			container.append(textArea);
			container.append($('<p class="deleteMe" />').html(submitButton))
		});
	});

	$('.discusNewThread').each(function (i) {

		var container = $(this);
		container.text('');  // erase warning for non-JS users

		var newThreadUrl = container.attr('data-new-thread-url');
		var showThreadUrl = container.attr('data-show-thread-url');
		var catId = container.attr('data-category-id');
		var loggedUser = container.attr('data-user') == 'true';

		if (!newThreadUrl || !showThreadUrl) {
			return;
		}

		var newThreadButton = $('<input type="submit" value="Create new thread" />');
		container.append(newThreadButton);

		newThreadButton.click(function () {
			$(this).hide();

			var tbTitle = $('<input id="threadTitle" type="text" />');
			var tbName = $('<input id="userName" type="text" />');
			var textArea = $('<textarea class="deleteMe" cols="60" rows="4" />');
			var submitButton = $('<input type="submit" name="submit" value="Create thread" />');
			var messageArea = $('<p style="white-space: pre-line;"></p>');

			submitButton.click(function () {

				var title = tbTitle.val();
				var name = tbName.val();
				var msg = textArea.val();
				if (!title || !msg || (!loggedUser && !name)) {
					messageArea.text('Please fill all fields.');
					return;
				}

				$.ajax({
					type: 'POST',
					url: newThreadUrl,
					data: {
						CategoryId: catId,
						Title: title,
						FirstMessage: msg,
						AuthorNameNonRegistered: name
					},
					beforeSend: function () {
						submitButton.disable();
						messageArea.text('');
					},
					success: function (recvData) {
						//console.log(recvData);
						if (recvData.error) {
							// show error message
							messageArea.text('Failed to add the comment.\n' + recvData.message);
						}
						else {
							var url = showThreadUrl.replace('-1', recvData.threadId);
							window.location.href = showThreadUrl.replace('-1', recvData.threadId);
							// if redirect is not working, show link and remove used elements
							$('.deleteMe').remove();
							messageArea.html('Thread was successfully added, failed to redirect to: <a href="' + url + '">' + url + "</a>");
						}
					},
					error: function (jqXHR, textStatus, errorThrown) {
						messageArea.text(errorThrown);
					},
					complete: function () {
						submitButton.enable();
					}
				});
			});

			container.append(messageArea);
			container.append($('<p class="deleteMe" />').html($('<label for="threadTitle">Title: </label>').append(tbTitle)));
			if (!loggedUser) {
				container.append($('<p class="deleteMe" />').html($('<label for="userName">Name: </label>').append(tbName)));
			}
			container.append(textArea);
			container.append($('<p class="deleteMe" />').html(submitButton))
		});
	});


} (jQuery));

