/**
* Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
* All rights reserved.
*/

var controls = [];

(function ($) {

	function process(domElement, callback) {
		$domElement = $(domElement);
		var $loaderElement = $domElement.find('.dots');
		var sceneBaseUrl = $domElement.attr('data-url');
		if (!sceneBaseUrl) {
			return;
		}

		$loaderElement.append('.');

		$.getJSON(sceneBaseUrl + '/meta', function (metadata) {
			$loaderElement.append('.');
			createScene($domElement, sceneBaseUrl + '/obj', sceneBaseUrl + '/mtl', metadata, $loaderElement, callback);
		}).fail(function (jqxhr, textStatus, error) {
			var err = textStatus + ", " + error;
			//console.log("Request Failed: " + err);
		});

		//var statsDisplay = domElement.attr('data-stats-display');

		//if (statsDisplay) {
		//	stats = new Stats();
		//	stats.domElement.style.position = 'absolute';
		//	stats.domElement.style.top = '0px';
		//	stats.domElement.style.right = '0px';
		//	stats.domElement.style.opacity = '0.8';
		//	domElement.append(stats.domElement);
		//	ctrl.addEventListener('change', function () {
		//		renderer.render(scene, camera);
		//		stats.update();
		//		update(domElement, ctrl);
		//	});
		//}
		//else {
		//	ctrl.addEventListener('change', function () {
		//		renderer.render(scene, camera);
		//		update(domElement, ctrl);
		//	});
		//}
	}

	function createScene($domElement, objUrl, mtlUrl, metadata, $loaderElement, callback) {
		var width = $domElement.width();
		var height = $domElement.height();

		var scene = new THREE.Scene();
		var camera = new THREE.PerspectiveCamera(60, width / height, 0.1, 10000);
		var cp = metadata.cameraPosition;
		camera.position = new THREE.Vector3(cp[0], cp[1], cp[2]);
		var cu = metadata.cameraUpVector;
		camera.up = new THREE.Vector3(cu[0], cu[1], cu[2]);
		//scene.add(camera);

		var renderer;
		if (Detector.webgl) {
			renderer = new THREE.WebGLRenderer({
				preserveDrawingBuffer: true  // Allow screenshot.
			});
		}
		else {
			renderer = new THREE.CanvasRenderer();
			renderer.sortObjects = true;
		}

		renderer.setSize(width, height);

		var clearClrHex = parseInt(metadata.bgColor || 'FFFFFF', 16);
		var bgColor = new THREE.Color(0);
		bgColor.setHex(clearClrHex);
		renderer.setClearColor(bgColor, 1);
		renderer.render(scene, camera);  // Clear BG.


		var directionalLight = new THREE.DirectionalLight(0xF0F0F0);
		directionalLight.position.set(-0.9, -1.2, -1.1).normalize();
		scene.add(directionalLight);

		var directionalLight = new THREE.DirectionalLight(0x999999);
		directionalLight.position.set(0, 1.2, 0.9).normalize();
		scene.add(directionalLight);

		var directionalLight = new THREE.DirectionalLight(0x999999);
		directionalLight.position.set(1, 1.1, 0).normalize();
		scene.add(directionalLight);


		var loader = new THREE.OBJMTLLoader();
		$loaderElement.append('.');

		loader.load(objUrl, mtlUrl, function (object) {
			$loaderElement.append('.');
			object.traverse(function (node) {
				if (node.material) {
					node.material.side = THREE.DoubleSide;
				}
			});
			scene.add(object);
			$domElement.empty();
			$domElement.append(renderer.domElement);
			$domElement.append('<div class="threed"> </div>');

			// The renderer.domElement has to be in DOM in order to initialize trackball contorls.
			var noZoom = $domElement.attr('data-no-zoom') === 'true';
			var autoRotate = $domElement.attr('data-auto-rotate') === 'true';
			var ctrl = new THREE.TrackballControls(camera, renderer.domElement, noZoom, autoRotate);
			controls.push(ctrl);

			var ct = metadata.cameraTarget;
			ctrl.target = new THREE.Vector3(ct[0], ct[1], ct[2]);

			ctrl.noPan = $domElement.attr('data-no-pan') === 'true';
			ctrl.update();

			if ($domElement.attr('data-show-cam-coords') === 'true') {
				$pos = $('<li class="pos"></li>');
				$up = $('<li class="pos"></li>');
				$tgt = $('<li class="tgt"></li>');
				$cont = $('<div class="camDetails"></div>').append(
					$('<ul></ul>').append($pos).append($up).append($tgt));
				$domElement.append($cont);
				update(ctrl, $pos, $up, $tgt);

				ctrl.addEventListener('change', function () {
					renderer.render(scene, camera);
					if ($cont) {
						update(ctrl, $pos, $up, $tgt);
					}
				});
			}
			else {
				ctrl.addEventListener('change', function () {
					renderer.render(scene, camera);
				});
			}

			renderer.render(scene, camera);
			callback();
		});

	}

	function round(number) {
		return Math.round(number * 100) / 100;
	}

	function update(ctrl, $pos, $up, $tgt) {
		//console.log('updatepos');
		var pos = ctrl.object.position;
		$pos.text('set cameraPosition = {' + Math.round(pos.x) + ', ' + Math.round(pos.y) + ', ' + Math.round(pos.z) + '};');

		var up = ctrl.object.up;
		$up.text('set cameraUpVector = {' + round(up.x) + ', ' + round(up.y) + ', ' + round(up.z) + '};');

		var tgt = ctrl.target;
		$tgt.text('set cameraTarget = {' + Math.round(tgt.x) + ', ' + Math.round(tgt.y) + ', ' + Math.round(tgt.z) + '};');
	};


	var index = 0;
	var elementsToProcess = $('.threeJsScene');

	function processNext() {
		if (index >= elementsToProcess.length) {
			return;
		}

		setTimeout(function () {
			process(elementsToProcess[index++], processNext)
		}, 500);
	}

	$(document).ready(processNext());

}(jQuery));


function animate() {

	requestAnimationFrame(animate);
	for (var i in controls) {
		controls[i].update();
		//controls[i].object
	}
	// Do not render scene, scenes are rendered on camera move.

}

// Start animation loop.
animate();