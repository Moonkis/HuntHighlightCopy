using System;
using System.IO;
using System.Configuration;

namespace HuntHighlightCopy
{
    class Program
    {
        static string SrcPath { get; set; }
        static string DstPath { get; set; }

        static void Main(string[] args)
        {
            SrcPath = ConfigurationManager.AppSettings["SourceFolder"];
            DstPath = ConfigurationManager.AppSettings["DestinationFolder"];

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Monitoring the folder: {SrcPath}");
            Console.WriteLine($"Configurated to copy to the following folder: {DstPath}");
            Console.ResetColor();

            FileSystemWatcher watcher = new FileSystemWatcher(SrcPath);
            watcher.Changed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;


            Console.WriteLine("Type q then press Enter to quit.");
            while (Console.Read() != 'q')
            {


            }
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            FileInfo file = new FileInfo(e.FullPath);

            if (IsFileLocked(file))
            {
                Console.WriteLine($"File {file.Name} is not finalized.");
                return;
            }

            string dst = $@"{DstPath}\{file.Name}";
            Console.WriteLine($"File is finalized, trying to copy {file.FullName} to {dst}");
            try
            {
                file.CopyTo(dst, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }

            return false;
        }
    }
}
