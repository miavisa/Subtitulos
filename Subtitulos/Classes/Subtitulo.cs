using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Subtitulos.Classes
{
    public class Subtitulo
    {
        public string id;
        public string currentEspisode;
        public string show;
        public string localLocation;

        public bool SetId()
        {
            try
            {
                Console.Write("Getting Id -> ");
                WebClient w = new WebClient();
                string s = w.DownloadString("http://www.subtitulos.es/" + this.show + "/" + this.currentEspisode);
                Match match = Regex.Match(s, @"subID='.....", RegexOptions.IgnoreCase);
                string m = match.Groups[0].Value;
                m = m.Replace("subID='", "");
                this.id = m;
                Console.WriteLine(m);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("      [ERROR] Maybe the show has another name or the episode is not there. Skipping.");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public void SetCurrentEpisode(string episode)
        {
            Match match = Regex.Match(episode, @"S..E..", RegexOptions.IgnoreCase);
            string m = match.Groups[0].Value;
            m = m.ToUpper();
            m = m.Replace("S0", "").Replace("S", "").Replace("E", "x");
            this.currentEspisode = m;
        }

        public void SetShow(string folder)
        {
            this.show = folder.Replace(" ", "-").ToLower();
        }

        public void GetSub(int version)
        {
            if (version < 5)
            {
                WebClient w = new WebClient();
                w.Headers.Add("Referer", "http://www.subtitulos.es/" + this.show + "/" + this.currentEspisode);
                Console.Write("Downloading " + this.show + " episode " + this.currentEspisode);
                try
                {
                    w.DownloadFile("http://www.subtitulos.es/updated/5/" + this.id + "/" + version.ToString(), this.localLocation);
                    FileInfo f = new FileInfo(this.localLocation);
                    if (f.Length > 0)
                    {
                        Console.WriteLine("  [OK]");
                    }
                    else
                    {
                        Console.WriteLine("      [FAILED] Version " + version + " not found. Retrying with another version.");
                        version++;
                        this.GetSub(version);
                    }
                }
                catch
                {
                    Console.WriteLine("       [FAILED] Version " + version + " not found. Retrying with another version.");
                    version++;
                    this.GetSub(version);
                }
            }
            else
            {
                Console.WriteLine("Too many tries. Skipped.");
            }
        }

        public void SetLocalLocation(string location, string file)
        {
            file = file.Replace(".mkv", ".srt");
            file = file.Replace(".avi", ".srt");
            this.localLocation = location + @"\" + file;
        }
    }
}
