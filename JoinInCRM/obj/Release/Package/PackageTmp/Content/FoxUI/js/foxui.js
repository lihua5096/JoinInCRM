var FoxUI = {
	defaults: {
		confirmText:"确认",
		cancelText: "取消",
		loaderText: "处理中......"
	}
};

//support
$.supports = (function() {
	var supports = {
		touch: !!(('ontouchstart' in window) || window.DocumentTouch && document instanceof window.DocumentTouch)
	};
	return supports;
})();

$.touchEvents = {
	start: $.supports.touch ? 'touchstart' : 'mousedown',
	move: $.supports.touch ? 'touchmove' : 'mousemove',
	end: $.supports.touch ? 'touchend' : 'mouseup'
};

function _bindCssEvent(events, callback) {
	var dom = this;  
	function fireCallBack(e) {
 
		if (e.target !== this) {
			return;
		}
		callback.call(this, e);
		for (var i = 0; i < events.length; i++) {
			dom.off(events[i], fireCallBack);
		}
	}
	if (callback) {
		for (var i = 0; i < events.length; i++) {
			dom.on(events[i], fireCallBack);
		}
	}
}
$.fn.animationEnd = function(callback) {
	 
	_bindCssEvent.call(this, ['webkitAnimationEnd', 'animationend'], callback);
	return this;
};
$.fn.transitionEnd = function(callback) {
	_bindCssEvent.call(this, ['webkitTransitionEnd', 'transitionend'], callback);
	return this;
};
$.fn.transition = function(duration) {
	if (typeof duration !== 'string') {
		duration = duration + 'ms';
	}
	for (var i = 0; i < this.length; i++) {
		var elStyle = this[i].style;
		elStyle.webkitTransitionDuration = elStyle.MozTransitionDuration = elStyle.transitionDuration = duration;
	}
	return this;
};
$.fn.transform = function(transform) {
	for (var i = 0; i < this.length; i++) {
		var elStyle = this[i].style;
		elStyle.webkitTransform = elStyle.MozTransform = elStyle.transform = transform;
	}
	return this;
};

$.getTranslate = function(el, axis) {
	var matrix, curTransform, curStyle, transformMatrix;
	if (typeof axis === 'undefined') {
		axis = 'x';
	}
	curStyle = window.getComputedStyle(el, null);
	if (window.WebKitCSSMatrix) {
		transformMatrix = new WebKitCSSMatrix(curStyle.webkitTransform === 'none' ? '' : curStyle.webkitTransform);
	} else {
		transformMatrix = curStyle.MozTransform || curStyle.transform || curStyle.getPropertyValue('transform').replace('translate(', 'matrix(1, 0, 0, 1,');
		matrix = transformMatrix.toString().split(',');
	}

	if (axis === 'x') {
		if (window.WebKitCSSMatrix)
			curTransform = transformMatrix.m41;
		else if (matrix.length === 16)
			curTransform = parseFloat(matrix[12]);
		else
			curTransform = parseFloat(matrix[4]);
	}
	if (axis === 'y') {
		if (window.WebKitCSSMatrix)
			curTransform = transformMatrix.m42;
		else if (matrix.length === 16)
			curTransform = parseFloat(matrix[13]);
		else
			curTransform = parseFloat(matrix[5]);
	}

	return curTransform || 0;
};

$.requestAnimationFrame = function(callback) {
	if (requestAnimationFrame)  {
		return requestAnimationFrame(callback);
	}
	else if (webkitRequestAnimationFrame)  {
		return webkitRequestAnimationFrame(callback);
	}
	else if (mozRequestAnimationFrame)  {
		return mozRequestAnimationFrame(callback);
	}
	else {
		return setTimeout(callback, 1000 / 60);
	}
};
$.cancelAnimationFrame = function(id) {
	if (cancelAnimationFrame) {
		return cancelAnimationFrame(id);
	}
	else if (webkitCancelAnimationFrame)  {
		return webkitCancelAnimationFrame(id);
	}
	else if (mozCancelAnimationFrame)  { 
		return mozCancelAnimationFrame(id);
	}
	else {
		return clearTimeout(id);
	}
};

$.device = (function() {
	var device = {};
	var ua = navigator.userAgent;
	var android = ua.match(/(Android);?[\s\/]+([\d.]+)?/);
	var ipad = ua.match(/(iPad).*OS\s([\d_]+)/);
	var ipod = ua.match(/(iPod)(.*OS\s([\d_]+))?/);
	var iphone = !ipad && ua.match(/(iPhone\sOS)\s([\d_]+)/);
	device.ios = device.android = device.iphone = device.ipad = device.androidChrome = false;
	if (android) {
		device.os = 'android';
		device.osVersion = android[2];
		device.android = true;
		device.androidChrome = ua.toLowerCase().indexOf('chrome') >= 0;
	}
	if (ipad || iphone || ipod) {
		device.os = 'ios';
		device.ios = true;
	}
	if (iphone && !ipod) {
		device.osVersion = iphone[2].replace(/_/g, '.');
		device.iphone = true;
	}
	if (ipad) {
		device.osVersion = ipad[2].replace(/_/g, '.');
		device.ipad = true;
	}
	if (ipod) {
		device.osVersion = ipod[3] ? ipod[3].replace(/_/g, '.') : null;
		device.iphone = true;
	}
	if (device.ios && device.osVersion && ua.indexOf('Version/') >= 0) {
		if (device.osVersion.split('.')[0] === '10') {
			device.osVersion = ua.toLowerCase().split('version/')[1].split(' ')[0];
		}
	}
	device.webView = (iphone || ipad || ipod) && ua.match(/.*AppleWebKit(?!.*Safari)/i);

	if (device.os && device.os === 'ios') {
		var osVersionArr = device.osVersion.split('.');
		device.minimalUi = !device.webView &&
			(ipod || iphone) &&
			(osVersionArr[0] * 1 === 7 ? osVersionArr[1] * 1 >= 1 : osVersionArr[0] * 1 > 7) &&
			$('meta[name="viewport"]').length > 0 && $('meta[name="viewport"]').attr('content').indexOf('minimal-ui') >= 0;
	}

	var windowWidth = $(window).width();
	var windowHeight = $(window).height();
	device.statusBar = false;
	if (device.webView && (windowWidth * windowHeight === screen.width * screen.height)) {
		device.statusBar = true;
	} else {
		device.statusBar = false;
	}

	var classNames = [];

	device.pixelRatio = window.devicePixelRatio || 1;
	classNames.push('pixel-ratio-' + Math.floor(device.pixelRatio));
	if (device.pixelRatio >= 2) {
		classNames.push('retina');
	}

	if (device.os) {
		classNames.push(device.os, device.os + '-' + device.osVersion.split('.')[0], device.os + '-' + device.osVersion.replace(/\./g, '-'));
		if (device.os === 'ios') {
			var major = parseInt(device.osVersion.split('.')[0], 10);
			for (var i = major - 1; i >= 6; i--) {
				classNames.push('ios-gt-' + i);
			}
		}

	}
	device.isWeixin = /MicroMessenger/i.test(ua);
	return device;
})();

FoxUI.mask = {
	transparent: false,
	show: function(transparent) {
		this.transparent = transparent;
		$('.fui-mask').remove();
		var mask = $("<div class='fui-mask " + (transparent ? 'fui-mask-transparent' : '') + "'></div>");
		$(document.body).append(mask);
		mask.show().addClass('visible');
	},
	hide: function() {
		if (this.transparent) {
			$('.fui-mask').remove();
			return;
		}
		$('.fui-mask').removeClass('visible').transitionEnd(function(e) {
			$('.fui-mask').remove();
		});
	}
}

FoxUI.toast = {
	show: function(text, center, duration) {
		var toast = $("<div class='fui-toast'><span class='fui-toast-text'>" + text + "</span></div>");
		$(document.body).append(toast);
		toast.show();
		if (center) {
			toast.css({
				'top': '50%',
				'margin-top': -toast.outerHeight() + 'px'
			});
		} else {
			toast.css({
				'bottom': '3rem'
			});

		}
		toast.addClass('in');
		setTimeout(function() {
			FoxUI.toast.hide();
		}, duration || 2000);
	},
	hide: function() {
		$('.fui-toast').addClass('out').transitionEnd(function(e) {
			$('.fui-toast').remove();
		});
	}
}
FoxUI.alert = function(text, title, callback) {

	FoxUI.dialog({
		title: title,
		text: text,
		buttons: [{
			text: FoxUI.defaults.confirmText,
			onclick: callback
		}]
	});
}
FoxUI.confirm = function(text, title, confirmCallback, cancelCallback) {

	FoxUI.dialog({
		title: title,
		text: text,
		extraClass: 'fui-dialog-confirm',
		buttons: [{
			text: FoxUI.defaults.cancelText,
			onclick: cancelCallback
		}, {
			text: FoxUI.defaults.confirmText,
			onclick: confirmCallback
		}]
	});
}
FoxUI.prompt = function(text, title, placeholder, callback) {

	FoxUI.dialog({
		title: title,
		text: text,
		extraClass: 'fui-dialog-confirm',
		prompt: {
			placeholder: placeholder
		},
		buttons: [{
			text: FoxUI.defaults.confirmText,
			onclick: callback
		}]
	});
}
FoxUI.modal = {
	show: function(html, mask, maskTransparent) {
		$('.fui-modal').remove();
		$('.fui-mask').remove();

		if (mask) {
			FoxUI.mask.show(maskTransparent);
		}

		var modal = $("<div class='fui-modal'>" + html + "</div>");
		$(document.body).append(modal);
		if (modal.find('.fui-dialog').length > 0) {
			//濡傛灉鏄璇� 
			var dialog = modal.find('.fui-dialog');
			dialog.show().css({
				marginTop: -Math.round(dialog.outerHeight() / 2) + 'px'
			}).addClass('fui-dialog-in');
		}
		return modal;
	},
	hide: function() {
		$('.fui-mask').remove();
	}
}

FoxUI.dialog = function(params) {
	params = params || {};

	var titleHTML = params.title ? '<div class="fui-dialog-title">' + params.title + '</div>' : '';
	var textHTML = params.text ? '<div class="fui-dialog-text">' + params.text + '</div>' : '';

	var buttonsHTML = '';
	if (params.buttons && params.buttons.length > 0) {
		buttonsHTML += '<div class="fui-dialog-btns">';
		for (var i = 0; i < params.buttons.length; i++) {
			buttonsHTML += '<a class="' + (params.buttons[i].extraClass ? params.buttons[i].extraClass : '') + '" href="javascript:;">' + params.buttons[i].text + '</a>';
		}
		buttonsHTML += '</div>';
	}

	var promptHTML = '';
	if (params.prompt) {
		promptHTML += '<div class="fui-dialog-prompt"><input type="text" class="fui-dialog-prompt-input" placeholder="' + (params.prompt.placeholder ? params.prompt.placeholder : '') + '" value="' + (params.prompt.value ? params.prompt.value : '') + '"/></div>';
	}

	var dialogHTML = '<div class="fui-dialog  ' + (params.extraClass ? params.extraClass : '') + '">';
	dialogHTML += titleHTML + textHTML + promptHTML + buttonsHTML;
	dialogHTML += '</div>';

	var modal = FoxUI.modal.show(dialogHTML, true);


	$('.fui-dialog-btns a', modal).each(function(index, el) {
		$(el).click(function(e) {


			$('.fui-dialog').addClass('fui-dialog-out').transitionEnd(function(e) {

				modal.remove();
			});
			FoxUI.mask.hide();

			if (params.buttons[index].onclick) {
				if (params.prompt) {
					params.buttons[index].onclick(modal, modal.find(':input').val(), e);
					return;
				}
				params.buttons[index].onclick(modal, e);
			}
		});
	});
	return modal;
}

FoxUI.loader = {

	show: function(text, iconClass, duration) {
		FoxUI.mask.show(true);

		var loader = "<div class='fui-loader " + (text != 'mini' ? 'fui-loader-tip' : '') + "'>";
		if (text == 'mini' || text == 'loading') {
			loader += "<span class='fui-preloader fui-preloader-white'></span>";
			if (text == 'loading') {
				loader += "<span class='fui-loader-text'>" + FoxUI.defaults.loaderText + "</span>";
			}
		} else {

			if (iconClass) {
				loader += "<span class='fui-loader-icon'><i class='" + iconClass + "'></i></span>";
			}
			loader += "<span class='fui-loader-text'>" + text + "</span>";
		}
		loader += "</div>";
		$(document.body).append(loader);

	},
	hide: function() {
		FoxUI.mask.hide();
		$('.fui-loader').remove();
	}

}

FoxUI.notify = {

	show: function(text, title, css, autoClose) {

		if ($('.fui-notify').length > 0) {
			$('.fui-notify').remove();
		}

		if (!css) {
			css = 'default';
		}
		var notifyHTML = "<div class='fui-notify fui-notify-" + css + "'>";
		if (title) {
			notifyHTML += "<span class='fui-notify-title'>" + title + "</span>";
		}
		if (text) {
			notifyHTML += "<span class='fui-notify-text'>" + text + "</span>";
		}
		notifyHTML += "<span class='fui-notify-close'><i class='fa fa-remove'></i></span>";
		notifyHTML += "</div>";
		var notify = $(notifyHTML);
		var modal = FoxUI.modal.show(notifyHTML, false);
		setTimeout(function() {
			$('.fui-notify').addClass('in');
		}, 0);


		autoClose = autoClose || false;
		if (autoClose) {
			notify.find('fui-notify-close').remove();
			setTimeout(function() {
				FoxUI.notify.hide();
			}, 2000);
		}

	},
	hide: function() {
		$('.fui-notify').addClass('out').transitionEnd(function(e) {
			$('.fui-notify').remove();
		});
	}

}
FoxUI.list = {
	init: function() {
		$(".fui-list").each(function() {

			$(this).find('.fui-checkbox,.fui-radio').each(function() {
				var box = $(this);
				var list = box.closest('.fui-list');
				list.unbind('click').click(function() {

					if (box.hasClass('fui-radio')) {
						if (!box[0].checked) {
							box[0].checked = true;
						}
						return;
					}
					box[0].checked = !box[0].checked;

				})
			})
		})
	}
}

FoxUI.message = {

	show: function(params) {

		var titleHTML = '';
		if (params.title) {
			titleHTML = '<div class="title">' + params.title + '</div>';
		}

		var contentHTML = '';
		if (params.content) {
			contentHTML = '<div class="content">' + params.content + '</div>';
		}

		var iconHTML = '';
		if (params.icon) {
			iconHTML = '<div class="icon"><i class="' + params.icon + '"></i></div>';
		}

		var buttonsHTML = '';
		if (params.buttons) {
			$.each(params.buttons, function(i, btn) {

				buttonsHTML += '<a href="javascript:;" class="btn btn-default ' + (btn.extraClass ? btn.extraClass : '') + '">' + btn.text + '</a>';
			});
		}
		var messageHTML = "<div class='fui-message fui-message-popup'>" + iconHTML + titleHTML + contentHTML + buttonsHTML + "</div>";

		var modal = FoxUI.modal.show(messageHTML, true);
		setTimeout(function() {
			$('.fui-message-popup').addClass('in').transitionEnd(function(e) {
				if (params.url) {
					setTimeout(function() {
						location.href = params.url;
						return;
					}, params.duration || 2000)
				}
			});
		}, 0);

		$('.fui-mask').click(function() {
			FoxUI.message.hide();
		});

		$('.fui-message-popup a', modal).each(function(index, el) {
			$(el).click(function(e) {
				FoxUI.message.hide();
				if (params.buttons) {
					if (params.buttons[index].onclick) {
						params.buttons[index].onclick(modal, e);
					}
				}
			});
		});
	},
	confirm: function(title, content, click) {
		FoxUI.message.show({
			title: title,
			content: content,
			icon: 'fa fa-question-circle danger',
			buttons: [{
				text: FoxUI.defaults.confirmText,
				extraClass: 'btn-success',
				onclick: click
			}, {
				text: FoxUI.defaults.cancelText
			}]
		});
	},
	success: function(title, content, urlOrClick) {
		var params = {
			title: title,
			content: content,
			icon: 'fa fa-check-circle success ',
			buttons: [{
				text: FoxUI.defaults.confirmText,
				extraClass: 'btn-success'
			}]
		};
		if (urlOrClick) {

			if (typeof(urlOrClick) == 'function') {
				params.buttons[0].onclick = urlOrClick;
			} else {
				params.url = urlOrClick;
			}
		}
		FoxUI.message.show(params);
	},
	info: function(title, content, urlOrClick) {
		var params = {
			title: title,
			content: content,
			icon: 'fa fa-info-circle warning',
			buttons: [{
				text: FoxUI.defaults.confirmText,
				extraClass: 'btn-warning'
			}]
		}
		if (urlOrClick) {
			if (typeof(urlOrClick) == 'function') {
				params.buttons[0].onclick = urlOrClick;
			} else {
				params.url = urlOrClick;
			}
		}

		FoxUI.message.show(params);
	},
	error: function(title, content, urlOrClick) {
		var params = {
			title: title,
			content: content,
			icon: 'fa fa-times-circle danger',
			buttons: [{
				text: FoxUI.defaults.confirmText,
				extraClass: 'btn-danger'
			}]
		}
		if (urlOrClick) {
			if (typeof(urlOrClick) == 'function') {
				params.buttons[0].onclick = urlOrClick;
			} else {
				params.url = urlOrClick;
			}
		}


		FoxUI.message.show(params);

	},
	hide: function() {

		$('.fui-message-popup').addClass('out').transitionEnd(function(e) {
			$('.fui-message-popup').remove();
		});
		FoxUI.mask.hide();
	}
}