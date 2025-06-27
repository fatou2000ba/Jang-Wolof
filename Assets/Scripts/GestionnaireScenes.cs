using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GestionnaireScenes : MonoBehaviour
{
    [Header("Noms des Scènes")]
    public string sceneMainMenu = "MainMenu";
    public string sceneMenuPrincipal = "MenuPrincipal";
    public string sceneApprentissage = "Apprentissage";
    public string sceneQuiz = "Quiz";
    public string sceneConversation = "Conversation";
    public string sceneClassements = "Classements";
    public string sceneCreationProfil = "CreationProfil";
    
    [Header("Animation de Transition")]
    public bool avecAnimation = true;
    public float dureeTransition = 0.5f;
    
    // Instance singleton (optionnel)
    public static GestionnaireScenes Instance;
    
    void Awake()
    {
        // Singleton - une seule instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // ========== FONCTIONS PUBLIQUES POUR LES BOUTONS ==========
    
    public void AllerMenuPrincipal()
    {
        ChargerScene(sceneMenuPrincipal);
    }
    
    public void AllerMainMenu()
    {
        ChargerScene(sceneMainMenu);
    }
    
    public void AllerApprentissage()
    {
        ChargerScene(sceneApprentissage);
    }
    
    public void AllerQuiz()
    {
        ChargerScene(sceneQuiz);
    }
    
    public void AllerConversation()
    {
        ChargerScene(sceneConversation);
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
    
    void ChargerScene(string nomScene)
    {
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
        ChargerScene(sceneMainMenu);
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