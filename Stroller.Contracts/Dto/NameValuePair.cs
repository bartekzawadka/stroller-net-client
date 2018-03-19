namespace Stroller.Contracts.Dto
{
    public class NameValuePair<T>
    {
        public string Name { get; set; }

        public T Value { get; set; }

        public NameValuePair(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
