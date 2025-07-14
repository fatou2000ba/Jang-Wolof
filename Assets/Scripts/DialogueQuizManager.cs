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

    [Header("UI")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI chronoText;
    public Button[] optionButtons;
    public Button retryButton;
    public Button menuButton;

    [Header("Timer")]
    public float timePerQuestion = 10f;

    private List<DialogueQuestion> questions;
    private int currentIndex = 0;
    private float timer;
    private bool answered = false;

  // void Start()
  // {
  //     retryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
  //     menuButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));

  //     retryButton.gameObject.SetActive(false);
  //     menuButton.gameObject.SetActive(false);
  //     feedbackText.text = "";

  //     LoadQuestions();
  //     ShowQuestion();
  // }

void Start()
{
    if (feedbackText == null) Debug.LogError("feedbackText n'est pas assigné !");
    if (chronoText == null) Debug.LogError("chronoText n'est pas assigné !");
    if (questionText == null) Debug.LogError("questionText n'est pas assigné !");
    if (optionButtons == null || optionButtons.Length == 0) Debug.LogError("optionButtons non assignés !");
    if (retryButton == null) Debug.LogError("retryButton non assigné !");
    if (menuButton == null) Debug.LogError("menuButton non assigné !");

    // suite normale
    retryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
    menuButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));

    retryButton.gameObject.SetActive(false);
    menuButton.gameObject.SetActive(false);
    feedbackText.text = "";

    LoadQuestions();
    ShowQuestion();
}

  void LoadQuestions()
    {
        TextAsset json = Resources.Load<TextAsset>("Dialogues/dialogues");

        if (json == null)
        {
            Debug.LogError("❌ Fichier JSON non trouvé dans Resources/Dialogues/dialogues.json");
            return;
        }

        DialogueQuestionList data = JsonUtility.FromJson<DialogueQuestionList>("{\"items\":" + json.text + "}");
        questions = data.items;
    }

    void ShowQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            questionText.text = "✅ Fin du quiz de dialogues !";
            feedbackText.text = "";
            retryButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);
            return;
        }

        DialogueQuestion q = questions[currentIndex];
        questionText.text = q.question;
        feedbackText.text = "";
        answered = false;
        timer = timePerQuestion;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < q.options.Count)
            {
                string answer = q.options[i];
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answer;

                int index = i;
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
            feedbackText.text = current.feedback.correct;
            HighlightButton(selected, Color.green);
        }
        else
        {
            feedbackText.text = current.feedback.incorrect;
            HighlightButton(selected, Color.red);
            HighlightButton(current.correctAnswer, Color.green);
        }

        StartCoroutine(NextQuestionDelayed());
    }

    void HighlightButton(string answerText, Color color)
    {
        foreach (var btn in optionButtons)
        {
            if (btn.GetComponentInChildren<TextMeshProUGUI>().text == answerText)
            {
                btn.image.color = color;
            }
        }
    }

    IEnumerator NextQuestionDelayed()
    {
        yield return new WaitForSeconds(2.5f);
        currentIndex++;
        ShowQuestion();
    }

    void Update()
    {
        if (answered || currentIndex >= questions.Count) return;

        timer -= Time.deltaTime;
        chronoText.text = "Temps : " + Mathf.Ceil(timer).ToString();

        if (timer <= 0f)
        {
            answered = true;
            feedbackText.text = "⏰ Temps écoulé !";
            HighlightButton(questions[currentIndex].correctAnswer, Color.green);

            foreach (var btn in optionButtons)
                btn.interactable = false;

            StartCoroutine(NextQuestionDelayed());
        }
    }

    [System.Serializable]
    public class DialogueQuestionList
    {
        public List<DialogueQuestion> items;
    }
}
