using System;
using Godot;

namespace Game
{
    public partial class InventoryItemStackRenderer : Area3D, IInteractable
    {
        public ItemStack itemStack;
        public Node3D itemInstance;
        public InventoryItemsRenderer itemsRenderer;
        public bool isDragging = false;
        public float cameraDistance = 0.0f;
        public IInventoryPlaceRegion CurrentRegion;

        public InteractionTypeEnum InteractionType
        {
            get
            {
                if (itemsRenderer == null) return InteractionTypeEnum.NONE;
                if (isDragging) return InteractionTypeEnum.NONE;
                if (itemsRenderer.inventoryManager.player.Controllable) return InteractionTypeEnum.INVENTORY_DRAG;
                return InteractionTypeEnum.NONE;
            }
        }

        public void Interact(IUser user)
        {
            cameraDistance = itemsRenderer.inventoryManager.player.Camera.GlobalPosition.DistanceTo(GlobalPosition) - 0.05f;
            CallDeferred(MethodName.CheckRegions);
            isDragging = true;
        }

        public override void _Process(double delta)
        {
            if (isDragging)
            {
                var camera = itemsRenderer.inventoryManager.player.Camera;
                var pos = itemsRenderer.inventoryManager.player.Camera.ProjectPosition(GetViewport().GetMousePosition(), cameraDistance);
                GlobalPosition = pos;

                if (CurrentRegion != null)
                {
                    if (Input.IsActionJustPressed("fire"))
                    {
                        CurrentRegion.Interact(this);
                    }
                }
            }
            else
            {
                CurrentRegion = null;
            }
        }

        public void Setup(ItemStack stack, InventoryItemsRenderer renderer)
        {
            itemStack = stack;
            itemsRenderer = renderer;

            itemInstance = itemStack.ItemRes.MeshScene.Instantiate<Node3D>();
            AddChild(itemInstance);
            if (itemInstance.GetChild<Node3D>(0) is VisualInstance3D visInstance)
            {
                var aabb = visInstance.GetAabb();
                var size = aabb.Size;
                var scale = 0.1f / size[(int)size.MaxAxisIndex()];
                itemInstance.Scale = Vector3.One * scale;
            }

            AreaEntered += OnAreaEntered;
            AreaExited += OnAreaExited;
        }

        public void CheckRegions()
        {
            if (!isDragging) return;
            foreach (var area in GetOverlappingAreas())
            {
                OnAreaEntered(area);
                return;
            }
        }
        public void OnAreaEntered(Area3D area)
        {
            if (!isDragging) return;
            if (area is IInventoryPlaceRegion region)
            {
                if (CurrentRegion != null) CurrentRegion.OnExit();
                region.OnEnter(this);
                CurrentRegion = region;
            }
        }
        public void OnAreaExited(Area3D area)
        {
            if (area is IInventoryPlaceRegion region)
            {
                region.OnExit();
                CurrentRegion = null;
            }
        }
    }
}