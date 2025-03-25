using System.Collections;
using UnityEngine;

public class ScreenEvent : MonoBehaviour
{

    public GameObject fadeScreenIn;
    public GameObject charAkane;
    public GameObject AkaneActionSad;
    public GameObject AkaneActionVSad;
    public GameObject charFulu;
    public GameObject textName;
    public GameObject textBox;
    public GameObject bgTitle;

    [SerializeField] GameObject title;
    [SerializeField] AudioSource walkSand;
    [SerializeField] AudioSource violineM;
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject charName;
    [SerializeField] GameObject SubtextTitle;
    [SerializeField] int eventPas = 0;

    [SerializeField] string textToSpeak;
    [SerializeField] int currentTextLength;
    [SerializeField] int textLength;
    [SerializeField] GameObject mainTextObject;
    [SerializeField] GameObject panelBtn;
    [SerializeField] GameObject panelTopUi;


    [SerializeField] GameObject fadeOut;

    void Update()
    {
        textLength = textCreater.chatCount;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EventStarter());
    }   

    IEnumerator EventStarter()
    {
        // evennpas 0 คือ การเริ่มต้นเกม
        yield return new WaitForSeconds(2);
        fadeScreenIn.SetActive(false);
        bgTitle.SetActive(true);
        title.GetComponent<TMPro.TMP_Text>().text ="ภาพในความฝัน";
        yield return new WaitForSeconds(4);
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(2);

        yield return new WaitForSeconds(2);
        bgTitle.SetActive(false);

        title.SetActive(false);
        fadeOut.SetActive(false);
        SubtextTitle.SetActive(true);
        yield return new WaitForSeconds(7);
        SubtextTitle.SetActive(false);
        yield return new WaitForSeconds(2);
        charAkane.SetActive(true);
        charFulu.SetActive(true);

        yield return new WaitForSeconds(2);
        violineM.Play();
        mainTextObject.SetActive(true);
        yield return new WaitForSeconds(1);

        panelBtn.SetActive(true);
        panelTopUi.SetActive(true);
        textName.SetActive(true);
        charName.GetComponent<TMPro.TMP_Text>().text = "ฉัน";

        textBox.SetActive(true);
        textBox.GetComponent<TMPro.TMP_Text>().text = "";  // เคลียร์ข้อความเดิม
        textToSpeak = "อาคาเนะ... คือว่า ผม... ผมชอบเธอ เธอคือรักแรกของผม! เพราะฉะนั้น... ช่วยคบกับผมได้ไหม? ";
        textBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        currentTextLength = textToSpeak.Length;
        textCreater.runTextPrint = true;
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLength == currentTextLength);
        yield return new WaitForSeconds(0.5f);

        nextButton.SetActive(true);
        eventPas = 1;

    }

    IEnumerator EventOne()
    {
        //event 1
        nextButton.SetActive(false);
        textName.SetActive(false);
        //เป็นส่วนแสดงข้อความบรรยาย
        textBox.GetComponent<TMPro.TMP_Text>().text = "";  // เคลียร์ข้อความเดิม
        textBox.SetActive(true); 

        textToSpeak = "เธอเงียบไปครู่หนึ่ง ดวงตาสั่นระริก ก่อนจะเม้มริมฝีปากแน่น ";
        AkaneActionSad.SetActive(true);
        yield return new WaitForSeconds(1);
        charAkane.SetActive(false);
        yield return new WaitForSeconds(1);
        textBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        currentTextLength = textToSpeak.Length;
        textCreater.runTextPrint = true;
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLength == currentTextLength);
        yield return new WaitForSeconds(0.5f);

        // หลังจากรอข้อความบรรยายเสร็จจะแสดง ไดไอะล้อกของตัวละครพูดต่อเลยอัตโนมัติ
        textName.SetActive(true);
        textBox.GetComponent<TMPro.TMP_Text>().text = "";  // เคลียร์ข้อความเดิม

        charName.GetComponent<TMPro.TMP_Text>().text = "อาคาเนะ";
        textToSpeak = "ฉัน...ขอโทษนะ แต่ฉันคิดว่ามันคงเป็นไปไม่ได้";
        textBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        textBox.SetActive(true);
        currentTextLength = textToSpeak.Length;
        textCreater.runTextPrint = true;
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLength == currentTextLength);
        yield return new WaitForSeconds(0.5f);
        nextButton.SetActive(true);
        eventPas = 2;
    }

    IEnumerator EventTwo()
    {
        //Even Two
        nextButton.SetActive(false);
        textBox.SetActive(true);
        textName.SetActive(true);
        charName.GetComponent<TMPro.TMP_Text>().text = "ฉัน";

        textToSpeak = "เข้าใจแล้ว... ไม่เป็นไร แต่ขอถามเหตุผลได้ไหม? ";
        textBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        currentTextLength = textToSpeak.Length;
        textCreater.runTextPrint = true;
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLength == currentTextLength);
        yield return new WaitForSeconds(0.5f);
        nextButton.SetActive(true);
        eventPas = 3;
    }

    IEnumerator EventThree()
    {
        //Even Three
        nextButton.SetActive(false);
        AkaneActionVSad.SetActive(true);
        yield return new WaitForSeconds(1);
        AkaneActionSad.SetActive(false);
        textBox.GetComponent<TMPro.TMP_Text>().text = "";  // เคลียร์ข้อความเดิม
        textBox.SetActive(true);
        textName.SetActive(true);
        charName.GetComponent<TMPro.TMP_Text>().text = "อาคาเนะ";

        textToSpeak = "ก็... ฉันไม่ได้... ฉันไม่ได้คิดแบบนั้นกับนาย แล้วก็... นายกล้ามากเลยนะที่มาสารภาพรักแบบนี้ ฉันตกใจมากเลย";
        textBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        currentTextLength = textToSpeak.Length;
        textCreater.runTextPrint = true;
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLength == currentTextLength);
        yield return new WaitForSeconds(0.5f);
        nextButton.SetActive(true);
        eventPas = 4;
    }

    IEnumerator EventFour()
    {
        //Even Three
        nextButton.SetActive(false);
        textBox.SetActive(true);
        textName.SetActive(true);
        charName.GetComponent<TMPro.TMP_Text>().text = "ฉัน";

        textToSpeak = "งั้นเหรอ... เป็นแบบนี้เอง...";
        textBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        currentTextLength = textToSpeak.Length;
        textCreater.runTextPrint = true;
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLength == currentTextLength);
        yield return new WaitForSeconds(0.5f);

        nextButton.SetActive(true);
        eventPas = 5;
    }

    IEnumerator EventFire()
    {
        //Even Three
        nextButton.SetActive(false);
        textBox.SetActive(true);
        textName.SetActive(false);
        //charName.GetComponent<TMPro.TMP_Text>().text = "ผู้บรรยาย";

        textToSpeak = "เธอเดินจากไปอย่างช้าๆ ทิ้งไว้เพียงเงาหลังที่ค่อยๆ จางหายไปในแสงสุดท้ายของวัน...";
        textBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        currentTextLength = textToSpeak.Length;
        textCreater.runTextPrint = true;
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLength == currentTextLength);
        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(2);
        fadeOut.SetActive(true);
        eventPas = 6;

    }

    public void NextButton()
    {
        if (eventPas == 1) { StartCoroutine(EventOne()); }
        if (eventPas == 2) { StartCoroutine(EventTwo()); }
        if (eventPas == 3) { StartCoroutine(EventThree()); }
        if (eventPas == 4) { StartCoroutine(EventFour()); }
        if (eventPas == 5) { StartCoroutine(EventFire()); }

    }
}
