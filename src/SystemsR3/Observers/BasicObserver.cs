using System;
using R3;

namespace SystemsR3.Observers
{
    public class BasicObserver<T> : Observer<T>
    {
        public Action<T> OnNext { get; }
        public Action<Exception> OnErrorResume { get; }
        public Action<Result> OnCompleted { get; }
        
        public BasicObserver(Action<T> onNext, Action<Exception> onErrorResume = null, Action<Result> onCompleted = null)
        {
            OnNext = onNext;
            OnErrorResume = onErrorResume ??  ObservableSystem.GetUnhandledExceptionHandler();
            OnCompleted = onCompleted ?? AutoHandleResult;
        }
        
        private void AutoHandleResult(Result result)
        {
            if (result.IsFailure)
            { ObservableSystem.GetUnhandledExceptionHandler().Invoke(result.Exception); }
        }
        
        protected override void OnNextCore(T value) => OnNext(value);
        protected override void OnErrorResumeCore(Exception error) => OnErrorResume(error);
        protected override void OnCompletedCore(Result complete) => OnCompleted(complete);
    }
}