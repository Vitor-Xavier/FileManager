namespace FileManager.Helpers
{
    public static class FileManagerHelper
    {
        public static string GetContentDisposition(string fileName, string disposition) => new System.Net.Mime.ContentDisposition
        {
            FileName = fileName,
            DispositionType = disposition,
        }.ToString();
    }
}
