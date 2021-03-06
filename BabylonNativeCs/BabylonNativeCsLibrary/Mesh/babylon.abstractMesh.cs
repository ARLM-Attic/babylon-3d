// --------------------------------------------------------------------------------------------------------------------
// <copyright file="babylon.abstractMesh.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BABYLON
{
    using System;

    /// <summary>
    /// </summary>
    public partial class AbstractMesh : Node, IDisposable
    {
        /// <summary>
        /// </summary>
        public BoundingInfo _boundingInfo;

        /// <summary>
        /// </summary>
        public bool _checkCollisions = false;

        /// <summary>
        /// </summary>
        public Array<AbstractMesh> _intersectionsInProgress = new Array<AbstractMesh>();

        /// <summary>
        /// </summary>
        public bool _isDisposed = false;

        /// <summary>
        /// </summary>
        public Material _material;

        /// <summary>
        /// </summary>
        public int _physicImpostor = PhysicsEngine.NoImpostor;

        /// <summary>
        /// </summary>
        public double _physicRestitution;

        /// <summary>
        /// </summary>
        public double _physicsFriction;

        /// <summary>
        /// </summary>
        public double _physicsMass;

        /// <summary>
        /// </summary>
        public int _renderId = 0;

        /// <summary>
        /// </summary>
        public Skeleton _skeleton;

        /// <summary>
        /// </summary>
        public Octree<SubMesh> _submeshesOctree;

        /// <summary>
        /// </summary>
        public double _visibility = 1.0;

        /// <summary>
        /// </summary>
        public ActionManager actionManager;

        /// <summary>
        /// </summary>
        public int billboardMode = BILLBOARDMODE_NONE;

        /// <summary>
        /// </summary>
        public Vector3 ellipsoid = new Vector3(0.5, 1, 0.5);

        /// <summary>
        /// </summary>
        public Vector3 ellipsoidOffset = new Vector3(0, 0, 0);

        /// <summary>
        /// </summary>
        public bool infiniteDistance = false;

        /// <summary>
        /// </summary>
        public bool isVisible = true;

        /// <summary>
        /// </summary>
        public uint layerMask = 0xFFFFFFFF;

        /// <summary>
        /// </summary>
        public System.Action onDispose = null;

        /// <summary>
        /// </summary>
        public Vector3 position = new Vector3(0, 0, 0);

        /// <summary>
        /// </summary>
        public int renderingGroupId = 0;

        /// <summary>
        /// </summary>
        public Vector3 rotation = new Vector3(0, 0, 0);

        /// <summary>
        /// </summary>
        public Quaternion rotationQuaternion;

        /// <summary>
        /// </summary>
        public Vector3 scaling = new Vector3(1, 1, 1);

        /// <summary>
        /// </summary>
        public bool showBoundingBox = false;

        /// <summary>
        /// </summary>
        public bool showSubMeshesBoundingBox = false;

        /// <summary>
        /// </summary>
        public Array<SubMesh> subMeshes;

        /// <summary>
        /// </summary>
        public bool useOctreeForCollisions = true;

        /// <summary>
        /// </summary>
        public bool useOctreeForPicking = true;

        /// <summary>
        /// </summary>
        public bool useOctreeForRenderingSelection = true;

        /// <summary>
        /// </summary>
        private readonly Vector3 _absolutePosition = Vector3.Zero();

        /// <summary>
        /// </summary>
        private readonly Collider _collider = new Collider();

        /// <summary>
        /// </summary>
        private readonly Matrix _collisionsScalingMatrix = Matrix.Zero();

        /// <summary>
        /// </summary>
        private readonly Matrix _collisionsTransformMatrix = Matrix.Zero();

        /// <summary>
        /// </summary>
        private readonly Vector3 _diffPositionForCollisions = new Vector3(0, 0, 0);

        /// <summary>
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// </summary>
        private bool _isPickable = true;

        /// <summary>
        /// </summary>
        private readonly Matrix _localBillboard = Matrix.Zero();

        /// <summary>
        /// </summary>
        private readonly Matrix _localPivotScaling = Matrix.Zero();

        /// <summary>
        /// </summary>
        private readonly Matrix _localPivotScalingRotation = Matrix.Zero();

        /// <summary>
        /// </summary>
        private readonly Matrix _localRotation = Matrix.Zero();

        /// <summary>
        /// </summary>
        private readonly Matrix _localScaling = Matrix.Zero();

        /// <summary>
        /// </summary>
        private readonly Matrix _localTranslation = Matrix.Zero();

        /// <summary>
        /// </summary>
        private readonly Matrix _localWorld = Matrix.Zero();

        /// <summary>
        /// </summary>
        private readonly Vector3 _newPositionForCollisions = new Vector3(0, 0, 0);

        /// <summary>
        /// </summary>
        private readonly Vector3 _oldPositionForCollisions = new Vector3(0, 0, 0);

        /// <summary>
        /// </summary>
        private Matrix _pivotMatrix = Matrix.Identity();

        /// <summary>
        /// </summary>
        private bool _receiveShadows = false;

        /// <summary>
        /// </summary>
        private readonly Matrix _rotateYByPI = Matrix.RotationY(Math.PI);

        /// <summary>
        /// </summary>
        private readonly Matrix _worldMatrix = Matrix.Zero();

        /// <summary>
        /// field to save temporary data
        /// </summary>
        public Material _savedMaterial;

        /// <summary>
        /// </summary>
        /// <param name="name">
        /// </param>
        /// <param name="scene">
        /// </param>
        public AbstractMesh(string name, Scene scene)
            : base(name, scene)
        {
            scene.meshes.Add(this);
        }

        /// <summary>
        /// </summary>
        public virtual Array<Vector3> _positions { get; set; }

        /// <summary>
        /// </summary>
        public virtual Vector3 absolutePosition
        {
            get
            {
                return this._absolutePosition;
            }
        }

        /// <summary>
        /// </summary>
        public virtual bool checkCollisions
        {
            get
            {
                return this._checkCollisions;
            }

            set
            {
                this._checkCollisions = value;
            }
        }

        /// <summary>
        /// </summary>
        public virtual bool isPickable
        {
            get
            {
                return this._isPickable;
            }

            set
            {
                this._isPickable = value;
            }
        }

        /// <summary>
        /// </summary>
        public virtual Material material
        {
            get
            {
                return this._material;
            }

            set
            {
                this._material = value;
            }
        }

        /// <summary>
        /// </summary>
        public virtual bool receiveShadows
        {
            get
            {
                return this._receiveShadows;
            }

            set
            {
                this._receiveShadows = value;
            }
        }

        /// <summary>
        /// </summary>
        public virtual Skeleton skeleton
        {
            get
            {
                return this._skeleton;
            }

            set
            {
                this._skeleton = value;
            }
        }

        /// <summary>
        /// </summary>
        public virtual double visibility
        {
            get
            {
                return this._visibility;
            }

            set
            {
                this._visibility = value;
            }
        }

        /// <summary>
        /// </summary>
        public virtual Matrix worldMatrixFromCache
        {
            get
            {
                return this._worldMatrix;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="renderId">
        /// </param>
        public virtual void _activate(int renderId)
        {
            this._renderId = renderId;
        }

        /// <summary>
        /// </summary>
        /// <param name="collider">
        /// </param>
        public virtual void _checkCollision(Collider collider)
        {
            if (!this._boundingInfo._checkCollision(collider))
            {
                return;
            }

            Matrix.ScalingToRef(1.0 / collider.radius.x, 1.0 / collider.radius.y, 1.0 / collider.radius.z, this._collisionsScalingMatrix);
            this.worldMatrixFromCache.multiplyToRef(this._collisionsScalingMatrix, this._collisionsTransformMatrix);
            this._processCollisionsForSubMeshes(collider, this._collisionsTransformMatrix);
        }

        /// <summary>
        /// </summary>
        /// <param name="subMesh">
        /// </param>
        /// <param name="transformMatrix">
        /// </param>
        /// <param name="collider">
        /// </param>
        public virtual void _collideForSubMesh(SubMesh subMesh, Matrix transformMatrix, Collider collider)
        {
            this._generatePointsArray();
            if (subMesh._lastColliderWorldVertices == null || !subMesh._lastColliderTransformMatrix.equals(transformMatrix))
            {
                subMesh._lastColliderTransformMatrix = transformMatrix.clone();
                subMesh._lastColliderWorldVertices = new Array<Vector3>();
                subMesh._trianglePlanes = new Array<Plane>();
                var start = subMesh.verticesStart;
                var end = subMesh.verticesStart + subMesh.verticesCount;
                for (var i = start; i < end; i++)
                {
                    subMesh._lastColliderWorldVertices.Add(Vector3.TransformCoordinates(this._positions[i], transformMatrix));
                }
            }

            collider._collide(
                subMesh,
                subMesh._lastColliderWorldVertices,
                this.getIndices(),
                subMesh.indexStart,
                subMesh.indexStart + subMesh.indexCount,
                subMesh.verticesStart);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual bool _generatePointsArray()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        public override void _initCache()
        {
            base._initCache();
            this._cache.localMatrixUpdated = false;
            this._cache.position = Vector3.Zero();
            this._cache.scaling = Vector3.Zero();
            this._cache.rotation = Vector3.Zero();
            this._cache.rotationQuaternion = new Quaternion(0, 0, 0, 0);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public override bool _isSynchronized()
        {
            if (this._isDirty)
            {
                return false;
            }

            if (this.billboardMode != BILLBOARDMODE_NONE)
            {
                return false;
            }

            if (this._cache.pivotMatrixUpdated)
            {
                return false;
            }

            if (this.infiniteDistance)
            {
                return false;
            }

            if (!this._cache.position.equals(this.position))
            {
                return false;
            }

            if (this.rotationQuaternion != null)
            {
                if (!this._cache.rotationQuaternion.equals(this.rotationQuaternion))
                {
                    return false;
                }
            }
            else
            {
                if (!this._cache.rotation.equals(this.rotation))
                {
                    return false;
                }
            }

            if (!this._cache.scaling.equals(this.scaling))
            {
#if _DEBUG
                Tools.Log(string.Format("scaling not equal: new {0}, {1}, {2}", this.scaling.x, this.scaling.y, this.scaling.z));
#endif
                return false;
            }

            return true;
        }

        /// <summary>
        /// </summary>
        public virtual void _preActivate()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="collider">
        /// </param>
        /// <param name="transformMatrix">
        /// </param>
        public virtual void _processCollisionsForSubMeshes(Collider collider, Matrix transformMatrix)
        {
            Array<SubMesh> subMeshes;
            int len;
            if (this._submeshesOctree != null && this.useOctreeForCollisions)
            {
                var radius = collider.velocityWorldLength + Math.Max(Math.Max(collider.radius.x, collider.radius.y), collider.radius.z);
                var intersections = this._submeshesOctree.intersects(collider.basePointWorld, radius);
                len = intersections.Length;
                subMeshes = intersections;
            }
            else
            {
                subMeshes = this.subMeshes;
                len = subMeshes.Length;
            }

            for (var index = 0; index < len; index++)
            {
                var subMesh = subMeshes[index];
                if (len > 1 && !subMesh._checkCollision(collider))
                {
                    continue;
                }

                this._collideForSubMesh(subMesh, transformMatrix, collider);
            }
        }

        /// <summary>
        /// </summary>
        public virtual void _updateBoundingInfo()
        {
            this._boundingInfo = this._boundingInfo ?? new BoundingInfo(this.absolutePosition, this.absolutePosition);
            this._boundingInfo._update(this.worldMatrixFromCache);
            if (this.subMeshes == null)
            {
                return;
            }

            for (var subIndex = 0; subIndex < this.subMeshes.Length; subIndex++)
            {
                var subMesh = this.subMeshes[subIndex];
                subMesh.updateBoundingInfo(this.worldMatrixFromCache);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="force">
        /// </param>
        /// <param name="contactPoint">
        /// </param>
        public virtual void applyImpulse(Vector3 force, Vector3 contactPoint)
        {
            if (this._physicImpostor == PhysicsEngine.NoImpostor)
            {
                return;
            }

            this.getScene().getPhysicsEngine()._applyImpulse(this, force, contactPoint);
        }

        /// <summary>
        /// </summary>
        /// <param name="name">
        /// </param>
        /// <param name="newParent">
        /// </param>
        /// <param name="doNotCloneChildren">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual AbstractMesh clone(string name = null, Node newParent = null, bool doNotCloneChildren = false)
        {
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="force">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual Matrix computeWorldMatrix(bool force = false)
        {
            if (!force && (this._currentRenderId == this.getScene().getRenderId() || this.isSynchronized(true)))
            {
                return this._worldMatrix;
            }

            this._cache.position.copyFrom(this.position);
            this._cache.scaling.copyFrom(this.scaling);
            this._cache.pivotMatrixUpdated = false;
            this._currentRenderId = this.getScene().getRenderId();
            this._isDirty = false;
            Matrix.ScalingToRef(this.scaling.x, this.scaling.y, this.scaling.z, this._localScaling);
            if (this.rotationQuaternion != null)
            {
                this.rotationQuaternion.toRotationMatrix(this._localRotation);
                this._cache.rotationQuaternion.copyFrom(this.rotationQuaternion);
            }
            else
            {
                Matrix.RotationYawPitchRollToRef(this.rotation.y, this.rotation.x, this.rotation.z, this._localRotation);
                this._cache.rotation.copyFrom(this.rotation);
            }

            if (this.infiniteDistance && this.parent == null)
            {
                var camera = this.getScene().activeCamera;
                var cameraWorldMatrix = camera.getWorldMatrix();
                var cameraGlobalPosition = new Vector3(cameraWorldMatrix.m[12], cameraWorldMatrix.m[13], cameraWorldMatrix.m[14]);
                Matrix.TranslationToRef(
                    this.position.x + cameraGlobalPosition.x,
                    this.position.y + cameraGlobalPosition.y,
                    this.position.z + cameraGlobalPosition.z,
                    this._localTranslation);
            }
            else
            {
                Matrix.TranslationToRef(this.position.x, this.position.y, this.position.z, this._localTranslation);
            }

            this._pivotMatrix.multiplyToRef(this._localScaling, this._localPivotScaling);
            this._localPivotScaling.multiplyToRef(this._localRotation, this._localPivotScalingRotation);
            if (this.billboardMode != BILLBOARDMODE_NONE)
            {
                var localPosition = this.position.clone();
                var zero = this.getScene().activeCamera.position.clone();
                if (this.parent != null && ((AbstractMesh)this.parent).position != null)
                {
                    localPosition.addInPlace(((AbstractMesh)this.parent).position);
                    Matrix.TranslationToRef(localPosition.x, localPosition.y, localPosition.z, this._localTranslation);
                }

                if ((this.billboardMode & BILLBOARDMODE_ALL) == BILLBOARDMODE_ALL)
                {
                    zero = this.getScene().activeCamera.position;
                }
                else
                {
                    if ((this.billboardMode & BILLBOARDMODE_X) > 0)
                    {
                        zero.x = localPosition.x + Engine.Epsilon;
                    }

                    if ((this.billboardMode & BILLBOARDMODE_Y) > 0)
                    {
                        zero.y = localPosition.y + 0.001;
                    }

                    if ((this.billboardMode & BILLBOARDMODE_Z) > 0)
                    {
                        zero.z = localPosition.z + 0.001;
                    }
                }

                Matrix.LookAtLHToRef(localPosition, zero, Vector3.Up(), this._localBillboard);
                this._localBillboard.m[12] = this._localBillboard.m[13] = this._localBillboard.m[14] = 0;
                this._localBillboard.invert();
                this._localPivotScalingRotation.multiplyToRef(this._localBillboard, this._localWorld);
                this._rotateYByPI.multiplyToRef(this._localWorld, this._localPivotScalingRotation);
            }

            this._localPivotScalingRotation.multiplyToRef(this._localTranslation, this._localWorld);
            if (this.parent != null && this.billboardMode == BILLBOARDMODE_NONE)
            {
                this._localWorld.multiplyToRef(this.parent.getWorldMatrix(), this._worldMatrix);
            }
            else
            {
                this._worldMatrix.copyFrom(this._localWorld);
            }

            this._updateBoundingInfo();
            this._absolutePosition.copyFromFloats(this._worldMatrix.m[12], this._worldMatrix.m[13], this._worldMatrix.m[14]);
            return this._worldMatrix;
        }

        /// <summary>
        /// </summary>
        /// <param name="maxCapacity">
        /// </param>
        /// <param name="maxDepth">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual Octree<SubMesh> createOrUpdateSubmeshesOctree(int maxCapacity = 64, int maxDepth = 2)
        {
            if (this._submeshesOctree == null)
            {
                this._submeshesOctree = new Octree<SubMesh>(Octree<SubMesh>.CreationFuncForSubMeshes, maxCapacity, maxDepth);
            }

            this.computeWorldMatrix(true);
            var bbox = this.getBoundingInfo().boundingBox;
            this._submeshesOctree.update(bbox.minimumWorld, bbox.maximumWorld, this.subMeshes);
            return this._submeshesOctree;
        }

        /// <summary>
        /// </summary>
        /// <param name="doNotRecurse">
        /// </param>
        public virtual void dispose(bool doNotRecurse = false)
        {
            if (this.getPhysicsImpostor() != PhysicsEngine.NoImpostor)
            {
                this.setPhysicsState(PhysicsEngine.NoImpostor);
            }

            int index;
            for (index = 0; index < this._intersectionsInProgress.Length; index++)
            {
                var other = this._intersectionsInProgress[index];
                var pos = other._intersectionsInProgress.IndexOf(this);
                other._intersectionsInProgress.RemoveAt(pos);
            }

            this._intersectionsInProgress = new Array<AbstractMesh>();
            this.releaseSubMeshes();
            index = this.getScene().meshes.IndexOf(this);
            this.getScene().meshes.RemoveAt(index);
            if (!doNotRecurse)
            {
                for (index = 0; index < this.getScene().particleSystems.Length; index++)
                {
                    if (this.getScene().particleSystems[index].emitter == this)
                    {
                        this.getScene().particleSystems[index].dispose();
                        index--;
                    }
                }

                var objects = this.getScene().meshes.slice(0);
                for (index = 0; index < objects.Length; index++)
                {
                    if (objects[index].parent == this)
                    {
                        objects[index].dispose();
                    }
                }
            }
            else
            {
                for (index = 0; index < this.getScene().meshes.Length; index++)
                {
                    var obj = this.getScene().meshes[index];
                    if (obj.parent == this)
                    {
                        obj.parent = null;
                        obj.computeWorldMatrix(true);
                    }
                }
            }

            this._isDisposed = true;
            if (this.onDispose != null)
            {
                this.onDispose();
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual Vector3 getAbsolutePosition()
        {
            this.computeWorldMatrix();
            return this._absolutePosition;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual BoundingInfo getBoundingInfo()
        {
            if (this._boundingInfo == null)
            {
                this._updateBoundingInfo();
            }

            return this._boundingInfo;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual Array<int> getIndices()
        {
            return null;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual double getPhysicsFriction()
        {
            return this._physicsFriction;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual int getPhysicsImpostor()
        {
            return this._physicImpostor;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual double getPhysicsMass()
        {
            return this._physicsMass;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual double getPhysicsRestitution()
        {
            return this._physicRestitution;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual Matrix getPivotMatrix()
        {
            return this._pivotMatrix;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual Vector3 getPositionExpressedInLocalSpace()
        {
            this.computeWorldMatrix();
            var invLocalWorldMatrix = this._localWorld.clone();
            invLocalWorldMatrix.invert();
            return Vector3.TransformNormal(this.position, invLocalWorldMatrix);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual int getTotalVertices()
        {
            return 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="kind">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual Array<double> getVerticesData(VertexBufferKind kind)
        {
            return null;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public override Matrix getWorldMatrix()
        {
            if (this._currentRenderId != this.getScene().getRenderId())
            {
                this.computeWorldMatrix();
            }

            return this._worldMatrix;
        }

        /// <summary>
        /// </summary>
        /// <param name="ray">
        /// </param>
        /// <param name="fastCheck">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual PickingInfo intersects(Ray ray, bool fastCheck = false)
        {
            var pickingInfo = new PickingInfo();
            if (this.subMeshes == null || this._boundingInfo == null || !ray.intersectsSphere(this._boundingInfo.boundingSphere)
                || !ray.intersectsBox(this._boundingInfo.boundingBox))
            {
                return pickingInfo;
            }

            if (!this._generatePointsArray())
            {
                return pickingInfo;
            }

            IntersectionInfo intersectInfo = null;
            Array<SubMesh> subMeshes;
            int len;
            if (this._submeshesOctree != null && this.useOctreeForPicking)
            {
                var worldRay = Ray.Transform(ray, this.getWorldMatrix());
                var intersections = this._submeshesOctree.intersectsRay(worldRay);
                len = intersections.Length;
                subMeshes = intersections;
            }
            else
            {
                subMeshes = this.subMeshes;
                len = subMeshes.Length;
            }

            for (var index = 0; index < len; index++)
            {
                var subMesh = subMeshes[index];
                if (len > 1 && !subMesh.canIntersects(ray))
                {
                    continue;
                }

                var currentIntersectInfo = subMesh.intersects(ray, this._positions, this.getIndices(), fastCheck);
                if (currentIntersectInfo != null)
                {
                    if (fastCheck || intersectInfo == null || currentIntersectInfo.distance < intersectInfo.distance)
                    {
                        intersectInfo = currentIntersectInfo;
                        if (fastCheck)
                        {
                            break;
                        }
                    }
                }
            }

            if (intersectInfo != null)
            {
                var world = this.getWorldMatrix();
                var worldOrigin = Vector3.TransformCoordinates(ray.origin, world);
                var direction = ray.direction.clone();
                direction.normalize();
                direction = direction.scale(intersectInfo.distance);
                var worldDirection = Vector3.TransformNormal(direction, world);
                var pickedPoint = worldOrigin.add(worldDirection);
                pickingInfo.hit = true;
                pickingInfo.distance = Vector3.Distance(worldOrigin, pickedPoint);
                pickingInfo.pickedPoint = pickedPoint;
                pickingInfo.pickedMesh = this;
                pickingInfo.bu = intersectInfo.bu;
                pickingInfo.bv = intersectInfo.bv;
                pickingInfo.faceId = intersectInfo.faceId;
                return pickingInfo;
            }

            return pickingInfo;
        }

        /// <summary>
        /// </summary>
        /// <param name="mesh">
        /// </param>
        /// <param name="precise">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual bool intersectsMesh(AbstractMesh mesh, bool precise = false)
        {
            if (this._boundingInfo == null || mesh._boundingInfo == null)
            {
                return false;
            }

            return this._boundingInfo.intersects(mesh._boundingInfo, precise);
        }

        /// <summary>
        /// </summary>
        /// <param name="point">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual bool intersectsPoint(Vector3 point)
        {
            if (this._boundingInfo == null)
            {
                return false;
            }

            return this._boundingInfo.intersectsPoint(point);
        }

        /// <summary>
        /// </summary>
        /// <param name="frustumPlanes">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual bool isInFrustum(Array<Plane> frustumPlanes)
        {
            if (!this._boundingInfo.isInFrustum(frustumPlanes))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="kind">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual bool isVerticesDataPresent(VertexBufferKind kind)
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="vector3">
        /// </param>
        public virtual void locallyTranslate(Vector3 vector3)
        {
            this.computeWorldMatrix();
            this.position = Vector3.TransformCoordinates(vector3, this._localWorld);
        }

        /// <summary>
        /// </summary>
        /// <param name="targetPoint">
        /// </param>
        /// <param name="yawCor">
        /// </param>
        /// <param name="pitchCor">
        /// </param>
        /// <param name="rollCor">
        /// </param>
        public virtual void lookAt(Vector3 targetPoint, double yawCor = 0.0, double pitchCor = 0.0, double rollCor = 0.0)
        {
            var dv = targetPoint.subtract(this.position);
            var yaw = -Math.Atan2(dv.z, dv.x) - Math.PI / 2;
            var len = Math.Sqrt(dv.x * dv.x + dv.z * dv.z);
            var pitch = Math.Atan2(dv.y, len);
            this.rotationQuaternion = Quaternion.RotationYawPitchRoll(yaw + yawCor, pitch + pitchCor, rollCor);
        }

        /// <summary>
        /// </summary>
        /// <param name="property">
        /// </param>
        public virtual void markAsDirty(string property)
        {
            if (property == "rotation")
            {
                this.rotationQuaternion = null;
            }

            this._currentRenderId = int.MaxValue;
            this._isDirty = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="velocity">
        /// </param>
        public virtual void moveWithCollisions(Vector3 velocity)
        {
            var globalPosition = this.getAbsolutePosition();
            globalPosition.subtractFromFloatsToRef(0, this.ellipsoid.y, 0, this._oldPositionForCollisions);
            this._oldPositionForCollisions.addInPlace(this.ellipsoidOffset);
            this._collider.radius = this.ellipsoid;
            this.getScene()._getNewPosition(this._oldPositionForCollisions, velocity, this._collider, 3, this._newPositionForCollisions, this);
            this._newPositionForCollisions.subtractToRef(this._oldPositionForCollisions, this._diffPositionForCollisions);
            if (this._diffPositionForCollisions.Length() > Engine.CollisionsEpsilon)
            {
                this.position.addInPlace(this._diffPositionForCollisions);
            }
        }

        /// <summary>
        /// </summary>
        public virtual void releaseSubMeshes()
        {
            if (this.subMeshes != null)
            {
                while (this.subMeshes.Length > 0)
                {
                    this.subMeshes[0].dispose();
                }
            }
            else
            {
                this.subMeshes = new Array<SubMesh>();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="axis">
        /// </param>
        /// <param name="amount">
        /// </param>
        /// <param name="space">
        /// </param>
        public virtual void rotate(Vector3 axis, double amount, Space space)
        {
            if (this.rotationQuaternion == null)
            {
                this.rotationQuaternion = Quaternion.RotationYawPitchRoll(this.rotation.y, this.rotation.x, this.rotation.z);
                this.rotation = Vector3.Zero();
            }

            if (space == Space.LOCAL)
            {
                var rotationQuaternion = Quaternion.RotationAxis(axis, amount);
                this.rotationQuaternion = this.rotationQuaternion.multiply(rotationQuaternion);
            }
            else
            {
                if (this.parent != null)
                {
                    var invertParentWorldMatrix = this.parent.getWorldMatrix().clone();
                    invertParentWorldMatrix.invert();
                    axis = Vector3.TransformNormal(axis, invertParentWorldMatrix);
                }

                this.rotationQuaternion = Quaternion.RotationAxis(axis, amount);
                this.rotationQuaternion = this.rotationQuaternion.multiply(this.rotationQuaternion);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="absolutePosition">
        /// </param>
        public virtual void setAbsolutePosition(Vector3 absolutePosition)
        {
            if (absolutePosition == null)
            {
                return;
            }

            var absolutePositionX = absolutePosition.x;
            var absolutePositionY = absolutePosition.y;
            var absolutePositionZ = absolutePosition.z;
            if (this.parent != null)
            {
                var invertParentWorldMatrix = this.parent.getWorldMatrix().clone();
                invertParentWorldMatrix.invert();
                var worldPosition = new Vector3(absolutePositionX, absolutePositionY, absolutePositionZ);
                this.position = Vector3.TransformCoordinates(worldPosition, invertParentWorldMatrix);
            }
            else
            {
                this.position.x = absolutePositionX;
                this.position.y = absolutePositionY;
                this.position.z = absolutePositionZ;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="otherMesh">
        /// </param>
        /// <param name="pivot1">
        /// </param>
        /// <param name="pivot2">
        /// </param>
        public virtual void setPhysicsLinkWith(Mesh otherMesh, Vector3 pivot1, Vector3 pivot2)
        {
            if (this._physicImpostor == PhysicsEngine.NoImpostor)
            {
                return;
            }

            this.getScene().getPhysicsEngine()._createLink(this, otherMesh, pivot1, pivot2);
        }

        /// <summary>
        /// </summary>
        /// <param name="impostor">
        /// </param>
        /// <param name="options">
        /// </param>
        public virtual void setPhysicsState(int impostor = PhysicsEngine.NoImpostor, PhysicsBodyCreationOptions options = null)
        {
            var physicsEngine = this.getScene().getPhysicsEngine();
            if (physicsEngine == null)
            {
                return;
            }

            if (impostor == PhysicsEngine.NoImpostor)
            {
                physicsEngine._unregisterMesh(this);
                return;
            }

            options.friction = options.friction < Engine.Epsilon ? 0.2 : options.friction;
            options.restitution = options.restitution < Engine.Epsilon ? 0.9 : options.restitution;
            this._physicImpostor = impostor;
            this._physicsMass = options.mass;
            this._physicsFriction = options.friction;
            this._physicRestitution = options.restitution;
            physicsEngine._registerMesh(this, impostor, options);
        }

        /// <summary>
        /// </summary>
        /// <param name="matrix">
        /// </param>
        public virtual void setPivotMatrix(Matrix matrix)
        {
            this._pivotMatrix = matrix;
            this._cache.pivotMatrixUpdated = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="vector3">
        /// </param>
        public virtual void setPositionWithLocalVector(Vector3 vector3)
        {
            this.computeWorldMatrix();
            this.position = Vector3.TransformNormal(vector3, this._localWorld);
        }

        /// <summary>
        /// </summary>
        /// <param name="axis">
        /// </param>
        /// <param name="distance">
        /// </param>
        /// <param name="space">
        /// </param>
        public virtual void translate(Vector3 axis, double distance, Space space)
        {
            var displacementVector = axis.scale(distance);
            if (space == Space.LOCAL)
            {
                var tempV3 = this.getPositionExpressedInLocalSpace().add(displacementVector);
                this.setPositionWithLocalVector(tempV3);
            }
            else
            {
                this.setAbsolutePosition(this.getAbsolutePosition().add(displacementVector));
            }
        }

        /// <summary>
        /// </summary>
        public const int BILLBOARDMODE_NONE = 0;

        /// <summary>
        /// </summary>
        public const int BILLBOARDMODE_X = 1;

        /// <summary>
        /// </summary>
        public const int BILLBOARDMODE_Y = 2;

        /// <summary>
        /// </summary>
        public const int BILLBOARDMODE_Z = 4;

        /// <summary>
        /// </summary>
        public const int BILLBOARDMODE_ALL = 7;
    }
}