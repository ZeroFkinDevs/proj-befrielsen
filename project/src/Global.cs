using Godot;

namespace Game
{
	public partial class Global : Node
	{
		public static Global Instance { get { return _instance; } }
		private static Global _instance;

		[Export]
		public PackedScene LocationScene;

		public override void _EnterTree()
		{
			_instance = this;
		}
	}
}