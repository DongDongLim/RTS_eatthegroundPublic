using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayUI : MonoBehaviour
{
    [SerializeField]
    Text _dayText;

    [SerializeField]
    Image _dayImage;
    public void SetDayUI(float progress)
    {
        _dayImage.fillAmount = progress;
    }

    public void SetDayUI(int date)
    {
        _dayText.text = date.ToString();
    }
}
