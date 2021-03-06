#ifndef BABYLON_MATERIAL_H
#define BABYLON_MATERIAL_H

#include "decls.h"

#include "igl.h"
#include "iengine.h"
#include "tools_math.h"

#include "effect.h"
#include "animatable.h"

using namespace std;

namespace Babylon {

	class Mesh;
	typedef shared_ptr_t<Mesh> MeshPtr;

	// TODO: finish it (finish Animatable)
	class Material: public Animatable, public IDisposable, public enable_shared_from_this<Material> {

	public:
		typedef shared_ptr_t<Material> Ptr;
		typedef vector_t<Ptr> Array;
		typedef void (*OnDisposeFunc)();

		string id;
		string name;
		ScenePtr _scene;

		// Members
		bool checkReadyOnEveryCall;
		bool checkReadyOnlyOnce;
		float alpha;
		bool wireframe;
		bool backFaceCulling;
		Effect::Ptr _effect;
		bool _wasPreviouslyReady;

		OnDisposeFunc onDispose;

	public: 
		Material(string name, ScenePtr scene);

		virtual bool isReady(MeshPtr mesh) = 0;
		virtual Effect::Ptr getEffect();
		virtual bool needAlphaBlending();
		virtual bool needAlphaTesting();
		virtual void _preBind();
		virtual void bind(Matrix::Ptr world, MeshPtr mesh);
		virtual void unbind();
		virtual void baseDispose();
		virtual void dispose(bool doNotRecurse = false);

		// my addon 
		virtual IRenderable::Array getRenderTargetTextures();
		// Animatable
		virtual void markAsDirty(string property = "");
		virtual AnimationValue operator[](string key);
	};

};

#endif // BABYLON_MATERIAL_H