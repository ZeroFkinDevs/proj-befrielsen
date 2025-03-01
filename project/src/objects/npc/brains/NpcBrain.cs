using Godot;
using Godot.Collections;

namespace Game
{
	public partial class NpcBrain : Node3D
	{
		public NpcCharacterController _npcController;
		public NpcCharacterController npc
		{
			get { return _npcController; }
			set
			{
				if (value == null) BeforeDeactivate();
				_npcController = value;
				if (value != null) OnActivate();
			}
		}
		public bool IsActive { get { return npc != null; } }

		public virtual void BeforeDeactivate()
		{

		}

		public virtual void OnActivate()
		{

		}

		public override void _Ready()
		{

		}

		public override void _Process(double delta)
		{

		}
	}
}