using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/*
Copyright 2017 by kosmonautgames

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace DevilSoup
{
    /// <summary>
    /// A shader to draw a uniform color across a mesh
    /// </summary>
    public class AnimatedModelShader
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Variables

        private GraphicsDevice _graphicsDevice;

        private Effect _shaderEffect;

        private EffectParameter _worldViewProjParameter;
        private EffectParameter _worldITParameter;
        private EffectParameter _worldParameter;
        private EffectParameter _viewParameter;
        private EffectParameter _bonesParameter;
        private EffectParameter _cameraPositionParameter;
        private EffectParameter _normalMapParameter;
        private EffectParameter _albedoMapParameter;
        private EffectParameter _albedoColorParameter;
        private EffectParameter _metallicMapParameter;
        
        private EffectParameter _ambientColorParameter;
        private EffectParameter _ambientIntensityParameter;
        private EffectParameter _LightDirectionParameter;
        private EffectParameter _diffuseColorParameter;
        private EffectParameter _diffuseIntesityParameter;
        private EffectParameter _specularColorParameter;
        private EffectParameter _shineParameter;

        public Vector4 _ambientColor;
        public float _ambientIntensity = 0.1f;
        public Vector3 _lightDirection;
        public Vector4 _diffuseColor;
        public float _diffuseIntesity = 0.9f;
        public Vector4 _specularColor;
        public float _shine = 5f;

        private EffectPass _skinnedPass;

        private Texture2D _normalMap;
        public Texture2D NormalMap
        {
            get { return _normalMap; }
            set
            {
                if (_normalMap != value)
                {
                    _normalMap = value;
                }
            }
        }

        private Color _albedoColor;
        public Color AlbedoColor
        {
            get
            {
                return _albedoColor;
            }

            set
            {
                if (_albedoColor != value)
                {
                    _albedoColor = value;
                    _albedoColorParameter.SetValue(_albedoColor.ToVector4());
                }
            }
        }

        private Texture2D _albedoMap;

        public Texture2D AlbedoMap
        {
            get { return _albedoMap; }
            set
            {
                if (_albedoMap != value)
                {
                    _albedoMap = value;
                    _albedoMapParameter.SetValue(_albedoMap);
                }
            }
        }

        private Texture2D _heightMap;

        public Texture2D HeightMap
        {
            get { return _heightMap; }
            set
            {
                if (_heightMap != value)
                {
                    _heightMap = value;
                }
            }
        }

        public float Roughness
        {
            get { return _roughness; }
            set
            {
                if (Math.Abs(_roughness - value) > 0.0001f)
                {
                    _roughness = value;
                }
            }
        }

        private float _roughness;

        public float Metallic
        {
            get { return _metallic; }
            set
            {
                if (Math.Abs(_metallic - value) > 0.0001f)
                {
                    _metallic = value;
                }
            }
        }

        private float _metallic;

        private Texture2D _metallicMap;

        public Texture2D MetallicMap
        {
            get { return _metallicMap; }
            set
            {
                if (_metallicMap != value)
                {
                    _metallicMap = value;
                    _metallicMapParameter.SetValue(_metallicMap);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Functions

        /// <summary>
        /// Needs to be called to load all the shader fx files
        /// </summary>
        /// <param name="content"></param>
        /// <param name="shaderPath"></param>
        public void Load(ContentManager content, string shaderPath)
        {
            _shaderEffect = content.Load<Effect>(shaderPath);

            _worldViewProjParameter = _shaderEffect.Parameters["ViewProj"];
            _worldITParameter = _shaderEffect.Parameters["WorldIT"];
            _worldParameter = _shaderEffect.Parameters["World"];
            _viewParameter = _shaderEffect.Parameters["View"];
            _bonesParameter = _shaderEffect.Parameters["Bones"];
            _cameraPositionParameter = _shaderEffect.Parameters["CameraPosition"];

            _normalMapParameter = _shaderEffect.Parameters["NormalMap"];
            _albedoColorParameter = _shaderEffect.Parameters["AlbedoColor"];
            _albedoMapParameter = _shaderEffect.Parameters["ColorMap"];
            
            _metallicMapParameter = _shaderEffect.Parameters["SpecMap"];

            _ambientColorParameter = _shaderEffect.Parameters["AmbientColor"];
            _ambientIntensityParameter = _shaderEffect.Parameters["AmbientIntensity"];
            _LightDirectionParameter = _shaderEffect.Parameters["LightDirection"];
            _diffuseColorParameter = _shaderEffect.Parameters["DiffuseColor"];
            _diffuseIntesityParameter = _shaderEffect.Parameters["DiffuseIntensity"];
            _specularColorParameter = _shaderEffect.Parameters["SpecularColor"];
            _shineParameter = _shaderEffect.Parameters["Shine"];

            AlbedoColor = Color.White;
            Roughness = 0.0f;
            Metallic = 0.0f;

            _skinnedPass = _shaderEffect.Techniques["Skinned"].Passes[0];
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Base draw
        /// </summary>
        /// <param name="model"></param>
        /// <param name="world"></param>
        /// <param name="viewProjection"></param>
        /// <param name="cameraPosition"></param>
        /// <param name="effectPass"></param>
        /// <param name="bones"></param>
        public void DrawMesh(Model model, Matrix world, Matrix view, Matrix projection, Vector3 cameraPosition, Matrix[] bones = null, Vector3? color = null)
        {
            _worldViewProjParameter.SetValue(view * projection);
            _worldITParameter.SetValue(world);
            _worldParameter.SetValue(world);
            _viewParameter.SetValue(view);
            _shaderEffect.Parameters["Projection"].SetValue(projection);

            if (bones != null)
                _bonesParameter.SetValue(bones);

            _cameraPositionParameter.SetValue(cameraPosition);
            if (color != null) _shaderEffect.Parameters["addColor"].SetValue(color ?? new Vector3(0.0f, 0.0f, 0.0f));
            _albedoMapParameter.SetValue(_albedoMap);
            _normalMapParameter.SetValue(_normalMap);
            _metallicMapParameter.SetValue(_metallicMap);
            _ambientColorParameter.SetValue(_ambientColor);
            _ambientIntensityParameter.SetValue(_ambientIntensity);
            _LightDirectionParameter.SetValue(_lightDirection);
            _diffuseColorParameter.SetValue(_diffuseColor);
            _diffuseIntesityParameter.SetValue(_diffuseIntesity);
            _specularColorParameter.SetValue(_specularColor);
            _shineParameter.SetValue(_shine);

            for (int index = 0; index < model.Meshes.Count; index++)
            {
                var modelMesh = model.Meshes[index];

                for (int i = 0; i < modelMesh.MeshParts.Count; i++)
                {
                    var modelMeshPart = modelMesh.MeshParts[i];
                    DrawMeshPart(modelMeshPart);
                }
            }
        }


        /// <summary>
        /// Draw Mesh with the effect applied
        /// </summary>
        /// <param name="modelMeshPart"></param>
        /// <param name="worldViewProjection"></param>
        /// <param name="effectPass"></param>
        private void DrawMeshPart(ModelMeshPart modelMeshPart)
        {
            _graphicsDevice.SetVertexBuffer(modelMeshPart.VertexBuffer);
            _graphicsDevice.Indices = (modelMeshPart.IndexBuffer);
            int primitiveCount = modelMeshPart.PrimitiveCount;
            int vertexOffset = modelMeshPart.VertexOffset;
            int startIndex = modelMeshPart.StartIndex;
            _skinnedPass.Apply();

            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexOffset, startIndex, primitiveCount);
        }


    }
}
