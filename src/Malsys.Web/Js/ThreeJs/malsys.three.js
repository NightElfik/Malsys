/// <reference path="~/Js/jquery.js" />

var scene, renderer, camera, cameraControls;

(function ($) {

	var firstScene = true;

	$(".threeJsScene").each(function (i) {

		if (firstScene){
			firstScene = false;
		}
		else {
			$(this).text("Only one scene per page is supported in beta.");
			return;
		}

		var sceneUrl = $(this).attr("data-scene");
		if (!sceneUrl) {
			return;
		}

		if (Detector.webgl) {
			renderer = new THREE.WebGLRenderer({
				antialias: true,
				preserveDrawingBuffer: true  // to allow screenshot
			});
		}
		else {
			renderer = new THREE.CanvasRenderer();
			renderer.sortObjects = true;
		}

		renderer.setSize($(this).width(), $(this).height());
		var rendererDomElement = renderer.domElement;
		$(this).append(rendererDomElement);

		// load the scene
		var loader = new THREE.SceneLoader();
		//loader.callbackSync = callbackSync;
		//loader.callbackProgress = callbackProgress;
		loader.load(sceneUrl, function (result) {
			scene = result.scene;
			camera = result.currentCamera;
			cameraControls = new THREEx.DragPanControls(camera, camera.target, rendererDomElement);
			cameraControls.update();

			scene.add(camera);

			renderer.setClearColor(result.bgColor, result.bgColorAlpha);

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
	//renderer.clear();
	renderer.render(scene, camera);
}
