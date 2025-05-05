using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using TopDownGame.Models;

namespace TopDownGame.Services
{
    public static class SaveLoadService
    {
        private static readonly string SaveDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "MyGameSaves");

        public static void SaveGame(GameSaveData data, string saveName)
        {
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }

            string savePath = Path.Combine(SaveDirectory, $"{saveName}.sav");
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                formatter.Serialize(stream, data);
            }
        }

        public static GameSaveData LoadGame(string saveName)
        {
            string savePath = Path.Combine(SaveDirectory, $"{saveName}.sav");

            if (!File.Exists(savePath))
                return null;

            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(savePath, FileMode.Open))
            {
                return (GameSaveData)formatter.Deserialize(stream);
            }
        }

        public static bool SaveExists(string saveName)
        {
            string savePath = Path.Combine(SaveDirectory, $"{saveName}.sav");
            return File.Exists(savePath);
        }
    }
}