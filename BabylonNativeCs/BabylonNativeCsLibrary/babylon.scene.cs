using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace BABYLON
{
    public interface IDisposable
    {
        void dispose();
    }
    public class Scene
    {
        public static dynamic FOGMODE_NONE = 0;
        public static dynamic FOGMODE_EXP = 1;
        public static dynamic FOGMODE_EXP2 = 2;
        public static dynamic FOGMODE_LINEAR = 3;
        public static dynamic MinDeltaTime = 1.0;
        public static dynamic MaxDeltaTime = 1000.0;
        public dynamic autoClear = true;
        public dynamic clearColor = new BABYLON.Color3(0.2, 0.2, 0.3);
        public dynamic ambientColor = new BABYLON.Color3(0, 0, 0);
        public Func<object> beforeRender;
        public Func<object> afterRender;
        public Func<object> onDispose;
        public Func<Camera, object> beforeCameraRender;
        public Func<Camera, object> afterCameraRender;
        public dynamic forceWireframe = false;
        public Plane clipPlane;
        private Func<PointerEvent, object> _onPointerMove;
        private Func<PointerEvent, object> _onPointerDown;
        public Func<PointerEvent, PickingInfo, object> onPointerDown;
        public Camera cameraToUseForPointers = null;
        private float _pointerX;
        private float _pointerY;
        private AbstractMesh _meshUnderPointer;
        public dynamic fogMode = BABYLON.Scene.FOGMODE_NONE;
        public dynamic fogColor = new Color3(0.2, 0.2, 0.3);
        public dynamic fogDensity = 0.1;
        public dynamic fogStart = 0;
        public dynamic fogEnd = 1000.0;
        public dynamic lightsEnabled = true;
        public dynamic lights = new Array();
        public dynamic cameras = new Array();
        public dynamic activeCameras = new Array();
        public Camera activeCamera;
        public dynamic meshes = new Array();
        private dynamic _geometries = new Array();
        public dynamic materials = new Array();
        public dynamic multiMaterials = new Array();
        public dynamic defaultMaterial = new BABYLON.StandardMaterial("default materia", this);
        public dynamic texturesEnabled = true;
        public dynamic textures = new Array();
        public dynamic particlesEnabled = true;
        public dynamic particleSystems = new Array();
        public dynamic spriteManagers = new Array();
        public dynamic layers = new Array();
        public dynamic skeletons = new Array();
        public dynamic lensFlareSystems = new Array();
        public dynamic collisionsEnabled = true;
        public dynamic gravity = new BABYLON.Vector3(0, -9.0, 0);
        public dynamic postProcessesEnabled = true;
        public PostProcessManager postProcessManager;
        public PostProcessRenderPipelineManager postProcessRenderPipelineManager;
        public dynamic renderTargetsEnabled = true;
        public dynamic customRenderTargets = new Array();
        public bool useDelayedTextureLoading;
        public dynamic importedMeshesFiles = new Array();
        public dynamic database;
        public ActionManager actionManager;
        public dynamic _actionManagers = new Array();
        private dynamic _meshesForIntersections = new SmartArray(256);
        private Engine _engine;
        private dynamic _totalVertices = 0;
        public dynamic _activeVertices = 0;
        public dynamic _activeParticles = 0;
        private dynamic _lastFrameDuration = 0;
        private dynamic _evaluateActiveMeshesDuration = 0;
        private dynamic _renderTargetsDuration = 0;
        public dynamic _particlesDuration = 0;
        private dynamic _renderDuration = 0;
        public dynamic _spritesDuration = 0;
        private dynamic _animationRatio = 0;
        private float _animationStartDate;
        private dynamic _renderId = 0;
        private dynamic _executeWhenReadyTimeoutId = -1;
        public dynamic _toBeDisposed = new SmartArray(256);
        private dynamic _onReadyCallbacks = new Array();
        private dynamic _pendingData = new Array<object>();
        private dynamic _onBeforeRenderCallbacks = new Array();
        private dynamic _activeMeshes = new SmartArray(256);
        private dynamic _processedMaterials = new SmartArray(256);
        private dynamic _renderTargets = new SmartArray(256);
        public dynamic _activeParticleSystems = new SmartArray(256);
        private dynamic _activeSkeletons = new SmartArray(32);
        private RenderingManager _renderingManager;
        private PhysicsEngine _physicsEngine;
        public dynamic _activeAnimatables = new Array();
        private dynamic _transformMatrix = Matrix.Zero();
        private Matrix _pickWithRayInverseMatrix;
        private dynamic _scaledPosition = Vector3.Zero();
        private dynamic _scaledVelocity = Vector3.Zero();
        private BoundingBoxRenderer _boundingBoxRenderer;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;
        private Plane[] _frustumPlanes;
        private Octree<AbstractMesh> _selectionOctree;
        private AbstractMesh _pointerOverMesh;
        public Scene(Engine engine)
        {
            this._engine = engine;
            engine.scenes.push(this);
            this._renderingManager = new RenderingManager(this);
            this.postProcessManager = new PostProcessManager(this);
            this.postProcessRenderPipelineManager = new PostProcessRenderPipelineManager();
            this._boundingBoxRenderer = new BoundingBoxRenderer(this);
            this.attachControl();
        }
        public virtual BoundingBoxRenderer getBoundingBoxRenderer()
        {
            return this._boundingBoxRenderer;
        }
        public virtual Engine getEngine()
        {
            return this._engine;
        }
        public virtual float getTotalVertices()
        {
            return this._totalVertices;
        }
        public virtual float getActiveVertices()
        {
            return this._activeVertices;
        }
        public virtual float getActiveParticles()
        {
            return this._activeParticles;
        }
        public virtual float getLastFrameDuration()
        {
            return this._lastFrameDuration;
        }
        public virtual float getEvaluateActiveMeshesDuration()
        {
            return this._evaluateActiveMeshesDuration;
        }
        public virtual SmartArray<Mesh> getActiveMeshes()
        {
            return this._activeMeshes;
        }
        public virtual float getRenderTargetsDuration()
        {
            return this._renderTargetsDuration;
        }
        public virtual float getRenderDuration()
        {
            return this._renderDuration;
        }
        public virtual float getParticlesDuration()
        {
            return this._particlesDuration;
        }
        public virtual float getSpritesDuration()
        {
            return this._spritesDuration;
        }
        public virtual float getAnimationRatio()
        {
            return this._animationRatio;
        }
        public virtual float getRenderId()
        {
            return this._renderId;
        }
        private virtual void _updatePointerPosition(PointerEvent evt)
        {
            var canvasRect = this._engine.getRenderingCanvasClientRect();
            this._pointerX = evt.clientX - canvasRect.left;
            this._pointerY = evt.clientY - canvasRect.top;
            if (this.cameraToUseForPointers)
            {
                this._pointerX = this._pointerX - this.cameraToUseForPointers.viewport.x * this._engine.getRenderWidth();
                this._pointerY = this._pointerY - this.cameraToUseForPointers.viewport.y * this._engine.getRenderHeight();
            }
        }
        public virtual void attachControl()
        {
            this._onPointerMove = (PointerEvent evt) =>
            {
                var canvas = this._engine.getRenderingCanvas();
                this._updatePointerPosition(evt);
                var pickResult = this.pick(this._pointerX, this._pointerY, (AbstractMesh mesh) =>
                {
                }
                , false, this.cameraToUseForPointers);
                if (pickResult.hit)
                {
                    this.setPointerOverMesh(pickResult.pickedMesh);
                    canvas.style.cursor = "pointe";
                    this._meshUnderPointer = pickResult.pickedMesh;
                }
                else
                {
                    this.setPointerOverMesh(null);
                    canvas.style.cursor = "\"";
                    this._meshUnderPointer = null;
                }
            }
            ;
            this._onPointerDown = (PointerEvent evt) =>
            {
                var predicate = null;
                if (!this.onPointerDown)
                {
                    predicate = (AbstractMesh mesh) =>
                    {
                        return mesh.isPickable && mesh.isVisible && mesh.isReady() && mesh.actionManager && mesh.actionManager.hasPickTriggers;
                    }
                    ;
                }
                this._updatePointerPosition(evt);
                var pickResult = this.pick(this._pointerX, this._pointerY, predicate, false, this.cameraToUseForPointers);
                if (pickResult.hit)
                {
                    if (pickResult.pickedMesh.actionManager)
                    {
                        switch (evt.button)
                        {
                            case 0:
                                pickResult.pickedMesh.actionManager.processTrigger(BABYLON.ActionManager.OnLeftPickTrigger, ActionEvent.CreateNew(pickResult.pickedMesh));
                                break;
                            case 1:
                                pickResult.pickedMesh.actionManager.processTrigger(BABYLON.ActionManager.OnCenterPickTrigger, ActionEvent.CreateNew(pickResult.pickedMesh));
                                break;
                            case 2:
                                pickResult.pickedMesh.actionManager.processTrigger(BABYLON.ActionManager.OnRightPickTrigger, ActionEvent.CreateNew(pickResult.pickedMesh));
                                break;
                        }
                        pickResult.pickedMesh.actionManager.processTrigger(BABYLON.ActionManager.OnPickTrigger, ActionEvent.CreateNew(pickResult.pickedMesh));
                    }
                }
                if (this.onPointerDown)
                {
                    this.onPointerDown(evt, pickResult);
                }
            }
            ;
            var eventPrefix = Tools.GetPointerPrefix();
            this._engine.getRenderingCanvas().addEventListener(eventPrefix + "mov", this._onPointerMove, false);
            this._engine.getRenderingCanvas().addEventListener(eventPrefix + "dow", this._onPointerDown, false);
        }
        public virtual void detachControl()
        {
            var eventPrefix = Tools.GetPointerPrefix();
            this._engine.getRenderingCanvas().removeEventListener(eventPrefix + "mov", this._onPointerMove);
            this._engine.getRenderingCanvas().removeEventListener(eventPrefix + "dow", this._onPointerDown);
        }
        public virtual bool isReady()
        {
            if (this._pendingData.length > 0)
            {
                return false;
            }
            for (var index = 0; index < this._geometries.length; index++)
            {
                var geometry = this._geometries[index];
                if (geometry.delayLoadState == BABYLON.Engine.DELAYLOADSTATE_LOADING)
                {
                    return false;
                }
            }
            for (index = 0; index < this.meshes.length; index++)
            {
                var mesh = this.meshes[index];
                if (!mesh.isReady())
                {
                    return false;
                }
                var mat = mesh.material;
                if (mat)
                {
                    if (!mat.isReady(mesh))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public virtual void registerBeforeRender(Func<object> func)
        {
            this._onBeforeRenderCallbacks.push(func);
        }
        public virtual void unregisterBeforeRender(Func<object> func)
        {
            var index = this._onBeforeRenderCallbacks.indexOf(func);
            if (index > -1)
            {
                this._onBeforeRenderCallbacks.splice(index, 1);
            }
        }
        public virtual void _addPendingData(dynamic data)
        {
            this._pendingData.push(data);
        }
        public virtual void _removePendingData(dynamic data)
        {
            var index = this._pendingData.indexOf(data);
            if (index != -1)
            {
                this._pendingData.splice(index, 1);
            }
        }
        public virtual float getWaitingItemsCount()
        {
            return this._pendingData.length;
        }
        public virtual void executeWhenReady(Func<object> func)
        {
            this._onReadyCallbacks.push(func);
            if (this._executeWhenReadyTimeoutId != -1)
            {
                return;
            }
            this._executeWhenReadyTimeoutId = setTimeout(() =>
            {
                this._checkIsReady();
            }
            , 150);
        }
        public virtual void _checkIsReady()
        {
            if (this.isReady())
            {
                this._onReadyCallbacks.forEach((dynamic func) =>
                {
                    func();
                }
                );
                this._onReadyCallbacks = new Array<object>();
                this._executeWhenReadyTimeoutId = -1;
                return;
            }
            this._executeWhenReadyTimeoutId = setTimeout(() =>
            {
                this._checkIsReady();
            }
            , 150);
        }
        public virtual Animatable beginAnimation(object target, float from, float to, bool loop, float speedRatio, Func<object> onAnimationEnd, Animatable animatable)
        {
            if (speedRatio == undefined)
            {
                speedRatio = 1.0;
            }
            this.stopAnimation(target);
            if (!animatable)
            {
                animatable = new Animatable(this, target, from, to, loop, speedRatio, onAnimationEnd);
            }
            if (target.animations)
            {
                animatable.appendAnimations(target, target.animations);
            }
            if (target.getAnimatables)
            {
                var animatables = target.getAnimatables();
                for (var index = 0; index < animatables.length; index++)
                {
                    this.beginAnimation(animatables[index], from, to, loop, speedRatio, onAnimationEnd, animatable);
                }
            }
            return animatable;
        }
        public virtual Animatable beginDirectAnimation(object target, Animation[] animations, float from, float to, bool loop, float speedRatio, Func<object> onAnimationEnd)
        {
            if (speedRatio == undefined)
            {
                speedRatio = 1.0;
            }
            var animatable = new BABYLON.Animatable(this, target, from, to, loop, speedRatio, onAnimationEnd, animations);
            return animatable;
        }
        public virtual Animatable getAnimatableByTarget(object target)
        {
            for (var index = 0; index < this._activeAnimatables.length; index++)
            {
                if (this._activeAnimatables[index].target == target)
                {
                    return this._activeAnimatables[index];
                }
            }
            return null;
        }
        public virtual void stopAnimation(object target)
        {
            var animatable = this.getAnimatableByTarget(target);
            if (animatable)
            {
                animatable.stop();
            }
        }
        private virtual void _animate()
        {
            if (!this._animationStartDate)
            {
                this._animationStartDate = new Date().getTime();
            }
            var now = new Date().getTime();
            var delay = now - this._animationStartDate;
            for (var index = 0; index < this._activeAnimatables.length; index++)
            {
                if (!this._activeAnimatables[index]._animate(delay))
                {
                    this._activeAnimatables.splice(index, 1);
                    index--;
                }
            }
        }
        public virtual Matrix getViewMatrix()
        {
            return this._viewMatrix;
        }
        public virtual Matrix getProjectionMatrix()
        {
            return this._projectionMatrix;
        }
        public virtual Matrix getTransformMatrix()
        {
            return this._transformMatrix;
        }
        public virtual void setTransformMatrix(Matrix view, Matrix projection)
        {
            this._viewMatrix = view;
            this._projectionMatrix = projection;
            this._viewMatrix.multiplyToRef(this._projectionMatrix, this._transformMatrix);
        }
        public virtual Camera setActiveCameraByID(string id)
        {
            var camera = this.getCameraByID(id);
            if (camera)
            {
                this.activeCamera = camera;
                return camera;
            }
            return null;
        }
        public virtual Camera setActiveCameraByName(string name)
        {
            var camera = this.getCameraByName(name);
            if (camera)
            {
                this.activeCamera = camera;
                return camera;
            }
            return null;
        }
        public virtual Material getMaterialByID(string id)
        {
            for (var index = 0; index < this.materials.length; index++)
            {
                if (this.materials[index].id == id)
                {
                    return this.materials[index];
                }
            }
            return null;
        }
        public virtual Material getMaterialByName(string name)
        {
            for (var index = 0; index < this.materials.length; index++)
            {
                if (this.materials[index].name == name)
                {
                    return this.materials[index];
                }
            }
            return null;
        }
        public virtual Camera getCameraByID(string id)
        {
            for (var index = 0; index < this.cameras.length; index++)
            {
                if (this.cameras[index].id == id)
                {
                    return this.cameras[index];
                }
            }
            return null;
        }
        public virtual Camera getCameraByName(string name)
        {
            for (var index = 0; index < this.cameras.length; index++)
            {
                if (this.cameras[index].name == name)
                {
                    return this.cameras[index];
                }
            }
            return null;
        }
        public virtual Light getLightByName(string name)
        {
            for (var index = 0; index < this.lights.length; index++)
            {
                if (this.lights[index].name == name)
                {
                    return this.lights[index];
                }
            }
            return null;
        }
        public virtual Light getLightByID(string id)
        {
            for (var index = 0; index < this.lights.length; index++)
            {
                if (this.lights[index].id == id)
                {
                    return this.lights[index];
                }
            }
            return null;
        }
        public virtual Geometry getGeometryByID(string id)
        {
            for (var index = 0; index < this._geometries.length; index++)
            {
                if (this._geometries[index].id == id)
                {
                    return this._geometries[index];
                }
            }
            return null;
        }
        public virtual bool pushGeometry(Geometry geometry, bool force)
        {
            if (!force && this.getGeometryByID(geometry.id))
            {
                return false;
            }
            this._geometries.push(geometry);
            return true;
        }
        public virtual Geometry[] getGeometries()
        {
            return this._geometries;
        }
        public virtual AbstractMesh getMeshByID(string id)
        {
            for (var index = 0; index < this.meshes.length; index++)
            {
                if (this.meshes[index].id == id)
                {
                    return this.meshes[index];
                }
            }
            return null;
        }
        public virtual AbstractMesh getLastMeshByID(string id)
        {
            for (var index = this.meshes.length - 1; index >= 0; index--)
            {
                if (this.meshes[index].id == id)
                {
                    return this.meshes[index];
                }
            }
            return null;
        }
        public virtual Node getLastEntryByID(string id)
        {
            for (var index = this.meshes.length - 1; index >= 0; index--)
            {
                if (this.meshes[index].id == id)
                {
                    return this.meshes[index];
                }
            }
            for (index = this.cameras.length - 1; index >= 0; index--)
            {
                if (this.cameras[index].id == id)
                {
                    return this.cameras[index];
                }
            }
            for (index = this.lights.length - 1; index >= 0; index--)
            {
                if (this.lights[index].id == id)
                {
                    return this.lights[index];
                }
            }
            return null;
        }
        public virtual AbstractMesh getMeshByName(string name)
        {
            for (var index = 0; index < this.meshes.length; index++)
            {
                if (this.meshes[index].name == name)
                {
                    return this.meshes[index];
                }
            }
            return null;
        }
        public virtual Skeleton getLastSkeletonByID(string id)
        {
            for (var index = this.skeletons.length - 1; index >= 0; index--)
            {
                if (this.skeletons[index].id == id)
                {
                    return this.skeletons[index];
                }
            }
            return null;
        }
        public virtual Skeleton getSkeletonById(string id)
        {
            for (var index = 0; index < this.skeletons.length; index++)
            {
                if (this.skeletons[index].id == id)
                {
                    return this.skeletons[index];
                }
            }
            return null;
        }
        public virtual Skeleton getSkeletonByName(string name)
        {
            for (var index = 0; index < this.skeletons.length; index++)
            {
                if (this.skeletons[index].name == name)
                {
                    return this.skeletons[index];
                }
            }
            return null;
        }
        public virtual bool isActiveMesh(Mesh mesh)
        {
            return (this._activeMeshes.indexOf(mesh) != -1);
        }
        private virtual void _evaluateSubMesh(SubMesh subMesh, AbstractMesh mesh)
        {
            if (mesh.subMeshes.length == 1 || subMesh.isInFrustum(this._frustumPlanes))
            {
                var material = subMesh.getMaterial();
                if (mesh.showSubMeshesBoundingBox)
                {
                    this._boundingBoxRenderer.renderList.push(subMesh.getBoundingInfo().boundingBox);
                }
                if (material)
                {
                    if (material.getRenderTargetTextures)
                    {
                        if (this._processedMaterials.indexOf(material) == -1)
                        {
                            this._processedMaterials.push(material);
                            this._renderTargets.concat(material.getRenderTargetTextures());
                        }
                    }
                    this._activeVertices += subMesh.verticesCount;
                    this._renderingManager.dispatch(subMesh);
                }
            }
        }
        private virtual void _evaluateActiveMeshes()
        {
            this._activeMeshes.reset();
            this._renderingManager.reset();
            this._processedMaterials.reset();
            this._activeParticleSystems.reset();
            this._activeSkeletons.reset();
            this._boundingBoxRenderer.reset();
            if (!this._frustumPlanes)
            {
                this._frustumPlanes = BABYLON.Frustum.GetPlanes(this._transformMatrix);
            }
            else
            {
                BABYLON.Frustum.GetPlanesToRef(this._transformMatrix, this._frustumPlanes);
            }
            var meshes;
            var len;
            if (this._selectionOctree)
            {
                var selection = this._selectionOctree.select(this._frustumPlanes);
                meshes = selection.data;
                len = selection.length;
            }
            else
            {
                len = this.meshes.length;
                meshes = this.meshes;
            }
            for (var meshIndex = 0; meshIndex < len; meshIndex++)
            {
                var mesh = meshes[meshIndex];
                this._totalVertices += mesh.getTotalVertices();
                if (!mesh.isReady())
                {
                    continue;
                }
                mesh.computeWorldMatrix();
                mesh._preActivate();
                if (mesh.actionManager && mesh.actionManager.hasSpecificTriggers(new Array<object>()))
                {
                    this._meshesForIntersections.pushNoDuplicate(mesh);
                }
                if (mesh.isEnabled() && mesh.isVisible && mesh.visibility > 0 && ((mesh.layerMask & this.activeCamera.layerMask) != 0) && mesh.isInFrustum(this._frustumPlanes))
                {
                    this._activeMeshes.push(mesh);
                    mesh._activate(this._renderId);
                    this._activeMesh(mesh);
                }
            }
            var beforeParticlesDate = new Date().getTime();
            if (this.particlesEnabled)
            {
                for (var particleIndex = 0; particleIndex < this.particleSystems.length; particleIndex++)
                {
                    var particleSystem = this.particleSystems[particleIndex];
                    if (!particleSystem.isStarted())
                    {
                        continue;
                    }
                    if (!particleSystem.emitter.position || (particleSystem.emitter && particleSystem.emitter.isEnabled()))
                    {
                        this._activeParticleSystems.push(particleSystem);
                        particleSystem.animate();
                    }
                }
            }
            this._particlesDuration += new Date().getTime() - beforeParticlesDate;
        }
        private virtual void _activeMesh(AbstractMesh mesh)
        {
            if (mesh.skeleton)
            {
                this._activeSkeletons.pushNoDuplicate(mesh.skeleton);
            }
            if (mesh.showBoundingBox)
            {
                this._boundingBoxRenderer.renderList.push(mesh.getBoundingInfo().boundingBox);
            }
            if (mesh.subMeshes)
            {
                var len;
                var subMeshes;
                if (mesh._submeshesOctree && mesh.useOctreeForRenderingSelection)
                {
                    var intersections = mesh._submeshesOctree.select(this._frustumPlanes);
                    len = intersections.length;
                    subMeshes = intersections.data;
                }
                else
                {
                    subMeshes = mesh.subMeshes;
                    len = subMeshes.length;
                }
                for (var subIndex = 0; subIndex < len; subIndex++)
                {
                    var subMesh = subMeshes[subIndex];
                    this._evaluateSubMesh(subMesh, mesh);
                }
            }
        }
        public virtual void updateTransformMatrix(bool force)
        {
            this.setTransformMatrix(this.activeCamera.getViewMatrix(), this.activeCamera.getProjectionMatrix(force));
        }
        private virtual void _renderForCamera(Camera camera)
        {
            var engine = this._engine;
            this.activeCamera = camera;
            if (!this.activeCamera)
                throw new Exception("Active camera not se");
            engine.setViewport(this.activeCamera.viewport);
            this._renderId++;
            this.updateTransformMatrix();
            if (this.beforeCameraRender)
            {
                this.beforeCameraRender(this.activeCamera);
            }
            var beforeEvaluateActiveMeshesDate = new Date().getTime();
            this._evaluateActiveMeshes();
            this._evaluateActiveMeshesDuration += new Date().getTime() - beforeEvaluateActiveMeshesDate;
            for (var skeletonIndex = 0; skeletonIndex < this._activeSkeletons.length; skeletonIndex++)
            {
                var skeleton = this._activeSkeletons.data[skeletonIndex];
                skeleton.prepare();
            }
            for (var customIndex = 0; customIndex < this.customRenderTargets.length; customIndex++)
            {
                var renderTarget = this.customRenderTargets[customIndex];
                this._renderTargets.push(renderTarget);
            }
            var beforeRenderTargetDate = new Date().getTime();
            if (this.renderTargetsEnabled)
            {
                for (var renderIndex = 0; renderIndex < this._renderTargets.length; renderIndex++)
                {
                    renderTarget = this._renderTargets.data[renderIndex];
                    if (renderTarget._shouldRender())
                    {
                        this._renderId++;
                        renderTarget.render();
                    }
                }
                this._renderId++;
            }
            if (this._renderTargets.length > 0)
            {
                engine.restoreDefaultFramebuffer();
            }
            this._renderTargetsDuration = new Date().getTime() - beforeRenderTargetDate;
            this.postProcessManager._prepareFrame();
            var beforeRenderDate = new Date().getTime();
            if (this.layers.length)
            {
                engine.setDepthBuffer(false);
                var layerIndex;
                var layer;
                for (layerIndex = 0; layerIndex < this.layers.length; layerIndex++)
                {
                    layer = this.layers[layerIndex];
                    if (layer.isBackground)
                    {
                        layer.render();
                    }
                }
                engine.setDepthBuffer(true);
            }
            this._renderingManager.render(null, null, true, true);
            this._boundingBoxRenderer.render();
            for (var lensFlareSystemIndex = 0; lensFlareSystemIndex < this.lensFlareSystems.length; lensFlareSystemIndex++)
            {
                this.lensFlareSystems[lensFlareSystemIndex].render();
            }
            if (this.layers.length)
            {
                engine.setDepthBuffer(false);
                for (layerIndex = 0; layerIndex < this.layers.length; layerIndex++)
                {
                    layer = this.layers[layerIndex];
                    if (!layer.isBackground)
                    {
                        layer.render();
                    }
                }
                engine.setDepthBuffer(true);
            }
            this._renderDuration += new Date().getTime() - beforeRenderDate;
            this.postProcessManager._finalizeFrame(camera.isIntermediate);
            this.activeCamera._updateFromScene();
            this._renderTargets.reset();
            if (this.afterCameraRender)
            {
                this.afterCameraRender(this.activeCamera);
            }
        }
        private virtual void _processSubCameras(Camera camera)
        {
            if (camera.subCameras.length == 0)
            {
                this._renderForCamera(camera);
                return;
            }
            for (var index = 0; index < camera.subCameras.length; index++)
            {
                this._renderForCamera(camera.subCameras[index]);
            }
            this.activeCamera = camera;
            this.setTransformMatrix(this.activeCamera.getViewMatrix(), this.activeCamera.getProjectionMatrix());
            this.activeCamera._updateFromScene();
        }
        private virtual void _checkIntersections()
        {
            for (var index = 0; index < this._meshesForIntersections.length; index++)
            {
                var sourceMesh = this._meshesForIntersections.data[index];
                for (var actionIndex = 0; actionIndex < sourceMesh.actionManager.actions.length; actionIndex++)
                {
                    var action = sourceMesh.actionManager.actions[actionIndex];
                    if (action.trigger == ActionManager.OnIntersectionEnterTrigger || action.trigger == ActionManager.OnIntersectionExitTrigger)
                    {
                        var otherMesh = action.getTriggerParameter();
                        var areIntersecting = otherMesh.intersectsMesh(sourceMesh, false);
                        var currentIntersectionInProgress = sourceMesh._intersectionsInProgress.indexOf(otherMesh);
                        if (areIntersecting && currentIntersectionInProgress == -1 && action.trigger == ActionManager.OnIntersectionEnterTrigger)
                        {
                            sourceMesh.actionManager.processTrigger(ActionManager.OnIntersectionEnterTrigger, ActionEvent.CreateNew(sourceMesh));
                            sourceMesh._intersectionsInProgress.push(otherMesh);
                        }
                        else
                            if (!areIntersecting && currentIntersectionInProgress > -1 && action.trigger == ActionManager.OnIntersectionExitTrigger)
                            {
                                sourceMesh.actionManager.processTrigger(ActionManager.OnIntersectionExitTrigger, ActionEvent.CreateNew(sourceMesh));
                                var indexOfOther = sourceMesh._intersectionsInProgress.indexOf(otherMesh);
                                if (indexOfOther > -1)
                                {
                                    sourceMesh._intersectionsInProgress.splice(indexOfOther, 1);
                                }
                            }
                    }
                }
            }
        }
        public virtual void render()
        {
            var startDate = new Date().getTime();
            this._particlesDuration = 0;
            this._spritesDuration = 0;
            this._activeParticles = 0;
            this._renderDuration = 0;
            this._evaluateActiveMeshesDuration = 0;
            this._totalVertices = 0;
            this._activeVertices = 0;
            this._meshesForIntersections.reset();
            if (this.actionManager)
            {
                this.actionManager.processTrigger(ActionManager.OnEveryFrameTrigger, null);
            }
            if (this.beforeRender)
            {
                this.beforeRender();
            }
            for (var callbackIndex = 0; callbackIndex < this._onBeforeRenderCallbacks.length; callbackIndex++)
            {
                this._onBeforeRenderCallbacks[callbackIndex]();
            }
            var deltaTime = Math.max(Scene.MinDeltaTime, Math.min(BABYLON.Tools.GetDeltaTime(), Scene.MaxDeltaTime));
            this._animationRatio = deltaTime * (60.0 / 1000.0);
            this._animate();
            if (this._physicsEngine)
            {
                this._physicsEngine._runOneStep(deltaTime / 1000.0);
            }
            this._engine.clear(this.clearColor, this.autoClear || this.forceWireframe, true);
            for (var lightIndex = 0; lightIndex < this.lights.length; lightIndex++)
            {
                var light = this.lights[lightIndex];
                var shadowGenerator = light.getShadowGenerator();
                if (light.isEnabled() && shadowGenerator && shadowGenerator.getShadowMap().getScene().textures.indexOf(shadowGenerator.getShadowMap()) != -1)
                {
                    this._renderTargets.push(shadowGenerator.getShadowMap());
                }
            }
            this.postProcessRenderPipelineManager.update();
            if (this.activeCameras.length > 0)
            {
                var currentRenderId = this._renderId;
                for (var cameraIndex = 0; cameraIndex < this.activeCameras.length; cameraIndex++)
                {
                    this._renderId = currentRenderId;
                    this._processSubCameras(this.activeCameras[cameraIndex]);
                }
            }
            else
            {
                this._processSubCameras(this.activeCamera);
            }
            this._checkIntersections();
            if (this.afterRender)
            {
                this.afterRender();
            }
            for (var index = 0; index < this._toBeDisposed.length; index++)
            {
                this._toBeDisposed.data[index].dispose();
                this._toBeDisposed[index] = null;
            }
            this._toBeDisposed.reset();
            this._lastFrameDuration = new Date().getTime() - startDate;
        }
        public virtual void dispose()
        {
            this.beforeRender = null;
            this.afterRender = null;
            this.skeletons = new Array<object>();
            this._boundingBoxRenderer.dispose();
            if (this.onDispose)
            {
                this.onDispose();
            }
            this.detachControl();
            var canvas = this._engine.getRenderingCanvas();
            var index;
            for (index = 0; index < this.cameras.length; index++)
            {
                this.cameras[index].detachControl(canvas);
            }
            while (this.lights.length)
            {
                this.lights[0].dispose();
            }
            while (this.meshes.length)
            {
                this.meshes[0].dispose(true);
            }
            while (this.cameras.length)
            {
                this.cameras[0].dispose();
            }
            while (this.materials.length)
            {
                this.materials[0].dispose();
            }
            while (this.particleSystems.length)
            {
                this.particleSystems[0].dispose();
            }
            while (this.spriteManagers.length)
            {
                this.spriteManagers[0].dispose();
            }
            while (this.layers.length)
            {
                this.layers[0].dispose();
            }
            while (this.textures.length)
            {
                this.textures[0].dispose();
            }
            this.postProcessManager.dispose();
            if (this._physicsEngine)
            {
                this.disablePhysicsEngine();
            }
            index = this._engine.scenes.indexOf(this);
            this._engine.scenes.splice(index, 1);
            this._engine.wipeCaches();
        }
        public virtual void _getNewPosition(Vector3 position, Vector3 velocity, Collider collider, float maximumRetry, Vector3 finalPosition, AbstractMesh excludedMesh)
        {
            position.divideToRef(collider.radius, this._scaledPosition);
            velocity.divideToRef(collider.radius, this._scaledVelocity);
            collider.retry = 0;
            collider.initialVelocity = this._scaledVelocity;
            collider.initialPosition = this._scaledPosition;
            this._collideWithWorld(this._scaledPosition, this._scaledVelocity, collider, maximumRetry, finalPosition, excludedMesh);
            finalPosition.multiplyInPlace(collider.radius);
        }
        private virtual void _collideWithWorld(Vector3 position, Vector3 velocity, Collider collider, float maximumRetry, Vector3 finalPosition, AbstractMesh excludedMesh)
        {
            var closeDistance = BABYLON.Engine.CollisionsEpsilon * 10.0;
            if (collider.retry >= maximumRetry)
            {
                finalPosition.copyFrom(position);
                return;
            }
            collider._initialize(position, velocity, closeDistance);
            for (var index = 0; index < this.meshes.length; index++)
            {
                var mesh = this.meshes[index];
                if (mesh.isEnabled() && mesh.checkCollisions && mesh.subMeshes && mesh != excludedMesh)
                {
                    mesh._checkCollision(collider);
                }
            }
            if (!collider.collisionFound)
            {
                position.addToRef(velocity, finalPosition);
                return;
            }
            if (velocity.x != 0 || velocity.y != 0 || velocity.z != 0)
            {
                collider._getResponse(position, velocity);
            }
            if (velocity.length() <= closeDistance)
            {
                finalPosition.copyFrom(position);
                return;
            }
            collider.retry++;
            this._collideWithWorld(position, velocity, collider, maximumRetry, finalPosition, excludedMesh);
        }
        public virtual Octree<AbstractMesh> createOrUpdateSelectionOctree(dynamic maxCapacity, dynamic maxDepth)
        {
            if (!this._selectionOctree)
            {
                this._selectionOctree = new BABYLON.Octree(Octree.CreationFuncForMeshes, maxCapacity, maxDepth);
            }
            var min = new BABYLON.Vector3(Number.MAX_VALUE, Number.MAX_VALUE, Number.MAX_VALUE);
            var max = new BABYLON.Vector3(-Number.MAX_VALUE, -Number.MAX_VALUE, -Number.MAX_VALUE);
            for (var index = 0; index < this.meshes.length; index++)
            {
                var mesh = this.meshes[index];
                mesh.computeWorldMatrix(true);
                var minBox = mesh.getBoundingInfo().boundingBox.minimumWorld;
                var maxBox = mesh.getBoundingInfo().boundingBox.maximumWorld;
                Tools.CheckExtends(minBox, min, max);
                Tools.CheckExtends(maxBox, min, max);
            }
            this._selectionOctree.update(min, max, this.meshes);
            return this._selectionOctree;
        }
        public virtual Ray createPickingRay(float x, float y, Matrix world, Camera camera)
        {
            var engine = this._engine;
            if (!camera)
            {
                if (!this.activeCamera)
                    throw new Exception("Active camera not se");
                camera = this.activeCamera;
            }
            var cameraViewport = camera.viewport;
            var viewport = cameraViewport.toGlobal(engine);
            x = x / this._engine.getHardwareScalingLevel() - viewport.x;
            y = y / this._engine.getHardwareScalingLevel() - (this._engine.getRenderHeight() - viewport.y - viewport.height);
            return BABYLON.Ray.CreateNew(x, y, viewport.width, viewport.height, (world) ? world : BABYLON.Matrix.Identity(), camera.getViewMatrix(), camera.getProjectionMatrix());
        }
        private virtual PickingInfo _internalPick(Func<Matrix, Ray> rayFunction, Func<AbstractMesh, bool> predicate, bool fastCheck)
        {
            var pickingInfo = null;
            for (var meshIndex = 0; meshIndex < this.meshes.length; meshIndex++)
            {
                var mesh = this.meshes[meshIndex];
                if (predicate)
                {
                    if (!predicate(mesh))
                    {
                        continue;
                    }
                }
                else
                    if (!mesh.isEnabled() || !mesh.isVisible || !mesh.isPickable)
                    {
                        continue;
                    }
                var world = mesh.getWorldMatrix();
                var ray = rayFunction(world);
                var result = mesh.intersects(ray, fastCheck);
                if (!result || !result.hit)
                    continue;
                if (!fastCheck && pickingInfo != null && result.distance >= pickingInfo.distance)
                    continue;
                pickingInfo = result;
                if (fastCheck)
                {
                    break;
                }
            }
            return pickingInfo || new BABYLON.PickingInfo();
        }
        public virtual PickingInfo pick(float x, float y, Func<AbstractMesh, bool> predicate, bool fastCheck, Camera camera)
        {
            return this._internalPick((dynamic world) =>
            {
            }
            , predicate, fastCheck);
        }
        public virtual void pickWithRay(Ray ray, Func<Mesh, bool> predicate, bool fastCheck)
        {
            return this._internalPick((dynamic world) =>
            {
                if (!this._pickWithRayInverseMatrix)
                {
                    this._pickWithRayInverseMatrix = BABYLON.Matrix.Identity();
                }
                world.invertToRef(this._pickWithRayInverseMatrix);
                return BABYLON.Ray.Transform(ray, this._pickWithRayInverseMatrix);
            }
            , predicate, fastCheck);
        }
        public virtual void setPointerOverMesh(AbstractMesh mesh)
        {
            if (this._pointerOverMesh == mesh)
            {
                return;
            }
            if (this._pointerOverMesh && this._pointerOverMesh.actionManager)
            {
                this._pointerOverMesh.actionManager.processTrigger(ActionManager.OnPointerOutTrigger, ActionEvent.CreateNew(this._pointerOverMesh));
            }
            this._pointerOverMesh = mesh;
            if (this._pointerOverMesh && this._pointerOverMesh.actionManager)
            {
                this._pointerOverMesh.actionManager.processTrigger(ActionManager.OnPointerOverTrigger, ActionEvent.CreateNew(this._pointerOverMesh));
            }
        }
        public virtual AbstractMesh getPointerOverMesh()
        {
            return this._pointerOverMesh;
        }
        public virtual PhysicsEngine getPhysicsEngine()
        {
            return this._physicsEngine;
        }
        public virtual bool enablePhysics(Vector3 gravity, IPhysicsEnginePlugin plugin)
        {
            if (this._physicsEngine)
            {
                return true;
            }
            this._physicsEngine = new BABYLON.PhysicsEngine(plugin);
            if (!this._physicsEngine.isSupported())
            {
                this._physicsEngine = null;
                return false;
            }
            this._physicsEngine._initialize(gravity);
            return true;
        }
        public virtual void disablePhysicsEngine()
        {
            if (!this._physicsEngine)
            {
                return;
            }
            this._physicsEngine.dispose();
            this._physicsEngine = undefined;
        }
        public virtual bool isPhysicsEnabled()
        {
            return this._physicsEngine != undefined;
        }
        public virtual void setGravity(Vector3 gravity)
        {
            if (!this._physicsEngine)
            {
                return;
            }
            this._physicsEngine._setGravity(gravity);
        }
        public virtual object createCompoundImpostor(object parts, PhysicsBodyCreationOptions options)
        {
            if (parts.parts)
            {
                options = parts;
                parts = parts.parts;
            }
            if (!this._physicsEngine)
            {
                return null;
            }
            for (var index = 0; index < parts.length; index++)
            {
                var mesh = parts[index].mesh;
                mesh._physicImpostor = parts[index].impostor;
                mesh._physicsMass = options.mass / parts.length;
                mesh._physicsFriction = options.friction;
                mesh._physicRestitution = options.restitution;
            }
            return this._physicsEngine._registerMeshesAsCompound(parts, options);
        }
        public virtual void deleteCompoundImpostor(object compound)
        {
            for (var index = 0; index < compound.parts.length; index++)
            {
                var mesh = compound.parts[index].mesh;
                mesh._physicImpostor = BABYLON.PhysicsEngine.NoImpostor;
                this._physicsEngine._unregisterMesh(mesh);
            }
        }
        private virtual object[] _getByTags(object[] list, string tagsQuery)
        {
            if (tagsQuery == undefined)
            {
                return list;
            }
            var listByTags = new Array<object>();
            foreach (var i in list)
            {
                var item = list[i];
                if (BABYLON.Tags.MatchesQuery(item, tagsQuery))
                {
                    listByTags.push(item);
                }
            }
            return listByTags;
        }
        public virtual Mesh[] getMeshesByTags(string tagsQuery)
        {
            return this._getByTags(this.meshes, tagsQuery);
        }
        public virtual Camera[] getCamerasByTags(string tagsQuery)
        {
            return this._getByTags(this.cameras, tagsQuery);
        }
        public virtual Light[] getLightsByTags(string tagsQuery)
        {
            return this._getByTags(this.lights, tagsQuery);
        }
        public virtual Material[] getMaterialByTags(string tagsQuery)
        {
            return this._getByTags(this.materials, tagsQuery).concat(this._getByTags(this.multiMaterials, tagsQuery));
        }
    }
}
