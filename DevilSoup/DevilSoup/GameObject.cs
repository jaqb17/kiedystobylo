using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace DevilSoup
{
    public class GameObject
    {
        private List<Component> components = new List<Component>();
        private List<GameObject> children = new List<GameObject>();

        public List<Component> Components
        {
            get => components;
            set => components = value;
        }

        public List<GameObject> Children
        {
            get => children;
            set => children = value;
        }
        public bool IsDirty { get; set; } = true;
        public GameObject Parent { get; set; }
        public Matrix WorldMatrix { get; set; } = Matrix.Identity;
        public Transform Transform { get; set; } = new Transform(Matrix.Identity);
        public string Tag { get; set; }


        public GameObject()
        {
            Transform.Parent = this;
        }

        public void Update(Matrix worldMatrix)
        {

            foreach (var component in Components)
            {
                component.UpdateComponent();
            }

            if (IsDirty)
            {
                WorldMatrix = Transform.LocalMatrix * worldMatrix;
                IsDirty = false;
            }

            foreach (var child in children)
            {
                child.Update(WorldMatrix);
            }
        }

        public void AddChild(GameObject child)
        {
            children.Add(child);
            child.Parent = this;
        }

        public void AddComponent(Component component)
        {           

            components.Add(component);
            component.Parent = this;
        }

        public void SetDirty()
        {
            IsDirty = true;
            foreach (var child in Children)
            {
                child.SetDirty();
            }
        }
        public T GetComponent<T>()
        {
            return Components.OfType<T>().FirstOrDefault();
        }
    }
}
