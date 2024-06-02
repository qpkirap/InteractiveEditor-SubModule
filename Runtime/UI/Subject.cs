using System;

namespace UniRx
{
    public class Subject : IObservable, IDisposable
    {
        private readonly UniRx.Subject<object> subject = new();

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return subject.Subscribe(observer);
        }

        public void OnCompleted()
        {
            subject.OnCompleted();
        }

        public void OnError(Exception error)
        {
            subject.OnError(error);
        }

        public void OnNext()
        {
            subject.OnNext(null);
        }

        public void Dispose()
        {
            subject.Dispose();
        }
    }
}