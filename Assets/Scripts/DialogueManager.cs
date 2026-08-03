using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("Instruction References")]
    public GameObject instructionPanel;
    public TextMeshProUGUI instructionText;
    
    [Header("Narrator References")]
    public GameObject narratorPanel;
    public TextMeshProUGUI narratorText;

    [Header("Setting")]
    public float typingSpeed = 0.04f;

    private string[] currentLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool currentIsInstruction = false;
    
    private Action onDialogueComplete; 

    void Start()
    {
        CloseAllDialogues();
    }

    public void ShowSingleDialogue(string content, bool isInstruction)
    {
        if (isInstruction)
        {
            instructionPanel.SetActive(true);
            narratorPanel.SetActive(false);
            instructionText.text = content;
        }
        else
        {
            narratorPanel.SetActive(true);
            instructionPanel.SetActive(false);
            narratorText.text = content;
        }
    }

    public void CloseAllDialogues()
    {
        if (instructionPanel != null) instructionPanel.SetActive(false);
        if (narratorPanel != null) narratorPanel.SetActive(false);
    }
        
    public void StartDialogue(string[] lines, bool isInstruction, Action onComplete)
    {
        currentLines = lines;
        currentLineIndex = 0;
        currentIsInstruction = isInstruction;
        onDialogueComplete = onComplete;

        if (currentIsInstruction)
        {
            instructionPanel.SetActive(true);
            narratorPanel.SetActive(false);
            DisplayCurrentLineInstant();
        }
        else
        {
            narratorPanel.SetActive(true);
            instructionPanel.SetActive(false);
            StartCoroutine(TypeSentence(currentLines[currentLineIndex]));
        }
    }

    public void OnNextClicked()
    {
        if (isTyping && !currentIsInstruction)
        {
            StopAllCoroutines();
            narratorText.text = currentLines[currentLineIndex];
            isTyping = false;
        }
        else
        {
            currentLineIndex++;
            if (currentLineIndex < currentLines.Length)
            {
                if (currentIsInstruction)
                    DisplayCurrentLineInstant();
                else
                    StartCoroutine(TypeSentence(currentLines[currentLineIndex]));
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void DisplayCurrentLineInstant()
    {
        instructionText.text = currentLines[currentLineIndex];
        isTyping = false;
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        narratorText.text = "";
        
        foreach (char letter in sentence.ToCharArray())
        {
            narratorText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
    }

    private void EndDialogue()
    {
        CloseAllDialogues();
        onDialogueComplete?.Invoke(); 
    }
}
