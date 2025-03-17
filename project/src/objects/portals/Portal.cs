using Godot;
using Godot.Collections;
using Game.Utils;
using System.Diagnostics;

namespace Game
{
    public partial class Portal : Area3D
    {
        [Export]
        public PortalCamera Camera;

        [Export]
        public Portal OtherPortal;
        [Export]
        public Area3D PlayerPuppetArea;

        [Export]
        public MeshInstance3D Plane;
        [Export]
        public SubViewport Viewport;
        [Export]
        public int CullLayer = 2;

        Dictionary<string, Player> playerPuppets = new Dictionary<string, Player>();

        public override void _Ready()
        {
            Plane.SetLayerMaskValue(1, false);
            Plane.SetLayerMaskValue(CullLayer, true);
            Disable();

            BodyEntered += OnBodyEntered;
            BodyExited += OnBodyExited;
            PlayerPuppetArea.BodyEntered += OnPlayerEntered;
            PlayerPuppetArea.BodyExited += OnPlayerExited;
        }

        public void SetCullMask(int newMask)
        {
            Plane.SetLayerMaskValue(CullLayer, false);
            CullLayer = newMask;
            Plane.SetLayerMaskValue(CullLayer, true);
        }

        public override void _Process(double delta)
        {
            DoUpdates();
        }
        public override void _PhysicsProcess(double delta)
        {
            DoUpdates();
        }

        public void DoUpdates()
        {
            if (OtherPortal == null) return;
            if (!Visible) return;

            UpdateCameraPosition();
            UpdateBodies();
            UpdatePlayerPuppets();
        }

        public void Enable()
        {
            Visible = true;
        }
        public void Disable()
        {
            Visible = false;
        }

        private void UpdatePortalCameraNearClipPlane(Camera3D camera)
        {
            if (!camera.HasMethod("set_override_projection"))
                return; // Требуется ветка https://github.com/V-Sekai/godot/tree/override_projection_4.2

            const float NEAR_CLIP_OFFSET = 0.05f;
            const float NEAR_CLIP_LIMIT = 0.1f;

            // Вычисляем ближнюю плоскость отсечения в пространстве камеры
            Transform3D clipPlane = OtherPortal.GlobalTransform;
            Vector3 clipPlaneForward = -clipPlane.Basis.Z;
            float portalSide = NonZeroSign(clipPlaneForward.Dot(OtherPortal.GlobalTransform.Origin - camera.GlobalTransform.Origin));

            Transform3D camTransform = camera.GetCameraTransform().AffineInverse();
            Vector3 camSpacePos = camTransform * clipPlane.Origin;
            Vector3 camSpaceNormal = (camTransform.Basis * clipPlaneForward) * portalSide;
            float camSpaceDst = -camSpacePos.Dot(camSpaceNormal) + NEAR_CLIP_OFFSET;

            // Обликовая плоскость при близком расстоянии вызывает глитчи/артефакты, поэтому включаем её только на небольшом расстоянии
            if (Mathf.Abs(camSpaceDst) > NEAR_CLIP_LIMIT)
            {
                Projection proj = camera.GetCameraProjection();
                Plane nearClipPlane = new Plane(camSpaceNormal, camSpaceDst);
                proj = SetProjectionObliqueNearPlane(proj, nearClipPlane);
                camera.Call("set_override_projection", proj);
            }
            else
            {
                // Возвращаем стандартную проекцию, если камера слишком близко к порталу
                camera.Call("set_override_projection", new Projection(Vector4.Zero, Vector4.Zero, Vector4.Zero, Vector4.Zero));
            }
        }

        private float NonZeroSign(float value)
        {
            float s = Mathf.Sign(value);
            return s == 0 ? 1 : s;
        }

        private Projection SetProjectionObliqueNearPlane(Projection matrix, Plane clipPlane)
        {
            // Основано на статье:
            // Lengyel, Eric. “Oblique View Frustum Depth Projection and Clipping”.
            // Journal of Game Development, Vol. 1, No. 2 (2005), Charles River Media, pp. 5–16.

            // Вычисляем точку в пространстве клиппинга, противоположную плоскости отсечения,
            // как (sgn(clipPlane.x), sgn(clipPlane.y), 1, 1) и
            // преобразуем её в пространство камеры, умножив на обратную матрицу проекции.
            Vector4 q = new Vector4(
                (Mathf.Sign(clipPlane.Normal.X) + matrix.Z.X) / matrix.X.X,
                (Mathf.Sign(clipPlane.Normal.Y) + matrix.Z.Y) / matrix.Y.Y,
                -1.0f,
                (1.0f + matrix.Z.Z) / matrix.W.Z
            );

            Vector4 clipPlane4 = new Vector4(clipPlane.Normal.X, clipPlane.Normal.Y, clipPlane.Normal.Z, clipPlane.D);

            // Вычисляем масштабированный вектор плоскости
            Vector4 c = clipPlane4 * (2.0f / clipPlane4.Dot(q));

            // Заменяем третью строку матрицы проекции
            matrix.X = new Vector4(matrix.X.X, matrix.X.Y, c.X - matrix.X.W, matrix.X.W);
            matrix.Y = new Vector4(matrix.Y.X, matrix.Y.Y, c.Y - matrix.Y.W, matrix.Y.W);
            matrix.Z = new Vector4(matrix.Z.X, matrix.Z.Y, c.Z - matrix.Z.W, matrix.Z.W);
            matrix.W = new Vector4(matrix.W.X, matrix.W.Y, c.W - matrix.W.W, matrix.W.W);

            return matrix;
        }

        public void UpdateCameraPosition()
        {
            var curViewport = GetViewport();
            var curCamera = GetViewport().GetCamera3D();
            if (curCamera == null) return;
            if (OtherPortal == null) return;

            var movedToOtherPortal = TeleportTransform(curCamera.GlobalTransform);

            Camera.GlobalTransform = movedToOtherPortal;
            Camera.Fov = curCamera.Fov;
            Camera.Far = curCamera.Far;

            Camera.CullMask = curCamera.CullMask;
            Camera.SetCullMaskValue(OtherPortal.CullLayer, false);
            Camera.SetCullMaskValue(CullLayer, false);
            Camera.SetCullMaskValue(11, false); // player puppets

            Viewport.Size = new Vector2I((int)curViewport.GetVisibleRect().Size.X / 2, (int)curViewport.GetVisibleRect().Size.Y / 2);
            Viewport.Msaa3D = curViewport.Msaa3D;
            Viewport.ScreenSpaceAA = curViewport.ScreenSpaceAA;
            Viewport.UseTaa = curViewport.UseTaa;
            Viewport.UseDebanding = curViewport.UseDebanding;
            Viewport.UseOcclusionCulling = curViewport.UseOcclusionCulling;
            Viewport.MeshLodThreshold = curViewport.MeshLodThreshold;

            UpdatePortalCameraNearClipPlane(Camera);
        }

        public void UpdateBodies()
        {
            foreach (var body in GetOverlappingBodies())
            {
                if (body.HasMeta("dont_teleport"))
                {
                    continue;
                }

                if (body is StaticBody3D) continue;

                var localPos = ToLocal(body.GlobalPosition);
                var posOffset = Vector3.Zero;

                if (body is IProvidingTeleportPoint providingTeleportPoint)
                {
                    var point = providingTeleportPoint.GetTeleportPoint();
                    localPos = ToLocal(point);
                    posOffset = body.ToLocal(point);
                }

                if (body.HasMeta("portal_pos"))
                {
                    if (body is Player pl)
                    {
                        OtherPortal.Enable();
                    }

                    var prevPos = (Vector3)body.GetMeta("portal_pos");
                    if ((prevPos.Z < 0.0f && localPos.Z >= 0.0f) && (localPos - prevPos).Length() < 1.0f)
                    {
                        body.RemoveMeta("portal_pos");
                        var movedToOtherPortal = TeleportTransform(body.GlobalTransform);

                        body.GlobalTransform = movedToOtherPortal;

                        if (body is RigidBody3D rigid)
                        {
                            rigid.LinearVelocity = TeleportPoint(rigid.LinearVelocity);
                        }
                        if (body is CharacterBody3D character)
                        {
                            character.Velocity = TeleportPoint(character.Velocity);
                        }
                        if (body is Prop prop)
                        {
                            if (prop.grabber != null)
                            {
                                if (prop.grabber.player.HasMeta("is_puppet"))
                                {
                                    foreach (var path in OtherPortal.playerPuppets.Keys)
                                    {
                                        var puppet = OtherPortal.playerPuppets[path];
                                        if (puppet == prop.grabber.player)
                                        {
                                            var plyr = GetNode<Player>(path);
                                            RegrabProp(prop, plyr.grabber, true);
                                        }
                                    }
                                }
                                else
                                {
                                    if (playerPuppets.ContainsKey(prop.grabber.player.GetPath()))
                                    {
                                        var puppet = playerPuppets[prop.grabber.player.GetPath()];
                                        RegrabProp(prop, puppet.grabber, true);
                                    }
                                }
                            }
                            else
                            {
                                if (Multiplayer.IsServer())
                                {
                                    prop.RequestImpulse(Vector3.Zero, movedToOtherPortal);
                                }
                            }
                            prop.Teleport(movedToOtherPortal);
                        }
                        if (body is Player player)
                        {
                            body.GlobalTransform = movedToOtherPortal;
                            player.Teleport(movedToOtherPortal);

                            player.model.LastGlobalPosition = player.GlobalPosition;
                            player.StabilizeTimer = 0.2f;
                            OtherPortal.PlayerEntered(player);

                            if (player.grabber.GrabbingProp != null)
                            {
                                if (OtherPortal.playerPuppets.ContainsKey(player.GetPath()))
                                {

                                    var grabbingProp = player.grabber.GrabbingProp;
                                    var puppet = OtherPortal.playerPuppets[player.GetPath()];
                                    OtherPortal.RegrabProp(grabbingProp, puppet.grabber, true);
                                    GD.Print(puppet.GlobalPosition, player.GlobalPosition);
                                    continue;
                                }
                            }
                            else
                            if (playerPuppets.ContainsKey(player.GetPath()))
                            {
                                var puppet = playerPuppets[player.GetPath()];
                                if (puppet.grabber.GrabbingProp != null)
                                {
                                    RegrabProp(puppet.grabber.GrabbingProp, player.grabber, false);
                                }
                            }
                        }
                        var checlbodies = body.GetChildrenRecursively();
                        checlbodies.Add(body);
                        foreach (var node in checlbodies)
                        {
                            if (node is ITeleportableTransform teleportableTransform)
                            {
                                teleportableTransform.RecieveTransformTeleportation(TeleportTransform);
                            }
                            if (node is ITeleportablePoint teleportablePoint)
                            {
                                teleportablePoint.RecievePointTeleportation(TeleportPoint);
                            }
                        }

                        continue;
                    }
                }
                body.SetMeta("portal_pos", localPos);
            }
        }

        public void RegrabProp(Prop prop, ObjectGrabber otherGrabber, bool teleport)
        {
            var grabber = prop.grabber;
            if (grabber == null) return;
            grabber.UngrabSimply();
            grabber.RequestUngrabProp(0.0f);

            if (grabber.player.Controllable)
            {
                var movedToOtherPortal = TeleportTransform(grabber.connector.Target.GlobalTransform);
                var trans = teleport ? movedToOtherPortal : grabber.connector.Target.GlobalTransform;
                otherGrabber.RequestGrabProp(prop, trans);
                // GD.Print(otherGrabber.GlobalTransform.Origin - trans.Origin);
            }
        }

        public Transform3D TeleportTransform(Transform3D trans)
        {
            if (OtherPortal == null) return trans;
            var bodyTransRelativeToThisPortal = GlobalTransform.AffineInverse() * trans;
            var movedToOtherPortal = OtherPortal.GlobalTransform.RotatedLocal(Vector3.Up, Mathf.Pi) * bodyTransRelativeToThisPortal;
            trans = movedToOtherPortal;
            return trans;
        }

        public Vector3 TeleportPoint(Vector3 point)
        {
            if (OtherPortal == null) return point;
            Transform3D trans = GlobalTransform.Translated(point);
            trans = TeleportTransform(trans);
            point = trans.Origin - OtherPortal.GlobalPosition;
            return point;
        }

        public void UpdatePlayerPuppets()
        {
            foreach (var path in playerPuppets.Keys)
            {
                var player = GetNode<Player>(path);
                var puppet = playerPuppets[path];
                UpdatePuppet(player, puppet);
            }
        }
        public void UpdatePuppet(Player player, Player puppet)
        {
            // if (!puppet.Visible) return;
            puppet.GlobalTransform = TeleportTransform(player.GlobalTransform);
            puppet.Camera.GlobalTransform = TeleportTransform(player.Camera.GlobalTransform);
        }
        public void CreatePuppet(Player player)
        {
            if (playerPuppets.ContainsKey(player.GetPath()))
            {
                var existingpuppet = playerPuppets[player.GetPath()];
                existingpuppet.Visible = true;
                UpdatePuppet(player, existingpuppet);
                return;
            }
            var puppetScene = new PackedScene();
            puppetScene.Pack(player);
            var puppet = puppetScene.Instantiate<Player>();
            puppet.Name = player.Name;
            puppet.SetMeta("dont_teleport", true);
            puppet.SetMeta("is_puppet", true);
            puppet.IsPuppet = true;
            puppet.SetCollisionLayerValue(1, false);
            puppet.Visible = false;
            var sync = puppet.GetNode("MultiplayerSynchronizer");
            if (sync != null) sync.QueueFree();
            puppet.Camera.Current = false;
            AddChild(puppet);
            playerPuppets[player.GetPath()] = puppet;
            UpdatePuppet(player, puppet);
        }
        public void RemovePuppet(Player player)
        {
            if (OtherPortal != null)
            {
                if (OtherPortal.playerPuppets.ContainsKey(player.GetPath())) return;
            }
            if (playerPuppets.ContainsKey(player.GetPath()))
            {
                var puppet = playerPuppets[player.GetPath()];
                puppet.Visible = false;
                // puppet.QueueFree();
                // playerPuppets.Remove(player.GetPath());
            }
        }

        public void PlayerEntered(Player player)
        {
            if (player.HasMeta("dont_teleport")) return;
            CreatePuppet(player);
        }
        public void PlayerExited(Player player)
        {
            if (player.HasMeta("dont_teleport")) return;
            CallDeferred("RemovePuppet", player);
        }
        public void OnPlayerEntered(Node3D body)
        {
            if (body is Player player)
            {
                PlayerEntered(player);
            }
        }
        public void OnPlayerExited(Node3D body)
        {
            if (body is Player player)
            {
                PlayerExited(player);
            }
        }

        public void OnBodyEntered(Node3D body)
        {

        }
        public void OnBodyExited(Node3D body)
        {
            if (body.HasMeta("portal_pos"))
            {
                body.RemoveMeta("portal_pos");
            }
        }
    }
}