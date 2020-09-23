using System;

namespace SteamAPIProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            string inUserId = "";

            Console.WriteLine("Enter your Steam ID: ");
            inUserId = Console.ReadLine();
            
            while(!SteamLookUp.CheckID(inUserId) || inUserId == string.Empty)
            {
                Console.WriteLine("\nInvalid Steam ID");
                Console.WriteLine("Enter your Steam ID: ");
                inUserId = Console.ReadLine();
            }

            SteamLookUp steamLookUp = new SteamLookUp(inUserId);
            steamLookUp.PrintGameInformation();

            string answer;

            do
            {
                Console.WriteLine("Would you like to export this information to a .csv? (Y/N):");
                answer = Console.ReadLine().ToUpper();
            } while (answer != "Y" && answer != "N");

            if(answer == "Y")
            {
                Console.WriteLine("\nExporting to .csv");
                steamLookUp.ExportToCSV();
            }

            Console.WriteLine("\nThank you for using my program. Press any key to exit");
            Console.ReadKey();           
        }
    }
}
