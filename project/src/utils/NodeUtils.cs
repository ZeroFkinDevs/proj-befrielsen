using Godot;

namespace Game.Utils
{
    public static class NodeUtils
    {
        public static WorldContainer GetWorldContainer(this Node node)
        {
            return node.GetMultiplayerNode<WorldContainer>("/root/Main/Client/WorldContainer");
        }
        public static ResourcesManager GetResourcesManager(this Node node)
        {
            return node.GetWorldContainer().resourcesManager;
        }
        public static ResourcesBroadcaster GetResourcesBroadcaster(this Node node)
        {
            return node.GetResourcesManager().Broadcaster;
        }
        public static void PutChildrenInArray(this Node node, Godot.Collections.Array<Node> arr)
        {
            foreach (var child in node.GetChildren())
            {
                arr.Add(child);
                child.PutChildrenInArray(arr);
            }
        }
        public static Godot.Collections.Array<Node> GetChildrenRecursively(this Node node)
        {
            var arr = new Godot.Collections.Array<Node>();
            node.PutChildrenInArray(arr);
            return arr;
        }
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
        public static NodePath GetCommonMultiplayerPath(this Node node)
        {
            var path = node.GetPath();
            path = ((string)path).Replace("/Client/", "/");
            path = ((string)path).Replace("/SubViewport/Server/", "/");
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