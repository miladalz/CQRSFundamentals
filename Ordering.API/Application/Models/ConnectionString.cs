namespace Ordering.API.Application.Models
{
    public class ConnectionString
    {
        public ConnectionString(string value)
        {
            Value = value;
        }
        public string Value { get; }
    }
}
