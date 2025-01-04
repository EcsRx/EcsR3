namespace EcsR3.Tests.Models
{
    public class ComplexObject
    {
        public int Value { get; }
        public string Name { get; }

        public ComplexObject(int value, string name)
        {
            Value = value;
            Name = name;
        }
    }
}