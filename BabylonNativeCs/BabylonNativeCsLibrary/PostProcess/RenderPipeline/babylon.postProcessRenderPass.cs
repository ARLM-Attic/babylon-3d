using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Web;
namespace BABYLON
{
    public partial class PostProcessRenderPass
    {
        private Array<AbstractMesh> _renderList = new Array<AbstractMesh>();
        private RenderTargetTexture _renderTexture;
        private Scene _scene;
        private double _refCount = 0;
        public string _name;
        public PostProcessRenderPass(Scene scene, string name, Size size, Array<AbstractMesh> renderList, System.Action beforeRender, System.Action afterRender)
        {
            this._name = name;
            this._renderTexture = new RenderTargetTexture(name, size, scene);
            this.setRenderList(renderList);
            this._renderTexture.onBeforeRender = beforeRender;
            this._renderTexture.onAfterRender = afterRender;
            this._scene = scene;
        }
        public virtual double _incRefCount()
        {
            if (this._refCount == 0)
            {
                this._scene.customRenderTargets.Add(this._renderTexture);
            }
            return ++this._refCount;
        }
        public virtual double _decRefCount()
        {
            this._refCount--;
            if (this._refCount <= 0)
            {
                this._scene.customRenderTargets.RemoveAt(this._scene.customRenderTargets.IndexOf(this._renderTexture));
            }
            return this._refCount;
        }
        public virtual void _update()
        {
            this.setRenderList(this._renderList);
        }
        public virtual void setRenderList(Array<AbstractMesh> renderList)
        {
            this._renderTexture.renderList = renderList;
        }
        public virtual RenderTargetTexture getRenderTexture()
        {
            return this._renderTexture;
        }
    }
}