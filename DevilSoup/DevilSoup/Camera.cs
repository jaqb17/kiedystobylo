using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilSoup
{

    public class Camera : Component
    {
        
        public Matrix world { get; set; }
        public Matrix view { get; set; }
        public Matrix projection { get; set; }
        public Vector3 target { get; } = new Vector3(0, 7.5f, 0);        
        private Matrix lastTransform = Matrix.Identity;

        public Vector3 Position { get; } = new Vector3(0, 110, 40);

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

        public override void UpdateComponent()
        {
            if (Parent.IsDirty)
            {
                RecalculateTransformation();
            }
        }

        private void RecalculateTransformation()
        {
            Matrix matrixOffset = Matrix.Invert(lastTransform) * Parent.Transform.LocalMatrix;
            view *= matrixOffset;
            lastTransform = Parent.Transform.LocalMatrix;
        }
    }
}
