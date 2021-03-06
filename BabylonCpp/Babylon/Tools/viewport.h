#ifndef BABYLON_VIEWPORT_H
#define BABYLON_VIEWPORT_H

#include "decls.h"

#include "iengine.h"

namespace Babylon {

	struct Viewport: public enable_shared_from_this<Viewport> {

	public:
		typedef shared_ptr_t<Viewport> Ptr;

	public:
		int x;
		int y;
		int width;
		int height;

	public: 
		Viewport(int x, int y, int width, int height);		

		virtual Viewport::Ptr toGlobal(EnginePtr engine);
	};

};

#endif // BABYLON_VIEWPORT_H