using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NavMeshGrid
{
    public static class NavMeshGridBackupHelper
    {
        private const string BackUpPath = "Assets/Resources/";

        public static void MadeBackup(NavMeshGridNode[] nodes)
        {
            var date = DateTime.Now.ToString()
                .Replace('.', '-')
                .Replace(' ', '_')
                .Replace(':', '-');
            
            var fileName = $"{BackUpPath}Backup-{date}.json";

            var fileStream = File.Create(fileName);

            var nodesModels = nodes
                .Select(x => new NavMeshGridNodeModel() 
                { 
                    Index = x.Index, 
                    Position = x.PositionWithoutOffset, 
                    Offset = x.CustomOffset 
                })
                .ToArray();

            var json = JsonHelper.ArrayToJson(nodesModels);
            Debug.Log($"Backup was made:\n Number of saved nodes - { nodesModels.Length }");

            var streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(json);
            streamWriter.Close();

            fileStream.Close();
        }

        public static bool LoadModelFromFile(string backupName, out NavMeshGridNodeModel[] readedNodesModels)
        {
            try
            {
                var backupFile = File.Open($"{BackUpPath}{backupName}.json", FileMode.Open, FileAccess.Read);

                var streamReader = new StreamReader(backupFile);
                var readedJson = streamReader.ReadToEnd();

                var nodesModels = JsonHelper.ArrayFromJson<NavMeshGridNodeModel>(readedJson);

                readedNodesModels = nodesModels;

                return true;
            }
            catch(Exception ex)
            {
                Debug.Log($"ERROR: Can't load backup with name \"{backupName}\"\n {ex.Message}");
                readedNodesModels = null;
                return false;
            }
        }
    }
}