#include "shadowGenerator.h"
#include <string>
#include <numeric>
#include "light.h"

using namespace Babylon;

// Members
bool Babylon::ShadowGenerator::useVarianceShadowMap = true;

// TODO: finish it
Babylon::ShadowGenerator::ShadowGenerator(int width, int height, Light::Ptr light)
{
	/*
	this->_light = light;
	this->_scene = light->getScene();

	// TODO: resolve the issue of cercular reference
	////light->_shadowGenerator = this;

	// Render target
	this->_shadowMap = make_shared<RenderTargetTexture>(light->name + "_shadowMap", width, height, this->_scene, false);
	this->_shadowMap->wrapU = CLAMP_ADDRESSMODE;
	this->_shadowMap->wrapV = CLAMP_ADDRESSMODE;
	this->_shadowMap->renderParticles = false;

	// Custom render function
	auto that = this;

	this->_shadowMap->customRenderFunction = [](SubMesh::Array& opaqueSubMeshes, SubMesh::Array& alphaTestSubMeshes) {
		for (auto index = 0; index < opaqueSubMeshes.length; index++) {
			renderSubMesh(opaqueSubMeshes.data[index]);
		}

		for (auto index = 0; index < alphaTestSubMeshes.length; index++) {
			renderSubMesh(alphaTestSubMeshes.data[index]);
		}
	};

	// Internals
	this->_viewMatrix = Matrix::Zero();
	this->_projectionMatrix = Matrix::Zero();
	this->_transformMatrix = Matrix::Zero();
	this->_worldViewProjection = Matrix::Zero();
	*/
};

/*
void Babylon::ShadowGenerator::renderSubMesh(SubMesh::Ptr subMesh) {
	auto mesh = subMesh->getMesh();
	auto world = mesh->getWorldMatrix();
	auto engine = that->_scene->getEngine();

	auto that = this;
	if (that->isReady(mesh)) {
		engine->enableEffect(that->_effect);

		// Bones
		if (mesh->skeleton && mesh->isVerticesDataPresent(VertexBuffer::MatricesIndicesKind) && mesh->isVerticesDataPresent(VertexBuffer::MatricesWeightsKind)) {
			that->_effect->setMatrix("world", world);
			that->_effect->setMatrix("viewProjection", that->getTransformMatrix());

			that->_effect->setMatrices("mBones", mesh->skeleton->getTransformMatrices());
		} else {
			world.multiplyToRef(that->getTransformMatrix(), that->_worldViewProjection);
			that->_effect->setMatrix("worldViewProjection", that->_worldViewProjection);
		}

		// Bind and draw
		mesh->bindAndDraw(subMesh, that->_effect, false);
	}
};

// Properties
bool Babylon::ShadowGenerator::isReady(Mesh::Ptr mesh) {
	vector<string> defines;

	if (this->useVarianceShadowMap) {
		defines.push_back("#define VSM");
	}

	vector<string> attribs;
	attribs.push_back("position");
	if (mesh->skeleton && mesh->isVerticesDataPresent(VertexBuffer_MatricesIndicesKind) && mesh->isVerticesDataPresent(VertexBuffer_MatricesWeightsKind)) {
		attribs.push_back("matricesIndices");
		attribs.push_back("matricesWeights");
		defines.push_back("#define BONES");
		defines.push_back("#define BonesPerMesh " + mesh->skeleton->bones.size());
	}

	// Get correct effect      
	auto join = accumulate( defines.begin(), defines.end(), string("\n") );

	vector<string> uniformNames; 
	uniformNames.push_back("world");
	uniformNames.push_back("mBones");
	uniformNames.push_back("viewProjection");
	uniformNames.push_back("worldViewProjection");

	vector<int> samples; 

	if (this->_cachedDefines != join) {
		this->_cachedDefines = join;
		this->_effect = this->_scene->getEngine()->createEffect(
				"shadowMap",
				attribs,
				uniformNames,
				samples, 
				join);
	}

	return this->_effect->isReady();
};

RenderTargetTexture::Ptr Babylon::ShadowGenerator::getShadowMap() {
	return this->_shadowMap;
};

ILight::Ptr Babylon::ShadowGenerator::getLight() {
	return this->_light;
};

// Methods
Matrix::Ptr Babylon::ShadowGenerator::getTransformMatrix() {
	auto lightPosition = this->_light->position;
	auto lightDirection = this->_light->direction;

	if (this->_light->_computeTransformedPosition()) {
		lightPosition = this->_light->_transformedPosition;
	}

	if (!this->_cachedPosition || !this->_cachedDirection || lightPosition != this->_cachedPosition || lightDirection != this->_cachedDirection) {

		this->_cachedPosition = lightPosition->clone();
		this->_cachedDirection = lightDirection->clone();

		auto activeCamera = this->_scene->activeCamera;

		Matrix::LookAtLHToRef(lightPosition, this->_light->position->add(lightDirection), Vector3::Up(), this->_viewMatrix);
		auto PI = 4. * atan(1.);
		Matrix::PerspectiveFovLHToRef(PI / 2.0, 1.0, activeCamera->minZ, activeCamera->maxZ, this->_projectionMatrix);

		this->_viewMatrix->multiplyToRef(this->_projectionMatrix, this->_transformMatrix);
	}

	return this->_transformMatrix;
};

void Babylon::ShadowGenerator::dispose() {
	this->_shadowMap->dispose();
};
*/