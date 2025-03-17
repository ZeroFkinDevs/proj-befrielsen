using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Game.Dialog
{
    public partial class GirlChattingMember : ChattingMember, ISubtitlesProvider
    {
        [Export]
        public SoundPlayer soundPlayer;

        [Export]
        public NpcBrain npcBrain;
        private IGirlBrain girlBrain => npcBrain as IGirlBrain;

        [Export]
        public SubtitlesUnit subtitlesUnit { get; set; }

        private RandomGeneratorNode randomGenerator;

        public override void _Ready()
        {
            base._Ready();
            ChattingMembers.OnAdded += OnMemberAdded;

            randomGenerator = new RandomGeneratorNode();
            randomGenerator.Name = "RandomGenerator";
            AddChild(randomGenerator);
        }
        protected override void SetupConversation()
        {
            base.SetupConversation();

            NodesCollection.AddNode(new ChattingNode("_greet")
            .WithCondition(() => girlBrain.mood == GirlMood.GOOD)
            .WithAction(async () =>
            {
                ChattingMembers.ForEachMember((member) =>
                {
                    member.ResponsesCollection.SetForMember(this, new List<ChattingResponse>{
                        new ChattingResponse("give_me_digital_eye", "дай мне глаз комнаты с компами", async ()=>{
                            ClearResponses();
                            await SayYes();
                            await girlBrain.RequestToGiveProp("res://scenes/persistent/tools/eyes/eye_digital/eye_digital_item.tres");
                            TryExecuteNode("_greet");
                        }),
                        new ChattingResponse("give_me_freeme_eye", "дай мне глаз от твоей клетки", async ()=>{
                            ClearResponses();
                            await SayYes();
                            await girlBrain.RequestToGiveProp("res://scenes/persistent/tools/eyes/eye_freeme/eye_freeme_item.tres");
                            TryExecuteNode("_greet");
                        }),
                    });
                });
            }));

            NodesCollection.AddNode(new ChattingNode("_greet")
            .WithCondition(() => girlBrain.mood == GirlMood.BAD)
            .WithAction(async () =>
            {
                ChattingMembers.ForEachMember((member) =>
                {
                    member.ResponsesCollection.SetForMember(this, new List<ChattingResponse>{
                        new ChattingResponse("hi_girl", "Что с тобой?", ()=>{TryExecuteNode("hi_girl");}),
                    });
                });
            }));
        }

        public async Task SayYes()
        {
            var sounds = new List<string>{
                "res://resources/audio/sheril/common/yes_1.ogg",
                "res://resources/audio/sheril/common/wait_a_sec.ogg",
                "res://resources/audio/sheril/common/yes_2.ogg",
            };
            var texts = new List<string>{
                "угу...",
                "да, сейчас...",
                "ага...",
            };
            var randIdx = await randomGenerator.FetchRandomIntInRange("sheril_yes_sound", 0, sounds.Count - 1);
            await Say(texts[randIdx], sounds[randIdx]);
        }

        public Task Say(string text, string soundPath)
        {
            var task = soundPlayer.PlaySound(soundPath);
            subtitlesUnit.PlaySubtitlesForTask(text, task);
            GD.Print(text);
            return task;
        }

        public void ClearResponses()
        {
            ChattingMembers.ForEachMember((member) =>
                {
                    member.ResponsesCollection.SetForMember(this, null);
                }
            );
        }

        private void OnMemberAdded(IChattingMember member)
        {
            TryExecuteNodeDeffered("_greet");
        }
    }
}