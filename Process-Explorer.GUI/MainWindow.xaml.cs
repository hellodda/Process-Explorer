using Microsoft.UI.Xaml;
using Process_Explorer.BLL;
using System;

namespace Process_Explorer.GUI
{
    public sealed partial class MainWindow : Window
    {
        private readonly Tester _tester;

        public MainWindow(Tester tester)
        {
            _tester = tester;
            InitializeComponent();
            Setup();
            
        }

        public void Setup()
        {
            ExtendsContentIntoTitleBar = true;
            var list = _tester.GetInfo();
            
            foreach(var info in list)
            {
                Console.WriteLine($"Process ID: {info.PID}");
                Console.WriteLine($"Process Name: {info.Name}");
                Console.WriteLine($"Working Set: {info.WorkingSet}");
                Console.WriteLine($"Private Bytes: {info.PrivateUsage}");
                Console.WriteLine($"Company: {info.Company}");
                Console.WriteLine($"Description: {info.Description}");
            }

        }
    }
}
