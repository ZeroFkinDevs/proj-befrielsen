using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Game.Dialog
{
    public class ChattingMembersCollection
    {
        protected HashSet<IChattingMember> Members;
        public event Action OnUpdate;
        public event Action<IChattingMember> OnAdded;
        public event Action<IChattingMember> OnRemoved;

        public ChattingMembersCollection()
        {
            Members = new HashSet<IChattingMember>();
        }

        public void ForEachMember(Action<IChattingMember> func)
        {
            foreach (var member in Members)
            {
                func(member);
            }
        }
        public List<IChattingMember> GetList()
        {
            return Members.ToList();
        }
        public IEnumerable<string> GetNames()
        {
            foreach (var member in Members)
            {
                yield return member.Name;
            }
        }
        public IChattingMember GetMemberByCode(string code)
        {
            foreach (var member in Members)
            {
                if (member.Code == code) return member;
            }
            return null;
        }

        public void Add(IChattingMember member)
        {
            if (!Members.Contains(member))
            {
                Members.Add(member);
                OnAdded?.Invoke(member);
                OnUpdate?.Invoke();
            }
        }
        public void Remove(IChattingMember member)
        {
            if (Members.Contains(member))
            {
                Members.Remove(member);
                OnRemoved?.Invoke(member);
                OnUpdate?.Invoke();
            }
        }
    }
}