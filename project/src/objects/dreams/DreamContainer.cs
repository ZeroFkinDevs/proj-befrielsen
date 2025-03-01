using Godot;
using Godot.Collections;

namespace Game
{
    public partial class DreamContainer : Node3D
    {
        [Export]
        public string DreamCode;

        [Export]
        public Array<NodePath> _adapters;

        public Array<DreamAdapter> Adapters
        {
            get
            {
                var adapters = new Array<DreamAdapter>(); foreach (var adptPath in _adapters)
                {
                    adapters.Add(GetNode<DreamAdapter>(adptPath));
                }
                return adapters;
            }
        }
        public Array<DreamAdapter> GetFreeAdapters()
        {
            Array<DreamAdapter> freeAdapters = new Array<DreamAdapter>();
            foreach (var item in Adapters)
            {
                if (item.IsFree) freeAdapters.Add(item);
            }
            return freeAdapters;
        }
    }
}