#if UNITY_EDITOR

using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Words
{
    public abstract class AbstractGlossaryBuilder : MonoBehaviour
    {
        [SerializeField] private TextAsset inputFile;
        
        protected abstract string FolderName { get; }
        
        private const string OutputPath = "Assets/Dictionaries/{0}/{1}.json";

        protected abstract void Init();

        protected abstract object Object { get; }

        [ContextMenu("Build")]
        public void Build()
        {
            Init();
            
            var lines = inputFile.text.Split('\n');
            Debug.Log($"{lines.Length} lines are read from the file.");
            
            var wordsAdded = 0;
            var skippedLines = 0;

            foreach (var line in lines)
            {
                var word = line.Trim().ToLowerInvariant();
                
                AddWord(word);
                    
                if (string.IsNullOrEmpty(word) || word.StartsWith("#") || !word.All(char.IsLetter))
                {
                    skippedLines++;
                    continue;
                }
                
                wordsAdded++;
            }
            
            Debug.Log($"Added {wordsAdded} words, skipped {skippedLines} lines");
            
            if (wordsAdded == 0)
            {
                Debug.LogWarning("No words been added!");
                EditorUtility.DisplayDialog("Warning!", "No words been added!", "OK");
                return;
            }
            
            var outputPath = string.Format(OutputPath, FolderName, inputFile.name);

            var directory = Path.GetDirectoryName(outputPath);

            if (directory == null)
            {
                Debug.LogWarning("Directory is null!");
                EditorUtility.DisplayDialog("Warning!", "Directory is null!", "OK");
                return;
            }
            
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            };
            
            File.WriteAllText(outputPath, JsonConvert.SerializeObject(Object, settings), Encoding.UTF8);
            Debug.Log($"JSON saved in: {outputPath}");
        }

        protected abstract void AddWord(string word);
    }
}

#endif