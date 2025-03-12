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

		public void SetResponse(ChattingResponse res)
		{
			response = res;
			Text = response.Text;
			FocusMode = FocusModeEnum.None;
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

		private void OnClick()
		{
			if (response == null) return;
			response.Execute();
		}
	}
}