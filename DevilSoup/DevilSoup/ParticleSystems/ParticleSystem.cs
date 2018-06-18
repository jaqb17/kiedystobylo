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
    class ParticleSystem
    {
        //Vertex Index
        VertexBuffer verts;
        IndexBuffer ints;

        GraphicsDevice gDevice;
        Effect eff;

        int particlesNumber;
        Vector2 particleSize;
        double lifespan = 1;
        Vector3 wind;
        Texture2D texture;
        double fadeInTime;

        ParticleVertex[] particles;
        int[] indices;

        int activeStart = 0, nActive = 0;

        //Time of creation
        DateTime start;

        public ParticleSystem(GraphicsDevice _graphicsDevice, ContentManager _content, Texture2D _texture, int _particlesNumber, Vector2 _particleSize, double _lifespan, Vector3 _wind, double _fadeInTime)
        {
            this.particlesNumber = _particlesNumber;
            this.particleSize = _particleSize;
            this.gDevice = _graphicsDevice;
            this.lifespan = _lifespan;
            this.wind = _wind;
            this.texture = _texture;
            this.fadeInTime = _fadeInTime;

            verts = new VertexBuffer(gDevice, typeof(ParticleVertex), particlesNumber * 4, BufferUsage.WriteOnly);
            ints = new IndexBuffer(gDevice, IndexElementSize.ThirtyTwoBits, particlesNumber * 6, BufferUsage.WriteOnly);
            generateParticles();

            eff = _content.Load<Effect>("Assets\\Effects\\ParticlesEffect");

            start = DateTime.Now;
        }

        private void generateParticles()
        {
            particles = new ParticleVertex[particlesNumber * 4];
            indices = new int[particlesNumber * 6];
            Vector3 z = Vector3.Zero;

            int x = 0;
            for (int i = 0; i < particlesNumber * 4; i += 4)
            {
                particles[i + 0] = new ParticleVertex(z, new Vector2(0, 0), z, 0, -1);
                particles[i + 1] = new ParticleVertex(z, new Vector2(0, 1), z, 0, -1);
                particles[i + 2] = new ParticleVertex(z, new Vector2(1, 1), z, 0, -1);
                particles[i + 3] = new ParticleVertex(z, new Vector2(1, 0), z, 0, -1);

                indices[x++] = i + 0;
                indices[x++] = i + 3;
                indices[x++] = i + 2;
                indices[x++] = i + 2;
                indices[x++] = i + 1;
                indices[x++] = i + 0;

            }
        }

        public void AddParticle(Vector3 _position, Vector3 _direction, double _speed)
        {
            if (particlesNumber + 4 == particlesNumber * 4)
                return;
            int index = offsetIndex(activeStart, nActive);
            nActive += 4;

            double startTime = (double)(DateTime.Now - start).TotalSeconds;
            for(int i =0;i<4;i++)
            {
                particles[index + i].StartPosition = _position;
                particles[index + i].Speed = _speed;
                particles[index + i].Direction = _direction;
                particles[index + i].StartTime = startTime;
            }
        }


        private int offsetIndex(int start, int count)
        {
            for(int i=0;i<count; i++)
            {
                start++;
                if (start == particles.Length)
                    start = 0;
            }
            return start;
        }

        public void Update()
        {
            double now = (double)(DateTime.Now - start).TotalSeconds;
            int startIndex = activeStart;
            int end = nActive;

            for(int i=0;i<end; i++)
            {
                if (particles[activeStart].StartTime < now - lifespan)
                {
                    activeStart++;
                    nActive--;
                    if (activeStart == particles.Length)
                        activeStart = 0;
                }
            }

            verts.SetData<ParticleVertex>(particles);
            ints.SetData<int>(indices);
        }

        public void Draw(Matrix _view, Matrix _projection, Vector3 _Up, Vector3 _Right)
        {
            gDevice.SetVertexBuffer(verts);
            gDevice.Indices = ints;

            eff.Parameters["ParticleTexture"].SetValue(texture);
            eff.Parameters["View"].SetValue(_view);
            eff.Parameters["Time"].SetValue((float)(DateTime.Now - start).TotalSeconds);
            eff.Parameters["Lifespan"].SetValue((float)lifespan);
            eff.Parameters["Wind"].SetValue(wind);
            eff.Parameters["Size"].SetValue(particleSize / 2f);
            eff.Parameters["Up"].SetValue(_Up);
            eff.Parameters["Side"].SetValue(_Right);
            eff.Parameters["FadeInTime"].SetValue((float)fadeInTime);

            gDevice.BlendState = BlendState.AlphaBlend;
            gDevice.DepthStencilState = DepthStencilState.DepthRead;

            eff.CurrentTechnique.Passes[0].Apply();
            //Drawing
            gDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, particlesNumber * 4, 0, particlesNumber * 2);
            //unsetting buffers
            gDevice.SetVertexBuffer(null);
            gDevice.Indices = null;

            gDevice.BlendState = BlendState.Opaque;
            gDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}
