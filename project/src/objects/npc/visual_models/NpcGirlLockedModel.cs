using System;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

namespace Game
{
	public interface ITalkableModel
	{
		public void Greet();
		public void Leave();
		public void Alone();
		public Task Give(Action onInstantiate);
		public Task Release();
		public void SetLookTarget(Vector3 target);
	}

	public partial class NpcGirlLockedModel : NpcCharacterModel, ITalkableModel
	{
		bool CanLookAtTarget = false;
		float LookTargetWeight = 0.0f;
		Vector3 LookTarget;

		int HeadBoneId;
		int NeckBoneId;
		Transform3D headRelativeTransform;

		public override void _Ready()
		{
			base._Ready();
			HeadBoneId = skeleton3D.FindBone("head");
			NeckBoneId = skeleton3D.FindBone("neck");
			headRelativeTransform = skeleton3D.GetBonePose(HeadBoneId);
		}

		public async void Greet()
		{
			CanLookAtTarget = true;
			SetState("base_state", "greet");
			await WaitForAnimationToFinish("locked_talk_start");
			SetState("base_state", "ready_to_talk");
		}

		public void Leave()
		{
			SetState("base_state", "leave");
		}

		public void Alone()
		{
			CanLookAtTarget = false;
			SetState("base_state", "alone");
		}
		public async Task Give(Action onInstantiate)
		{
			CanLookAtTarget = false;
			SetState("action_state", "give");
			await animWithEvents.WaitForEvent("instantiate");
			onInstantiate();
			await animWithEvents.WaitForEvent("continue_looking");
			CanLookAtTarget = true;
		}
		public async Task Release()
		{
			SetState("action_state", "release");
			await WaitForAnimationToFinish("locked_talk_release");
			SetState("action_state", "default");
		}

		public override void OnAnimationFinished(StringName animationName)
		{
			base.OnAnimationFinished(animationName);
			if (animationName == "locked_talk_end")
			{
				if (GetState("base_state") == "leave")
				{
					if (animationName == "locked_talk_end") Alone();
				}
			}
		}

		public void SetLookTarget(Vector3 target)
		{
			LookTarget = target;
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			LookAtTarget((float)delta);
		}

		public void LookAtTarget(float delta)
		{
			float targetWeight = CanLookAtTarget ? 1.0f : 0.0f;
			LookTargetWeight = Mathf.Lerp(LookTargetWeight, targetWeight, 0.1f);

			var headInitPose = skeleton3D.GetBoneGlobalPose(NeckBoneId) * headRelativeTransform;
			var neckPose = GetBoneGlobalPose(NeckBoneId);

			var trans = GetBoneGlobalPose(HeadBoneId);
			var targetBasis = new Basis(new Quaternion(trans.LookingAt(LookTarget, neckPose.Basis.Y.Lerp(Vector3.Up, 0.5f)).RotatedLocal(Vector3.Up, -Mathf.Pi / 2.0f).Basis).Normalized());
			trans.Basis = new Basis(new Quaternion(trans.Basis).Normalized()).Slerp(targetBasis, 0.05f);
			var skeletonRelativeTrans = (skeleton3D.GlobalTransform.AffineInverse() * trans);
			skeletonRelativeTrans.Origin = headInitPose.Origin;
			skeleton3D.SetBoneGlobalPoseOverride(HeadBoneId, skeletonRelativeTrans, LookTargetWeight, true);
		}
	}
}