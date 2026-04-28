using System.Collections;
using UnityEngine;
using TMPro;

public class TypingEffectSimple : MonoBehaviour
{
    public float typeSpeed = 0.05f;
    private TextMeshProUGUI textMesh;
    private string[] words;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        words = textMesh.text.Split(' ');
        textMesh.text = "";
        StartCoroutine(TypeWords());
    }

    IEnumerator TypeWords()
    {
        foreach (string word in words)
        {
            textMesh.text += word + " ";
            yield return new WaitForSeconds(typeSpeed * 5);  // Word delay
        }
    }
}