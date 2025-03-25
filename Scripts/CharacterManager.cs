// CharacterManager.cs - จัดการตัวละคร การแสดงและอารมณ์
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Character
{
    public string characterID;  // ID ของตัวละคร
    public string displayName;  // ชื่อที่แสดงในกล่องข้อความ
    public GameObject characterObject;  // GameObject ของตัวละคร
    public Dictionary<string, GameObject> emotionObjects = new Dictionary<string, GameObject>();  // Object ของแต่ละอารมณ์
}

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<Character> characters = new List<Character>();
    private Dictionary<string, Character> characterDict = new Dictionary<string, Character>();
    
    void Awake()
    {
        // สร้าง dictionary เพื่อค้นหาตัวละครได้เร็วขึ้น
        foreach (Character character in characters)
        {
            characterDict[character.characterID] = character;
            
            // ค้นหา GameObject สำหรับทุกอารมณ์ใน hierarchy
            foreach (Transform child in character.characterObject.transform)
            {
                if (child.name.StartsWith(character.characterID + "Action"))
                {
                    string emotionName = child.name.Replace(character.characterID + "Action", "");
                    character.emotionObjects[emotionName] = child.gameObject;
                    child.gameObject.SetActive(false);  // ซ่อนทุกอารมณ์ตอนเริ่มต้น
                }
            }
        }
    }
    
    // แสดงตัวละคร
    public void ShowCharacter(string characterID)
    {
        if (characterDict.TryGetValue(characterID, out Character character))
        {
            character.characterObject.SetActive(true);
        }
    }
    
    // ซ่อนตัวละคร
    public void HideCharacter(string characterID)
    {
        if (characterDict.TryGetValue(characterID, out Character character))
        {
            character.characterObject.SetActive(false);
        }
    }
    
    // ตั้งค่าอารมณ์ของตัวละคร
    public void SetCharacterEmotion(string characterID, string emotion)
    {
        if (characterDict.TryGetValue(characterID, out Character character))
        {
            // ซ่อนทุกอารมณ์ก่อน
            foreach (var emotionObj in character.emotionObjects)
            {
                emotionObj.Value.SetActive(false);
            }
            
            // แสดงอารมณ์ที่ต้องการ
            if (character.emotionObjects.TryGetValue(emotion, out GameObject emotionObject))
            {
                emotionObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Emotion '{emotion}' not found for character '{characterID}'");
            }
        }
    }
    
    // ย้ายตัวละครไปยังตำแหน่งที่กำหนด
    public void MoveCharacter(string characterID, Vector3 position, float duration = 1.0f)
    {
        if (characterDict.TryGetValue(characterID, out Character character))
        {
            StartCoroutine(MoveCharacterCoroutine(character.characterObject, position, duration));
        }
    }
    
    private System.Collections.IEnumerator MoveCharacterCoroutine(GameObject characterObj, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = characterObj.transform.position;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            characterObj.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        characterObj.transform.position = targetPosition;
    }
}
