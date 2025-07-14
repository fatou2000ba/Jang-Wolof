using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class QuizImageValidator : EditorWindow
{
    [MenuItem("Tools/Vérifier les images du quiz")]
    public static void CheckMissingImages()
    {
        string path = "Assets/Resources/QuizData/quiz.json";
        if (!File.Exists(path))
        {
            Debug.LogError("❌ Fichier quiz.json introuvable dans Resources/QuizData/");
            return;
        }

        string jsonContent = File.ReadAllText(path);
        string wrappedJson = "{ \"array\": " + jsonContent + "}";
        QuizWrapper wrapper = JsonUtility.FromJson<QuizWrapper>(wrappedJson);

        HashSet<string> allImages = new HashSet<string>();
        foreach (var q in wrapper.array)
        {
            allImages.Add(q.imageCorrecte);
            foreach (string p in q.propositions)
                allImages.Add(p);
        }

        List<string> missing = new List<string>();

        foreach (string imgName in allImages)
        {
            var sprite = Resources.Load<Sprite>("MotsImages/" + imgName);
            if (sprite == null)
            {
                missing.Add(imgName);
            }
        }

        if (missing.Count == 0)
        {
            Debug.Log("✅ Toutes les images sont présentes dans Resources/MotsImages/");
        }
        else
        {
            Debug.LogWarning("⚠️ Images manquantes :");
            foreach (var name in missing)
            {
                Debug.LogWarning("- " + name);
            }
        }
    }

    [System.Serializable]
    public class QuizItem
    {
        public string motWolof;
        public string imageCorrecte;
        public List<string> propositions;
    }

    [System.Serializable]
    public class QuizWrapper
    {
        public List<QuizItem> array;
    }
}
