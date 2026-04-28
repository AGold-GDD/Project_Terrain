using System.Collections;
using UnityEngine;
using TMPro;

public class TypingEffect : MonoBehaviour
{
    [Header("Typing Settings")]
    public float typeSpeed = 0.05f;
    public KeyCode skipKey = KeyCode.Space;

    private TextMeshProUGUI textMesh;
    private string fullText;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("TypingEffect: No TextMeshProUGUI found!");
            enabled = false;
            return;
        }

        Debug.Log($"TypingEffect Awake - Text length: {textMesh.text.Length}");
    }

    void Start()
    {
        //  DIAGNOSTIC: Check initial text
        Debug.Log($"TypingEffect Start - Initial text: '{textMesh.text}' | Length: {textMesh.text.Length}");

        if (!string.IsNullOrEmpty(textMesh.text))
        {
            fullText = textMesh.text;
            textMesh.text = "";
            Debug.Log($"Auto-starting with text: '{fullText}'");
            TypeText();
        }
        else
        {
            Debug.LogWarning("TypingEffect: No text to type! Add text in Inspector.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(skipKey) && isTyping)
        {
            Debug.Log("Space pressed - skipping");
            SkipTyping();
        }
    }

    public void TypeText(string textToType = "")
    {
        Debug.Log($"TypeText called - textToType length: {textToType.Length}");

        if (isTyping)
        {
            Debug.LogWarning("Already typing!");
            return;
        }

        if (!string.IsNullOrEmpty(textToType))
            fullText = textToType;

        if (string.IsNullOrEmpty(fullText))
        {
            Debug.LogError("No text to type! fullText is empty.");
            return;
        }

        textMesh.text = "";
        typingCoroutine = StartCoroutine(TypeTextCoroutine());
    }

    IEnumerator TypeTextCoroutine()
    {
        Debug.Log($"Starting typing coroutine - fullText: '{fullText}'");
        isTyping = true;

        for (int i = 0; i <= fullText.Length; i++)
        {
            textMesh.text = fullText.Substring(0, i);
            textMesh.ForceMeshUpdate();

            Debug.Log($"Typed {i + 1}/{fullText.Length}: '{textMesh.text}'");

            yield return new WaitForSeconds(typeSpeed);
        }

        Debug.Log("Typing complete");
        isTyping = false;
        typingCoroutine = null;
    }

    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        textMesh.text = fullText;
        isTyping = false;
        typingCoroutine = null;
        Debug.Log("Typing skipped");
    }
}