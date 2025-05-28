using System;

namespace Process_Explorer.GUI.Models
{
    public class MemorySize
    {
        public MemorySize(string name, int memorySizeValue, Func<int, double, double> calculator)
        {
            Name = name;
            MemorySizeValue = memorySizeValue;
            Calculator = calculator;
        }

        public string Name { get; set; }
        public int MemorySizeValue { get; set; }

        public Func<int, double, double> Calculator { get; set; }

        public double Calculate(double value)
        {
            return Calculator(MemorySizeValue, value);
        }

        //
        // constants \/ \/ \/
        //

        public static MemorySize Cpu => new MemorySize("Cpu %", 1, (size, value) =>
        {
            int processorCount = Environment.ProcessorCount;
            double totalCpuPercentage = value / processorCount;

            return Math.Round(totalCpuPercentage);
        });

        public static MemorySize Byte => new MemorySize("B", 1, (size, value) => value / size);
        public static MemorySize KiloByte => new MemorySize("KB", 1024, (size, value) => value / size);
        public static MemorySize MegaByte => new MemorySize("MB", 1048576, (size, value) => value / size);
        public static MemorySize GigaByte => new MemorySize("GB", 1073741824, (size, value) => value / size);
    }
}
