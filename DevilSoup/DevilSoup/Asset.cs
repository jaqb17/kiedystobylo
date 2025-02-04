﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using MGSkinnedAnimationAux;
using System.Collections.Generic;

namespace DevilSoup
{
    public class Asset
    {
        public Model model { get; private set; }
        public Matrix world { get; set; }
        public Vector3 cameraPos { get; set; }
        public Vector3 center { get; private set; }
        public float radius { get; private set; }
        public bool ifPlay { get; set; } = false;
        public int animationDelay { get; set; } = 0;
        private int animationDelayCounter { get; set; } = 0;

        private bool ifDamageAfterPlay = false;
        public bool IfDamageAfterPlay
        {
            get { return ifDamageAfterPlay; }
            set
            {
                ifDamageAfterPlay = value;
                player.Looping = !ifDamageAfterPlay;
            }
        }

        public bool finishedAnimation { get; private set; } = false;
        private ModelExtra modelExtra = null;

        public Effect renderEffect;

        private Texture colorMap = null;
        private Texture normalMap = null;
        private Texture specMap = null;
        private TextureCube skyBoxTexture = null;

        /// <summary>
        /// The model bones
        /// </summary>
        private List<Bone> bones = new List<Bone>();

        private AnimationPlayer player = null;

        private Vector4 ambientColor;
        private float ambientIntensity = 0.1f;
        private Vector3 LightDirection;
        private Vector4 diffuseColor;
        private float diffuseIntesity = 0.9f;
        private Vector4 specularColor;
        private float shine = 5f;
        private bool shaderAttached = false;
        private GraphicsDevice graphicsDevice;

        //public double animationLength { get; private set; }
        //public DateTime animationStarted { get; private set; }

        #region Properties

        /// <summary>
        /// The actual underlying XNA model
        /// </summary>
        public Model Model
        {
            get { return model; }
        }

        /// <summary>
        /// The underlying bones for the model
        /// </summary>
        public List<Bone> Bones { get { return bones; } }

        /// <summary>
        /// The model animation clips
        /// </summary>
        public List<AnimationClip> Clips { get { return modelExtra.Clips; } }

        public bool HasAnimation
        {
            get { return modelExtra != null; }
        }

        #endregion

        public Asset() { }

        public Asset(Model model, Vector3 cameraPos)
        {
            this.model = model;
            this.cameraPos = cameraPos;
            shaderAttached = false;
            computeCenter();
        }

        public Asset(Model model, Camera camera)
        {
            this.model = model;
            this.cameraPos = camera.Position;
            this.world = camera.world;
            shaderAttached = false;
            computeCenter();
        }
        public Asset(ContentManager content, string modelPath, string effectPath, Camera camera)
        {
            this.model = content.Load<Model>(modelPath);
            this.cameraPos = camera.Position;
            this.world = camera.world;
            this.renderEffect = content.Load<Effect>(effectPath);
            shaderAttached = true;
            computeCenter();
        }

        public Asset(ContentManager content, string modelPath, string colorTexturePath, string normalTexturePath, string specTexturePath,
            Camera camera, string effectPath = "Assets/Effects/CNS")
        {
            this.model = content.Load<Model>(modelPath);
            this.renderEffect = content.Load<Effect>(effectPath);
            loadTextures(content, colorTexturePath, normalTexturePath, specTexturePath, null);
            shaderAttached = true;

            this.cameraPos = camera.Position;
            initShaderData();
            computeCenter();
        }

        public Asset(ContentManager content, string modelPath, string colorTexturePath, string normalTexturePath, Camera camera,
            string effectPath = "Assets/Effects/CN")
        {
            this.model = content.Load<Model>(modelPath);
            this.renderEffect = content.Load<Effect>(effectPath);
            loadTextures(content, colorTexturePath, normalTexturePath, null, null);
            shaderAttached = true;
            this.cameraPos = camera.Position;
            initShaderData();
            computeCenter();
        }

        public Asset(ContentManager content, string modelPath, string colorTexturePath, string normalTexturePath, string specTexturePath,
            string skyboxPath, Camera camera, string effectPath = "Assets/Effects/CNS_E")
        {
            this.model = content.Load<Model>(modelPath);
            this.renderEffect = content.Load<Effect>(effectPath);
            loadTextures(content, colorTexturePath, normalTexturePath, specTexturePath, skyboxPath);
            shaderAttached = true;

            this.cameraPos = camera.Position;
            initShaderData();
            computeCenter();
        }

        private void initShaderData()
        {
            ambientColor = new Vector4(1, 1, 1, 1);
            LightDirection = new Vector3(0, 0, -1);
            specularColor = new Vector4(1, 1, 1, 1);
            diffuseColor = new Vector4(1, 1, 1, 1);
        }

        #region Bones Management

        /// <summary>
        /// Get the bones from the model and create a bone class object for
        /// each bone. We use our bone class to do the real animated bone work.
        /// </summary>
        private void ObtainBones()
        {
            bones.Clear();
            foreach (ModelBone bone in model.Bones)
            {
                // Create the bone object and add to the heirarchy
                Bone newBone = new Bone(bone.Name, bone.Transform, bone.Parent != null ? bones[bone.Parent.Index] : null);

                // Add to the bones for this model
                bones.Add(newBone);
            }
        }

        /// <summary>
        /// Find a bone in this model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bone FindBone(string name)
        {
            foreach (Bone bone in Bones)
            {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }

        #endregion

        #region Animation Management

        public void animationUpdate(GameTime gameTime)
        {
            if (animationDelay > 0)
            {
                animationDelayCounter++;
                if (animationDelayCounter > animationDelay)
                    animationDelayCounter = 0;
            }

            if (ifDamageAfterPlay)
            {
                if (player?.Position >= player?.Duration)
                {
                    this.finishedAnimation = true;
                }
            }

            if (ifPlay && animationDelayCounter == 0)
            {
                player.Looping = !ifDamageAfterPlay;
                player?.Update(gameTime);
            }
        }

        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip)
        {
            // Create a clip player and assign it to this model
            player = new AnimationPlayer(clip, this);
            return player;
        }

        #endregion

        public void loadModel(ContentManager content, String modelPath)
        {
            shaderAttached = false;
            this.model = content.Load<Model>(modelPath);
            modelExtra = model.Tag as ModelExtra;

            if (modelExtra != null)
            {
                ObtainBones();
            }

            computeCenter();
        }

        private void loadTextures(ContentManager content, string aldeboTexturePath, string normalTexturePath, string specTexturePath,
            string skyboxPath)
        {

            if (aldeboTexturePath != null)
            {
                colorMap = content.Load<Texture>(aldeboTexturePath);
                renderEffect.Parameters["ColorMap"].SetValue(colorMap);
            }

            if (normalTexturePath != null)
            {
                normalMap = content.Load<Texture>(normalTexturePath);
                renderEffect.Parameters["NormalMap"].SetValue(normalMap);
            }

            if (specTexturePath != null)
            {
                specMap = content.Load<Texture>(specTexturePath);
                renderEffect.Parameters["SpecMap"].SetValue(specMap);
            }
            if (skyboxPath != null)
            {
                skyBoxTexture = content.Load<TextureCube>(skyboxPath);
                renderEffect.Parameters["SkyboxTexture"].SetValue(skyBoxTexture);
            }
        }

        public void loadModel(ContentManager content, GraphicsDevice graphicsDevice, String modelPath, string aldeboTexturePath, 
            string normalTexturePath, string shaderPath, string specTexturePath = null, string skyboxPath = null)
        {
            this.graphicsDevice = graphicsDevice;
            shaderAttached = true;
            this.model = content.Load<Model>(modelPath);
            this.renderEffect = content.Load<Effect>(shaderPath);
            loadTextures(content, aldeboTexturePath, normalTexturePath, specTexturePath, skyboxPath);
            initShaderData();
            modelExtra = model.Tag as ModelExtra;

            if (modelExtra != null)
            {
                ObtainBones();
            }

            computeCenter();
        }
        public void loadModel(ContentManager content, GraphicsDevice graphicsDevice, 
            String modelPath, string normalTexturePath,  string shaderPath, string skyboxPath = null)
        {
            this.graphicsDevice = graphicsDevice;
            shaderAttached = true;
            this.model = content.Load<Model>(modelPath);
            this.renderEffect = content.Load<Effect>(shaderPath);
            loadTextures(content, null, normalTexturePath, null, skyboxPath);
            initShaderData();
            modelExtra = model.Tag as ModelExtra;

            if (modelExtra != null)
            {
                ObtainBones();
            }

            computeCenter();
        }

        public void unloadModel()
        {
            model = null;
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection, Vector3? color = null)
        {

            if (model == null) return;

            if (this.HasAnimation)
            {
                if (this.Clips.Count > 0)
                    this.animationUpdate(gameTime);

                if (!this.ifPlay)
                {
                    this.PlayClip(this.Clips[0]);
                }
            }

            Matrix[] boneTransforms = null;
            Matrix[] skeleton = null;

            if (modelExtra != null)
            {
                boneTransforms = new Matrix[this.bones.Count];

                for (int i = 0; i < this.bones.Count; i++)
                {
                    Bone bone = this.bones[i];
                    bone.ComputeAbsoluteTransform();

                    boneTransforms[i] = bone.AbsoluteTransform;
                }

                //
                // Determine the skin transforms from the skeleton
                //

                skeleton = new Matrix[modelExtra.Skeleton.Count];
                for (int s = 0; s < modelExtra.Skeleton.Count; s++)
                {
                    Bone bone = this.bones[modelExtra.Skeleton[s]];
                    skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform;
                }
            }

            if (!shaderAttached)
                DrawWithoutShader(view, projection, boneTransforms, skeleton, color);
            else
            {
                AnimatedDraw(view, projection, boneTransforms, skeleton, color);
            }
        }

        public void SimpleDraw(Matrix view, Matrix projection, Vector3? addColor = null)
        {
            //Vector4 diff = new Vector4(1, 1, 1, 1);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = renderEffect;
                    renderEffect.Parameters["addColor"].SetValue(addColor ?? new Vector3(0.0f, 0.0f, 0.0f));
                    renderEffect.Parameters["ColorMap"].SetValue(colorMap);
                    renderEffect.Parameters["NormalMap"].SetValue(normalMap);
                    if (skyBoxTexture != null)
                        renderEffect.Parameters["SkyboxTexture"].SetValue(skyBoxTexture);

                    if (specMap != null)
                    {
                        renderEffect.Parameters["SpecMap"].SetValue(specMap);
                    }
                    renderEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    renderEffect.Parameters["View"].SetValue(view);
                    renderEffect.Parameters["Projection"].SetValue(projection);
                    renderEffect.Parameters["CamPosition"].SetValue(cameraPos);
                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    // effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);

                    renderEffect.Parameters["AmbientColor"].SetValue(ambientColor);
                    renderEffect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                    renderEffect.Parameters["LightDirection"].SetValue(LightDirection);
                    renderEffect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                    renderEffect.Parameters["DiffuseIntensity"].SetValue(diffuseIntesity);
                    renderEffect.Parameters["SpecularColor"].SetValue(specularColor);
                    renderEffect.Parameters["Shine"].SetValue(shine);
                    renderEffect.Techniques["NotSkinned"].Passes[0].Apply();
                }
                mesh.Draw();
            }
        }

        private void AnimatedDraw(Matrix view, Matrix projection, Matrix[] boneTransforms, Matrix[] skeleton, Vector3? color = null)
        {
            if (colorMap == null)
                renderEffect.Parameters["addColor"].SetValue(color ?? new Vector3(0.0f, 0.0f, 1.0f));
            renderEffect.Parameters["addColor"].SetValue(color ?? new Vector3(0.0f, 0.0f, 0.0f));
            if (colorMap != null) renderEffect.Parameters["ColorMap"].SetValue(colorMap);
            renderEffect.Parameters["NormalMap"].SetValue(normalMap);

            if (specMap != null) renderEffect.Parameters["SpecMap"].SetValue(specMap);
            renderEffect.Parameters["ViewProj"].SetValue(view * projection);
            renderEffect.Parameters["View"].SetValue(view);
            renderEffect.Parameters["Projection"].SetValue(projection);
            renderEffect.Parameters["Bones"].SetValue(skeleton);
            renderEffect.Parameters["CamPosition"].SetValue(cameraPos);
            // effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
            if (colorMap != null)
            {

                renderEffect.Parameters["AmbientColor"].SetValue(ambientColor);
                renderEffect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                renderEffect.Parameters["LightDirection"].SetValue(LightDirection);
                renderEffect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                renderEffect.Parameters["DiffuseIntensity"].SetValue(diffuseIntesity);
                renderEffect.Parameters["SpecularColor"].SetValue(specularColor);
                renderEffect.Parameters["Shine"].SetValue(shine);
            }

            foreach (ModelMesh mesh in model.Meshes)
            {
                renderEffect.Parameters["WorldIT"].SetValue(boneTransforms[mesh.ParentBone.Index] * world);
                renderEffect.Parameters["World"].SetValue(boneTransforms[mesh.ParentBone.Index] * world);
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    DrawMeshPart(part, renderEffect.Techniques["Skinned"]);
                }
            }
        }

        private void DrawMeshPart(ModelMeshPart modelMeshPart, EffectTechnique effectTechnique)
        {
            graphicsDevice.SetVertexBuffer(modelMeshPart.VertexBuffer);
            graphicsDevice.Indices = (modelMeshPart.IndexBuffer);
            int primitiveCount = modelMeshPart.PrimitiveCount;
            int vertexOffset = modelMeshPart.VertexOffset;
            int startIndex = modelMeshPart.StartIndex;
            effectTechnique.Passes[0].Apply();

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexOffset, startIndex, primitiveCount);
        }

        private void DrawWithoutShader(Matrix view, Matrix projection, Matrix[] boneTransforms, Matrix[] skeleton, Vector3? color = null)
        {
            //// Draw the model.
            // Render the skinned mesh.
            foreach (ModelMesh modelMesh in model.Meshes)
            {
                foreach (Effect effect in modelMesh.Effects)
                {
                    if (effect is BasicEffect)
                    {
                        BasicEffect beffect = effect as BasicEffect;

                        beffect.World = world;
                        beffect.View = view;
                        beffect.Projection = projection;
                        //beffect.EnableDefaultLighting();
                        beffect.PreferPerPixelLighting = true;
                        if (color != null)
                            beffect.DiffuseColor = color ?? new Vector3(0.0f, 0.0f, 0.0f);
                    }

                    if (effect is SkinnedEffect)
                    {
                        SkinnedEffect seffect = effect as SkinnedEffect;

                        seffect.SetBoneTransforms(skeleton);
                        seffect.World = boneTransforms[modelMesh.ParentBone.Index] * world;
                        seffect.View = view;
                        seffect.Projection = projection;

                        seffect.EnableDefaultLighting();

                        if (color != null)
                            seffect.DiffuseColor = color ?? new Vector3(0.0f, 0.0f, 0.0f);

                        seffect.SpecularColor = new Vector3(0.25f);
                        seffect.SpecularPower = 16;
                    }
                }

                modelMesh.Draw();
            }
        }

        public void DrawReflected(Matrix view, Matrix projection, TextureCube skybox, Vector3 camPosition, Vector3? addColor = null)
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = renderEffect;
                    renderEffect.Parameters["World"].SetValue(this.world * mesh.ParentBone.Transform);
                    renderEffect.Parameters["View"].SetValue(view);
                    renderEffect.Parameters["Projection"].SetValue(projection);
                    renderEffect.Parameters["SkyboxTexture"].SetValue(skybox);
                    renderEffect.Parameters["CameraPosition"].SetValue(camPosition);
                    renderEffect.Parameters["WorldInverseTranspose"].SetValue(
                                            Matrix.Transpose(Matrix.Invert(this.world * mesh.ParentBone.Transform)));

                    renderEffect.Parameters["addColor"].SetValue(addColor ?? new Vector3(0.0f, 0.0f, 0.0f));


                }
                mesh.Draw();
            }

        }

        public void scaleAset(float scale)
        {
            this.world = Matrix.CreateScale(scale) * this.world;
        }

        public void scaleAset(float scaleX, float scaleY, float scaleZ)
        {
            this.world = Matrix.CreateScale(scaleX, scaleY, scaleZ) * this.world;
        }

        public void rotateAsset(int degreesX, int degreesY, int degreesZ)
        {
            this.world = Matrix.CreateRotationZ(-degreesX * (MathHelper.Pi / 180f)) * this.world;
            this.world = Matrix.CreateRotationX(-degreesY * (MathHelper.Pi / 180f)) * this.world;
            this.world = Matrix.CreateRotationY(-degreesZ * (MathHelper.Pi / 180f)) * this.world;
        }

        public void setShine(float shine)
        {
            renderEffect.Parameters["Shine"].SetValue(shine);
        }


        public void setAmbientColor(Vector4 color)
        {
            ambientColor = color;
        }

        public void setAmbientIntensity(float intensity)
        {
            ambientIntensity = intensity;
        }

        public void setLightDirection(Vector3 direction)
        {
            LightDirection = direction;
        }

        public void setDiffuseColor(Vector4 color)
        {
            diffuseColor = color;
        }

        public void setDiffuseIntensity(float intensity)
        {
            diffuseIntesity = intensity;
        }


        public void setSpecularColor(Vector4 color)
        {
            specularColor = color;
        }

        private void computeCenter()
        {
            if (model.Meshes.Count > 0)
            {
                center = model.Meshes[0].BoundingSphere.Center;
                radius = model.Meshes[0].BoundingSphere.Radius;
            }
            else
            {
                for (int i = 0; i < model.Bones.Count; i++)
                {
                    if (model.Bones[i].Meshes.Count > 0)
                    {
                        center = model.Meshes[0].BoundingSphere.Center;
                        radius = model.Meshes[0].BoundingSphere.Radius;
                    }
                }
            }
        }
    }
}
