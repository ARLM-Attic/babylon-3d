#include "baseTexture.h"
#include <string>

using namespace Babylon;

Babylon::BaseTexture::BaseTexture(string url, IScene::Ptr scene) 
	: delayLoadState(DELAYLOADSTATE_NONE), hasAlpha(false), level(1), _texture(nullptr), onDispose(nullptr) {
		this->_scene = scene;
		this->_scene->getTextures().push_back(shared_from_this());
};

IGLTexture::Ptr Babylon::BaseTexture::getInternalTexture () {
	return this->_texture;
};

bool Babylon::BaseTexture::isReady () {
	if (this->delayLoadState == DELAYLOADSTATE_NOTLOADED) {
		return true;
	}

	if (this->_texture) {
		return this->_texture->isReady;
	}

	return false;
};

// Methods
Size Babylon::BaseTexture::getSize () {
	if (this->_texture->_width) {
		return Size(this->_texture->_width, this->_texture->_height);
	}

	if (this->_texture->_size) {
		return Size(this->_texture->_size, this->_texture->_size);
	}

	return Size(0, 0);
};

Size Babylon::BaseTexture::getBaseSize () {
	if (!this->isReady())
		return Size( 0, 0 );

	if (this->_texture->_size) {
		return  Size( this->_texture->_size, this->_texture->_size );
	}

	return Size( this->_texture->_baseWidth, this->_texture->_baseHeight );
};

IGLTexture::Ptr BaseTexture::_getFromCache (string url, bool noMipmap) {
	auto texturesCache = this->_scene->getEngine()->getLoadedTexturesCache();
	for (auto texturesCacheEntry : texturesCache) {
		if (texturesCacheEntry->url == url && texturesCacheEntry->noMipmap == noMipmap) {
			texturesCacheEntry->references++;
			return texturesCacheEntry;
		}
	}

	return nullptr;
};

void Babylon::BaseTexture::delayLoad () {
};

void Babylon::BaseTexture::releaseInternalTexture () {
	if (!this->_texture) {
		return;
	}
	auto texturesCache = this->_scene->getEngine()->getLoadedTexturesCache();
	this->_texture->references--;

	// Final reference ?
	if (this->_texture->references == 0) {
		auto it = find ( begin( texturesCache ), end( texturesCache ), this->_texture);
		if (it != end (texturesCache))
		{
			texturesCache.erase(it);
		}

		this->_scene->getEngine()->_releaseTexture(this->_texture);

		// TODO: what to do with it?
		//delete this->_texture;
		this->_texture.reset();
	}
};

void Babylon::BaseTexture::dispose () {
	// Remove from scene
	auto it = find ( begin( this->_scene->getTextures() ), end( this->_scene->getTextures() ), this);

	if (it != end ( this->_scene->getTextures() )) {
		this->_scene->getTextures().erase(it);
	}

	if (this->_texture == nullptr) {
		return;
	}

	this->releaseInternalTexture();

	// Callback
	if (this->onDispose) {
		this->onDispose();
	}
};
