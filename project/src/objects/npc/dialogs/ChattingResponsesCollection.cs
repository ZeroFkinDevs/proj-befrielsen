using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Game.Dialog
{
    public class ChattingResponsesCollection
    {
        protected Dictionary<string, List<ChattingResponse>> ResponsesMap { get; set; }
        public event Action OnUpdate;
        public ChattingResponsesCollection()
        {
            ResponsesMap = new Dictionary<string, List<ChattingResponse>>();
        }

        public void SetForMember(IChattingMember member, List<ChattingResponse> responses)
        {
            if (responses == null) responses = new List<ChattingResponse>();
            ResponsesMap[member.Code] = responses;
            OnUpdate?.Invoke();
        }
        public void ClearForMember(IChattingMember member)
        {
            if (ResponsesMap.ContainsKey(member.Code))
            {
                ResponsesMap.Remove(member.Code);
                OnUpdate?.Invoke();
            }
        }
        public List<ChattingResponse> GetForMember(IChattingMember member)
        {
            if (ResponsesMap.ContainsKey(member.Code))
            {
                return ResponsesMap[member.Code];
            }
            return null;
        }
        public ChattingResponse GetByCode(string code)
        {
            foreach (var memberCode in ResponsesMap.Keys)
            {
                foreach (var response in ResponsesMap[memberCode])
                {
                    if (response.Code == code) return response;
                }
            }
            return null;
        }
        public void ExecuteResponse(ChattingResponse response)
        {
            response.Execute();
            OnUpdate?.Invoke();
        }
        public List<string> GetMembersCodes()
        {
            return ResponsesMap.Keys.ToList();
        }
    }
}