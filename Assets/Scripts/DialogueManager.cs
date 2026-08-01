using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Pengaturan")]
    public float typingSpeed = 0.04f;

    private string[] currentLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    
    private Action onDialogueComplete; 

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string[] lines, Action onComplete)
    {
        currentLines = lines;
        currentLineIndex = 0;
        onDialogueComplete = onComplete;

        dialoguePanel.SetActive(true);
        StartCoroutine(TypeSentence(currentLines[currentLineIndex]));
    }

    public void OnNextClicked()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentLines[currentLineIndex];
            isTyping = false;
        }
        else
        {
            currentLineIndex++;
            if (currentLineIndex < currentLines.Length)
            {
                StartCoroutine(TypeSentence(currentLines[currentLineIndex]));
            }
            else
            {
                EndDialogue();
            }
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        
        onDialogueComplete?.Invoke(); 
    }
}
