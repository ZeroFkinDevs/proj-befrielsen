using System;
using Godot;
using Godot.Collections;

namespace Game
{
	// этот класс предоставляет функции для управления body в физичном мире, но сам ни о чем не думает. решений не предпринимает
	public partial class NpcMovementUnit : Node3D
	{
		[Export]
		public NpcCharacterController npc;
		[Export]
		public bool IsActive = true;

		public override void _Ready()
		{

		}

		public override void _Process(double delta)
		{

		}

		public override void _PhysicsProcess(double delta)
		{

		}
	}
}