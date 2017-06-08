using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;




/*
 
 _____ ______  _____    _                       
|  ___|___  / |_   _|  (_)                      
| |__    / /    | |_ __ _  __ _  __ _  ___ _ __ 
|  __|  / /     | | '__| |/ _` |/ _` |/ _ \ '__|
| |___./ /___   | | |  | | (_| | (_| |  __/ |   
\____/\_____/   \_/_|  |_|\__, |\__, |\___|_|   
                           __/ | __/ |          
                          |___/ |___/    

   [+] A very simple CSGO triggerbot with tidy memory management class
   [+] Made by Dondor17
   [+] As basic as it gets....


     
     */

namespace EzTrigger
{
    class Program
    {
        public static Thread tt = new Thread(tgl);
        public static Thread trgr = new Thread(triggered);
        public static bool toggle = false;
        public static int delay = 120;
        public static bool loaded = false;


        struct Offsets
        {
            public static uint team = 0x000000F0;
            public static uint m_dwLocalPlayer = 0x00000178; //Offset for the local player
            public static uint m_iCrossHairID = 0x0000AA70; //Offset for crosshair ID
        }

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 2; //FLag for pressed button
        private const int MOUSEEVENTF_LEFTUP = 4; //Flag for released button

        
        static void Main(string[] args)
        {

            Console.Title = "EzTrigger!" + " Delay[" + delay.ToString() +" ms]";
            do
            {
                Console.WriteLine();
                Console.WriteLine("HEI, IZ ME ! THE EZ TRIGGER!");
                Console.WriteLine();
                Console.WriteLine("1) Start");
                Console.WriteLine("2) Change Delay");
                 Console.WriteLine("3) Choose Preset");


                switch (Console.ReadLine())
                {

                    case ("1"):
                        {

                         if(!loaded)
                            {


                                Process[] pname = Process.GetProcessesByName("csgo");
                                if (pname.Length == 0)
                                {
                                    Console.WriteLine("Game is not running! Try launching as admin! Fag.");
                                }
                                else
                                {
                                    loaded = true;
                                    Console.WriteLine("");
                                    Console.WriteLine("Activated!");
                                    Console.WriteLine("Mid. mouse button -> Toggle ON/OFF");
                                    tt.IsBackground = true;
                                    tt.Start();

                                    trgr.IsBackground = true;
                                    trgr.Start();
                                }


                                
                            }
                            else
                            {
                                Console.WriteLine("");

                            }
                            
                           
                            

                            break;
                        }

                    case ("2"):
                        {

                            toggle = false;
                            Console.Write("Chosen Delay[ms]: ");
                            try
                            {
                                delay = Convert.ToInt32(Console.ReadLine());
                                Console.Title = "EzTrigger!" + " Delay[" + delay.ToString() + " ms]";
                                Console.WriteLine("Delay changed to " + delay + " ms");
                            }
                            catch
                            {
                                Console.WriteLine("Not a number!");
                                Console.WriteLine();
                            }

                            break;

                        }

                    case ("3"):
                        {
                            toggle = false;
                            Console.WriteLine();
                            Console.WriteLine("Choose Preset:");
                            Console.WriteLine("1) Deagle");
                            Console.WriteLine("2) AK47/M4/M4A1S");
                            Console.WriteLine("3) Scout/AWP");
                            Console.WriteLine("4) INHUMAN REACTIONS");
                            switch (Console.ReadLine())
                            {

                                case ("1"):
                                    {
                                        delay = 60;
                                        Console.Title = "[OFF]-EzTrigger!" + " Delay[" + delay.ToString() + " ms]";
                                        Console.WriteLine("Preset Selected!");
                                        break;
                                    }
                                case ("2"):
                                    {
                                        delay = 10;
                                        Console.Title = "[OFF]-EzTrigger!" + " Delay[" + delay.ToString() + " ms]";
                                        Console.WriteLine("Preset Selected!");
                                        break;
                                    }
                                case ("3"):
                                    {
                                        delay = 120;
                                        Console.Title = "[OFF]-EzTrigger!" + " Delay[" + delay.ToString() + " ms]";
                                        Console.WriteLine("Preset Selected!");
                                        break;
                                    }
                                case ("4"):
                                    {
                                        delay = 1;
                                        Console.Title = "[OFF]-EzTrigger!" + " Delay[" + delay.ToString() + " ms]";
                                        Console.WriteLine("Preset Selected!");
                                        break;
                                    }

                            }

                            break;
                        }
                }
            }
            while (true);

  
        }
        public static void triggered()
        {




            MemoryManagment mem = new MemoryManagment();
            if (!mem.Initialize())
            {
                Console.ReadKey();
                Environment.Exit(-1);
            }

            while (true)
            {
                //PRead the addr.

                uint pLocal = mem.Read<uint>((IntPtr)(mem.clientBaseAddres.ToInt64() + Offsets.m_dwLocalPlayer));
                int myTeam = mem.Read<int>((IntPtr)(pLocal + Offsets.team));
                //Read entity ID which is inside crosshait (0 = ground. 1 to 64 = player)
                int crossID = mem.Read<int>((IntPtr)(pLocal + Offsets.m_iCrossHairID));

               
                if (crossID < 64 && crossID > 0)
                {


                    if (toggle)
                    {
                        Thread.Sleep(delay);

                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        Thread.Sleep(delay);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    }
                    else if (!toggle)
                    {
                        //   Console.WriteLine("Wait!");
                    }
                }
                Thread.Sleep(1);
            }
        }



        public static void tgl()
        {
            do
            {
                if (!Convert.ToBoolean(GetAsyncKeyState(0x04) & 0x8000))
                {                              
                }
                else
                {
                    if (toggle == true)
                    {
                        toggle = false;
                        Console.Title = "[OFF]-EzTrigger!" + " Delay[" + delay.ToString() + " ms]";

                        Console.Beep(400, 250);
                        Console.Beep(300, 250);
                    }
                    else
                    {
                        toggle = true;
                        Console.Title = "[ON]-EzTrigger!" + " Delay[" + delay.ToString() + " ms]";
                        Console.Beep(300, 250);
                        Console.Beep(400, 250);
                       
                    }
                    System.Threading.Thread.Sleep(300);
                }
            }
            
            while (true);
        
        }
    }
}
