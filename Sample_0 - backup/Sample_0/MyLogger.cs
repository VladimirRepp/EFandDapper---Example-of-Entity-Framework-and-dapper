using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample_0
{
    // Либо делаем его одиночкой 
    public static class MyLogger
    {
        public static void ConsoleLog(string message)
        {
            Console.WriteLine("Сообщение: {0} ", message);
        }
    }
}
