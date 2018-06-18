using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevilSoup
{
    public struct ParticleVertex : IVertexType
    {
        Vector3 startPosition;
        Vector2 uv;
        Vector3 direction;

        double speed;
        double startTime;

        public Vector3 StartPosition
        {
            get { return startPosition; }
            set { startPosition = value; }
        }

        public Vector2 UV
        {
            get { return uv; }
            set { uv = value; }
        }

        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public double Speed
        {
            get { return speed; }
            set { speed = value; }

        }

        public double StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public ParticleVertex(Vector3 StartPosition, Vector2 UV, Vector3 Direction, double Speed, double StartTime)
        {
            this.startPosition = StartPosition;
            this.uv = UV;
            this.direction = Direction;
            this.speed = Speed;
            this.startTime = StartTime;
        }

        //Vertex Declaration
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),              //Starting Position
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),    //UV Coordinates
            new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1),    //Movement Dir
            new VertexElement(32, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),     //Movement Spd
            new VertexElement(36, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3));    //Start Time

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

    }
}
