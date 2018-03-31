using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{

    public class Camera
    {
        public Matrix world { get; set; }
        public Matrix view { get; set; }
        public Matrix projection { get; set; }

        public Camera() { }

        public Camera(Matrix world, Matrix view, Matrix projection)
        {
            this.world = world;
            this.view = view;
            this.projection = projection;
        }

        public void setWorldMatrix(Vector3 vector)
        {
            world = Matrix.CreateTranslation(vector);
        }
    }
}
