#include "multiMaterial.h"
#include "engine.h"
#include "mesh.h"

using namespace Babylon;

Babylon::MultiMaterial::MultiMaterial(string name, Scene::Ptr scene) {
	this->name = name;
	this->id = name;

	this->_scene = scene;
	scene->multiMaterials.push_back(shared_from_this());
	subMaterials.clear();
};

// Properties
bool Babylon::MultiMaterial::isReady(Mesh::Ptr mesh) {
	auto result = true;
	for (auto subMaterial : this->subMaterials) {
		if (subMaterial) {
			result &= subMaterial->isReady(mesh);
		}
	}

	return result;
};

Material::Ptr Babylon::MultiMaterial::getSubMaterial(int index) {
	if (index < 0 || index >= this->subMaterials.size()) {
		return this->_scene->defaultMaterial;
	}

	return this->subMaterials[index];
};