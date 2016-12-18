using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseGenerator.Readers;
using DataBaseGenerator.DataBase;

namespace DataBaseGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = DataReader.Read();
            DataWriter.Write(data);

            Console.WriteLine("Press any key to finish...");
            Console.ReadKey();
        }
    }
}
