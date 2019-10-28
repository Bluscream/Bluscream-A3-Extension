using System;
using System.Collections.Generic;

namespace Arma
{
    public class FileList
    {
        public System.DateTime StartTime { get; internal set; }
        public System.DateTime EndTime { get; set; }
        public string Error { get; set; }
        public Directory BaseDirectory { get; set; }

        public FileList(string basePath, bool recursive = false, bool calcMD5 = false)
        {
            StartTime = System.DateTime.UtcNow;
            BaseDirectory = new Directory(basePath, recursive, calcMD5);
            EndTime = System.DateTime.UtcNow;
        }
    }

    public class Directory
    {
        public string Path { get; set; }
        public List<Directory> Directories { get; set; } = new List<Directory>();
        public List<File> Files { get; set; } = new List<File>();
        public List<string> Errors { get; set; } = new List<string>();

        public Directory(string path, bool recursive, bool calcMD5)
        {
            Path = System.IO.Path.GetFullPath(path);
            try
            {
                if (recursive)
                {
                    foreach (string d in System.IO.Directory.GetDirectories(Path))
                    {
                        try
                        {
                            Directories.Add(new Directory(d, recursive, calcMD5));
                        }
                        catch (Exception ex)
                        {
                            Errors.Add(ex.Message);
                        }
                    }
                }
                foreach (string f in System.IO.Directory.GetFiles(Path))
                {
                    Files.Add(new File(f, calcMD5));
                }
            }
            catch (Exception ex)
            {
                Errors.Add(ex.Message);
            }
        }
    }

    public class File
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public long SizeBytes { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Modified { get; set; }

        // public System.DateTime Accessed { get; set; }
        public string MD5 { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public File(string path, bool calcMD5 = false)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Extension = System.IO.Path.GetExtension(path)?.Replace(".", "").ToLower();
            try
            {
                var fileInfo = new System.IO.FileInfo(path);
                SizeBytes = fileInfo.Length;
                Created = fileInfo.CreationTimeUtc;
                Modified = fileInfo.LastWriteTimeUtc;
                // Accessed = fileInfo.LastAccessTimeUtc;
            }
            catch (Exception ex)
            {
                Errors.Add(ex.Message);
            }
            if (calcMD5)
            {
                try
                {
                    using (var md5 = System.Security.Cryptography.MD5.Create())
                    {
                        using (var stream = System.IO.File.OpenRead(path))
                        {
                            var hash = md5.ComputeHash(stream);
                            MD5 = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Errors.Add(ex.Message);
                }
            }
        }
    }

    public class DateTime
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }

        public DateTime(System.DateTime now)
        {
            Year = now.Year; Month = now.Month; Day = now.Day; Hour = now.Hour; Minute = now.Minute;
        }
    }
}