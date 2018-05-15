using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using SkinnedModel;

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
        private AnimationPlayer animationPlayer;

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

        public void animationUpdate(TimeSpan timeSpan, bool relativeToCurrentTime = true, Matrix? rootTransform = null)
        {
            if (!ifPlay)
                return;

            if (rootTransform == null)
                rootTransform = Matrix.Identity;

            animationPlayer.Update(timeSpan, relativeToCurrentTime, (Matrix) rootTransform);
        }

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

        public void DrawModel(Matrix view, Matrix projection, Vector3? color = null)
        {

            if (model == null) return;

            Matrix[] bones = null;
            if (animationPlayer != null)
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
