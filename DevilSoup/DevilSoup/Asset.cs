using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using System;
using MGSkinnedAnimationAux;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text;
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
        public bool ifDamageAfterPlay { get; set; } = false;
        public bool finishedAnimation { get; private set; } = false;
        private ModelExtra modelExtra = null;

        public Effect renderEffect;

        private Texture colorMap = null;
        private Texture normalMap = null;
        private Texture specMap = null;

        /// <summary>
        /// The model bones
        /// </summary>
        private List<Bone> bones = new List<Bone>();

        private AnimationPlayer player = null;

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

        public bool HasAnimation()
        {
            return modelExtra != null;
        }

        #endregion

        public Asset() { }

        public Asset(Model model, Vector3 cameraPos)
        {
            this.model = model;
            this.cameraPos = cameraPos;
            computeCenter();
        }

        public Asset(Model model, Camera camera)
        {
            this.model = model;
            this.cameraPos = camera.Position;
            this.world = camera.world;
            computeCenter();
        }

        public Asset(ContentManager content, string modelPath, string colorTexturePath, string normalTexturePath, string SpecTexturePath, Camera camera,string effectPath= "Assets/Effects/CNS")
        {

            this.colorMap = content.Load<Texture>(colorTexturePath);
            this.normalMap = content.Load<Texture>(normalTexturePath);
            this.specMap = content.Load<Texture>(SpecTexturePath);

            this.renderEffect = content.Load<Effect>(effectPath);
            this.model = content.Load<Model>(modelPath);

            renderEffect.Parameters["ColorMap"].SetValue(colorMap);
            renderEffect.Parameters["NormalMap"].SetValue(normalMap);
            renderEffect.Parameters["SpecMap"].SetValue(specMap);

            this.cameraPos = camera.Position;

            computeCenter();
        }

        public Asset(ContentManager content, string modelPath, string colorTexturePath, string normalTexturePath,  Camera camera, string effectPath = "Assets/Effects/CN")
        {

            this.colorMap = content.Load<Texture>(colorTexturePath);
            this.normalMap = content.Load<Texture>(normalTexturePath);
            

            this.renderEffect = content.Load<Effect>(effectPath);
            this.model = content.Load<Model>(modelPath);

            renderEffect.Parameters["ColorMap"].SetValue(colorMap);
            renderEffect.Parameters["NormalMap"].SetValue(normalMap);
           

            this.cameraPos = camera.Position;

            computeCenter();
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
                player?.Update(gameTime);
        }


        #region Animation Management

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

        public void loadModel(ContentManager content, String path)
        {

            this.model = content.Load<Model>(path);
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

            if (this.HasAnimation())
            {

                if (this.Clips.Count > 0)
                    this.animationUpdate(gameTime);

                if (!this.ifPlay)
                {
                    this.PlayClip(this.Clips[0]);
                    //this.ifPlay = true;
                }

            }

            Matrix[] boneTransforms = null;
            Matrix[] skeleton = null;

            if (modelExtra != null)
            {
                boneTransforms = new Matrix[bones.Count];

                for (int i = 0; i < bones.Count; i++)
                {
                    Bone bone = bones[i];
                    bone.ComputeAbsoluteTransform();

                    boneTransforms[i] = bone.AbsoluteTransform;
                }

                //
                // Determine the skin transforms from the skeleton
                //

                skeleton = new Matrix[modelExtra.Skeleton.Count];
                for (int s = 0; s < modelExtra.Skeleton.Count; s++)
                {
                    Bone bone = bones[modelExtra.Skeleton[s]];
                    skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform;
                }
            }

            //// Draw the model.
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
                        seffect.SetBoneTransforms(skeleton);
                    }
                }

                modelMesh.Draw();
            }
        }

        public void SimpleDraw( Matrix view, Matrix projection, Vector3? addColor=null)
        {
            Vector4 diff = new Vector4(1, 1, 1, 1);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = renderEffect;
                    if (addColor != null)
                    {
                        renderEffect.Parameters["addColor"].SetValue(addColor ?? new Vector3(0.0f, 0.0f, 0.0f));
                    }
                    renderEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    renderEffect.Parameters["View"].SetValue(view);
                    renderEffect.Parameters["Projection"].SetValue(projection);
                    renderEffect.Parameters["CamPosition"].SetValue(cameraPos);
                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    // effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                    
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
            renderEffect.Parameters["AmbientColor"].SetValue(color);
        }

        public void setAmbientIntensity(float intensity)
        {
            renderEffect.Parameters["AmbientIntensity"].SetValue(intensity);
        }

        public void setLightDirection(Vector3 direction)
        {
            renderEffect.Parameters["LightDirection"].SetValue(direction);
        }

        public void setDiffuseColor(Vector4 color)
        {
            renderEffect.Parameters["DiffuseColor"].SetValue(color);
        }

        public void setDiffuseIntensity(float intensity)
        {
            renderEffect.Parameters["DiffuseIntensity"].SetValue(intensity);
        }


        public void setSpecularColor(Vector4 color)
        {
            renderEffect.Parameters["SpecularColor"].SetValue(color);
        }

        private void computeCenter()
        {
            if(model.Meshes.Count > 0)
            {
                center = model.Meshes[0].BoundingSphere.Center;
                radius = model.Meshes[0].BoundingSphere.Radius;
            }
            else
            {
                for(int i = 0; i < model.Bones.Count; i++)
                {
                    if(model.Bones[i].Meshes.Count > 0)
                    {
                        center = model.Meshes[0].BoundingSphere.Center;
                        radius = model.Meshes[0].BoundingSphere.Radius;
                    }
                }
            }
        }
    }
}
