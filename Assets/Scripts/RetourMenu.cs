using UnityEngine;
using UnityEngine.UI;

public class RetourMenu : MonoBehaviour
{
    void Start()
    {
        Button bouton = GetComponent<Button>();
        if (bouton == null)
        {
            Debug.LogError("❌ Le bouton Retour au menu n’a pas de composant Button !");
            return;
        }

        bouton.onClick.AddListener(() =>
        {
            if (GestionnaireScenes.Instance != null)
            {
                GestionnaireScenes.Instance.AllerMenuPrincipal();
            }
            else
            {
                Debug.LogError("❌ GestionnaireScenes est introuvable !");
            }
        });
    }
}
