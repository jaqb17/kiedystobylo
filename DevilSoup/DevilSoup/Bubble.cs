using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{
    public class Bubble
    {
        const string modelPath = "Assets/bubble";
        const string effectPath = "Assets/Effects/Reflection";
        const string skyboxPath = "Assets/Skybox/helll";
        private Asset bubble;
        private Vector3 position;
        private Camera camera;
        private ContentManager content;
        private TextureCube skyBoxTexture;

        public Bubble(ContentManager content)
        {
            this.content = content;
        }

        public void Initialize(Camera camera, Vector3 position)
        {

            this.camera = camera;
            bubble = new Asset(content, modelPath, effectPath, camera);
            skyBoxTexture = content.Load<TextureCube>("Assets/Skybox/helll");
            this.position = position;
            bubble.world = Matrix.CreateTranslation(position);
        }

        public void setPosition(Vector3 _position)
        {
            this.position = _position;
            this.position.Y += 10f;
            this.position.Z += 3f;
            bubble.world = Matrix.CreateTranslation(position);
        }

        public void scale(float x, float y = 1f, float z = 1f)
        {
            bubble.scaleAset(x, y, z);
        }

        public void Draw(Vector3? color = null)
        {
            bubble.DrawReflected(camera.view, camera.projection, skyBoxTexture, camera.Position, color);
        }
    }
}
