using System;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class DreamAdapter : Node3D
    {
        public LocationLoader locationLoader { get { return this.GetMultiplayerNode<LocationLoader>("/root/Main/Client/LocationLoader"); } }
        [Export]
        public Portal portal;
        [Export]
        public DreamAdapterDoorModelController doorModel;
        [Export]
        public DreamAdapter OtherAdapter;
        public bool IsFree { get { return OtherAdapter == null; } }
        public event Action<bool> OnFreeStateChange;

        public override void _Ready()
        {
            Owner = locationLoader.LocationInstance;
            SetEditableInstance(this, true);

            if (OtherAdapter != null)
            {
                ConnectToAdapter(OtherAdapter);
            }
        }

        public void ServerConnectToDream(PackedScene dreamScene)
        {
            if (locationLoader.LocationInstance != null)
            {
                locationLoader.LocationInstance.dreamsManager.ServerGetOrCreateDream(dreamScene, this, MethodName.RecieveDreamContainer);
            }
        }

        public void ServerDisconnectFromDream()
        {
            Rpc(MethodName.RecieveDisconnectFromDream);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveDisconnectFromDream()
        {
            if (OtherAdapter != null) OtherAdapter.DisconnectFromAdapter();
            DisconnectFromAdapter();
        }

        public async void RecieveDreamContainer(DreamContainer dreamContainer)
        {
            var freeAdapters = dreamContainer.GetFreeAdapters();

            if (freeAdapters.Count > 0)
            {
                var adapter = await dreamContainer.GetRandomFreeAdapter();
                portal.SetCullMask(3);
                adapter.portal.SetCullMask(4);
                ConnectToAdapter(adapter);
                adapter.ConnectToAdapter(this);
            }
        }
        public void ConnectToAdapter(DreamAdapter adapter)
        {
            OtherAdapter = adapter;
            portal.OtherPortal = OtherAdapter.portal;
            portal.Enable();
            doorModel.Open();
            OnFreeStateChange?.Invoke(IsFree);
        }
        public void DisconnectFromAdapter()
        {
            if (IsFree) return;
            portal.OtherPortal = null;
            portal.Disable();
            OtherAdapter = null;
            doorModel.Close();
            OnFreeStateChange?.Invoke(IsFree);
        }
    }
}