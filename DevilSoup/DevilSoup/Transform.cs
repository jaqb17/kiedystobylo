using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DevilSoup
{
    public class Transform
    {
        private Matrix localMatrix = Matrix.Identity;
        private Vector3 position;
        private Vector3 rotation;
        private Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

        public GameObject Parent { get; set; }

        public Vector3 Position
        {
            get => position;
            set
            {
                position = value;
                UpdateLocalMatrix();
            }
        }

        public Vector3 Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                UpdateLocalMatrix();
            }
        }

        public Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                UpdateLocalMatrix();
            }
        }

        public Matrix LocalMatrix
        {
            get => localMatrix;
            set => localMatrix = value;
        }

        public Transform(Matrix localMatrix)
        {
            this.localMatrix = localMatrix;
        }

        public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            UpdateLocalMatrix();
        }

        private void UpdateLocalMatrix()
        {
            localMatrix = Matrix.CreateTranslation(position) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X)) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y)) *
                          Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z)) *
                          Matrix.CreateScale(scale);

            Parent.SetDirty();
        }
    }
}
