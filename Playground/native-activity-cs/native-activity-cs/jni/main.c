/*
* Copyright (C) 2010 The Android Open Source Project
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

// --target=armv7-none-linux-androideabi

//BEGIN_INCLUDE(all)
#include <jni.h>
#include <errno.h>

#include <EGL/egl.h>
#include <GLES/gl.h>

#include <android/sensor.h>
#include <android/log.h>
#include "../native_app_glue/android_native_app_glue.h"

#include <assert.h>
#include <unistd.h>

#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "native-activity-cs", __VA_ARGS__))
#define LOGW(...) ((void)__android_log_print(ANDROID_LOG_WARN, "native-activity-cs", __VA_ARGS__))
#define LOGE(...) ((void)__android_log_print(ANDROID_LOG_ERROR, "native-activity-cs", __VA_ARGS__))

/**
* Our saved state data.
*/
struct saved_state {
	int32_t pointerId;
	int32_t x;
	int32_t y;
};

typedef void (*defEmptyFunc)();
typedef void (*defTheeIntFunc)(int32_t, int32_t, int32_t);
typedef void (*defFourIntFunc)(int32_t, int32_t, int32_t, int32_t);

/**
* Shared state for our app.
*/
struct engine {
	struct android_app* app;

	ASensorManager* sensorManager;
	const ASensor* accelerometerSensor;
	ASensorEventQueue* sensorEventQueue;

	int active;
	EGLDisplay display;
	EGLSurface surface;
	EGLContext context;
	int32_t width;
	int32_t height;
	struct saved_state state;

	defEmptyFunc displayFunc;
	defEmptyFunc initFunc;
	defFourIntFunc mouseFunc;
	defTheeIntFunc motionFunc;
};

/**
* Initialize an EGL context for the current display.
*/
static int engine_init_display(struct engine* engine) {
	// initialize OpenGL ES and EGL

	/*
	* Here specify the attributes of the desired configuration.
	* Below, we select an EGLConfig with at least 8 bits per color
	* component compatible with on-screen windows0
	*/
	const EGLint attribs[] = {
		EGL_SURFACE_TYPE, EGL_WINDOW_BIT,
		EGL_BLUE_SIZE, 8,
		EGL_GREEN_SIZE, 8,
		EGL_RED_SIZE, 8,
		EGL_DEPTH_SIZE, 16,
		EGL_NONE
	};
	EGLint w, h, dummy, format;
	EGLint numConfigs;
	EGLConfig config;
	EGLSurface surface;
	EGLContext context;

	EGLDisplay display = eglGetDisplay(EGL_DEFAULT_DISPLAY);

	eglInitialize(display, 0, 0);

	/* Here, the application chooses the configuration it desires. In this
	* sample, we have a very simplified selection process, where we pick
	* the first EGLConfig that matches our criteria */
	eglChooseConfig(display, attribs, &config, 1, &numConfigs);

	/* EGL_NATIVE_VISUAL_ID is an attribute of the EGLConfig that is
	* guaranteed to be accepted by ANativeWindow_setBuffersGeometry().
	* As soon as we picked a EGLConfig, we can safely reconfigure the
	* ANativeWindow buffers to match, using EGL_NATIVE_VISUAL_ID. */
	eglGetConfigAttrib(display, config, EGL_NATIVE_VISUAL_ID, &format);

	ANativeWindow_setBuffersGeometry(engine->app->window, 0, 0, format);

	surface = eglCreateWindowSurface(display, config, engine->app->window, NULL);

	int attrib_list[3] = {EGL_CONTEXT_CLIENT_VERSION, 2, EGL_NONE };
	context = eglCreateContext(display, config, NULL, attrib_list);

	if (eglMakeCurrent(display, surface, surface, context) == EGL_FALSE) {
		LOGW("Unable to eglMakeCurrent");
		return -1;
	}

	EGLint ver = 0;
	eglQueryContext(display, context, EGL_CONTEXT_CLIENT_VERSION, &ver);
	LOGI("OpenGL Version: %d", ver);

	eglQuerySurface(display, surface, EGL_WIDTH, &w);
	eglQuerySurface(display, surface, EGL_HEIGHT, &h);

	engine->display = display;
	engine->context = context;
	engine->surface = surface;
	engine->width = w;
	engine->height = h;

	LOGI("width/height %d/%d", w, h);

	if (engine->initFunc != NULL)
	{
		engine->initFunc();
	}

	return 0;
}

/**
* Just the current frame in the display.
*/
static void engine_draw_frame(struct engine* engine) {
	if (engine->display == NULL) {
		// No display.
		return;
	}

	// Just fill the screen with a color.
	////glClearColor(((float)engine->state.x)/engine->width, 0.5, ((float)engine->state.y)/engine->height, 1);
	////glClear(GL_COLOR_BUFFER_BIT);

	if (engine->displayFunc != NULL)
	{
		engine->displayFunc();
	}

	eglSwapBuffers(engine->display, engine->surface);
}

/**
* Tear down the EGL context currently associated with the display.
*/
static void engine_term_display(struct engine* engine) {
	if (engine->display != EGL_NO_DISPLAY) {
		eglMakeCurrent(engine->display, EGL_NO_SURFACE, EGL_NO_SURFACE, EGL_NO_CONTEXT);
		if (engine->context != EGL_NO_CONTEXT) {
			eglDestroyContext(engine->display, engine->context);
		}
		if (engine->surface != EGL_NO_SURFACE) {
			eglDestroySurface(engine->display, engine->surface);
		}

		eglTerminate(engine->display);
	}

	engine->active = 0;
	engine->display = EGL_NO_DISPLAY;
	engine->context = EGL_NO_CONTEXT;
	engine->surface = EGL_NO_SURFACE;
}

/**
* Process the next input event.
*/
static int32_t engine_handle_input(struct android_app* app, AInputEvent* event) {
	struct engine* engine = (struct engine*)app->userData;
	if (AInputEvent_getType(event) == AINPUT_EVENT_TYPE_MOTION) {

		int action = AMotionEvent_getAction(event) & AMOTION_EVENT_ACTION_MASK;

		engine->state.pointerId = AMotionEvent_getPointerId(event, 0);
		engine->state.x = AMotionEvent_getX(event, 0);
		engine->state.y = AMotionEvent_getY(event, 0);

		if (engine->mouseFunc != NULL)
		{
			switch (action)
			{
			case AMOTION_EVENT_ACTION_DOWN:
				engine->mouseFunc(engine->state.pointerId, AMOTION_EVENT_ACTION_DOWN, engine->state.x, engine->state.y);
				break;
			case AMOTION_EVENT_ACTION_UP:
				engine->mouseFunc(engine->state.pointerId, AMOTION_EVENT_ACTION_UP, engine->state.x, engine->state.y);
				break;
			default:
				engine->motionFunc(engine->state.pointerId, engine->state.x, engine->state.y);
				break;
			}
		}

		return 1;
	}

	return 0;
}

#ifdef __cplusplus
extern "C" {
#endif
	extern int main(int argc, wchar_t** args);

	defEmptyFunc _initFunc;
	defEmptyFunc _displayFunc;
	defTheeIntFunc _motionFunc;
	defFourIntFunc _mouseFunc;

	void* _assetManager;

	void InitFunc(void* initFunc)
	{
		_initFunc = (defEmptyFunc)initFunc;	
	}

	void DisplayFunc(void* displayFunc)
	{
		_displayFunc = (defEmptyFunc)displayFunc;	
	}

	void MotionFunc(void* motionFunc)
	{
		_motionFunc = (defTheeIntFunc)motionFunc;	
	}

	void MouseFunc(void* mouseFunc)
	{
		_mouseFunc = (defFourIntFunc)mouseFunc;	
	}

	void* GetAssetManagerFunc()
	{
		return _assetManager;
	}

	void _logi(char * msg)
	{
		LOGI("%s", msg);
	}

	void _logw(char * msg)
	{
		LOGW("%s", msg);
	}

	void _loge(char * msg)
	{
		LOGE("%s", msg);
	}

#ifdef __cplusplus
}
#endif

/**
* Process the next main command.
*/
static void engine_handle_cmd(struct android_app* app, int32_t cmd) {
	struct engine* engine = (struct engine*)app->userData;
	switch (cmd) {
	case APP_CMD_SAVE_STATE:
		// The system has asked us to save our current state.  Do so.
		engine->app->savedState = malloc(sizeof(struct saved_state));
		*((struct saved_state*)engine->app->savedState) = engine->state;
		engine->app->savedStateSize = sizeof(struct saved_state);
		break;
	case APP_CMD_INIT_WINDOW:

		LOGI("GC_INIT start test %d", GC_get_heap_size());

		main(0, NULL);

		// The window is being shown, get it ready.
		if (engine->app->window != NULL) {

			if (_initFunc != NULL)
			{
				engine->initFunc = _initFunc;
				_initFunc = NULL;
			}

			if (_displayFunc != NULL)
			{
				engine->displayFunc = _displayFunc;
				_displayFunc = NULL;
			}

			if (_motionFunc != NULL)
			{
				engine->motionFunc = _motionFunc;
				_motionFunc = NULL;
			}

			if (_mouseFunc != NULL)
			{
				engine->mouseFunc = _mouseFunc;
				_mouseFunc = NULL;
			}

			engine_init_display(engine);
			engine_draw_frame(engine);
		}
		break;
	case APP_CMD_TERM_WINDOW:
		// The window is being hidden or closed, clean it up.
		engine_term_display(engine);
		break;
	case APP_CMD_GAINED_FOCUS:
		// When our app gains focus, we start monitoring the accelerometer.
		engine->active = 1;
		if (engine->accelerometerSensor != NULL) {
			ASensorEventQueue_enableSensor(engine->sensorEventQueue,
				engine->accelerometerSensor);
			// We'd like to get 60 events per second (in us).
			ASensorEventQueue_setEventRate(engine->sensorEventQueue,
				engine->accelerometerSensor, (1000L/60)*1000);
		}
		break;
	case APP_CMD_LOST_FOCUS:
		// When our app loses focus, we stop monitoring the accelerometer.
		// This is to avoid consuming battery while not being used.
		if (engine->accelerometerSensor != NULL) {
			ASensorEventQueue_disableSensor(engine->sensorEventQueue,
				engine->accelerometerSensor);
		}
		// Also stop animating.
		engine->active = 0;
		engine_draw_frame(engine);
		break;
	}
}

/**
* This is the main entry point of a native application that is using
* android_native_app_glue.  It runs in its own thread, with its own
* event loop for receiving input events and doing other things.
*/
void android_main(struct android_app* state) {
	struct engine engine;

	// Make sure glue isn't stripped.
	app_dummy();

	//sleep(20);

	// init functions bridge
	_initFunc = NULL;
	_displayFunc = NULL;
	_motionFunc = NULL;
	_mouseFunc = NULL;

	// store asset manager pointer
	_assetManager = state->activity->assetManager;

	memset(&engine, 0, sizeof(engine));
	state->userData = &engine;
	state->onAppCmd = engine_handle_cmd;
	state->onInputEvent = engine_handle_input;
	engine.app = state;

	// Prepare to monitor accelerometer
	engine.sensorManager = ASensorManager_getInstance();
	engine.accelerometerSensor = ASensorManager_getDefaultSensor(engine.sensorManager,
		ASENSOR_TYPE_ACCELEROMETER);
	engine.sensorEventQueue = ASensorManager_createEventQueue(engine.sensorManager,
		state->looper, LOOPER_ID_USER, NULL, NULL);

	if (state->savedState != NULL) {
		// We are starting with a previous saved state; restore from it.
		engine.state = *(struct saved_state*)state->savedState;
	}

	// loop waiting for stuff to do.

	while (1) {
		// Read all pending events.
		int ident;
		int events;
		struct android_poll_source* source;

		// If not animating, we will block forever waiting for events.
		// If animating, we loop until all events are read, then continue
		// to draw the next frame of animation.
		while ((ident=ALooper_pollAll(engine.active ? 1 : 250, NULL, &events,
			(void**)&source)) >= 0) {

				// Process this event.
				if (source != NULL) {
					source->process(state, source);
				}

				// If a sensor has data, process it now.
				if (ident == LOOPER_ID_USER) {
					if (engine.accelerometerSensor != NULL) {
						ASensorEvent event;
						while (ASensorEventQueue_getEvents(engine.sensorEventQueue,
							&event, 1) > 0) {
								/*
								LOGI("accelerometer: x=%f y=%f z=%f",
								event.acceleration.x, event.acceleration.y,
								event.acceleration.z);
								*/
						}
					}
				}

				// Check if we are exiting.
				if (state->destroyRequested != 0) {
					engine_term_display(&engine);
					return;
				}
		}
		
		if (engine.active) {
			// Drawing is throttled to the screen update rate, so there
			// is no need to do timing here.
			engine_draw_frame(&engine);
		}
	}
}
//END_INCLUDE(all)
