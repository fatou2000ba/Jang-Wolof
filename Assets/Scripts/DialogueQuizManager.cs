using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueQuizManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueQuestion
    {
        public string question;
        public List<string> options;
        public string correctAnswer;
        public Feedback feedback;
    }

    [System.Serializable]
    public class Feedback
    {
        public string correct;
        public string incorrect;
    }

    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI chronoText;
    public TextMeshProUGUI scoreText; // Nouveau : affichage du score
    public Button[] optionButtons;
    public Button retryButton;
    public Button menuButton;
    public GameObject endGamePanel; // Panneau de fin de jeu

    [Header("Timer Settings")]
    public float timePerQuestion = 10f;
    
    [Header("Audio (Optional)")]
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip timeUpSound;
    private AudioSource audioSource;

    [Header("Score Settings")]
    public int pointsPerCorrectAnswer = 10;
    public int bonusTimePoints = 1; // Points bonus par seconde restante

    [Header("Quiz Settings")]
    public bool shuffleQuestions = true; // Mélanger les questions
    public bool shuffleAnswers = true; // Mélanger les réponses
    public int maxQuestionsToShow = 10; // Limiter le nombre de questions (-1 = toutes)

    private List<DialogueQuestion> questions;
    private int currentIndex = 0;
    private int score = 0;
    private int correctAnswers = 0;
    private float timer;
    private bool answered = false;
    private bool quizCompleted = false;

    void Start()
    {
        ValidateComponents();
        InitializeUI();
        LoadQuestions();
        ShowQuestion();
    }

    void ValidateComponents()
    {
        bool hasErrors = false;
        
        if (feedbackText == null) 
        {
            Debug.LogError("❌ feedbackText n'est pas assigné !");
            hasErrors = true;
        }
        if (chronoText == null) 
        {
            Debug.LogError("❌ chronoText n'est pas assigné !");
            hasErrors = true;
        }
        if (questionText == null) 
        {
            Debug.LogError("❌ questionText n'est pas assigné !");
            hasErrors = true;
        }
        if (optionButtons == null || optionButtons.Length == 0) 
        {
            Debug.LogError("❌ optionButtons non assignés !");
            hasErrors = true;
        }
        if (retryButton == null) 
        {
            Debug.LogError("❌ retryButton non assigné !");
            hasErrors = true;
        }
        if (menuButton == null) 
        {
            Debug.LogError("❌ menuButton non assigné !");
            hasErrors = true;
        }
        
        // Vérifier les boutons d'options
        if (optionButtons != null)
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (optionButtons[i] == null)
                {
                    Debug.LogError($"❌ optionButtons[{i}] est null !");
                    hasErrors = true;
                }
                else if (optionButtons[i].GetComponentInChildren<TextMeshProUGUI>() == null)
                {
                    Debug.LogError($"❌ optionButtons[{i}] n'a pas de TextMeshProUGUI enfant !");
                    hasErrors = true;
                }
            }
        }
        
        if (hasErrors)
        {
            Debug.LogError("🔴 ERREUR: Veuillez assigner tous les composants UI dans l'inspecteur !");
            enabled = false; // Désactiver le script si des composants manquent
            return;
        }
        
        // Initialiser l'AudioSource si disponible
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (correctSound || incorrectSound || timeUpSound))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        Debug.Log("✅ Tous les composants sont correctement assignés");
    }

    void InitializeUI()
    {
        retryButton.onClick.AddListener(RestartQuiz);
        menuButton.onClick.AddListener(GoToMenu);

        retryButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        
        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        feedbackText.text = "";
        UpdateScoreDisplay();
    }

    void RestartQuiz()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void LoadQuestions()
    {
        TextAsset json = Resources.Load<TextAsset>("Dialogues/dialogues");

        if (json == null)
        {
            Debug.LogError("❌ Fichier JSON non trouvé dans Resources/Dialogues/dialogues.json");
            ShowError("Erreur : Fichier de questions non trouvé !");
            return;
        }

        try
        {
            // Le fichier JSON est déjà un tableau, pas besoin d'ajouter "items"
            DialogueQuestionList data = JsonUtility.FromJson<DialogueQuestionList>("{\"items\":" + json.text + "}");
            questions = data.items;
            
            if (questions == null || questions.Count == 0)
            {
                ShowError("Erreur : Aucune question trouvée !");
                return;
            }
            
            // Mélanger les questions pour plus de variété (optionnel)
            if (shuffleQuestions)
            {
                ShuffleQuestions();
            }
            
            Debug.Log($"✅ {questions.Count} questions en wolof chargées avec succès");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Erreur lors du parsing JSON : {e.Message}");
            ShowError("Erreur : Format de fichier invalide !");
        }
    }

    void ShowError(string errorMessage)
    {
        questionText.text = errorMessage;
        feedbackText.text = "Veuillez vérifier la configuration du quiz.";
        retryButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
    }

    void ShowQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            EndQuiz();
            return;
        }

        DialogueQuestion q = questions[currentIndex];
        questionText.text = $"Question {currentIndex + 1}/{questions.Count}\n\n{q.question}";
        feedbackText.text = "";
        answered = false;
        timer = timePerQuestion;

        SetupAnswerButtons(q);
    }

    void SetupAnswerButtons(DialogueQuestion q)
    {
        List<string> answersToShow = new List<string>(q.options);
        
        // Mélanger les réponses si activé
        if (shuffleAnswers)
        {
            for (int i = 0; i < answersToShow.Count; i++)
            {
                string temp = answersToShow[i];
                int randomIndex = Random.Range(i, answersToShow.Count);
                answersToShow[i] = answersToShow[randomIndex];
                answersToShow[randomIndex] = temp;
            }
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < answersToShow.Count)
            {
                string answer = answersToShow[i];
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answer;

                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => SelectAnswer(answer));
                optionButtons[i].image.color = Color.white;
                optionButtons[i].interactable = true;
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void SelectAnswer(string selected)
    {
        if (answered) return;

        answered = true;
        DialogueQuestion current = questions[currentIndex];

        foreach (var btn in optionButtons)
            btn.interactable = false;

        if (selected == current.correctAnswer)
        {
            HandleCorrectAnswer(current);
        }
        else
        {
            HandleIncorrectAnswer(current, selected);
        }

        StartCoroutine(NextQuestionDelayed());
    }

    void HandleCorrectAnswer(DialogueQuestion current)
    {
        correctAnswers++;
        int basePoints = pointsPerCorrectAnswer;
        int timeBonus = Mathf.RoundToInt(timer * bonusTimePoints);
        int totalPoints = basePoints + timeBonus;
        
        score += totalPoints;
        
        if (feedbackText != null)
        {
            feedbackText.text = $"✅ {current.feedback.correct}\n+{totalPoints} points ({basePoints} + {timeBonus} bonus temps)";
            feedbackText.color = Color.green;
        }
        
        HighlightButton(current.correctAnswer, Color.green);
        PlaySound(correctSound);
        UpdateScoreDisplay();
    }

    void HandleIncorrectAnswer(DialogueQuestion current, string selected)
    {
        if (feedbackText != null)
        {
            feedbackText.text = $"❌ {current.feedback.incorrect}";
            feedbackText.color = Color.red;
        }
        
        HighlightButton(selected, Color.red);
        HighlightButton(current.correctAnswer, Color.green);
        PlaySound(incorrectSound);
    }

    void HandleTimeUp()
    {
        answered = true;
        
        if (feedbackText != null)
        {
            feedbackText.text = "⏰ Temps écoulé !";
            feedbackText.color = Color.orange;
        }
        
        if (questions != null && currentIndex < questions.Count)
        {
            HighlightButton(questions[currentIndex].correctAnswer, Color.green);
        }
        
        PlaySound(timeUpSound);

        if (optionButtons != null)
        {
            foreach (var btn in optionButtons)
            {
                if (btn != null)
                    btn.interactable = false;
            }
        }

        StartCoroutine(NextQuestionDelayed());
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void HighlightButton(string answerText, Color color)
    {
        foreach (var btn in optionButtons)
        {
            if (btn.GetComponentInChildren<TextMeshProUGUI>().text == answerText)
            {
                btn.image.color = color;
                break;
            }
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    IEnumerator NextQuestionDelayed()
    {
        yield return new WaitForSeconds(2.5f);
        currentIndex++;
        ShowQuestion();
    }

    void EndQuiz()
    {
        quizCompleted = true;
        float percentage = (float)correctAnswers / questions.Count * 100f;
        
        questionText.text = "🎉 Quiz Wolof Terminé !";
        feedbackText.text = $"Résultats:\n" +
                           $"• Score final: {score} points\n" +
                           $"• Bonnes réponses: {correctAnswers}/{questions.Count}\n" +
                           $"• Pourcentage: {percentage:F1}%\n" +
                           $"• {GetPerformanceMessage(percentage)}";
        
        feedbackText.color = GetPerformanceColor(percentage);
        
        if (endGamePanel != null)
            endGamePanel.SetActive(true);
            
        retryButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
        
        // Sauvegarder le meilleur score
        SaveBestScore();
    }

    void ShuffleQuestions()
    {
        for (int i = 0; i < questions.Count; i++)
        {
            DialogueQuestion temp = questions[i];
            int randomIndex = Random.Range(i, questions.Count);
            questions[i] = questions[randomIndex];
            questions[randomIndex] = temp;
        }
        
        // Limiter le nombre de questions si spécifié
        if (maxQuestionsToShow > 0 && maxQuestionsToShow < questions.Count)
        {
            questions = questions.GetRange(0, maxQuestionsToShow);
        }
        
        Debug.Log($"Questions mélangées. {questions.Count} questions sélectionnées.");
    }

    string GetPerformanceMessage(float percentage)
    {
        if (percentage >= 90f) return "Excellent ! 🌟";
        if (percentage >= 70f) return "Très bien ! 👍";
        if (percentage >= 50f) return "Bien ! 👌";
        return "Continuez vos efforts ! 💪";
    }

    Color GetPerformanceColor(float percentage)
    {
        if (percentage >= 70f) return Color.green;
        if (percentage >= 50f) return Color.yellow;
        return Color.red;
    }

    void SaveBestScore()
    {
        int bestScore = PlayerPrefs.GetInt("DialogueQuizBestScore", 0);
        if (score > bestScore)
        {
            PlayerPrefs.SetInt("DialogueQuizBestScore", score);
            PlayerPrefs.Save();
            Debug.Log($"🏆 Nouveau record ! Score: {score}");
        }
    }

    void Update()
    {
        if (answered || quizCompleted || currentIndex >= questions.Count) return;

        timer -= Time.deltaTime;
        
        // Mise à jour du chrono avec couleur (vérification null)
        if (chronoText != null)
        {
            int secondsLeft = Mathf.CeilToInt(timer);
            chronoText.text = $"Temps : {secondsLeft}s";
            
            // Changer la couleur selon le temps restant
            if (secondsLeft <= 3)
                chronoText.color = Color.red;
            else if (secondsLeft <= 5)
                chronoText.color = Color.yellow;
            else
                chronoText.color = Color.white;
        }

        if (timer <= 0f)
        {
            HandleTimeUp();
        }
    }

    // Classe pour la désérialisation JSON
    [System.Serializable]
    public class DialogueQuestionList
    {
        public List<DialogueQuestion> items;
    }

    // Méthodes publiques pour l'interface
    public void PauseQuiz()
    {
        Time.timeScale = 0f;
    }

    public void ResumeQuiz()
    {
        Time.timeScale = 1f;
    }

    public int GetCurrentScore()
    {
        return score;
    }

    public int GetCorrectAnswers()
    {
        return correctAnswers;
    }

    public float GetProgress()
    {
        return (float)currentIndex / questions.Count;
    }
}