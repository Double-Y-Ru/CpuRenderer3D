using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DoubleY.CpuRenderer3D
{
    public class SceneNode
    {
        private Transform _localTransform;

        private SceneNode? _parent;
        private readonly List<SceneNode> _children;

        private readonly IRenderer[] _renderers;

        public SceneNode? Parent => _parent;

        public Transform LocalTransform { get => _localTransform; set => _localTransform = value; }

        public Transform GlobalTransform
        {
            get
            {
                if (Parent == null) return LocalTransform;

                return new Transform(
                    origin: Vector3.Transform(LocalTransform.Origin, Parent.GlobalTransform.GetMatrix()),
                    rotation: LocalTransform.Rotation * Parent.GlobalTransform.Rotation);
            }
        }

        public SceneNode()
        {
            _parent = null;
            _children = new List<SceneNode>();
            _localTransform = new Transform();
            _renderers = new IRenderer[0];
        }

        public SceneNode(Transform localTransform, IEnumerable<IRenderer> renderers)
        {
            _parent = null;
            _children = new List<SceneNode>();
            _localTransform = localTransform;
            _renderers = renderers.ToArray();
        }

        public void AddChild(SceneNode child)
        {
            if (child.Parent != null)
                throw new NotSupportedException("Child already has its parent. Reparenting is not supported");

            child.SetParent(this);
            _children.Add(child);
        }

        private void SetParent(SceneNode newParent)
        {
            _parent = newParent;
        }

        public IEnumerable<SceneNode> GetChildren() => _children;
        public IEnumerable<IRenderer> GetRenderers() => _renderers;
    }
}
