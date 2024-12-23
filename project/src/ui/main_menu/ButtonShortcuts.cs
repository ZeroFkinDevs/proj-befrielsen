using Godot;
using Godot.Collections;

namespace Game.UI
{
	public partial class ButtonShortcuts : Control
	{
		[Export]
		public Dictionary<string, NodePath> Bindings = new Dictionary<string, NodePath>();
		public override void _Input(InputEvent @event)
		{
			var eventKey = @event as InputEventKey;

			if (eventKey is not null)
			{
				if (eventKey.Pressed)
				{
					var code = eventKey.Keycode.ToString();
					if (Bindings.ContainsKey(code))
					{
						var button = GetNode<BaseButton>(Bindings[code]);
						if (button.IsVisibleInTree())
						{
							button.EmitSignal("pressed");
						}
					}
				}
			}

		}
	}
}