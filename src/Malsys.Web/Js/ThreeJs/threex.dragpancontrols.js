/** @namespace */
var THREEx = THREEx || {};

THREEx.DragPanControls = function (camera, target, domElement) {

	this.camera = camera;
	this.target = target;
	this.domElement = domElement;

	this.rotation = { x: 0, y: 0 };
	this.rotationTarget = { x: 0, y: 0 };
	this.rotationOnDown = { x: 0, y: 0 };

	this.distance = 100;
	this.distanceTarget = 200;

	this.mouse = { x: 0, y: 0 };
	this.mouseOnDown = { x: 0, y: 0 };

	this.camera.aspect = this.domElement.width / this.domElement.height;
	this.camera.updateProjectionMatrix();

	this.isMouseOverElement = false;

	var _this = this;
	this._$onMouseDown = function () { _this._onMouseDown.apply(_this, arguments); };
	this._$onMouseMove = function () { _this._onMouseMove.apply(_this, arguments); };
	this._$onMouseUp = function () { _this._onMouseUp.apply(_this, arguments); };
	this._$onMouseOver = function () { _this._onMouseOver.apply(_this, arguments); };
	this._$onMouseOut = function () { _this._onMouseOut.apply(_this, arguments); };
	this._$onMouseWheel = function () { _this._onMouseWheel.apply(_this, arguments); };
	//	this._$onTouchStart = function () { _this._onTouchStart.apply(_this, arguments); };
	//	this._$onTouchMove = function () { _this._onTouchMove.apply(_this, arguments); };

	this.domElement.addEventListener('mousedown', this._$onMouseDown, false);
	this.domElement.addEventListener('mouseover', this._$onMouseOver, false);
	this.domElement.addEventListener('mouseout', this._$onMouseOut, false);
	this.domElement.addEventListener('mousewheel', this._$onMouseWheel, false);
	//	this.domElement.addEventListener('touchstart', this._$onTouchStart, false);
	//	this.domElement.addEventListener('touchmove', this._$onTouchMove, false);
}

THREEx.DragPanControls.prototype.destroy = function () {

	this.domElement.removeEventListener('mouseout', this._$onMouseUp, false);
	this.domElement.removeEventListener('mouseup', this._$onMouseUp, false);
	this.domElement.removeEventListener('mousemove', this._$onMouseMove, false);
	this.domElement.removeEventListener('mousedown', this._$onMouseDown, false);
	this.domElement.removeEventListener('mouseover', this._$onMouseOver, false);
	this.domElement.removeEventListener('mouseout', this._$onMouseOut, false);
//	this.domElement.removeEventListener('touchstart', this._$onTouchStart, false);
//	this.domElement.removeEventListener('touchmove', this._$onTouchMove, false);
}

THREEx.DragPanControls.prototype.update = function (event) {

	this.rotation.x += (this.rotationTarget.x - this.rotation.x) * 0.1;
	this.rotation.y += (this.rotationTarget.y - this.rotation.y) * 0.1;
	this.distance += (this.distanceTarget - this.distance) * 0.3;

	this.camera.position.x = this.target.x + this.distance * Math.sin(this.rotation.x) * Math.cos(this.rotation.y);
	this.camera.position.y = this.target.y + this.distance * Math.sin(this.rotation.y);
	this.camera.position.z = this.target.z + this.distance * Math.cos(this.rotation.x) * Math.cos(this.rotation.y);
	this.camera.lookAt(this.target);
}

THREEx.DragPanControls.prototype._onMouseDown = function (event) {

	this.domElement.addEventListener('mouseup', this._$onMouseUp, false);
	this.domElement.addEventListener('mousemove', this._$onMouseMove, false);

	this.mouseOnDown.x = -event.clientX;
	this.mouseOnDown.y = event.clientY;

	this.rotationOnDown.x = this.rotationTarget.x;
	this.rotationOnDown.y = this.rotationTarget.y;

	this.isMouseOverElement = true;
}

THREEx.DragPanControls.prototype._onMouseMove = function (event) {

	this.mouse.x = -event.clientX;
	this.mouse.y = event.clientY;

	this.rotationTarget.x = this.rotationOnDown.x + (this.mouse.x - this.mouseOnDown.x) * 0.01;
	this.rotationTarget.y = this.rotationOnDown.y + (this.mouse.y - this.mouseOnDown.y) * 0.01;

	var PI_HALF = Math.PI / 2;
	this.rotationTarget.y = this.rotationTarget.y > PI_HALF ? PI_HALF : this.rotationTarget.y;
	this.rotationTarget.y = this.rotationTarget.y < -PI_HALF ? -PI_HALF : this.rotationTarget.y;
}

THREEx.DragPanControls.prototype._onMouseUp = function (event) {

	this.domElement.removeEventListener('mouseout', this._$onMouseUp, false);
	this.domElement.removeEventListener('mouseup', this._$onMouseUp, false);
	this.domElement.removeEventListener('mousemove', this._$onMouseMove, false);
}

THREEx.DragPanControls.prototype._onMouseOver = function (event) {
	this.isMouseOverElement = true;
}

THREEx.DragPanControls.prototype._onMouseOut = function (event) {

	this.isMouseOverElement = false;
	this.domElement.removeEventListener('mouseout', this._$onMouseUp, false);
	this.domElement.removeEventListener('mouseup', this._$onMouseUp, false);
	this.domElement.removeEventListener('mousemove', this._$onMouseMove, false);
}

THREEx.DragPanControls.prototype._onMouseWheel = function (event) {

	event.preventDefault();
	if (this.isMouseOverElement) {
		var delta = event.wheelDelta * 0.3;
		this.distanceTarget -= delta;
		this.distanceTarget = this.distanceTarget > 1000 ? 1000 : this.distanceTarget;
		this.distanceTarget = this.distanceTarget < 10 ? 10 : this.distanceTarget;
	}
	return false;
}

/*THREEx.DragPanControls.prototype._onTouchStart = function (event) {

	if( event.touches.length != 1 )	return;

	// no preventDefault to get click event on ios

	this._mouseX	= ( event.touches[ 0 ].pageX / window.innerWidth ) - 0.5;
	this._mouseY	= ( event.touches[ 0 ].pageY / window.innerHeight) - 0.5;
}

THREEx.DragPanControls.prototype._onTouchMove = function (event) {

	if( event.touches.length != 1 )	return;

	event.preventDefault();

	this._mouseX	= ( event.touches[ 0 ].pageX / window.innerWidth ) - 0.5;
	this._mouseY	= ( event.touches[ 0 ].pageY / window.innerHeight) - 0.5;
}*/

