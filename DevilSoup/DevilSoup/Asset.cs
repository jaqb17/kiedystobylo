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

        public Asset() { }

        public Asset(Model model)
        {
            this.model = model;
        }

        public Asset(Model model, Matrix world, Matrix view, Matrix projection)
        {
            this.model = model;
            this.world = world;
        }

        public void loadModel(ContentManager content, String path)
        {
            model = content.Load<Model>(path);
        }

        public void DrawModel(Matrix view, Matrix projection, GraphicsDeviceManager graphics)
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
                }

                mesh.Draw();
            }
        }
    }
}
