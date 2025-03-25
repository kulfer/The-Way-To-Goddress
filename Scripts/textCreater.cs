using System;
using System.Collections;
using UnityEngine;

public class textCreater : MonoBehaviour
{

    public static TMPro.TMP_Text viewText;
    public static bool runTextPrint;
    public static int chatCount;
    [SerializeField] string transferText;
    [SerializeField] int internalCount;

    // Update is called once per frame
    void Update()
    {
        internalCount = chatCount;
        chatCount = GetComponent<TMPro.TMP_Text>().text.Length;
        if (runTextPrint == true)
        {
            runTextPrint = false;
            viewText = GetComponent<TMPro.TMP_Text>();
            transferText = viewText.text;
            viewText.text = "";
            StartCoroutine(RollText());

        }
    }

    IEnumerator RollText()
    {
        foreach (char c in transferText)
        {
            viewText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
    }
}
