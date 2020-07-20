
/*

file-watcher.cs
-John Taylor
Jul-17-2020

This program monitors a given file location for create, change, rename and delete file events and
then exits with a specific OS exit code once triggered.

The default path that is monitored is the UserProfile directory, defined by `pathToFolder`
Paths can be ignored by placing them in the 'ignore_list'
The default OS exit code is defined by 'success_exit_code'

This program can be compiled on **any** Windows 10 system by running this command:
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /nologo /debug:full /out:.\file-watcher.exe /target:exe file-watcher.cs

Change these as needed (see below):
success_exit_code
ignore_list
pathToFolder

*/

using System;
using System.IO;

namespace FileWatcher {
    class Program {
        const string pgm_version = "1.1.0";
        const string pgm_url = "https://github.com/jftuga/file-watcher";
        
        const int success_exit_code = 80211;
        static string[] ignore_list = { @"\AppData\", @"\temp\", @"ntuser.dat", ".tmp" };

         // Start a FileSystemWatch and it's event handlers
        // filterPath: An empty string ("") watches all files.
        static void InitFSWatcher(string pathToFolder, string filterPath) {
            // lowercase the ignorelist
            for( int i = 0; i < ignore_list.Length; i++) {
                ignore_list[i] = ignore_list[i].ToLower();
            }
            // https://msdn.microsoft.com/en-us/library/system.io.filesystemwatcher.aspx
            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = pathToFolder;
            watcher.Filter = filterPath;
            watcher.IncludeSubdirectories = true;
            watcher.InternalBufferSize = 64 * 1024;

            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | 
                NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | 
                NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // check to see if path is containted in the ignore_list - case-insensitive
        private static bool isIgnored(string path) {
            bool dbg=true;
            path = path.ToLower();

            foreach(string ignore in ignore_list) {
                if(path.Contains(ignore)) {
                    if(dbg) Console.WriteLine("Ignoring file: {0}", path);
                    return true;
                }
            }
            return false;
        }

        // FileSystemWatcher event handler
        // if the filename is in monitored_dict, then save it to the tsv file
        // Specify what is done when a file is changed, created, or deleted.
        private static void OnChanged(object source, FileSystemEventArgs e) {
            bool dbg=true;

            if(isIgnored(e.FullPath.ToString())) return;

            if(dbg) Console.WriteLine("File created, deleted, or changed: {0} ", e.FullPath.ToString());
            System.Environment.Exit(success_exit_code);
        }

        // FileSystemWatcher event handler
        // if the filename is in monitored_dict, then save it to the tsv file
        // Specify what is done when a file is renamed.
        private static void OnRenamed(object source, RenamedEventArgs e) {
            bool dbg=true;

            if(isIgnored(e.OldFullPath.ToString())) return;

            if(dbg) Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath.ToString(), e.FullPath.ToString());
            System.Environment.Exit(success_exit_code);
        }

        static void Main(string[] args) {
            bool dbg=true;
            string pathToFolder = null;

            pathToFolder = Environment.GetEnvironmentVariable("UserProfile");
            if( ! Directory.Exists(pathToFolder)) {
                Console.WriteLine("Error\nInvalid path: {0}", pathToFolder);
                System.Environment.Exit(1);
            }

            InitFSWatcher( pathToFolder, "" );
            Console.WriteLine("file-watcher, version: {0}", pgm_version);
            Console.WriteLine("{0}", pgm_url);
            Console.WriteLine("");
            Console.WriteLine("Monitoring File System Activity on: {0}", pathToFolder);

            DateTime previous_t = DateTime.Now;
            DateTime current_t = DateTime.Now;

            while( true ) {
                System.Threading.Thread.Sleep(1000); // 1 second
                current_t = DateTime.Now;
                TimeSpan t = current_t - previous_t;
                int secs = (int)t.TotalSeconds;
                if( secs >= 60 ) {
                    if( dbg) Console.WriteLine("[{0}] monitoring folder: {1}", current_t.ToString(),pathToFolder);
                    previous_t = current_t;
                }
            }
        }
    }
}
