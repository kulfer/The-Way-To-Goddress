// SceneEffectManager.cs - จัดการเอฟเฟกต์ภาพและการเปลี่ยนฉาก
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SceneEffectManager : MonoBehaviour
{
    [Header("Backgrounds")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float backgroundTransitionTime = 1.0f;
    [SerializeField] private Sprite[] backgroundSprites;
    [SerializeField] private string[] backgroundIDs;
    
    [Header("Screen Effects")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeTime = 1.0f;
    
    private Dictionary<string, Sprite> backgroundDict = new Dictionary<string, Sprite>();
    
    void Awake()
    {
        // สร้าง dictionary สำหรับค้นหา background
        for (int i = 0; i < backgroundIDs.Length; i++)
        {
            if (i < backgroundSprites.Length)
            {
                backgroundDict[backgroundIDs[i]] = backgroundSprites[i];
            }
        }
    }
    
    // เปลี่ยนพื้นหลัง
    public void ChangeBackground(string backgroundID)
    {
        if (backgroundDict.TryGetValue(backgroundID, out Sprite sprite))
        {
            StartCoroutine(TransitionBackground(sprite));
        }
        else
        {
            Debug.LogWarning($"Background '{backgroundID}' not found");
        }
    }
    
    // ทำการเฟดพื้นหลังไปเป็นภาพใหม่
    private IEnumerator TransitionBackground(Sprite newBackground)
    {
        // บันทึกภาพเดิม (ถ้ามี)
        Sprite oldBackground = backgroundImage.sprite;
        Color originalColor = backgroundImage.color;
        
        // สร้าง GameObject ชั่วคราวเพื่อทำเฟดข้าม
        GameObject tempObj = new GameObject("TempBackground");
        tempObj.transform.SetParent(backgroundImage.transform.parent);
        tempObj.transform.SetSiblingIndex(backgroundImage.transform.GetSiblingIndex());
        tempObj.transform.localPosition = Vector3.zero;
        tempObj.transform.localScale = Vector3.one;
        
        RectTransform tempRect = tempObj.AddComponent<RectTransform>();
        tempRect.anchorMin = new Vector2(0, 0);
        tempRect.anchorMax = new Vector2(1, 1);
        tempRect.offsetMin = new Vector2(0, 0);
        tempRect.offsetMax = new Vector2(0, 0);
        
        Image tempImage = tempObj.AddComponent<Image>();
        tempImage.sprite = oldBackground;
        tempImage.color = originalColor;
        
        // เปลี่ยนรูปพื้นหลังเดิมเป็นรูปใหม่
        backgroundImage.sprite = newBackground;
        
        // เฟดออกจากรูปเดิม
        float elapsed = 0f;
        while (elapsed < backgroundTransitionTime)
        {
            float alpha = 1 - (elapsed / backgroundTransitionTime);
            tempImage.color = new Color(tempImage.color.r, tempImage.color.g, tempImage.color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // ลบ GameObject ชั่วคราว
        Destroy(tempObj);
    }
    
    // เฟดเข้าฉาก (จากดำไปปกติ)
    public void FadeIn()
    {
        StartCoroutine(FadeCoroutine(1, 0));
    }
    
    // เฟดออกจากฉาก (จากปกติไปดำ)
    public void FadeOut()
    {
        StartCoroutine(FadeCoroutine(0, 1));
    }
    
    // เฟดระหว่างสองค่า alpha
    private IEnumerator FadeCoroutine(float fromAlpha, float toAlpha)
    {
        fadeCanvasGroup.alpha = fromAlpha;
        fadeCanvasGroup.blocksRaycasts = true;
        
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / fadeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        fadeCanvasGroup.alpha = toAlpha;
        fadeCanvasGroup.blocksRaycasts = (toAlpha > 0.5f);  // บล็อกการกดปุ่มเมื่อจอมืด
    }
    
    // เอฟเฟกต์สั่นหน้าจอ
    public void ShakeScreen(float intensity = 0.5f, float duration = 0.5f)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }
    
    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        Transform canvasTransform = backgroundImage.canvas.transform;
        Vector3 originalPos = canvasTransform.localPosition;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            
            canvasTransform.localPosition = new Vector3(x, y, originalPos.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        canvasTransform.localPosition = originalPos;
    }
    
    // เอฟเฟกต์แฟลชหน้าจอ
    public void FlashScreen(Color flashColor, float duration = 0.2f)
    {
        StartCoroutine(FlashCoroutine(flashColor, duration));
    }
    
    private IEnumerator FlashCoroutine(Color flashColor, float duration)
    {
        // สร้าง GameObject ชั่วคราวสำหรับแฟลช
        GameObject flashObj = new GameObject("ScreenFlash");
        flashObj.transform.SetParent(backgroundImage.canvas.transform);
        flashObj.transform.SetAsLastSibling();  // แสดงทับทุกอย่าง
        
        RectTransform flashRect = flashObj.AddComponent<RectTransform>();
        flashRect.anchorMin = new Vector2(0, 0);
        flashRect.anchorMax = new Vector2(1, 1);
        flashRect.offsetMin = new Vector2(0, 0);
        flashRect.offsetMax = new Vector2(0, 0);
        
        Image flashImage = flashObj.AddComponent<Image>();
        flashImage.color = flashColor;
        
        // เฟดออก
        float elapsed = 0f;
        while (elapsed < duration)
        {
            flashImage.color = new Color(
                flashColor.r, 
                flashColor.g, 
                flashColor.b, 
                Mathf.Lerp(1, 0, elapsed / duration)
            );
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // ลบ GameObject
        Destroy(flashObj);
    }
}