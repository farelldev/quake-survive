using UnityEngine;
using TMPro;
using System.Collections;
using System;
using Microsoft.Unity.VisualStudio.Editor;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    [Header("Instruction References")]
    public GameObject instructionPanel;
    public TextMeshProUGUI instructionText;
    public GameObject instructionIcon;
    private Tween iconPulseTween;
    private Vector3 originalIconScale;
    
    [Header("Narrator References")]
    public GameObject narratorPanel;
    public TextMeshProUGUI narratorText;

    [Header("Setting")]
    public float typingSpeed = 0.04f;

    private string[] currentLines;
    private int currentLineIndex = 0;
    [HideInInspector] public bool isTyping = false;
    [HideInInspector] public bool isLocked = false;
    private bool currentIsInstruction = false;
    
    private Action onDialogueComplete; 

    void Start()
    {
        CloseAllDialogues();

        if (instructionIcon != null)
        {
            originalIconScale = instructionIcon.transform.localScale;
        }
    }
    
    public void CloseAllDialogues()
    {
        isLocked = false;
        
        if (instructionPanel != null) instructionPanel.SetActive(false);
        if (narratorPanel != null) narratorPanel.SetActive(false);

        if (iconPulseTween != null)
        {
            iconPulseTween.Kill();
            if (instructionIcon != null) instructionIcon.transform.localScale = originalIconScale; // Kembalikan ke ukuran normal
        }
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

            if (instructionIcon != null)
            {
                instructionIcon.transform.localScale = originalIconScale;
                
                iconPulseTween = instructionIcon.transform.DOScale(originalIconScale * 1.05f, 1f)
                                  .SetLoops(-1, LoopType.Yoyo)
                                  .SetEase(Ease.InOutSine);
            }
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
            if (!isLocked)
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

            return;
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
