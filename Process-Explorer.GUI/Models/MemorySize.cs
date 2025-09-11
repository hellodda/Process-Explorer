using System;

namespace Process_Explorer.GUI.Models;

public class MemorySize
{
    public MemorySize(string name, int memorySizeValue, Func<double, double, double> calculator)
    {
        Name = name;
        MemorySizeValue = memorySizeValue;
        Calculator = calculator;
    }

    public string Name { get; set; }
    public int MemorySizeValue { get; set; }

    public Func<double, double, double> Calculator { get; set; }

    public double Calculate(double value)
    {
        return Calculator(MemorySizeValue, value);
    }

    //
    // constants \/ \/ \/
    //

    public static MemorySize Cpu => new("Cpu %", 1, (size, value) => value / size);
    public static MemorySize Byte => new("B", 1, (size, value) => value / size);
    public static MemorySize KiloByte => new("KB", 1024, (size, value) => value / size);
    public static MemorySize MegaByte => new("MB", 1048576, (size, value) => value / size);
    public static MemorySize GigaByte => new("GB", 1073741824, (size, value) => value / size);
}
