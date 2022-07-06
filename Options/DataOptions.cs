﻿namespace FileManager.Options
{
    public record DataOptions
    {
        public string BasePath { get; set; }

        public string TempPath { get; set; }

        public string[] AllowedTypes { get; set; }
    }
}
