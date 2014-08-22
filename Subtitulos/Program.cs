using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Subtitulos.Classes;

namespace Subtitulos
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] rutasMovies = System.Configuration.ConfigurationManager.AppSettings["pathMovies"].Split(',');
            string[] rutasShows = System.Configuration.ConfigurationManager.AppSettings["pathShows"].Split(',');
            string[] exts = System.Configuration.ConfigurationManager.AppSettings["ext"].Split(',');

            Buscar(rutasMovies, rutasShows, exts);
        }

        #region Methods

        public static void Buscar(string[] pathMovies, string[] pathShows, string[] exts)
        {
            BuscarPeliculas(pathMovies, exts);
            BuscarSeries(pathShows, exts);
        }
        
        public static void BuscarPeliculas(string [] path,string [] exts)
        {
            Console.WriteLine("Movies"); 
            foreach (string ext in exts)
            {
                //Console.WriteLine("Buscando peliculas en " + ext);
                foreach (string camino in path)
                {
                    Console.WriteLine("   in " + camino);
                    DirectoryInfo dir = new DirectoryInfo(camino);
                    foreach (DirectoryInfo dir2 in dir.GetDirectories())
                    {
                        //Console.WriteLine(dir2);
                        foreach (FileInfo file in dir2.GetFiles("*." + ext))
                        {
                            if (File.Exists(Path.ChangeExtension(file.FullName, ".srt")))
                            {
                                /* Nothing to do */
                            }
                            else
                            {
                                Console.WriteLine("{0}", file.Name);
                            }
                        }
                    }
                }
            }
        }
        public static void BuscarSeries(string [] path, string [] exts)
        {
            Console.WriteLine("TV Shows");
            foreach (string ext in exts)
            {
                //Console.WriteLine("Searching for " + ext);
                foreach (string camino in path)
                {
                    Console.WriteLine("   in " + camino);
                    DirectoryInfo dir = new System.IO.DirectoryInfo(camino);
                    foreach (DirectoryInfo dir2 in dir.GetDirectories())
                    {
                        Console.WriteLine("      " + dir2.Name);
                        foreach (DirectoryInfo dir3 in dir2.GetDirectories())
                        {
                            foreach (FileInfo file in dir3.GetFiles("*." + ext))
                            {

                                if (File.Exists(Path.ChangeExtension(file.FullName, ".srt")))
                                {
                                    /* Nothing to do */
                                }
                                else
                                {
                                    Console.WriteLine("{0} has no subtitles.", file.Name);
                                    BajarSubtitulo(dir2.Name, file.Name, dir3.FullName);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void BajarSubtitulo(string directory, string file, string location)
        {
            Subtitulo sub = new Subtitulo();

            sub.SetShow(directory);
            sub.SetCurrentEpisode(file);
            if (sub.SetId())
            {
                sub.SetLocalLocation(location, file);
                sub.GetSub(0);
            }
        }

        #endregion

     }
}

