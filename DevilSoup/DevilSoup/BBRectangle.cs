using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DevilSoup
{
    class BBRectangle
    {
        private Texture2D texture;
        private VertexPositionColor[] vertices = new VertexPositionColor[6];
        private VertexBuffer vBuffer;
        public Vector3 position { get; set; }
        public bool isDestroyable { get; set; }
        public BBRectangle(string texturePath, ContentManager content, Vector3 _position, GraphicsDevice dev)
        {
            position = _position;
            texture = content.Load<Texture2D>(texturePath);
            isDestroyable = false;
            VertexPositionColor[] vertices = new VertexPositionColor[6];
            vertices[0] = new VertexPositionColor(new Vector3(0.5f, 0f, 0f), Color.Green);
            vertices[1] = new VertexPositionColor(new Vector3(-0.5f, 0f, 0f), Color.Blue);
            vertices[2] = new VertexPositionColor(new Vector3(-0.5f, 1, 0f), Color.Red);
            vertices[3] = new VertexPositionColor(new Vector3(-0.5f, 1f, 0f), Color.Red);
            vertices[4] = new VertexPositionColor(new Vector3(0.5f, 1f, 0f), Color.Aqua);
            vertices[5] = new VertexPositionColor(new Vector3(0.5f, 0f, 0f), Color.Green);
            vBuffer = new VertexBuffer(dev, typeof(VertexPositionColor), 6, BufferUsage.WriteOnly);
            vBuffer.SetData<VertexPositionColor>(vertices);
        }

        public void DrawRect(BasicEffect effect, GraphicsDevice dev, Matrix view, Matrix projection, Matrix world)
        {
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            dev.RasterizerState = rasterizerState;
            //effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;
            //effect.Texture = texture;
            dev.SetVertexBuffer(vBuffer);
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    dev.DrawPrimitives(PrimitiveType.TriangleList,0,2);
                }
            }
        }
    }
}
