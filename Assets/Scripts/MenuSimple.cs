using UnityEngine;
using UnityEngine.UI;

public class MenuSimple : MonoBehaviour
{
    [Header("Références aux boutons")]
    public Button boutonCommencer;
    public Button boutonCreerProfil;

    private GestionnaireScenes gestionnaire;

    void Start()
{
    // Trouver automatiquement le gestionnaire de scènes (version moderne)
    gestionnaire = Object.FindFirstObjectByType<GestionnaireScenes>();

    if (gestionnaire == null)
    {
        Debug.LogError("❌ GestionnaireScenes non trouvé dans la scène !");
        return;
    }

    // Ajouter les fonctions de clic
    if (boutonCommencer != null)
        boutonCommencer.onClick.AddListener(QuandOnCliqueCommencer);

    if (boutonCreerProfil != null)
        boutonCreerProfil.onClick.AddListener(QuandOnCliqueCreerProfil);
}


    void QuandOnCliqueCommencer()
    {
        Debug.Log("🟢 Bouton 'Commencer' cliqué !");
        gestionnaire.AllerMenuPrincipal();
    }

    void QuandOnCliqueCreerProfil()
    {
        Debug.Log("🟡 Bouton 'Créer Profil' cliqué !");
        gestionnaire.AllerCreationProfil();
    }
}
