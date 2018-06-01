using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using System;
using SkinnedModel;
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
        private AnimationPlayer animationPlayer = null;
        private TimeSpan animationLastTime;

        public bool HasAnimation
        {
            get
            {
                return animationPlayer != null;
            }
        }

        //public double animationLength { get; private set; }
        //public DateTime animationStarted { get; private set; }

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

        public void initializeClip(String clipName)
        {
            if (animationPlayer == null)
                return;

            SkinningData skinningData = model.Tag as SkinningData;
            AnimationClip clip = skinningData.AnimationClips[clipName];
            animationPlayer.StartClip(clip);
        }

        public void animationUpdate(GameTime gameTime, bool relativeToCurrentTime = true, Matrix? rootTransform = null)
        {
            if (!ifPlay)
                return;

            if (animationDelay > 0)
            {
                animationDelayCounter++;
                if (animationDelayCounter > animationDelay)
                    animationDelayCounter = 0;
            }

            if (rootTransform == null)
                rootTransform = Matrix.Identity;

            if (ifDamageAfterPlay)
            {
                if (animationLastTime != null && animationPlayer?.CurrentTime < animationLastTime)
                {
                    this.finishedAnimation = true;
                }
            }

            if (animationDelayCounter == 0)
            {
                animationLastTime = animationPlayer.CurrentTime;
                animationPlayer.Update(gameTime.ElapsedGameTime, relativeToCurrentTime, (Matrix)rootTransform);
            }
        }


        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        /*public AnimationPlayer PlayClip(AnimationClip clip, bool looping = true, int keyframestart = 0, int keyframeend = 0, int fps = 24)
        {
            // Create a clip player and assign it to this model
            //animationPlayer = new AnimationPlayer(clip, this, looping, keyframestart, keyframeend, fps);
            animationPlayer.StartClip(clip);
            return animationPlayer;
        }*/

        public void loadModel(ContentManager content, String path)
        {

            this.model = content.Load<Model>(path);
            SkinningData skinningData = model.Tag as SkinningData;

            if (skinningData != null)
                animationPlayer = new AnimationPlayer(skinningData);

            computeCenter();
        }

        public void unloadModel()
        {
            model = null;
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection, Vector3? color = null)
        {

            if (model == null) return;

            Matrix[] bones = null;
            if (this.HasAnimation)
            {
                if (!ifPlay)
                    animationPlayer.Update(new TimeSpan(0), true, Matrix.Identity);

                bones = animationPlayer.GetSkinTransforms();
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

                        seffect.SetBoneTransforms(bones);

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
