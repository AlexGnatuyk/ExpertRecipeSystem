using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertRecipeSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var expertSystem = new ExpertSystem("Таблица переходов.csv", "Таблица вопросов.csv");
            var endOf = false;
            var result = expertSystem.NextQuestion("", out endOf);
            while (true)
            {
                if (result != null)
                {
                    Console.WriteLine(result);
                    result = expertSystem.NextQuestion(Console.ReadLine(), out endOf);
                }
                else
                {
                    Console.WriteLine(result);
                    Console.WriteLine("End");
                    break;
                }
            }
            Console.Read();
        }
    }
}