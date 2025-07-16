using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
  [Header("Références UI")]
  public TextMeshProUGUI userText;

  public Button boutonApprentissage;
  public Button boutonQuiz;
  public Button boutonDialogue;
  public Button boutonRecompenses;
  public Button boutonDeconnexion;

  private GestionnaireScenes gestionnaire;

  void Start()
  {
    // Trouver le gestionnaire de scène
    gestionnaire = Object.FindFirstObjectByType<GestionnaireScenes>();
    if (gestionnaire == null)
    {
      Debug.LogError("❌ GestionnaireScenes non trouvé !");
      return;
    }

    // Récupérer l'utilisateur connecté
    string username = PlayerPrefs.GetString("current_user", "Utilisateur inconnu");
    userText.text = "Utilisateur : " + username;

    // Ajouter les écouteurs de clics
    boutonApprentissage.onClick.AddListener(() => gestionnaire.AllerApprentissage());
    boutonQuiz.onClick.AddListener(() => gestionnaire.AllerQuiz());
    boutonDialogue.onClick.AddListener(() => gestionnaire.AllerConversation());
    boutonRecompenses.onClick.AddListener(() => gestionnaire.AllerClassements());
    if (boutonDeconnexion != null)
    boutonDeconnexion.onClick.AddListener(Deconnexion);
  }
void Deconnexion()
{
    PlayerPrefs.DeleteKey("current_user");
    PlayerPrefs.Save();
    GestionnaireScenes.Instance.AllerCreationProfil();
}

}
