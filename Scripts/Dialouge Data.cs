// DialogueData.cs - เก็บข้อมูลบทสนทนา
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public string speakerID;  // ID ของผู้พูด (ชื่อที่แสดง)
    public string text;       // ข้อความที่พูด
    public string emotion;    // สถานะอารมณ์ของตัวละคร (เชื่อมโยงกับ animation)
    public AudioClip voiceClip; // เสียงพูด (ถ้ามี)
    public float delay = 0;   // ดีเลย์ก่อนแสดงข้อความนี้
}

[Serializable]
public class Choice
{
    public string text;       // ข้อความของตัวเลือก
    public string nextNodeID; // ID ของโหนดถัดไปที่จะไปเมื่อเลือกตัวเลือกนี้
}

[Serializable]
public class DialogueNode
{
    public string nodeID;            // ID ของโหนดปัจจุบัน
    public List<DialogueLine> lines; // รายการบรรทัดข้อความในโหนดนี้
    public List<Choice> choices;     // ตัวเลือกที่จะแสดงหลังจากแสดงข้อความในโหนดนี้
    public string nextNodeID;        // ID ของโหนดถัดไปถ้าไม่มีตัวเลือก
    public bool isEndNode;           // ระบุว่าเป็นโหนดสุดท้ายหรือไม่
    
    // เอฟเฟกต์พิเศษที่จะเกิดขึ้นในโหนดนี้
    public string backgroundChange;  // เปลี่ยน background (ถ้ามี)
    public string bgmChange;         // เปลี่ยนเพลง BGM (ถ้ามี)
    public string effect;            // เอฟเฟกต์พิเศษ เช่น "fadeIn", "fadeOut" (ถ้ามี)
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Visual Novel/Dialogue")]
public class DialogueData : ScriptableObject
{
    public string startNodeID;               // ID โหนดเริ่มต้น
    public List<DialogueNode> dialogueNodes; // รายการโหนดทั้งหมด

    // ค้นหาโหนดจาก ID
    public DialogueNode GetNodeByID(string id)
    {
        return dialogueNodes.Find(node => node.nodeID == id);
    }
}