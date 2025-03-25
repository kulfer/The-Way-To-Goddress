// SaveLoadManager.cs - จัดการการบันทึกและโหลดข้อมูลเกม
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string saveTime;           // เวลาที่บันทึก
    public string currentNodeID;      // ID ของโหนดข้อความปัจจุบัน
    public int currentLineIndex;      // ตำแหน่งบรรทัดปัจจุบันใน node
    public string currentBackgroundID; // ID ของพื้นหลังปัจจุบัน
    public string currentBGMID;       // ID ของเพลงปัจจุบัน
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();  // ตัวแปรธงต่างๆ
    public Dictionary<string, int> variables = new Dictionary<string, int>(); // ตัวแปรค่าต่างๆ
    
    // ข้อมูลตัวละคร
    public List<SavedCharacterState> characterStates = new List<SavedCharacterState>();
    
    [Serializable]
    public class SavedCharacterState
    {
        public string characterID;
        public bool isVisible;
        public string emotion;
        public Vector3 position;
    }
}

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private SceneEffectManager effectManager;
    [SerializeField] private AudioManager audioManager;
    
    // ตัวแปรระบบ
    private static Dictionary<string, bool> gameFlags = new Dictionary<string, bool>();
    private static Dictionary<string, int> gameVariables = new Dictionary<string, int>();
    
    // ตัวแปรสำหรับบันทึกสถานะปัจจุบัน
    [HideInInspector] public string currentNodeID;
    [HideInInspector] public int currentLineIndex;
    [HideInInspector] public DialogueData currentDialogue;
    [HideInInspector] public string currentBackgroundID;
    [HideInInspector] public string currentBGMID;
    
    [SerializeField] private int maxSaveSlots = 5;
    
    // สร้างข้อมูลสำหรับบันทึก
    public SaveData CreateSaveData()
    {
        SaveData data = new SaveData
        {
            saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            currentNodeID = currentNodeID,
            currentLineIndex = currentLineIndex,
            currentBackgroundID = currentBackgroundID,
            currentBGMID = currentBGMID,
            flags = new Dictionary<string, bool>(gameFlags),
            variables = new Dictionary<string, int>(gameVariables)
        };
        
        // TODO: บันทึกสถานะของตัวละครทั้งหมด
        // จำเป็นต้องเพิ่มฟังก์ชันใน CharacterManager เพื่อให้เข้าถึงข้อมูลตัวละครได้
        
        return data;
    }
    
    // บันทึกเกม
    public void SaveGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSaveSlots)
        {
            Debug.LogError($"Save slot index out of range: {slotIndex}");
            return;
        }
        
        SaveData saveData = CreateSaveData();
        string json = JsonUtility.ToJson(saveData);
        
        string savePath = Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.json");
        File.WriteAllText(savePath, json);
        
        Debug.Log($"Game saved to slot {slotIndex}");
    }
    
    // โหลดเกม
    public void LoadGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSaveSlots)
        {
            Debug.LogError($"Save slot index out of range: {slotIndex}");
            return;
        }
        
        string savePath = Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.json");
        if (!File.Exists(savePath))
        {
            Debug.LogWarning($"No save file found at slot {slotIndex}");
            return;
        }
        
        string json = File.ReadAllText(savePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
        
        // โหลดข้อมูลธงและตัวแปร
        gameFlags = new Dictionary<string, bool>(saveData.flags);
        gameVariables = new Dictionary<string, int>(saveData.variables);
        
        // โหลดสถานะฉาก
        currentBackgroundID = saveData.currentBackgroundID;
        currentBGMID = saveData.currentBGMID;
        
        effectManager.ChangeBackground(currentBackgroundID);
        audioManager.ChangeBGM(currentBGMID);
        
        // โหลดตัวละคร
        // TODO: ตั้งค่าสถานะตัวละครทั้งหมดตามข้อมูลที่บันทึกไว้
        
        // โหลดบทสนทนา
        currentNodeID = saveData.currentNodeID;
        currentLineIndex = saveData.currentLineIndex;
        
        // ต้องเพิ่มฟังก์ชันใน DialogueManager เพื่อให้โหลดสถานะการสนทนาได้
        // dialogueManager.LoadDialogueState(currentDialogue, currentNodeID, currentLineIndex);
        
        Debug.Log($"Game loaded from slot {slotIndex}");
    }
    
    // ฟังก์ชันสำหรับตรวจสอบ SaveData จากสล็อต
    public SaveData GetSaveDataInfo(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSaveSlots)
        {
            return null;
        }
        
        string savePath = Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.json");
        if (!File.Exists(savePath))
        {
            return null;
        }
        
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }
    
    // สำหรับระบบ flag (ตัวแปรแบบ boolean)
    public static void SetFlag(string flagName, bool value)
    {
        gameFlags[flagName] = value;
    }
    
    public static bool GetFlag(string flagName, bool defaultValue = false)
    {
        if (gameFlags.TryGetValue(flagName, out bool value))
        {
            return value;
        }
        return defaultValue;
    }
    
    // สำหรับระบบตัวแปร (ตัวแปรแบบ int)
    public static void SetVariable(string varName, int value)
    {
        gameVariables[varName] = value;
    }
    
    public static int GetVariable(string varName, int defaultValue = 0)
    {
        if (gameVariables.TryGetValue(varName, out int value))
        {
            return value;
        }
        return defaultValue;
    }
    
    // เพิ่มค่าตัวแปร
    public static void AddToVariable(string varName, int amount)
    {
        int currentValue = GetVariable(varName);
        SetVariable(varName, currentValue + amount);
    }
}