

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Utils;
using Godot;

namespace Game
{
    public partial class RandomGeneratorNode : Node
    {
        private class GenerationResult<T>
        {
            public string code;
            public T value;
            public ulong time;
        }
        private event Action<GenerationResult<int>> OnIntGenerated;
        private Dictionary<string, GenerationResult<int>> IntGenerationResults = new Dictionary<string, GenerationResult<int>>();

        public async Task<int> FetchRandomIntInRange(string code, int from, int to)
        {
            var rnd = new RandomNumberGenerator();
            RpcId(1, MethodName.ServerFetchRandomInt, code, from, to);
            GenerationResult<int> res = await new EventAwait<GenerationResult<int>>()
            .OnConnect((func) => OnIntGenerated += func)
            .OnDisconnect((func) => OnIntGenerated -= func)
            .WithCondition((val) => val.code == code)
            .Await();
            return res.value;
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void ServerFetchRandomInt(string code, int from, int to)
        {
            var rnd = new RandomNumberGenerator();
            var value = rnd.RandiRange(from, to);
            GenerationResult<int> res = null;

            if (IntGenerationResults.ContainsKey(code))
            {
                if (Time.GetTicksMsec() - IntGenerationResults[code].time <= 1000)
                {
                    res = IntGenerationResults[code];
                }
            }

            if (res != null)
            {
                value = IntGenerationResults[code].value;
            }
            else
            {
                res = new GenerationResult<int>();
                res.code = code;
                res.value = value;
                res.time = Time.GetTicksMsec();
                IntGenerationResults[code] = res;
            }

            Rpc(MethodName.RecieveRandomInt, code, value);
        }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RecieveRandomInt(string code, int number)
        {
            var res = new GenerationResult<int>();
            res.code = code;
            res.value = number;
            res.time = Time.GetTicksMsec();
            OnIntGenerated?.Invoke(res);
        }
    }
}