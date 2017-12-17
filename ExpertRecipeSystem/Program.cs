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

            var system = new CSVExpertSystem("Таблица переходов.csv", "Таблица вопросов.csv");
            var endOf = false;
            var result = system.NextQuestion("", out endOf);
            while (true)
            {

                if (result != null)
                {
                    Console.WriteLine(result);
                    result = system.NextQuestion(Console.ReadLine(), out endOf);
                }
                else
                {
                    Console.WriteLine("End");
                    break;
                }
            }
            Console.Read();
        }
    }
}
