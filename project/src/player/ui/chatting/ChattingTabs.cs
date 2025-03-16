using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Game.Dialog;
using Game.Utils;
using Godot;

namespace Game
{
	public partial class ChattingTabs : Control
	{
		[Export]
		public PlayerUI playerUI;

		public PlayerChattingMember chattingMember => playerUI.player.chattingMember;
		public ChattingResponsesCollection responsesCollection => playerUI.player.chattingMember.ResponsesCollection;

		public override void _Ready()
		{
			responsesCollection.OnUpdate += Render;
		}

		public override void _Process(double delta)
		{
			if (playerUI.player.ControlGroup == Player.ControlGroupEnum.HANDS)
			{
				Visible = true;
			}
			else
			{
				Visible = false;
			}
		}

		private void Render()
		{
			RemoveNotExistingTabs();
			foreach (var member in chattingMember.ChattingMembers.GetList())
			{
				GD.Print(member.Code);
				UpdateTabForMember(member);
			}
		}

		private void UpdateTabForMember(IChattingMember member)
		{
			var tab = GetOrCreateTab(member.Name);
			var responses = responsesCollection.GetForMember(member);
			var list = tab.GetNode<Control>("list");
			foreach (var child in list.GetChildren())
			{
				child.QueueFree();
			}
			if (responses != null)
			{
				foreach (var res in responses)
				{
					var button = new ChattingResponseButton();
					button.SetResponse(res);
					button.SetMember(chattingMember);
					list.AddChild(button);
				}
			}
		}

		private Control GetOrCreateTab(string name)
		{
			foreach (var child in GetChildren())
			{
				if (child.Name == name && child is Control control) return control;
			}
			var newTab = new MarginContainer();
			newTab.AddThemeConstantOverride("margin_left", 10);
			newTab.AddThemeConstantOverride("margin_top", 10);
			newTab.AddThemeConstantOverride("margin_right", 10);
			newTab.AddThemeConstantOverride("margin_bottom", 10);
			newTab.Name = name;
			AddChild(newTab);

			var list = new VBoxContainer();
			list.AddThemeConstantOverride("separation", 5);
			list.Name = "list";
			newTab.AddChild(list);

			return newTab;
		}

		private void RemoveNotExistingTabs()
		{
			var names = new List<string>(chattingMember.ChattingMembers.GetNames());
			foreach (var child in GetChildren())
			{
				if (!names.Contains(child.Name))
				{
					child.QueueFree();
				}
			}
		}
	}
}