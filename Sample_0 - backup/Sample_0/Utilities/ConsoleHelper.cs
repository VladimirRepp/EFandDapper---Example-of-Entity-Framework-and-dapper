using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class ConsoleHelper
    {
        public static void ConsolePause()
        {
            Console.WriteLine("Нажмите ВВОД для продолжения . . .");
            Console.ReadLine();
        }
    }
}
