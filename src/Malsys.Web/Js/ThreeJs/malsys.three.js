/// <reference path="~/Js/jquery.js" />

var controls = [];

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

		$.getScript(sceneUrl, function() {

			if (!Scene) {
				return;
			}

			var returnedScene = Scene();

			var scene = returnedScene.getScene();
			var target = returnedScene.getCameraTarget();

			var camera = new THREE.PerspectiveCamera(60, width / height, 0.1, 10000);

			camera.position = returnedScene.getCameraPosition();

			scene.add(camera);

			var ctrl = new THREE.TrackballControls(camera, rendererDomElement);
			ctrl.staticMoving = false;
			ctrl.target = target;
			controls.push(ctrl);

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
				});
			}
			else {
				ctrl.addEventListener('change', function() {
					renderer.render(scene, camera);
				});
			}


		});

	});

} (jQuery));




function animate() {

	requestAnimationFrame(animate);
	// update all controls
	for (var i in controls) {
		controls[i].update();
	}
	// do not render scene, scenes are rendered on camera move

}

// start animation loop
animate();
