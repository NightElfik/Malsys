/// <reference path="~/Js/jquery.js" />


var scene, renderer, camera, cameraControls;

(function ($) {

	$(".threeJsScene").each(function (i) {

		var sceneUrl = $(this).attr("data-scene");
		if (!sceneUrl) {
			return;
		}

		if (Detector.webgl) {
			renderer = new THREE.WebGLRenderer({
				antialias: true,
				preserveDrawingBuffer: true  // to allow screenshot
			});
			renderer.setClearColorHex(0x000000, 1);
		}
		else {
			renderer = new THREE.CanvasRenderer();
		}

		renderer.setSize($(this).width(), $(this).height());
		$(this).append(renderer.domElement);

		// load the scene
		var loader = new THREE.SceneLoader();
		//loader.callbackSync = callbackSync;
		//loader.callbackProgress = callbackProgress;
		loader.load(sceneUrl, function (result) {
			scene = result.scene;
			camera = result.currentCamera;
			cameraControls	= new THREEx.DragPanControls(camera)

			scene.add(camera);

			camera.aspect = window.innerWidth / window.innerHeight;
			camera.updateProjectionMatrix();

			renderer.setClearColor(result.bgColor, result.bgAlpha);

			animate();
		});

	});

} (jQuery));



// animation loop
function animate() {

	// loop on request animation loop
	// - it has to be at the begining of the function
	// - see details at http://my.opera.com/emoller/blog/2011/12/20/requestanimationframe-for-smart-er-animating
	requestAnimationFrame(animate);

	// do the render
	render();
}

// render the scene
function render() {

	// update camera controls
	cameraControls.update();

	// actually render the scene
	renderer.render(scene, camera);
}
