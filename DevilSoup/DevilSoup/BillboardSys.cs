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
    class BillboardSys
    {
        // Vertex Buff Index Buff particle

        public VertexBuffer vBuffer;
        public IndexBuffer iBufffer;
        public VertexPositionTexture[] particles;
        public int[] indices;

        //Billboard settings
        public int nBillboards;
        public Vector2 BBsize;
        public Texture2D tex2D;

        // Graph FX
        public GraphicsDevice gDevice;
        public Effect eff;

        public BillboardSys(GraphicsDevice _graphicDevice, ContentManager content, Texture2D texture, Vector2 _BBSize, Vector3[] parPositions)
        {
            this.nBillboards = parPositions.Length;
            this.BBsize = _BBSize;
            this.gDevice = _graphicDevice;
            this.tex2D = texture;
            eff = content.Load<Effect>("Assets\\Effects\\BillBoardEff");
            generateParticles(parPositions);
        }

        public void Draw(Matrix View, Matrix Projection, Vector3 Up, Vector3 Right)
        {
            // Set the vertex and index buffer to the graphics card
            gDevice.SetVertexBuffer(vBuffer);
            gDevice.Indices = iBufffer;
            setEffectParameters(View, Projection, Up, Right);
            // Draw the billboards
            gDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4 * nBillboards, 0, nBillboards * 2);
            
            // Un-set the vertex and index buffer
            gDevice.SetVertexBuffer(null);
            gDevice.Indices = null;

        }

        private void generateParticles(Vector3[] positions)
        {
            particles = new VertexPositionTexture[nBillboards * 4];
            indices = new int[nBillboards * 6];
            int x = 0;
            for (int i = 0; i < nBillboards * 4; i += 4)
            {
                Vector3 pos = positions[i / 4];
                // Add 4 vertices at the billboard's position
                particles[i + 0] = new VertexPositionTexture(pos, new Vector2(0, 0));
                particles[i + 1] = new VertexPositionTexture(pos, new Vector2(0, 1));
                particles[i + 2] = new VertexPositionTexture(pos, new Vector2(1, 1));
                particles[i + 3] = new VertexPositionTexture(pos, new Vector2(1, 0));
                // Add 6 indices to form two triangles
                indices[x++] = i + 0;
                indices[x++] = i + 3;
                indices[x++] = i + 2;
                indices[x++] = i + 2;
                indices[x++] = i + 1;
                indices[x++] = i + 0;
            }
            vBuffer = new VertexBuffer(gDevice, typeof(VertexPositionTexture), nBillboards * 4, BufferUsage.WriteOnly);
            vBuffer.SetData<VertexPositionTexture>(particles);

            iBufffer = new IndexBuffer(gDevice, IndexElementSize.ThirtyTwoBits, nBillboards * 6, BufferUsage.WriteOnly);
            iBufffer.SetData<int>(indices);
        }

        private void setEffectParameters(Matrix view, Matrix Projection, Vector3 up, Vector3 right)
        {
            eff.Parameters["ParticleTexture"].SetValue(tex2D);
            eff.Parameters["View"].SetValue(view);
            eff.Parameters["Projection"].SetValue(Projection);
            eff.Parameters["Size"].SetValue(BBsize / 2f);
            eff.Parameters["Up"].SetValue(up);
            eff.Parameters["Side"].SetValue(right);

            eff.CurrentTechnique.Passes[0].Apply();
        }
    }
}
