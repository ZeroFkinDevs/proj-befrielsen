using System;
using System.Threading.Tasks;

namespace Game.Utils
{
    public class EventAwait<T>
    {
        private TaskCompletionSource<T> tcs;
        private Action<Action<T>> OnConnectAction;
        private Action<Action<T>> OnDisconnectAction;
        private Func<T, bool> ConditionFunc;

        public EventAwait()
        {
            if (tcs != null)
            {
                tcs.SetResult(default);
                tcs = null;
            }
            tcs = new TaskCompletionSource<T>();
        }

        public EventAwait<T> OnConnect(Action<Action<T>> onConnectAction)
        {
            OnConnectAction = onConnectAction;
            OnConnectAction(OnFinished);
            return this;
        }
        public EventAwait<T> OnDisconnect(Action<Action<T>> onDisconnectAction)
        {
            OnDisconnectAction = onDisconnectAction;
            return this;
        }
        public EventAwait<T> WithCondition(Func<T, bool> conditionFunc)
        {
            ConditionFunc = conditionFunc;
            return this;
        }

        public Task<T> Await()
        {
            return tcs.Task;
        }

        private void OnFinished(T result)
        {
            if (ConditionFunc != null && !ConditionFunc(result)) return;

            if (tcs != null)
            {
                tcs.SetResult(result);
                tcs = null;
            }
            OnDisconnectAction(OnFinished);
        }
    }

    public class EventAwait
    {
        private TaskCompletionSource<bool> tcs;
        private Action<Action> OnConnectAction;
        private Action<Action> OnDisconnectAction;
        private Func<bool> ConditionFunc;

        public EventAwait()
        {
            if (tcs != null)
            {
                tcs.SetResult(false);
                tcs = null;
            }
            tcs = new TaskCompletionSource<bool>();
        }

        public EventAwait OnConnect(Action<Action> onConnectAction)
        {
            OnConnectAction = onConnectAction;
            OnConnectAction(OnFinished);
            return this;
        }

        public EventAwait OnDisconnect(Action<Action> onDisconnectAction)
        {
            OnDisconnectAction = onDisconnectAction;
            return this;
        }

        public EventAwait WithCondition(Func<bool> conditionFunc)
        {
            ConditionFunc = conditionFunc;
            return this;
        }

        public Task Listen()
        {
            return tcs.Task;
        }

        private void OnFinished()
        {
            if (ConditionFunc != null && !ConditionFunc()) return;

            if (tcs != null)
            {
                tcs.SetResult(true);
                tcs = null;
            }
            OnDisconnectAction(OnFinished);
        }
    }
}