namespace TinyCity.Services
{
    public class FileBackupService
    {
        public void CreateBackup(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            string backupPath = filePath + ".bak";
            File.Copy(filePath, backupPath, overwrite: true);
        }
    }
}
