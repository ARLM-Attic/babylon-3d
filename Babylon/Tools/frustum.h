#ifndef BABYLON_Frustum_H
#define BABYLON_Frustum_H

#include <memory>
#include <vector>

#include "iengine.h"
#include "plane.h"

using namespace std;

namespace Babylon {

	class Frustum: public enable_shared_from_this<Frustum> {

	public: 
	    static Plane::Array GetPlanes(Matrix::Ptr transform);
		static void GetPlanesToRef(Matrix::Ptr transform, Plane::Array& frustumPlanes);

	};

};

#endif // BABYLON_Frustum_H