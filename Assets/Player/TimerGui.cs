using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerGui : MonoBehaviour
{
    private TMP_Text textMesh;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();

    }

    void FixedUpdate()
    {
        if (SystemController.Controller.Running)
        {
            int minutes = (int)Mathf.Floor(SystemController.Controller.TimeLeft / 60);
            int seconds = Mathf.Abs((int)((minutes * 60) - SystemController.Controller.TimeLeft));
            textMesh.text = ((minutes > 9) ? minutes.ToString() : "0" + minutes) + ":" + (((seconds > 9) ? seconds.ToString() : "0" + seconds));
        }
    }


}
