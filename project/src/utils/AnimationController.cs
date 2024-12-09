using System;
using Godot;

namespace Game
{
	public partial class AnimationController : Node3D
	{
		[Export]
		public Skeleton3D skeleton3D;

		public float ModelScale = 1.0f;

		private AnimationTree _animationThree;
		public AnimationTree AnimTree { get { return _animationThree; } }

		public Transform3D GetBoneGlobalPose(int boneId)
		{
			var pose = skeleton3D.GlobalTransform * skeleton3D.GetBoneGlobalPose(boneId);
			pose.Basis = pose.Basis.Scaled(Vector3.One / ModelScale);
			return pose;
		}

		public override void _Ready()
		{
			_animationThree = GetNode<AnimationTree>("AnimationTree");
			AnimTree.AnimationFinished += OnAnimationFinished;
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
		}

		public virtual void OnAnimationFinished(StringName animationName)
		{

		}

		public void SetState(string transitionNodeName, string stateName)
		{
			AnimTree.Set("parameters/" + transitionNodeName + "/transition_request", stateName);
		}
		public void SetTimeScale(string timeScaleName, float scale)
		{
			AnimTree.Set("parameters/" + timeScaleName + "/scale", scale);
		}
		public void SetBlend(string blendName, float value)
		{
			AnimTree.Set("parameters/" + blendName + "/blend_amount", value);
		}
		public void SetBlend2D(string blendName, Vector2 value)
		{
			AnimTree.Set("parameters/" + blendName + "/blend_position", value);
		}
		public Vector2 GetBlend2D(string blendName)
		{
			return (Vector2)AnimTree.Get("parameters/" + blendName + "/blend_position");
		}
		public string GetState(string transitionNodeName)
		{
			return (string)AnimTree.Get("parameters/" + transitionNodeName + "/current_state");
		}
	}
}