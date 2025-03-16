using System;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class NpcMemory : Node3D
    {
        [Export]
        private Dictionary<string, bool> facts = new();

        public void SetFact(string key, bool value = true) => facts[key] = value;
        public bool GetFact(string key) => facts.TryGetValue(key, out var value) && value;
    }
}