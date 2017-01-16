using System;
using System.Collections.Generic;

namespace Acoross.Game2d
{
    public class GameObject
    {
        public Vector2d position { get; private set; }
        public Vector2d rotation { get; private set; }

        public GameObject(Vector2d position, Vector2d rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public void Move(Vector2d dir)
        {
            position += dir;
        }

        public void Rotate(float rad)
        {
            rotation = rotation.Rotate(rad);
        }

        public virtual void OnColliding(Collider collider)
        { }
    }

    public class PotentialContact
    {
        public Tuple<Collider, Collider> particle;
        public Vector2d contactNormal;
    }
    
    public class BoundingVolume
    {
        public Vector2d position;
        public Vector2d halfSize;
    }

    public class BVNode
    {
        public BVNode right;
        public BVNode left;
        BoundingVolume volume;
        Collider collider;

        bool isLeaf()
        {
            return (collider != null);
        }
    }
}
