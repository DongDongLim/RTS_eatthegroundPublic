using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayRepeat
{
    int _day = 0;
    public int Day { private set { _day = value; } get { return _day; } }

    // 몇 초에 하루인가
    int _dayRealTime = 10;
    public int DayRealTime { get { return _dayRealTime; } }

    float _dayProgress;
    public float DayProgress { get { return _dayProgress; } }

    int _dayEndTime = 20;

    int _waitingJudgmentTime = 5;



    DayUI _dayUI;

    public IEnumerator SetDay()
    {
        _dayUI = UIMng.instance.uiList["날짜"].GetComponent<DayUI>();
        while (true)
        {
            _dayProgress = 0;
            while (_dayProgress <= 1)
            {
                _dayProgress += Time.deltaTime / DayRealTime;
                _dayUI.SetDayUI(DayProgress);
                yield return null;
            }
            ++Day;
            if (Day <= GetDayActionStopDate())
                GameMng.instance.DayAction?.Invoke();
            else
            {
                if (Day == _dayEndTime)
                    GameMng.instance.GameEnd();
            }
            _dayUI.SetDayUI(Day);
        }
    }

    public int GetDayActionStopDate()
    {
        return _dayEndTime - _waitingJudgmentTime;
    }

}
