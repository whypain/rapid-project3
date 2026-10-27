using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SoundHelper
{
    [MenuItem("Assets/Create/Sound Config From Selected")]
    public static void CreateSoundConfigFromSelected()
    {
        List<string> ListAudioClips(string folderPath)
        {
            string[] audioClipGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { folderPath });
            for (int i = 0; i < audioClipGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(audioClipGuids[i]);
                audioClipGuids[i] = path;
            }

            return audioClipGuids.ToList();
        }
        void AddPathRange(ref List<string> paths, List<string> pathsToAdd)
        {
            foreach (string path in pathsToAdd)
            {
                AddPath(ref paths, path);
            }
        }
        void AddPath(ref List<string> paths, string path)
        {
            if (!paths.Contains(path))
            {
                paths.Add(path);
            }
        }

        List<string> clips = new List<string>();
        foreach (Object obj in Selection.objects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(assetPath))
            {
                if (Directory.Exists(assetPath))
                {
                    AddPathRange(ref clips, ListAudioClips(assetPath));
                }
                else if (File.Exists(assetPath) && clips.Contains(assetPath) == false)
                {
                    AddPath(ref clips, assetPath);
                }
            }
        }

        string configDir = "Assets/_Main/Audio/_SoundConfigs";
        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }

        foreach (string clipPath in clips)
        {
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(clipPath);
            if (clip != null)
            {
                SoundConfig config = ScriptableObject.CreateInstance<SoundConfig>();
                string configPath = Path.Combine(configDir, $"{clip.name}.asset");
                config.clip = clip;
                config.volume = 0.5f;
                config.randomPitchRange = new Vector2(1f, 1f);
                AssetDatabase.CreateAsset(config, configPath);
            }
        }
    }    
}
