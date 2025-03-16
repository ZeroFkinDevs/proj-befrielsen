using System.Threading.Tasks;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class DreamContainer : Node3D
    {
        [Export]
        public string DreamCode;
        [Export]
        public bool ReuseAdapters = false;

        [Export]
        public Array<NodePath> _adapters;

        private RandomGeneratorNode randomGenerator;

        public override void _EnterTree()
        {
            randomGenerator = new RandomGeneratorNode();
            randomGenerator.Name = "RandomGenerator";
            AddChild(randomGenerator);
        }

        public Array<DreamAdapter> Adapters
        {
            get
            {
                var adapters = new Array<DreamAdapter>();
                foreach (var adptPath in _adapters)
                {
                    adapters.Add(GetNode<DreamAdapter>(adptPath));
                }
                return adapters;
            }
        }
        public Array<DreamAdapter> GetFreeAdapters()
        {
            if (ReuseAdapters) return Adapters;

            Array<DreamAdapter> freeAdapters = new Array<DreamAdapter>();
            foreach (var item in Adapters)
            {
                if (item.IsFree) freeAdapters.Add(item);
            }
            return freeAdapters;
        }

        public async Task<DreamAdapter> GetRandomFreeAdapter()
        {
            var freeAdapters = GetFreeAdapters();
            if (freeAdapters.Count == 0) return null;

            var randIdx = await randomGenerator.FetchRandomIntInRange(this.GetCommonMultiplayerPath(), 0, freeAdapters.Count - 1);
            var adapter = freeAdapters[randIdx];
            return adapter;
        }
    }
}