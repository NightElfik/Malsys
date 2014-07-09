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
			console.log("Request Failed: " + err);
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
		scene.add(camera);

		var ctrl = new THREE.TrackballControls(camera, $domElement[0], true);
		controls.push(ctrl);

		var ct = metadata.cameraTarget;
		ctrl.target = new THREE.Vector3(ct[0], ct[1], ct[2]);

		ctrl.rotateSpeed = 1.0;
		ctrl.zoomSpeed = 1.2;
		ctrl.panSpeed = 0.8;

		ctrl.noPan = true;

		//ctrl.staticMoving = true;
		//ctrl.dynamicDampingFactor = 0.3;

		ctrl.addEventListener('change', function () {
			renderer.render(scene, camera);
			update($domElement, ctrl);
		});

		var directionalLight = new THREE.DirectionalLight(0xF0F0F0);
		directionalLight.position.set(-0.9, -1.2, -1.1).normalize();
		scene.add(directionalLight);

		var directionalLight = new THREE.DirectionalLight(0x999999);
		directionalLight.position.set(0, 1.2, 0.9).normalize();
		scene.add(directionalLight);

		var directionalLight = new THREE.DirectionalLight(0x999999);
		directionalLight.position.set(1, 1.1, 0).normalize();
		scene.add(directionalLight);

		var renderer;
		if (Detector.webgl) {
			renderer = new THREE.WebGLRenderer({
				preserveDrawingBuffer: true  // Allow screenshot.
			});
			// $domElement.children('.webgl').remove();
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
			renderer.render(scene, camera);
			$domElement.empty();
			$domElement.append(renderer.domElement);
			callback();
		});

	}

	function round(number) {
		return Math.round(number * 100) / 100;
	}

	function update($domElement, ctrl) {
		if ($domElement.is(":hover")) {
			ctrl.rotate();
		}
		//$domElement.children('.cameraPosition').each(function () {
		//	var pos = ctrl.object.position;
		//	$(this).text('set cameraPosition = {' + Math.round(pos.x) + ', ' + Math.round(pos.y) + ', ' + Math.round(pos.z) + '};');
		//});
		//$domElement.children('.cameraUp').each(function () {
		//	var pos = ctrl.object.up;
		//	$(this).text('set cameraUpVector = {' + round(pos.x) + ', ' + round(pos.y) + ', ' + round(pos.z) + '};');
		//});
		//$domElement.children('.cameraTarget').each(function () {
		//	var pos = ctrl.target;
		//	$(this).text('set cameraTarget = {' + Math.round(pos.x) + ', ' + Math.round(pos.y) + ', ' + Math.round(pos.z) + '};');
		//});
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

	processNext();

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