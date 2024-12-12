using System;
using System.Net.Http;
using Godot;

namespace Game
{
	public partial class ToolsManager : Node3D
	{
		[Export]
		public InventoryContainer inventoryContainer;

		[Export]
		public string BoneName;
		private int boneId;

		[Export]
		public int CurrentItemIndex = 0;

		public ToolItemResource CurrentToolItem;

		public Node3D CurrentTool;

		private ItemsStorage storage { get { return inventoryContainer.storage; } }
		public Player player { get { return inventoryContainer.inventoryManager.player; } }

		public override void _Ready()
		{
			boneId = player.model.skeleton3D.FindBone(BoneName);

			// inventoryContainer.storage.OnUpdate += SetupCurrentItem;
			SetupCurrentItem();
		}

		public override void _Process(double delta)
		{
			GlobalTransform = player.model.GetBoneGlobalPose(boneId).RotatedLocal(Vector3.Up, -Mathf.Pi / 2.0f);
		}

		public void SetupCurrentItem()
		{
			SetCurrentItemIdx(CurrentItemIndex);
		}

		public void SetCurrentItemIdx(int itemIdx)
		{
			itemIdx = Math.Min(itemIdx, storage.ItemsStacks.Count - 1);
			itemIdx = Math.Max(itemIdx, 0);
			CurrentItemIndex = itemIdx;

			if (storage.ItemsStacks.Count == 0)
			{
				SetTool(null);
				return;
			}

			var itemRes = storage.ItemsStacks[CurrentItemIndex].ItemRes;
			if (itemRes is ToolItemResource toolItemRes)
			{
				SetTool(toolItemRes);
				return;
			}
		}

		public void SetTool(ToolItemResource toolItem)
		{
			if (CurrentToolItem == toolItem) return;

			if (CurrentTool != null)
			{
				CurrentTool.QueueFree();
				CurrentTool = null;
			}

			if (toolItem != null)
			{
				CurrentToolItem = toolItem;
				var newTool = toolItem.ToolScene.Instantiate<Node3D>();
				if (newTool is ITool tool)
				{
					tool.toolsManager = this;
				}
				AddChild(newTool);
				CurrentTool = newTool;
			}

			CurrentToolItem = toolItem;
		}
	}
}