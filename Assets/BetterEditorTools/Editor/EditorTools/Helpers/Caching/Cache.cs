namespace Better.EditorTools.Helpers.Caching
{
    public class Cache<T> where T : class
    {
        public bool IsValid { get; private set; }
        public T Value { get; private set; }

        public void Set(bool isValid, T value)
        {
            IsValid = isValid;
            Value = value;
        }
    }
}