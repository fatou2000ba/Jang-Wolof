using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GestionnaireScenes : MonoBehaviour
{
    [Header("Noms des Scènes")]
    public string sceneCommencer = "Commencer";
    public string sceneMenu = "Menu";
    public string sceneApprentissage = "Apprentissage";
    public string sceneQuiz = "Quiz";
    public string sceneDialogue = "Dialogue";
    public string sceneClassements = "Progression";
    public string sceneCreationProfil = "Accueil";
    
    [Header("Animation de Transition")]
    public bool avecAnimation = true;
    public float dureeTransition = 0.5f;
    
    // Instance singleton (optionnel)
    public static GestionnaireScenes Instance;

    //    void Awake()
    // {
    //     if (string.IsNullOrWhiteSpace(sceneApprentissage))
    //         Debug.LogWarning("⚠️ Le champ 'sceneApprentissage' n'est pas renseigné !");

    //     // Singleton - une seule instance
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }


    // ========== FONCTIONS PUBLIQUES POUR LES BOUTONS ==========
 
 void Awake()
{
    // Singleton amélioré
    if (Instance == null)
    {
        // Vérifiez si cette instance est bien configurée
        if (string.IsNullOrWhiteSpace(sceneApprentissage))
        {
            Debug.LogError("❌ Instance mal configurée dans : " + SceneManager.GetActiveScene().name);
            Debug.LogError("Ce GestionnaireScenes ne devrait pas exister ou être configuré !");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("✅ GestionnaireScenes configuré et actif");
    }
    else
    {
        Debug.Log("❌ Instance dupliquée supprimée dans : " + SceneManager.GetActiveScene().name);
        Destroy(gameObject);
    }
} 
    public void AllerMenuPrincipal()
    {
        Debug.Log("Aller au menu principal");
        ChargerScene(sceneMenu);
    }
    
    public void AllerMainMenu()
    {
        ChargerScene(sceneCommencer);
    }

    // public void AllerApprentissage()
    // {
    //     ChargerScene(sceneApprentissage);
    // }
    
    public void AllerApprentissage()
{
    Debug.Log("=== DEBUG AllerApprentissage ===");
    Debug.Log("Scène actuelle : " + SceneManager.GetActiveScene().name);
    Debug.Log("GameObject : " + gameObject.name);
    Debug.Log("sceneApprentissage : '" + sceneApprentissage + "'");
    Debug.Log("Instance actuelle : " + (Instance != null ? Instance.gameObject.name : "null"));
    
    if (string.IsNullOrEmpty(sceneApprentissage))
    {
        Debug.LogError("❌ sceneApprentissage est vide !");
        return;
    }
    
    ChargerScene(sceneApprentissage);
}
    
    public void AllerQuiz()
    {
        ChargerScene(sceneQuiz);
    }
    
    public void AllerConversation()
    {
        ChargerScene(sceneDialogue);
    }
    
    public void AllerClassements()
    {
        ChargerScene(sceneClassements);
    }
    
    public void AllerCreationProfil()
    {
        ChargerScene(sceneCreationProfil);
    }

    // ========== FONCTIONS INTERNES ==========

    //  void ChargerScene(string nomScene)
    // {
    //     if (string.IsNullOrEmpty(nomScene))
    //     {
    //         Debug.LogError("❌ Nom de scène vide !");
    //         return;
    //     }

    //     Debug.Log("Chargement de la scène : " + nomScene);

    //     if (avecAnimation)
    //     {
    //         StartCoroutine(ChargerAvecAnimation(nomScene));
    //     }
    //     else
    //     {
    //         SceneManager.LoadScene(nomScene);
    //     }
    // }

  
  void ChargerScene(string nomScene)
{
    if (string.IsNullOrEmpty(nomScene))
    {
        Debug.LogError("❌ Nom de scène vide !");
        return;
    }

    // Vérification que l'objet n'est pas détruit
    if (this == null)
    {
        Debug.LogError("❌ GestionnaireScenes détruit !");
        return;
    }

    Debug.Log("Chargement de la scène : " + nomScene);

    if (avecAnimation)
    {
        StartCoroutine(ChargerAvecAnimation(nomScene));
    }
    else
    {
        SceneManager.LoadScene(nomScene);
    }
}  
    IEnumerator ChargerAvecAnimation(string nomScene)
    {
        // Animation de sortie (fade out)
        yield return StartCoroutine(FadeOut());
        
        // Charger la nouvelle scène
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nomScene);
        
        // Attendre que la scène soit chargée
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // Animation d'entrée (fade in)
        yield return StartCoroutine(FadeIn());
    }
    
    IEnumerator FadeOut()
    {
        // Créer un overlay noir pour la transition
        GameObject overlay = CreerOverlayTransition();
        CanvasGroup cg = overlay.GetComponent<CanvasGroup>();
        
        float temps = 0f;
        while (temps < dureeTransition)
        {
            temps += Time.deltaTime;
            cg.alpha = temps / dureeTransition;
            yield return null;
        }
    }
    
    IEnumerator FadeIn()
    {
        GameObject overlay = GameObject.Find("TransitionOverlay");
        if (overlay != null)
        {
            CanvasGroup cg = overlay.GetComponent<CanvasGroup>();
            
            float temps = 0f;
            while (temps < dureeTransition)
            {
                temps += Time.deltaTime;
                cg.alpha = 1f - (temps / dureeTransition);
                yield return null;
            }
            
            Destroy(overlay);
        }
    }
    
    GameObject CreerOverlayTransition()
    {
        GameObject overlay = new GameObject("TransitionOverlay");
        
        Canvas canvas = overlay.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        
        CanvasGroup cg = overlay.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(overlay.transform);
        
        UnityEngine.UI.Image image = panel.AddComponent<UnityEngine.UI.Image>();
        image.color = Color.black;
        
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        
        return overlay;
    }
    
    // ========== FONCTIONS UTILES ==========
    
    public void RedemarrerJeu()
    {
        ChargerScene(sceneCommencer);
    }
    
    public void QuitterJeu()
    {
        Debug.Log("Quitter le jeu");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public string ObtenirSceneActuelle()
    {
        return SceneManager.GetActiveScene().name;
    }
}