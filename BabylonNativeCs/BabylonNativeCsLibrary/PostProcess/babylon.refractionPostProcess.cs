using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Web;
namespace BABYLON {
    public partial class RefractionPostProcess: PostProcess {
        private Texture _refRexture;
        public Color3 color;
        public double depth;
        public double colorLevel;
        public RefractionPostProcess(string name, string refractionTextureUrl, Color3 color, double depth, double colorLevel, double ratio, Camera camera, double samplingMode = 0.0, Engine engine = null, bool reusable = false): base(name, "refraction", new Array < object > ("baseColor", "depth", "colorLevel"), new Array < object > ("refractionSampler"), ratio, camera, samplingMode, engine, reusable) {
            this.onActivate = (Camera cam) => {
                this._refRexture = this._refRexture || new BABYLON.Texture(refractionTextureUrl, cam.getScene());
            };
            this.onApply = (Effect effect) => {
                effect.setColor3("baseColor", this.color);
                effect.setFloat("depth", this.depth);
                effect.setFloat("colorLevel", this.colorLevel);
                effect.setTexture("refractionSampler", this._refRexture);
            };
        }
        public virtual void dispose(Camera camera) {
            if (this._refRexture) {
                this._refRexture.dispose();
            }
            base.dispose(camera);
        }
    }
}