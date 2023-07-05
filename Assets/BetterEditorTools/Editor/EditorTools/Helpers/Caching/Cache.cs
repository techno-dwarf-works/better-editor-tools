namespace Better.EditorTools.Helpers.Caching
{
    public class Cache<T>
    {
        public bool IsValid { get; protected set; }
        public T Value { get; protected set; }

        public void Set(bool isValid, T value)
        {
            IsValid = isValid;
            Value = value;
        }

        public Cache<T> Copy()
        {
            return new Cache<T>()
            {
                IsValid = IsValid,
                Value = Value
            };
        }
    }
}