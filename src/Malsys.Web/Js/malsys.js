
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

	$('.flipContainer').each(function (i, ele) {
		var container = $(ele);
		var flipper = container.find('.flipper');
		var front = container.find('.front');
		var back = container.find('.back');
		var fontFlipLink = front.find('.flipLink');
		var backFlipLink = back.find('.flipLink');

		fontFlipLink.click(function () {
			fontFlipLink.hide();
			back.show();
			flipper.addClass('flipped');
			// Swap for browsers that did not perform animation.
			window.setTimeout(function () { front.hide(); backFlipLink.show(); }, 600);
		});

		backFlipLink.click(function () {
			backFlipLink.hide();
			front.show();
			flipper.removeClass('flipped');
			// Swap for browsers that did not perform animation.
			window.setTimeout(function () { back.hide(); fontFlipLink.show(); }, 600);
		});

		back.hide();
		backFlipLink.hide();
		fontFlipLink.show();
	});


} (jQuery));

