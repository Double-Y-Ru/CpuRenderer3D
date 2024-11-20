using System.Numerics;

namespace CpuRenderer3D
{
    public class SceneNode
    {
        private Transform _localTransform;

        private readonly SceneNode? _parent;
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

        public SceneNode(Transform localTransform, SceneNode parent, IEnumerable<IRenderer> renderers)
        {
            _parent = parent;
            _children = new List<SceneNode>();
            _localTransform = localTransform;
            _renderers = renderers.ToArray();
            _parent.AddChild(this);
        }

        private void AddChild(SceneNode child)
        {
            _children.Add(child);
        }

        public IEnumerable<SceneNode> GetChildren() => _children;
        public IEnumerable<IRenderer> GetRenderers() => _renderers;
    }
}
