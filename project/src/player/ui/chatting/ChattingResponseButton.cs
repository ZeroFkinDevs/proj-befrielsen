using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Game.Dialog;
using Godot;

namespace Game
{
	public partial class ChattingResponseButton : Button
	{
		private ChattingResponse response;
		private ChattingMember _member;

		public void SetResponse(ChattingResponse res)
		{
			response = res;
			Text = response.Text;
			AddThemeFontSizeOverride("font_size", 12);
			FocusMode = FocusModeEnum.None;
		}

		public void SetMember(ChattingMember member)
		{
			_member = member;
		}

		public override void _EnterTree()
		{
			base._EnterTree();
			Pressed += OnClick;
		}
		public override void _ExitTree()
		{
			base._ExitTree();
			Pressed -= OnClick;
		}
		public override void _Ready()
		{

		}

		private void OnClick()
		{
			if (response == null) return;
			_member.ExecuteResponse(response.Code);
		}
	}
}