using UnityEngine;

[RequireComponent(typeof(GestionnaireScenes))]
public class SceneNameValidator : MonoBehaviour
{
    void Start()
    {
        GestionnaireScenes g = GetComponent<GestionnaireScenes>();

        CheckSceneName(g.sceneCommencer, "sceneCommencer");
        CheckSceneName(g.sceneMenu, "sceneMenu");
        CheckSceneName(g.sceneApprentissage, "sceneApprentissage");
        CheckSceneName(g.sceneQuiz, "sceneQuiz");
        CheckSceneName(g.sceneDialogue, "sceneDialogue");
        CheckSceneName(g.sceneClassements, "sceneClassements");
        CheckSceneName(g.sceneCreationProfil, "sceneCreationProfil");
    }

    void CheckSceneName(string sceneName, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError($"❌ Le champ '{fieldName}' (nom de scène) n'est pas renseigné !");
        }
        else
        {
            Debug.Log($"✅ {fieldName} = {sceneName}");
        }
    }
}
