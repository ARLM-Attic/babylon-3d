#ifndef BABYLON_Sprite_H
#define BABYLON_Sprite_H

#include <memory>
#include <vector>
#include <map>

#include "iengine.h"
#include "vector3.h"
#include "color4.h"

using namespace std;

namespace Babylon {

	class SpriteManager;
	typedef shared_ptr<SpriteManager> SpriteManagerPtr;

	class Sprite : public enable_shared_from_this<Sprite> {

	public:

		typedef shared_ptr<Sprite> Ptr;
		typedef vector<Ptr> Array;

		string name;
		SpriteManagerPtr _manager;
		Vector3::Ptr position;
		Color4::Ptr color;
		float _frameCount;

		float size;
		float angle;
		int cellIndex;
		float invertU;
		float invertV;
		bool disposeWhenFinishedAnimating;

		bool _animationStarted;
		bool _loopAnimation;
		bool _fromIndex;
		bool _toIndex;
		bool _delay;
		int _direction;
		int _time;

	public: 
		Sprite(string name, SpriteManagerPtr manager);

		// Methods
		virtual void playAnimation(int from, int to, bool loop, bool delay);
		virtual void stopAnimation();
		virtual void _animate(int deltaTime);
		virtual void dispose();
	};

};

#endif // BABYLON_Sprite_H