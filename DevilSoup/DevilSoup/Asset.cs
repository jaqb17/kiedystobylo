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

        /// <summary>
        /// The model bones
        /// </summary>
        private List<Bone> bones = new List<Bone>();


        private Matrix[] skeleton;

        private Matrix[] boneTransforms;

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

        public Asset(){}

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
            if(animationDelay > 0)
            {
                animationDelayCounter++;
                if (animationDelayCounter > animationDelay)
                    animationDelayCounter = 0;
            }

            if(ifDamageAfterPlay)
            {
                if (player?.Position >= player?.Duration)
                {
                    this.finishedAnimation = true;
                }
            }
            
            if (animationDelayCounter == 0)
                player?.Update(gameTime);
        }


        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip, bool looping = true, int keyframestart = 0, int keyframeend = 0, int fps = 24)
        {
            // Create a clip player and assign it to this model
            player = new AnimationPlayer(clip, this, looping, keyframestart, keyframeend, fps);
            return player;
        }

        public void loadModel(ContentManager content, string modelType, String path)
        {

            if (ModelsInstancesClass.models[modelType] == null)
            {
                try
                {
                    this.model = content.Load<Model>(path);
                    ModelsInstancesClass.models[modelType] = this.model;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Element o tym kluczu juz byl dodany. Klasa: Asset, Linia: 74");
                }
            }
            else this.model = ModelsInstancesClass.models[modelType];

            modelExtra = model.Tag as ModelExtra;

            if (modelExtra != null)
            {
                ObtainBones();

                boneTransforms = new Matrix[bones.Count];
                skeleton = new Matrix[modelExtra.Skeleton.Count];
            }

            computeCenter();
        }

        public void unloadModel()
        {
            model = null;
        }

        public void DrawModel(Matrix view, Matrix projection, Vector3? color = null)
        {

            if (model == null) return;

            if (modelExtra != null)
            {

                for (int i = 0; i < bones.Count; i++)
                {
                    Bone bone = bones[i];
                    bone.ComputeAbsoluteTransform();

                    boneTransforms[i] = bone.AbsoluteTransform;
                }

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
                        seffect.World = world;
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

        private void computeCenter()
        {
            center = model.Meshes[0].BoundingSphere.Center;
            radius = model.Meshes[0].BoundingSphere.Radius;
        }
    }
}
