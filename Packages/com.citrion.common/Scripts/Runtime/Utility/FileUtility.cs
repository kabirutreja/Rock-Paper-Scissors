using System;
using System.Diagnostics;
using System.IO;

namespace CitrioN.Common
{
  public static class FileUtility
  {
    public static string GetFileDirectory(string filePath)
    {
      if (string.IsNullOrEmpty(filePath)) { return string.Empty; }
      return new FileInfo(filePath).Directory.FullName;
    }

    public static void OpenFileDirectory(string directoryPath)
    {
      if (Directory.Exists(directoryPath))
      {
        Process.Start($@"{directoryPath}");
      }
      else
      {
        ConsoleLogger.LogWarning($"Unable to open directory '{directoryPath}' because it doesn't exist.");
      }
    }

    public static void DeleteFile(string path)
    {
      if (File.Exists(path))
      {
        File.Delete(path);
        ConsoleLogger.Log($"Successfully deleted file at {path}");
      }
      else
      {
        ConsoleLogger.LogWarning($"No file found to delete at {path}");
      }
    }

    public static string RemoveFileExtension(string path)
    {
      if (string.IsNullOrEmpty(path)) { return string.Empty; }
      var fileExtension = Path.GetExtension(path);
      return path.Substring(0, path.Length - fileExtension.Length);
    }

    public static bool IsValidPathString(string path)
    {
      try
      {
        var fullPath = Path.GetFullPath(path);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public static bool CreateFileBackup(string originalFilePath, string extraPath = "Backups/", 
                                        bool allowExistingFilesOverride = false, bool useBakFormat = true)
    {
      if (string.IsNullOrEmpty(originalFilePath)) { return false; }

      var fileExtension = Path.GetExtension(originalFilePath);
      var filePathWithoutExtension = RemoveFileExtension(originalFilePath);
      var fileName = Path.GetFileNameWithoutExtension(originalFilePath);
      var pathWithoutFileName = filePathWithoutExtension.Substring(0, filePathWithoutExtension.Length - fileName.Length);

      var backupPath = $"{pathWithoutFileName}{extraPath}{fileName}{(useBakFormat ? ".bak" : fileExtension)}";

      if (File.Exists(backupPath))
      {
        backupPath = $"{pathWithoutFileName}{extraPath}{fileName}-{DateTime.UtcNow.ToString("yyyy-dd-MM--HH-mm-ss")}{(useBakFormat ? ".bak" : fileExtension)}";
      }

      if (File.Exists(backupPath) && !allowExistingFilesOverride)
      {
        ConsoleLogger.LogWarning($"Could not create a backup for {originalFilePath} because the backup path {backupPath} already exists.");
        return false;
      }

      if (!IsValidPathString(backupPath))
      {
        ConsoleLogger.LogWarning($"Could not create a backup for {originalFilePath} because the backup path {backupPath} is not a valid path.");
        return false;
      }

      string directoryPath = Path.GetDirectoryName(backupPath);

      if (!Directory.Exists(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }

      if (!Directory.Exists(directoryPath))
      {
        ConsoleLogger.LogWarning($"Could not create a backup for {originalFilePath} because the directory {directoryPath} could not be created.");
        return false;
      }

      try
      {
        var lines = File.ReadAllLines(originalFilePath);
        File.WriteAllLines(backupPath, lines);
      }
      catch (Exception e)
      {
        ConsoleLogger.LogError($"Could not create a backup for {originalFilePath} because the following error occured: {e}");
        return false;
        //throw;
      }

      return true;
    }
  }
}