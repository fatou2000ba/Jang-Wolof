using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ResponsiveUI : MonoBehaviour
{
    void Start()
    {
        AdjustForScreenSize();
    }
    
    void AdjustForScreenSize()
    {
        float screenRatio = (float)Screen.width / Screen.height;

        if (screenRatio < 0.6f) // Écrans très étroits (par ex : téléphone en portrait)
        {
            // Réduire légèrement la taille pour ne pas dépasser
            GetComponent<RectTransform>().localScale = Vector3.one * 0.9f;
        }
        else
        {
            GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }
}
