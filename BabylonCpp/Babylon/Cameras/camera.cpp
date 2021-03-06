#include "camera.h"
#include "defs.h"
#include "engine.h"

using namespace Babylon;

Babylon::Camera::Camera(string name, Vector3::Ptr position, Scene::Ptr scene) : Node(scene)
{
	this->_scene = scene;

	this->name = name;
	this->id = name;
	this->position = position;
	this->upVector = Vector3::Up();
	this->_childrenFlag = true;

	// moved to new
	////scene->cameras.push_back(enable_shared_from_this<Camera>::shared_from_this());

	////if (!scene->activeCamera) {
	////	scene->activeCamera = enable_shared_from_this<Camera>::shared_from_this();
	////}

	this->_computedViewMatrix = Matrix::Identity();
	this->_projectionMatrix = make_shared<Matrix>();

	// Animations
	this->animations.clear();

	// Postprocesses
	this->postProcesses.clear();

	// Viewport
	this->viewport = make_shared<Viewport>(0, 0, 1.0, 1.0);

	//Cache
	_initCache();

	orthoLeft = 0.;
	orthoRight = 0.;
	orthoBottom = 0.;
	orthoTop = 0.;
	fov = 0.8;
	minZ = 0.1;
	maxZ = 1000.0;
	inertia = 0.9;
	mode = PERSPECTIVE_CAMERA;
}

Camera::Ptr Babylon::Camera::New(string name, Vector3::Ptr position, Scene::Ptr scene)
{
	auto camera = make_shared<Camera>(Camera(name, position, scene));
	scene->cameras.push_back(camera);

	if (!scene->activeCamera) {
		scene->activeCamera = camera;
	}

	return camera;
}

// Members

// Properties
Scene::Ptr Babylon::Camera::getScene() {
	return this->_scene;
};

//Cache
void Babylon::Camera::_initCache() {
	this->_cache.position = make_shared<Vector3>(numeric_limits<float>::max(), numeric_limits<float>::max(), numeric_limits<float>::max());
	this->_cache.upVector = make_shared<Vector3>(numeric_limits<float>::max(), numeric_limits<float>::max(), numeric_limits<float>::max());

	this->_cache.mode = ORTHOGRAPHIC_CAMERA;
	this->_cache.minZ = 0.;
	this->_cache.maxZ = 0.;

	this->_cache.fov = 0.;
	this->_cache.aspectRatio = 0.;

	this->_cache.orthoLeft = 0.;
	this->_cache.orthoRight = 0.;
	this->_cache.orthoBottom = 0.;
	this->_cache.orthoTop = 0.;
	this->_cache.renderWidth = 0;
	this->_cache.renderHeight = 0;
};

void Babylon::Camera::_updateCache(bool ignoreParentClass) {
	if(!ignoreParentClass)
		Node::_updateCache();

	this->_cache.position->copyFrom(this->position);
	this->_cache.upVector->copyFrom(this->upVector);

	this->_cache.mode = this->mode;
	this->_cache.minZ = this->minZ;
	this->_cache.maxZ = this->maxZ;

	this->_cache.fov = this->fov;

	auto engine = this->_scene->getEngine();

	this->_cache.aspectRatio = engine->getAspectRatio();

	this->_cache.orthoLeft = this->orthoLeft;
	this->_cache.orthoRight = this->orthoRight;
	this->_cache.orthoBottom = this->orthoBottom;
	this->_cache.orthoTop = this->orthoTop;
	this->_cache.renderWidth = engine->getRenderWidth();
	this->_cache.renderHeight = engine->getRenderHeight();
};

// Synchronized
bool Babylon::Camera::_isSynchronized() {
	return this->_isSynchronizedViewMatrix() && this->_isSynchronizedProjectionMatrix();
};

bool Babylon::Camera::_isSynchronizedViewMatrix() {
	if (!Node::_isSynchronized())
		return false;

	return this->_cache.position == this->position 
		&& this->_cache.upVector == this->upVector;
};

bool Babylon::Camera::_isSynchronizedProjectionMatrix() {
	auto r = this->_cache.mode == this->mode
		&& this->_cache.minZ == this->minZ
		&& this->_cache.maxZ == this->maxZ;

	if (!r)
		return false;

	auto engine = this->_scene->getEngine();

	if (this->mode == PERSPECTIVE_CAMERA) {
		r = this->_cache.fov == this->fov
			&& this->_cache.aspectRatio == engine->getAspectRatio();
	}
	else {
		r = this->_cache.orthoLeft == this->orthoLeft
			&& this->_cache.orthoRight == this->orthoRight
			&& this->_cache.orthoBottom == this->orthoBottom
			&& this->_cache.orthoTop == this->orthoTop
			&& this->_cache.renderWidth == engine->getRenderWidth()
			&& this->_cache.renderHeight == engine->getRenderHeight();
	}
	return r;
};

// Methods
void Babylon::Camera::attachControl(ICanvas::Ptr canvas, bool noPreventDefault) {
};

void Babylon::Camera::detachControl(ICanvas::Ptr canvas) {
};

void Babylon::Camera::_update() {
};

void Babylon::Camera::_updateFromScene() {
	this->updateCache();
	this->_update();
};

// My implementation
void Babylon::Camera::attachPostProcess(PostProcess::Ptr postProcess) {
	this->postProcesses.push_back(postProcess);
};

void Babylon::Camera::detachPostProcess(PostProcess::Ptr postProcess) {
	auto it = find (begin(this->postProcesses), end(this->postProcesses), postProcess);
	if (it != end(this->postProcesses))
	{
		this->postProcesses.erase(it);
	}
};

bool Babylon::Camera::hasWorldMatrix() {
	return true;
}

Matrix::Ptr Babylon::Camera::getWorldMatrix() {
	if (!this->_worldMatrix) {
		this->_worldMatrix = Matrix::Identity();
	}

	auto viewMatrix = this->getViewMatrix();

	viewMatrix->invertToRef(this->_worldMatrix);

	return this->_worldMatrix;
};

Matrix::Ptr Babylon::Camera::_getViewMatrix() {
	return Matrix::Identity();
};

Matrix::Ptr Babylon::Camera::getViewMatrix() {
	this->_computedViewMatrix = this->_computeViewMatrix();

	if(!this->parent 
		||  !this->parent->hasWorldMatrix()
		|| (!this->hasNewParent() && this->parent->isSynchronized())) {
			return this->_computedViewMatrix;
	}

	if (!this->_worldMatrix) {
		this->_worldMatrix = Matrix::Identity();
	}

	this->_computedViewMatrix->invertToRef(this->_worldMatrix);

	this->_worldMatrix->multiplyToRef(this->parent->getWorldMatrix(), this->_computedViewMatrix);

	this->_computedViewMatrix->invert();

	return this->_computedViewMatrix;
};

Matrix::Ptr Babylon::Camera::_computeViewMatrix(bool force) {
	if (!force && this->_isSynchronizedViewMatrix()) {
		return this->_computedViewMatrix;
	}

	this->_computedViewMatrix = this->_getViewMatrix();
	return this->_computedViewMatrix;
};

Matrix::Ptr Babylon::Camera::getProjectionMatrix(bool force) {
	if(!force && this->_isSynchronizedProjectionMatrix()) {
		return this->_projectionMatrix;
	}

	auto engine = this->_scene->getEngine();
	if (this->mode == PERSPECTIVE_CAMERA) {
		Matrix::PerspectiveFovLHToRef(this->fov, engine->getAspectRatio(), this->minZ, this->maxZ, this->_projectionMatrix);
		return this->_projectionMatrix;
	}

	auto halfWidth = engine->getRenderWidth() / 2.0;
	auto halfHeight = engine->getRenderHeight() / 2.0;
	Matrix::OrthoOffCenterLHToRef(
		this->orthoLeft != 0. ? this->orthoLeft : -halfWidth, 
		this->orthoRight != 0. ? this->orthoRight : halfWidth, 
		this->orthoBottom != 0. ? this->orthoBottom : -halfHeight, 
		this->orthoTop != 0. ? this->orthoTop : halfHeight, 
		this->minZ, 
		this->maxZ, 
		this->_projectionMatrix);
	return this->_projectionMatrix;
};

void Babylon::Camera::dispose(bool doNotRecurse) {
	// Remove from scene
	auto it = find (begin(this->_scene->cameras), end(this->_scene->cameras), dynamic_pointer_cast<Camera>(shared_from_this()));
	if (it != end(this->_scene->cameras))
	{
		this->_scene->cameras.erase(it);
	}

	// Postprocesses
	for (auto postProcess : this->postProcesses) {
		postProcess->dispose();
	}

	this->postProcesses.clear();
};
