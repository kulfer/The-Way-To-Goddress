// DialogueManager.cs - จัดการการแสดงผลบทสนทนาและตัวเลือก
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject nextButton;

    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("Character Management")]
    [SerializeField] private CharacterManager characterManager;
    
    [Header("Effect Management")]
    [SerializeField] private SceneEffectManager effectManager;
    [SerializeField] private AudioManager audioManager;

    [Header("Text Settings")]
    [SerializeField] private float textSpeed = 0.05f;
    
    // ข้อมูลบทสนทนาปัจจุบัน
    private DialogueData currentDialogue;
    private DialogueNode currentNode;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isWaitingForInput = false;
    
    // สถานะการแสดงข้อความ
    private Coroutine typingCoroutine;

    // เริ่มต้นบทสนทนาใหม่
    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentNode = dialogue.GetNodeByID(dialogue.startNodeID);
        currentLineIndex = 0;
        
        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);
        
        DisplayNextLine();
    }

    // แสดงบรรทัดถัดไป
    public void DisplayNextLine()
    {
        if (isTyping)
        {
            // ถ้ากำลังพิมพ์อยู่ ให้แสดงข้อความทั้งหมดทันที
            CompleteTyping();
            return;
        }
        
        if (isWaitingForInput)
        {
            isWaitingForInput = false;
            
            // ถ้ายังมีบรรทัดต่อไป
            if (currentLineIndex < currentNode.lines.Count)
            {
                ProcessCurrentLine();
            }
            else
            {
                // จบการแสดงข้อความในโหนดนี้
                OnNodeComplete();
            }
        }
    }

    // แสดงข้อความปัจจุบัน
    private void ProcessCurrentLine()
    {
        DialogueLine line = currentNode.lines[currentLineIndex];
        
        // จัดการแสดงผลตัวละคร
        if (!string.IsNullOrEmpty(line.speakerID))
        {
            speakerNameText.text = line.speakerID;
            speakerNameText.gameObject.SetActive(true);
            
            // อัปเดตอารมณ์ของตัวละคร
            if (!string.IsNullOrEmpty(line.emotion))
            {
                characterManager.SetCharacterEmotion(line.speakerID, line.emotion);
            }
        }
        else
        {
            // กรณีเป็นข้อความบรรยาย
            speakerNameText.gameObject.SetActive(false);
        }
        
        // เริ่มพิมพ์ข้อความ
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        typingCoroutine = StartCoroutine(TypeText(line.text, line.delay));
        
        // เล่นเสียงพูด (ถ้ามี)
        if (line.voiceClip != null)
        {
            audioManager.PlayVoice(line.voiceClip);
        }
        
        currentLineIndex++;
    }

    // แสดงข้อความแบบทยอยพิมพ์
    private IEnumerator TypeText(string text, float initialDelay = 0)
    {
        isTyping = true;
        dialogueText.text = "";
        
        if (initialDelay > 0)
        {
            yield return new WaitForSeconds(initialDelay);
        }
        
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        
        isTyping = false;
        isWaitingForInput = true;
        
        nextButton.SetActive(true);
    }

    // แสดงข้อความทั้งหมดทันที
    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
        if (currentLineIndex > 0 && currentLineIndex <= currentNode.lines.Count)
        {
            dialogueText.text = currentNode.lines[currentLineIndex - 1].text;
        }
        
        isTyping = false;
        isWaitingForInput = true;
        
        nextButton.SetActive(true);
    }

    // เมื่อแสดงข้อความในโหนดหมดแล้ว
    private void OnNodeComplete()
    {
        // ทำเอฟเฟกต์ (ถ้ามี)
        ApplyNodeEffects();
        
        // ตรวจสอบว่ามีตัวเลือกหรือไม่
        if (currentNode.choices != null && currentNode.choices.Count > 0)
        {
            ShowChoices();
        }
        else if (!string.IsNullOrEmpty(currentNode.nextNodeID))
        {
            // ไปยังโหนดถัดไป
            currentNode = currentDialogue.GetNodeByID(currentNode.nextNodeID);
            currentLineIndex = 0;
            DisplayNextLine();
        }
        else if (currentNode.isEndNode)
        {
            // จบบทสนทนา
            EndDialogue();
        }
    }

    // ใช้เอฟเฟกต์ที่กำหนดในโหนด
    private void ApplyNodeEffects()
    {
        if (!string.IsNullOrEmpty(currentNode.backgroundChange))
        {
            effectManager.ChangeBackground(currentNode.backgroundChange);
        }
        
        if (!string.IsNullOrEmpty(currentNode.bgmChange))
        {
            audioManager.ChangeBGM(currentNode.bgmChange);
        }
        
        if (!string.IsNullOrEmpty(currentNode.effect))
        {
            switch (currentNode.effect)
            {
                case "fadeIn":
                    effectManager.FadeIn();
                    break;
                case "fadeOut":
                    effectManager.FadeOut();
                    break;
                // เพิ่มเอฟเฟกต์อื่นๆ ตามต้องการ
            }
        }
    }

    // แสดงตัวเลือก
    private void ShowChoices()
    {
        // ลบตัวเลือกเก่า
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }
        
        // สร้างปุ่มสำหรับแต่ละตัวเลือก
        foreach (Choice choice in currentNode.choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicePanel.transform);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            
            buttonText.text = choice.text;
            string nextNodeID = choice.nextNodeID;
            
            button.onClick.AddListener(() => {
                OnChoiceSelected(nextNodeID);
            });
        }
        
        // แสดงพาเนลตัวเลือก
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(true);
    }

    // เมื่อเลือกตัวเลือก
    private void OnChoiceSelected(string nextNodeID)
    {
        choicePanel.SetActive(false);
        dialoguePanel.SetActive(true);
        
        currentNode = currentDialogue.GetNodeByID(nextNodeID);
        currentLineIndex = 0;
        
        DisplayNextLine();
    }

    // จบบทสนทนา
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        // ส่งเหตุการณ์ไปยังระบบอื่นๆ ที่ต้องการทราบว่าบทสนทนาจบแล้ว
    }

    // ปุ่ม Next สำหรับผู้เล่นกด
    public void OnNextButtonClicked()
    {
        DisplayNextLine();
    }
}