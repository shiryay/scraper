using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("https://www.worldometers.info/coronavirus/country/russia/");

            var HeaderNames = doc.DocumentNode
                .SelectNodes("//div[@class='maincounter-number']").ToList();

            string stats_string = HeaderNames[0].InnerText;
            stats_string = stats_string.Replace(",", "");
            
            Int64 stats = Convert.ToInt64(stats_string);
            Int64 saved_stats_num = 0;

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("COVID19");
                if (key != null)
                    {
                        Object saved_stats = key.GetValue("Stats");
                        key.Close();
                        saved_stats_num = Convert.ToInt64(saved_stats);

                        if (stats > saved_stats_num)
                        {
                            Console.WriteLine($"Active cases: {stats} (+{stats - saved_stats_num})");
                            using (key = Registry.CurrentUser.OpenSubKey("COVID19", true))
                            {
                                key.SetValue("Stats", stats);
                                key.Close();
                            }
                        }
                        else
                        {
                            Console.WriteLine("No new data.");
                            Console.WriteLine($"Active cases: {saved_stats_num}");
                        }
                    }
                else
                {
                    Console.WriteLine($"Active cases: {stats}");
                    key = Registry.CurrentUser.CreateSubKey("COVID19");
                    key.SetValue("Stats", stats);
                    key.Close();
                    Console.WriteLine("Key created.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //Console.ReadLine();
        }
    }
}
