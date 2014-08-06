using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Web;
namespace BABYLON
{
    public partial class Skeleton
    {
        public Array<Bone> bones = new Array<Bone>();
        private Scene _scene;
        private bool _isDirty = true;
        private double[] _transformMatrices;
        private Array<IAnimatable> _animatables;
        private BABYLON.Matrix _identity = Matrix.Identity();
        public string name;
        public string id;
        public Skeleton(string name, string id, Scene scene)
        {
            this.name = name;
            this.id = id;
            this.bones = new Array<Bone>();
            this._scene = scene;
            scene.skeletons.Add(this);
        }
        public virtual double[] getTransformMatrices()
        {
            return this._transformMatrices;
        }
        public virtual void _markAsDirty()
        {
            this._isDirty = true;
        }
        public virtual void prepare()
        {
            if (!this._isDirty)
            {
                return;
            }
            if (this._transformMatrices == null || this._transformMatrices.Length != 16 * (this.bones.Length + 1))
            {
                this._transformMatrices = new double[16 * (this.bones.Length + 1)];
            }
            for (var index = 0; index < this.bones.Length; index++)
            {
                var bone = this.bones[index];
                var parentBone = bone.getParent();
                if (parentBone != null)
                {
                    bone.getLocalMatrix().multiplyToRef(parentBone.getWorldMatrix(), bone.getWorldMatrix());
                }
                else
                {
                    bone.getWorldMatrix().copyFrom(bone.getLocalMatrix());
                }
                bone.getInvertedAbsoluteTransform().multiplyToArray(bone.getWorldMatrix(), this._transformMatrices, index * 16);
            }
            this._identity.copyToArray(this._transformMatrices, this.bones.Length * 16);
            this._isDirty = false;
        }
        public virtual Array<IAnimatable> getAnimatables()
        {
            if (this._animatables == null || this._animatables.Length != this.bones.Length)
            {
                this._animatables = new Array<IAnimatable>();
                for (var index = 0; index < this.bones.Length; index++)
                {
                    this._animatables.Add(this.bones[index]);
                }
            }
            return this._animatables;
        }
        public virtual Skeleton clone(string name, string id)
        {
            var result = new BABYLON.Skeleton(name, id ?? name, this._scene);
            for (var index = 0; index < this.bones.Length; index++)
            {
                var source = this.bones[index];
                Bone parentBone = null;
                if (source.getParent() != null)
                {
                    var parentIndex = this.bones.IndexOf(source.getParent());
                    parentBone = result.bones[parentIndex];
                }
                var bone = new BABYLON.Bone(source.name, result, parentBone, source.getBaseMatrix());
                BABYLON.Tools.DeepCopy(source.animations, bone.animations);
            }
            return result;
        }
    }
}