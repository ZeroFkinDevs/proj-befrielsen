using System.Security.Cryptography.X509Certificates;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class EyeHooker : Area3D, IToolInteractable
    {
        [Export]
        public EyeHookerAnimationController model;
        [Export]
        public ObjectInstantiator objectInstantiator;
        [Export]
        public NodeToBoneConnector nodeToBoneConnector;
        [Export]
        public PlayerDetectArea playerDetectArea;
        [Export]
        public DreamAdapter dreamAdapter;

        public Player currentPlayerInArea = null;
        [Export]
        public Prop currentEyeToolProp = null;

        public void DisableArea()
        {
            SetCollisionLayerValue(1, false);
        }
        public void EnableArea()
        {
            SetCollisionLayerValue(1, true);
        }

        public void TakeEye(Prop eyeProp)
        {
            currentEyeToolProp = eyeProp;
            nodeToBoneConnector.skeleton = model.skeleton3D;
            nodeToBoneConnector.node = eyeProp;
            eyeProp.GravityScale = 0.0f;
            model.Take();
            DisableArea();

            nodeToBoneConnector.Sync();
            if (eyeProp.ModelSmoothConnector.Object != null)
            {
                eyeProp.ModelSmoothConnector.NoSmooth = true;
                eyeProp.ModelSmoothConnector.Teleport();
            }

            if (Multiplayer.IsServer())
            {
                foreach (var stack in eyeProp.ItemsStorageRes.ItemsStacks)
                {
                    var eyeItem = (EyeToolItemResource)stack.ItemRes;
                    var dream = eyeItem.dreams.PickRandom();
                    dreamAdapter.ServerConnectToDream(dream);
                    break;
                }
            }
        }
        public void OnAdapterStateChange(bool state)
        {
            if (!state)
            {
                model.Take();
            }
            else
            {
                model.Idle();
            }
        }
        public void RemoveEye()
        {
            if (currentEyeToolProp == null) return;
            nodeToBoneConnector.skeleton = model.skeleton3D;
            nodeToBoneConnector.node = null;
            currentEyeToolProp = null;
            model.Idle();
            EnableArea();

            if (Multiplayer.IsServer())
            {
                dreamAdapter.ServerDisconnectFromDream();
            }
        }

        public override void _Ready()
        {
            playerDetectArea.PlayerEntered += OnPlayerEntered;
            if (dreamAdapter != null) dreamAdapter.OnFreeStateChange += OnAdapterStateChange;

            if (currentEyeToolProp != null)
            {
                nodeToBoneConnector.skeleton = model.skeleton3D;
                nodeToBoneConnector.node = currentEyeToolProp;
                currentEyeToolProp.GravityScale = 0.0f;
                model.Take();
            }
        }

        public void OnPlayerEntered(Player player)
        {
            model.Wake();
        }

        public override void _Process(double delta)
        {
            if (currentEyeToolProp != null)
            {
                if (!IsInstanceValid(currentEyeToolProp)) RemoveEye();
            }

            UpdateLook();
        }

        public void UpdateLook()
        {
            Vector3 lookAtTarget = GlobalTransform.Origin - (GlobalTransform.Basis.Z * 100.0f);
            if (playerDetectArea.PlayersCount > 0)
            {
                foreach (var node in playerDetectArea.GetOverlappingBodies())
                {
                    if (node is Player player)
                    {
                        var newTarget = player.Camera.GlobalPosition;
                        if (ToLocal(newTarget).Z < -0.5f)
                        {
                            if (newTarget.DistanceTo(GlobalPosition) < lookAtTarget.DistanceTo(GlobalPosition))
                            {
                                lookAtTarget = newTarget;
                            }
                        }
                    }
                }
            }

            model.LookingTarget = lookAtTarget;
        }

        public InteractionTypeEnum GetInteractionType(ITool tool, ItemStack toolStack)
        {
            if (currentEyeToolProp != null) return InteractionTypeEnum.NONE;
            if (!(tool is EyeTool)) return InteractionTypeEnum.NONE;
            return InteractionTypeEnum.TOUCH;
        }

        public string GetInteractableDescription(ITool tool, ItemStack toolStack)
        {
            if (currentEyeToolProp != null) return "";
            if (!(tool is EyeTool)) return "";
            return "give eye";
        }

        public void Interact(ITool tool, ItemStack toolStack)
        {
            var currentToolItem = toolStack.ItemRes;
            if (currentToolItem != null)
            {
                if (currentToolItem is EyeToolItemResource eyeTool)
                {
                    tool.toolsManager.inventoryContainer.ConsumeItem(eyeTool, 1);
                    objectInstantiator.RequestInstantiateProp(tool.toolsManager.player.tmpStorage, currentToolItem, this, MethodName.TakeEye);
                }
            }
        }
    }
}