using System;

namespace VCAOpenCV
{
    class Program
    {
        static void Main(string[] args)
        {
            VCA.SelectAlgo(new AlgoBodyCascade2()).Analyse();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }
    }
}
