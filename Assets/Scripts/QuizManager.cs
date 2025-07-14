using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class QuizItem
    {
        public string motWolof;
        public string imageCorrecte;
        public List<string> propositions;
    }

    public Image[] imageButtons;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI chronoText;
    public Button menuButton;
    public Button retryButton;

    public AudioSource audioSource;
    public float timePerQuestion = 10f;

    private List<QuizItem> questions;
    private QuizItem currentQuestion;
    private float timer;
    private int currentIndex = 0;
    private bool answered;

    void Start()
    {
        menuButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
        retryButton.onClick.AddListener(() => SceneManager.LoadScene("Quiz"));

        menuButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);

        LoadQuizData();
        ShowQuestion();
    }

    // void LoadQuizData()
    // {
    //     TextAsset json = Resources.Load<TextAsset>("QuizData/quiz");
    //     questions = JsonUtilityWrapper.FromJson<QuizItem>(json.text);
    // }
    void LoadQuizData()
{
    TextAsset json = Resources.Load<TextAsset>("QuizData/quiz");
    
    if (json == null)
    {
        Debug.LogError("❌ Le fichier quiz.json n'a pas été trouvé dans Resources/QuizData/");
        return;
    }

    questions = JsonUtilityWrapper.FromJson<QuizItem>(json.text);

    if (questions == null || questions.Count == 0)
    {
        Debug.LogError("❌ Le fichier quiz.json est vide ou mal formé.");
    }
}


   void ShowQuestion()
{
    if (currentIndex >= questions.Count)
    {
        questionText.text = "✅ Fin du quiz !";
        menuButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        return;
    }

    answered = false;
    timer = timePerQuestion;

    currentQuestion = questions[currentIndex];
    questionText.text = $"Trouve l’image qui correspond à : ({currentQuestion.motWolof})";

    var propositions = currentQuestion.propositions.OrderBy(x => Random.value).ToList();

    if (propositions.Count < imageButtons.Length)
    {
        Debug.LogError($"❌ Pas assez de propositions pour la question : {currentQuestion.motWolof}");
        return;
    }

    for (int i = 0; i < imageButtons.Length; i++)
    {
        string imgName = propositions[i];
        Sprite img = Resources.Load<Sprite>("MotsImages/" + imgName);

        if (img == null)
        {
            Debug.LogWarning($"⚠️ L'image {imgName} est manquante dans le dossier Resources/MotsImages/");
        }

        imageButtons[i].sprite = img;
        imageButtons[i].color = Color.white;

        int index = i; // capture locale pour le bouton
        Debug.Log($"[DEBUG] index: {i}");

if (imageButtons[i] == null)
{
    Debug.LogError($"❌ imageButtons[{i}] est null !");
    return;
}

Button btn = imageButtons[i].GetComponent<Button>();
if (btn == null)
{
    Debug.LogError($"❌ imageButtons[{i}] n'a pas de composant Button !");
    return;
}

        imageButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
        imageButtons[i].GetComponent<Button>().onClick.AddListener(() => SelectImage(imgName, imageButtons[index]));
        imageButtons[i].gameObject.SetActive(true);
    }
}

    void SelectImage(string imgClicked, Image btnImage)
    {
        if (answered) return;
        answered = true;

        bool correct = imgClicked == currentQuestion.imageCorrecte;

        if (correct)
        {
            btnImage.color = Color.green;
        }
        else
        {
            btnImage.color = Color.red;
            // montrer aussi la bonne réponse
            foreach (var img in imageButtons)
            {
                if (img.sprite.name == currentQuestion.imageCorrecte)
                    img.color = Color.green;
            }
        }

        StartCoroutine(NextQuestionDelayed());
    }

    IEnumerator NextQuestionDelayed()
    {
        yield return new WaitForSeconds(2f);
        currentIndex++;
        ShowQuestion();
    }

    void Update()
    {
        if (answered) return;

        timer -= Time.deltaTime;
        chronoText.text = "Temps : " + Mathf.Ceil(timer).ToString();

        if (timer <= 0f)
        {
            answered = true;
            foreach (var img in imageButtons)
                img.color = Color.red;

            StartCoroutine(NextQuestionDelayed());
        }
    }

    // pour parser le tableau JSON
    public static class JsonUtilityWrapper
    {
        public static List<T> FromJson<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public List<T> array;
        }
    }
}
