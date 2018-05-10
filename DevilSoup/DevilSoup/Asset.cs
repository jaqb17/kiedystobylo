using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class Asset
    {
        public Model model { get; private set; }
        public Matrix world { get; set; }
        public Vector3 center { get; private set; }
        public float radius { get; private set; }

        public Asset() { }

        public Asset(Model model)
        {
            this.model = model;
            computeCenter();
        }

        public Asset(Model model, Matrix world)
        {
            this.model = model;
            this.world = world;
            computeCenter();
        }

        public void loadModel(ContentManager content, String path)
        {
            model = content.Load<Model>(path);
            computeCenter();
        }

        public void unloadModel()
        {
            model = null;
        }

        public void DrawModel(Matrix view, Matrix projection, Vector3? color = null)
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    //effect.PreferPerPixelLighting = true;
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    
                    if (color != null)
                        effect.DiffuseColor = color ?? new Vector3(0.0f, 0.0f, 0.0f);
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

        private void computeCenter()
        {
            center = model.Meshes[0].BoundingSphere.Center;
            radius = model.Meshes[0].BoundingSphere.Radius;
        }
    }
}
