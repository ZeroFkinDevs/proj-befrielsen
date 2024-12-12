using System;
using Game.Utils;
using Godot;

namespace Game
{
    public interface ITool
    {
        ToolsManager toolsManager { get; set; }
    }
}