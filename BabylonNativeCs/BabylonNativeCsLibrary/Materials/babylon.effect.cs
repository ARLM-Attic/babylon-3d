using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Web;
namespace BABYLON
{
    public partial class Effect
    {
        public EffectBaseName name;
        public string defines;
        public System.Action<Effect> onCompiled;
        public System.Action<Effect, string> onError;
        private Engine _engine;
        private Array<string> _uniformsNames;
        private Array<string> _samplers;
        private bool _isReady = false;
        private string _compilationError = "";
        private Array<VertexBufferKind> _attributes;
        private Array<string> _attributesNames;
        private Array<int> _attributes;
        private Array<WebGLUniformLocation> _uniforms;
        public string _key;
        public WebGLProgram _program;
        private Array<object> _valueCache = new Array<object>();

        private Web.Document document;

        public Effect(EffectBaseName baseName, Array<string> attributesNames, Array<string> uniformsNames, Array<string> samplers, Engine engine, string defines = null, Array<string> optionalDefines = null, System.Action<Effect> onCompiled = null, System.Action<Effect, string> onError = null)
        {
            this._engine = engine;
            this.name = baseName;
            this.defines = defines;
            this._uniformsNames = uniformsNames.concat(samplers);
            this._samplers = samplers;
            this._attributesNames = attributesNames;
            this.onError = onError;
            this.onCompiled = onCompiled;
            string vertexSource;
            string fragmentSource;
            if (!string.IsNullOrEmpty(baseName.vertexElement))
            {
                vertexSource = document.getElementById(baseName.vertexElement).innerText;
                fragmentSource = document.getElementById(baseName.fragmentElement).innerText;
            }
            else
            {
                vertexSource = baseName.vertexElement ?? baseName.vertex ?? baseName.baseName;
                fragmentSource = baseName.fragmentElement ?? baseName.fragment ?? baseName.baseName;
            }
            this._loadVertexShader(vertexSource, (object vertexCode) =>
            {
                this._loadFragmentShader(fragmentSource, (object fragmentCode) =>
                {
                    this._prepareEffect(vertexCode, fragmentCode, attributesNames, defines, optionalDefines);
                });
            });
        }
        public virtual bool isReady()
        {
            return this._isReady;
        }
        public virtual WebGLProgram getProgram()
        {
            return this._program;
        }
        public virtual Array<string> getAttributesNames()
        {
            return this._attributesNames;
        }
        public virtual Array<VertexBufferKind> getAttributes()
        {
            return this._attributes;
        }
        public virtual int getAttributeLocation(double index)
        {
            return this._attributes[index];
        }
        public virtual double getAttributeLocationByName(string name)
        {
            var index = this._attributesNames.indexOf(name);
            return this._attributes[index];
        }
        public virtual double getAttributesCount()
        {
            return this._attributes.Length;
        }
        public virtual double getUniformIndex(string uniformName)
        {
            return this._uniformsNames.indexOf(uniformName);
        }
        public virtual WebGLUniformLocation getUniform(string uniformName)
        {
            return this._uniforms[this._uniformsNames.indexOf(uniformName)];
        }
        public virtual Array<string> getSamplers()
        {
            return this._samplers;
        }
        public virtual string getCompilationError()
        {
            return this._compilationError;
        }
        public virtual void _loadVertexShader(object vertex, System.Action<object> callback)
        {
            if (vertex is HTMLElement)
            {
                var vertexCode = BABYLON.Tools.GetDOMTextContent(vertex);
                callback(vertexCode);
                return;
            }
            if (BABYLON.Effect.ShadersStore[vertex + "VertexShader"])
            {
                callback(BABYLON.Effect.ShadersStore[vertex + "VertexShader"]);
                return;
            }
            var vertexShaderUrl;
            if (vertex[0] == ".")
            {
                vertexShaderUrl = vertex;
            }
            else
            {
                vertexShaderUrl = BABYLON.Engine.ShadersRepository + vertex;
            }
            BABYLON.Tools.LoadFile(vertexShaderUrl + ".vertex.fx", callback);
        }
        public virtual void _loadFragmentShader(object fragment, System.Action<object> callback)
        {
            if (fragment is HTMLElement)
            {
                var fragmentCode = BABYLON.Tools.GetDOMTextContent(fragment);
                callback(fragmentCode);
                return;
            }
            if (BABYLON.Effect.ShadersStore[fragment + "PixelShader"])
            {
                callback(BABYLON.Effect.ShadersStore[fragment + "PixelShader"]);
                return;
            }
            var fragmentShaderUrl;
            if (fragment[0] == ".")
            {
                fragmentShaderUrl = fragment;
            }
            else
            {
                fragmentShaderUrl = BABYLON.Engine.ShadersRepository + fragment;
            }
            BABYLON.Tools.LoadFile(fragmentShaderUrl + ".fragment.fx", callback);
        }
        public virtual void _prepareEffect(string vertexSourceCode, string fragmentSourceCode, Array<string> attributesNames, string defines, Array<string> optionalDefines = null, bool useFallback = false)
        {
            try
            {
                var engine = this._engine;
                this._program = engine.createShaderProgram(vertexSourceCode, fragmentSourceCode, defines);
                this._uniforms = engine.getUniforms(this._program, this._uniformsNames);
                this._attributes = engine.getAttributes(this._program, attributesNames);
                for (var index = 0; index < this._samplers.Length; index++)
                {
                    var sampler = this.getUniform(this._samplers[index]);
                    if (sampler == null)
                    {
                        this._samplers.splice(index, 1);
                        index--;
                    }
                }
                engine.bindSamplers(this);
                this._isReady = true;
                if (this.onCompiled)
                {
                    this.onCompiled(this);
                }
            }
            catch (Exception e)
            {
                if (!useFallback && optionalDefines)
                {
                    for (index = 0; index < optionalDefines.Length; index++)
                    {
                        defines = defines.replace(optionalDefines[index], "");
                    }
                    this._prepareEffect(vertexSourceCode, fragmentSourceCode, attributesNames, defines, optionalDefines, true);
                }
                else
                {
                    Tools.Error("Unable to compile effect: " + this.name);
                    Tools.Error("Defines: " + defines);
                    Tools.Error("Optional defines: " + optionalDefines);
                    Tools.Error("Error: " + e.message);
                    this._compilationError = e.message;
                    if (this.onError)
                    {
                        this.onError(this, this._compilationError);
                    }
                }
            }
        }
        public virtual void _bindTexture(string channel, WebGLTexture texture)
        {
            this._engine._bindTexture(this._samplers.indexOf(channel), texture);
        }
        public virtual void setTexture(string channel, BaseTexture texture)
        {
            this._engine.setTexture(this._samplers.indexOf(channel), texture);
        }
        public virtual void setTextureFromPostProcess(string channel, PostProcess postProcess)
        {
            this._engine.setTextureFromPostProcess(this._samplers.indexOf(channel), postProcess);
        }
        public virtual void _cacheFloat2(string uniformName, double x, double y)
        {
            if (!this._valueCache[uniformName])
            {
                this._valueCache[uniformName] = new Array<object>(x, y);
                return;
            }
            this._valueCache[uniformName][0] = x;
            this._valueCache[uniformName][1] = y;
        }
        public virtual void _cacheFloat3(string uniformName, double x, double y, double z)
        {
            if (!this._valueCache[uniformName])
            {
                this._valueCache[uniformName] = new Array<object>(x, y, z);
                return;
            }
            this._valueCache[uniformName][0] = x;
            this._valueCache[uniformName][1] = y;
            this._valueCache[uniformName][2] = z;
        }
        public virtual void _cacheFloat4(string uniformName, double x, double y, double z, double w)
        {
            if (!this._valueCache[uniformName])
            {
                this._valueCache[uniformName] = new Array<object>(x, y, z, w);
                return;
            }
            this._valueCache[uniformName][0] = x;
            this._valueCache[uniformName][1] = y;
            this._valueCache[uniformName][2] = z;
            this._valueCache[uniformName][3] = w;
        }
        public virtual Effect setArray(string uniformName, Array<double> array)
        {
            this._engine.setArray(this.getUniform(uniformName), array);
            return this;
        }
        public virtual Effect setMatrices(string uniformName, Float32Array matrices)
        {
            this._engine.setMatrices(this.getUniform(uniformName), matrices);
            return this;
        }
        public virtual Effect setMatrix(string uniformName, Matrix matrix)
        {
            this._engine.setMatrix(this.getUniform(uniformName), matrix);
            return this;
        }
        public virtual Effect setFloat(string uniformName, double value)
        {
            if (this._valueCache[uniformName] && this._valueCache[uniformName] == value)
                return this;
            this._valueCache[uniformName] = value;
            this._engine.setFloat(this.getUniform(uniformName), value);
            return this;
        }
        public virtual Effect setBool(string uniformName, bool _bool)
        {
            if (this._valueCache[uniformName] && this._valueCache[uniformName] == _bool)
                return this;
            this._valueCache[uniformName] = _bool;
            this._engine.setBool(this.getUniform(uniformName), (_bool) ? 1 : 0);
            return this;
        }
        public virtual Effect setVector2(string uniformName, Vector2 vector2)
        {
            if (this._valueCache[uniformName] && this._valueCache[uniformName][0] == vector2.x && this._valueCache[uniformName][1] == vector2.y)
                return this;
            this._cacheFloat2(uniformName, vector2.x, vector2.y);
            this._engine.setFloat2(this.getUniform(uniformName), vector2.x, vector2.y);
            return this;
        }
        public virtual Effect setFloat2(string uniformName, double x, double y)
        {
            if (this._valueCache[uniformName] && this._valueCache[uniformName][0] == x && this._valueCache[uniformName][1] == y)
                return this;
            this._cacheFloat2(uniformName, x, y);
            this._engine.setFloat2(this.getUniform(uniformName), x, y);
            return this;
        }
        public virtual Effect setVector3(string uniformName, Vector3 vector3)
        {
            if (this._valueCache[uniformName] && this._valueCache[uniformName][0] == vector3.x && this._valueCache[uniformName][1] == vector3.y && this._valueCache[uniformName][2] == vector3.z)
                return this;
            this._cacheFloat3(uniformName, vector3.x, vector3.y, vector3.z);
            this._engine.setFloat3(this.getUniform(uniformName), vector3.x, vector3.y, vector3.z);
            return this;
        }
        public virtual Effect setFloat3(string uniformName, double x, double y, double z)
        {
            if (this._valueCache[uniformName] && this._valueCache[uniformName][0] == x && this._valueCache[uniformName][1] == y && this._valueCache[uniformName][2] == z)
                return this;
            this._cacheFloat3(uniformName, x, y, z);
            this._engine.setFloat3(this.getUniform(uniformName), x, y, z);
            return this;
        }
        public virtual Effect setFloat4(string uniformName, double x, double y, double z, double w)
        {
            if (this._valueCache[uniformName] && this._valueCache[uniformName][0] == x && this._valueCache[uniformName][1] == y && this._valueCache[uniformName][2] == z && this._valueCache[uniformName][3] == w)
                return this;
            this._cacheFloat4(uniformName, x, y, z, w);
            this._engine.setFloat4(this.getUniform(uniformName), x, y, z, w);
            return this;
        }
        public virtual Effect setColor3(string uniformName, Color3 color3)
        {
            if (this._valueCache[uniformName] && this._valueCache[uniformName][0] == color3.r && this._valueCache[uniformName][1] == color3.g && this._valueCache[uniformName][2] == color3.b)
                return this;
            this._cacheFloat3(uniformName, color3.r, color3.g, color3.b);
            this._engine.setColor3(this.getUniform(uniformName), color3);
            return this;
        }
        public virtual Effect setColor4(string uniformName, Color3 color3, double alpha)
        {
            if (this._valueCache[uniformName] && this._valueCache[uniformName][0] == color3.r && this._valueCache[uniformName][1] == color3.g && this._valueCache[uniformName][2] == color3.b && this._valueCache[uniformName][3] == alpha)
                return this;
            this._cacheFloat4(uniformName, color3.r, color3.g, color3.b, alpha);
            this._engine.setColor4(this.getUniform(uniformName), color3, alpha);
            return this;
        }
        public static dynamic ShadersStore = new { };
    }
}