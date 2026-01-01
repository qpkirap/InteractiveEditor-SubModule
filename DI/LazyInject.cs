namespace DepedencyInjection
{
    public class LazyInject<T> where T : class
    {
        private T value;

        public T Value => value ??= DI.Get<T>();
    }
}