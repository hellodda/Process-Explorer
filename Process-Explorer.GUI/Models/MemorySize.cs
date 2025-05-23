namespace Process_Explorer.GUI.Models
{
    public class MemorySize
    {
        public MemorySize(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; } = default!;
        public int Value { get; set; } = default!;
    }
}
