using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BearLib;
namespace DungeonRising
{
    public class Entry
    {
        public static void Run()
        {

            Terminal.Open(); // terminal_open();

            // Printing text
            Terminal.Print(1, 1, "Hello, world!"); // terminal_print(1, 1, "Hello, world!");
            Terminal.Refresh(); // terminal_refresh();
            int input = 0; 
            // Wait until user close the window
            while (input != Terminal.TK_CLOSE)
            {
                input = Terminal.Read();
            }

            Terminal.Close(); // terminal_close();
        }
    }
}
