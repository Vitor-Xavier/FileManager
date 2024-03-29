﻿namespace FileManager.Options
{
    public class MediaOptions
    {
        public string BasePath { get; set; }

        public string TempPath { get; set; }

        public long FileSizeLimit { get; set; }

        public string[] AllowedTypes { get; set; }
    }
}
