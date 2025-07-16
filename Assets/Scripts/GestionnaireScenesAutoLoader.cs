using UnityEngine;

public class GestionnaireScenesAutoLoader : MonoBehaviour
{
    public GameObject gestionnairePrefab;

    void Awake()
    {
        if (GestionnaireScenes.Instance == null)
        {
            Debug.Log("🛠️ Création automatique du GestionnaireScenes");
            Instantiate(gestionnairePrefab);
        }
    }
}
