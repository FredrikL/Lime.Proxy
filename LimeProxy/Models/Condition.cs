namespace LimeProxy.Models
{
    public class Condition
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
    }
}