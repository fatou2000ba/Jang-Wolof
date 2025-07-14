using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GestionnaireApprentissage : MonoBehaviour
{
    [Header("Références UI")]
    public Image imageMot;
    public TextMeshProUGUI texteQuestion;
    public Button[] boutonsReponses;
    public AudioSource audioSource;

    private MotData[] mots;
    private MotData motActuel;
    private int indexActuel = 0;

    void Start()
    {
        ChargerMots();
        AfficherMot(0);
    }

    void ChargerMots()
    {
        TextAsset json = Resources.Load<TextAsset>("Mots/mots");
        mots = JsonHelper.FromJson<MotData>(json.text);
    }

  void AfficherMot(int index)
{
    motActuel = mots[index];
    texteQuestion.text = $"Traduction de ({motActuel.fr})";

    Sprite img = Resources.Load<Sprite>("MotsImages/" + motActuel.image);
    imageMot.sprite = img;

    AudioClip son = Resources.Load<AudioClip>("Audios/" + motActuel.audio);
    audioSource.clip = son;

    List<string> reponsesMelangees = new List<string>(motActuel.wolof);
    reponsesMelangees = reponsesMelangees.OrderBy(r => Random.value).ToList();

    for (int i = 0; i < boutonsReponses.Length; i++)
    {
        Button bouton = boutonsReponses[i];

        if (i < reponsesMelangees.Count)
        {
            string rep = reponsesMelangees[i];
            bouton.interactable = true;
            bouton.gameObject.SetActive(true);

            bouton.GetComponentInChildren<TextMeshProUGUI>().text = rep;
            bouton.image.color = Color.white;
            bouton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

            bouton.onClick.RemoveAllListeners();

            // CLOSURE FIX : capturer dans une variable locale
            string reponseCapturee = rep;
            bouton.onClick.AddListener(() =>
            {
                bool bonne = reponseCapturee == motActuel.bonneReponse;
                Reponse(bonne, bouton);
            });
        }
        else
        {
            bouton.gameObject.SetActive(false); // Masquer s'il y a moins de réponses
        }
    }
}

   void Reponse(bool estBonne, Button bouton)
{
    var texteTMP = bouton.GetComponentInChildren<TextMeshProUGUI>();

    if (estBonne)
    {
        Debug.Log("Bonne réponse !");
        bouton.image.color = Color.green;
        texteTMP.color = Color.white;
        audioSource.Play();
    }
    else
    {
        Debug.Log("Mauvaise réponse !");
        bouton.image.color = Color.red;
        texteTMP.color = Color.white;
    }

    // Désactiver tous les boutons
    foreach (var b in boutonsReponses)
        b.interactable = false;

    StartCoroutine(AttendreEtPasserAuSuivant());
}

    IEnumerator AttendreEtPasserAuSuivant()
    {
        yield return new WaitForSeconds(1.5f);
        AfficherMotSuivant();
    }

    void AfficherMotSuivant()
    {
        indexActuel++;

        if (indexActuel >= mots.Length)
        {
            Debug.Log("✅ Tous les mots ont été affichés !");
            return;
        }

        AfficherMot(indexActuel);
    }
}
