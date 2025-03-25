// ScreenManager.cs - จัดการหน้าจอและเมนูต่างๆ
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject saveLoadScreen;
    [SerializeField] private GameObject historyScreen;
    
    [Header("Game Flow")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private SaveLoadManager saveLoadManager;
    
    [Header("Save/Load UI")]
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private Transform saveSlotContainer;
    [SerializeField] private bool isSaveMode; // true = save mode, false = load mode
    
    [Header("Settings UI")]
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;
    [SerializeField] private Slider textSpeedSlider;
    [SerializeField] private Toggle autoModeToggle;
    [SerializeField] private AudioManager audioManager;
    
    [Header("History UI")]
    [SerializeField] private Transform historyContainer;
    [SerializeField] private GameObject historyLinePrefab;
    private List<string> dialogueHistory = new List<string>();
    
    private enum ScreenState
    {
        MainMenu,
        Game,
        Pause,
        Settings,
        SaveLoad,
        History
    }
    
    private ScreenState currentState;
    
    void Start()
    {
        // เริ่มต้นที่หน้าเมนูหลัก
        SwitchToMainMenu();
        
        // ตั้งค่าการฟังอีเวนต์ต่างๆ
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.onValueChanged.AddListener((value) => audioManager.SetBGMVolume(value));
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener((value) => audioManager.SetSFXVolume(value));
            
        if (voiceVolumeSlider != null)
            voiceVolumeSlider.onValueChanged.AddListener((value) => audioManager.SetVoiceVolume(value));
    }
    
    // สลับไปหน้าเมนูหลัก
    public void SwitchToMainMenu()
    {
        HideAllScreens();
        mainMenuScreen.SetActive(true);
        currentState = ScreenState.MainMenu;
    }
    
  
    
    // เปิดหน้าพัก
    public void OpenPauseScreen()
    {
        if (currentState == ScreenState.Game)
        {
            pauseScreen.SetActive(true);
            currentState = ScreenState.Pause;
            // หยุดเกมชั่วคราว
            Time.timeScale = 0f;
        }
    }
    
    // ปิดหน้าพักและเล่นต่อ
    public void ClosePauseScreen()
    {
        if (currentState == ScreenState.Pause)
        {
            pauseScreen.SetActive(false);
            currentState = ScreenState.Game;
            // เล่นเกมต่อ
            Time.timeScale = 1f;
        }
    }

    

    // บันทึกการตั้งค่า
    public void SaveSettings()
    {
        
        // บันทึกการตั้งค่าลงในระบบ
        PlayerPrefs.SetFloat("BGMVolume", bgmVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("VoiceVolume", voiceVolumeSlider.value);
        PlayerPrefs.SetFloat("TextSpeed", textSpeedSlider.value);
        PlayerPrefs.SetInt("AutoMode", autoModeToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
        
        // กลับไปหน้าก่อนหน้า
        BackToPreviousScreen();
    }
    

    
    

    
    // เปิดหน้าประวัติบทสนทนา
    public void OpenHistoryScreen()
    {
        HideAllScreens();
        historyScreen.SetActive(true);
        currentState = ScreenState.History;
  
    }
    
    
    
    // กลับไปหน้าก่อนหน้า
    public void BackToPreviousScreen()
    {
        HideAllScreens();
        
        // ตรวจสอบว่ากำลังอยู่ในเกมหรือไม่
        if (currentState == ScreenState.Pause || 
            currentState == ScreenState.Settings || 
            currentState == ScreenState.SaveLoad || 
            currentState == ScreenState.History)
        {
            if (currentState == ScreenState.Settings && pauseScreen.activeSelf)
            {
                // กลับไปที่หน้าพัก
                pauseScreen.SetActive(true);
                currentState = ScreenState.Pause;
            }
            else
            {
                // กลับไปเล่นเกม
                gameScreen.SetActive(true);
                currentState = ScreenState.Game;
                // ให้แน่ใจว่าเกมกำลังเล่นอยู่
                Time.timeScale = 1f;
            }
        }
        else
        {
            // กลับไปที่เมนูหลัก
            mainMenuScreen.SetActive(true);
            currentState = ScreenState.MainMenu;
        }
    }
    
    // ปิดหน้าจอทั้งหมด
    private void HideAllScreens()
    {
        mainMenuScreen.SetActive(false);
        gameScreen.SetActive(false);
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
        saveLoadScreen.SetActive(false);
        historyScreen.SetActive(false);
    }
    
    // จัดการกับการกดปุ่ม Escape
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (currentState)
            {
                case ScreenState.Game:
                    OpenPauseScreen();
                    break;
                case ScreenState.Pause:
                    ClosePauseScreen();
                    break;
                case ScreenState.Settings:
                case ScreenState.SaveLoad:
                case ScreenState.History:
                    BackToPreviousScreen();
                    break;
                default:
                    break;
            }
        }
    }
    
    
    
    // ออกจากเกม
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}