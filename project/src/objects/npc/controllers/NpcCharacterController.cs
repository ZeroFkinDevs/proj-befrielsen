using System;
using Godot;
using Godot.Collections;

namespace Game
{
	// это класс аггрегатор, собирает компоненты, перенаправляет взаимодействия
	// это самостоятельный законченный класс который не нужно расширять и наследовать. он лишь связывает все компоненты воедино
	public partial class NpcCharacterController : CharacterBody3D, ILiving, IInteractable, IToolInteractable, ITeleportableTransform, ITeleportablePoint
	{
		// living state
		[Export]
		public LivingStateManager _livingStateManager;
		public LivingStateManager livingStateManager => _livingStateManager;

		// brain
		#region Brain
		[Export]
		public NpcBrain _currentBrain;
		public NpcBrain CurrentBrain
		{
			get { return _currentBrain; }
		}
		public void SetBrain(NpcBrain brain)
		{
			if (_currentBrain != null)
			{
				_currentBrain.npc = null;
			}
			_currentBrain = brain;
			if (_currentBrain != null)
			{
				_currentBrain.npc = this;
			}
		}
		#endregion

		// movement
		#region Movement
		[Export]
		public NpcMovementUnit _movementUnit;
		public NpcMovementUnit MovementUnit
		{
			get { return _movementUnit; }
		}
		public T GetMovementUnit<T>() where T : NpcMovementUnit
		{
			return MovementUnit as T;
		}
		#endregion

		// characterModel
		#region CharacterModel
		[Export]
		public NpcCharacterModel _characterModel;
		public NpcCharacterModel CharacterModel
		{
			get { return _characterModel; }
		}
		#endregion

		// IInteractable
		#region IInteractable
		public InteractionTypeEnum InteractionType
		{
			get
			{
				var value = InteractionTypeEnum.NONE;
				if (_currentBrain != null && _currentBrain is IInteractable interactable) value = interactable.InteractionType;
				return value;
			}
		}

		public string InteractableDescription
		{
			get
			{
				var value = "";
				if (_currentBrain != null && _currentBrain is IInteractable interactable) value = interactable.InteractableDescription;
				return value;
			}
		}

		public void Interact(IUser user)
		{
			if (_currentBrain == null) return;
			if (_currentBrain is IInteractable interactable) interactable.Interact(user);
			return;
		}
		#endregion

		// IToolInteractable
		#region IToolInteractable
		public InteractionTypeEnum GetInteractionType(ITool tool, ItemStack toolStack)
		{
			var value = InteractionTypeEnum.NONE;
			if (_currentBrain != null && _currentBrain is IToolInteractable toolInteractable)
				value = toolInteractable.GetInteractionType(tool, toolStack);
			return value;
		}

		public string GetInteractableDescription(ITool tool, ItemStack toolStack)
		{
			var value = "";
			if (_currentBrain != null && _currentBrain is IToolInteractable toolInteractable) value = toolInteractable.GetInteractableDescription(tool, toolStack);
			return value;
		}

		public void Interact(ITool tool, ItemStack toolStack)
		{
			if (_currentBrain == null) return;
			if (_currentBrain is IToolInteractable toolInteractable) toolInteractable.Interact(tool, toolStack);
			return;
		}
		#endregion

		// ITeleportableTransform
		#region ITeleportableTransform
		public void RecieveTransformTeleportation(Func<Transform3D, Transform3D> teleportTransform)
		{
			if (_movementUnit == null) return;
			if (_movementUnit is ITeleportableTransform teleportable) teleportable.RecieveTransformTeleportation(teleportTransform);
			return;
		}
		#endregion

		// ITeleportablePoint
		#region ITeleportablePoint
		public void RecievePointTeleportation(Func<Vector3, Vector3> teleportPoint)
		{
			if (_movementUnit == null) return;
			if (_movementUnit is ITeleportablePoint teleportable) teleportable.RecievePointTeleportation(teleportPoint);
			return;
		}
		#endregion

		// node logic
		public override void _Ready()
		{
			if (_currentBrain != null) SetBrain(_currentBrain);
		}
	}
}