﻿/**
* Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
* All rights reserved.
*/
/// <reference path="~/Js/jquery.js" />

var controls = [];
var threeJsScenes = [];

// http://api.jquery.com/jQuery.getScript/
jQuery.cachedScript = function (url, options) {
	// allow user to set any option except for dataType, cache, and url
	options = $.extend(options || {}, {
		dataType: "script",
		cache: true,
		url: url
	});
	// Use $.ajax() since it is more flexible than $.getScript
	// Return the jqXHR object so we can chain callbacks
	return jQuery.ajax(options);
};

(function ($) {

	$('.threeJsScene').each(function (i) {

		var domElement = $(this);
		var sceneUrl = domElement.attr('data-scene');
		if (!sceneUrl) {
			return;
		}

		var width = domElement.width();
		var height = domElement.height();

		var renderer;

		// renderer ============================================================

		if (Detector.webgl) {
			renderer = new THREE.WebGLRenderer({
				antialias: domElement.attr('data-anti-alias') ? true : false,
				preserveDrawingBuffer: true  // to allow screenshot
			});
			domElement.children('.webgl').remove();
		}
		else {
			renderer = new THREE.CanvasRenderer();
			renderer.sortObjects = true;
		}

		renderer.setSize(width, height);


		var clearClrHex = parseInt(domElement.attr('data-clear-color') || '000000', 16);

		var bgColor = new THREE.Color(0);
		bgColor.setHex(clearClrHex);
		renderer.setClearColor(bgColor, 1);

		var rendererDomElement = renderer.domElement;
		domElement.append(rendererDomElement);


		// load scene ==========================================================

		var statsDisplay = domElement.attr('data-stats-display');

		$.cachedScript(sceneUrl).done(function(script, textStatus) {

			if (!Scene) {
				return;
			}

			var returnedScene = Scene();

			var scene = returnedScene.getScene();
			var target = returnedScene.getCameraTarget();

			var camera = new THREE.PerspectiveCamera(60, width / height, 0.1, 10000);

			camera.position = returnedScene.getCameraPosition();
			camera.up = returnedScene.getCameraUpVector();

			scene.add(camera);

			var ctrl = new THREE.TrackballControls(camera, rendererDomElement);
			ctrl.staticMoving = false;
			ctrl.target = target;
			controls.push(ctrl);
			threeJsScenes.push(domElement);

			if (statsDisplay) {
				stats = new Stats();
				stats.domElement.style.position = 'absolute';
				stats.domElement.style.top = '0px';
				stats.domElement.style.right = '0px';
				stats.domElement.style.opacity = '0.8';
				domElement.append(stats.domElement);
				ctrl.addEventListener('change', function() {
					renderer.render(scene, camera);
					stats.update();
					update(domElement, ctrl);
				});
			}
			else {
				ctrl.addEventListener('change', function() {
					renderer.render(scene, camera);
					update(domElement, ctrl);
				});
			}

		});

	});

	update = function(domElement, ctrl) {
		domElement.children('.cameraPosition').each(function() {
			var pos = ctrl.object.position;
			$(this).text('set cameraPosition = {' + Math.round(pos.x) + ', ' + Math.round(pos.y) + ', ' + Math.round(pos.z) + '};');
		});
		domElement.children('.cameraUp').each(function() {
			var pos = ctrl.object.up;
			$(this).text('set cameraUpVector = {' + round(pos.x) + ', ' + round(pos.y) + ', ' + round(pos.z) + '};');
		});
		domElement.children('.cameraTarget').each(function() {
			var pos = ctrl.target;
			$(this).text('set cameraTarget = {' + Math.round(pos.x) + ', ' + Math.round(pos.y) + ', ' + Math.round(pos.z) + '};');
		});
	};

	round = function(number) {
		return Math.round(number * 100) / 100;
	}

} (jQuery));


function animate() {

	requestAnimationFrame(animate);
	// update all controls
	for (var i in controls) {
		controls[i].update();
		//controls[i].object
	}
	// do not render scene, scenes are rendered on camera move

}

// start animation loop
animate();
