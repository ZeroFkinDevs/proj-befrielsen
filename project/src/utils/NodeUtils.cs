using Godot;

namespace Game.Utils
{
    public static class NodeUtils
    {
        public static NodePath AdaptMultiplayerPath(this Node node, NodePath path)
        {
            if (node.Multiplayer.IsServer())
            {
                path = ((string)path).Replace("/Client/", "/SubViewport/Server/");
            }
            else
            {
                path = ((string)path).Replace("/SubViewport/Server/", "/Client/");
            }
            return path;
        }
        public static NodePath GetMultiplayerPath(this Node node)
        {
            var path = node.AdaptMultiplayerPath(node.GetPath());
            return path;
        }
        public static T GetMultiplayerNode<[MustBeVariant] T>(this Node thisNode, NodePath path) where T : Node
        {
            path = thisNode.AdaptMultiplayerPath(path);
            var node = thisNode.GetNode<T>(path);
            return node;
        }
    }
}