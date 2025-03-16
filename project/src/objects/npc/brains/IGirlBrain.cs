using System.Threading.Tasks;
using Godot;
using Godot.Collections;

namespace Game
{
	public enum GirlMood
	{
		GOOD,
		BAD
	}
	public interface IGirlBrain
	{
		public GirlMood mood { get; set; }
		public Task RequestToGiveProp(ItemResource itemResource);
		public Task RequestToGiveProp(string itemResourcePath);
	}
}