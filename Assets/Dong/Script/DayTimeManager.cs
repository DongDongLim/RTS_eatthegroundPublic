using System.Collections;
using System.Collections.Generic;
//using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class DayTimeManager : MonoBehaviour
{
    // 싱글턴
    private static DayTimeManager m_instance;
    public static DayTimeManager Instance => m_instance;
    
    // 시간
    [HideInInspector] public ushort day;
    [HideInInspector] public ushort hour;
    [HideInInspector] public ushort minute;
    public float updateInterval;

    [Header("Day Cycle")]
    public GameObject sun;
    public GameObject moon;
    public bool isDayTime;
    public float maxFogDensity;
    public float fogDensityIncrement;
    private float m_dayFogDensity;
    private float m_currentFogDensity;
    
    //private Transform m_curLight;
    private const ushort DAY_START_TIME = 6;
    private const ushort NIGHT_START_TIME = 18;

    public UnityAction onTimeUpdate;

    private void Awake()
    {
        m_instance = this;
        DontDestroyOnLoad(gameObject);

        m_dayFogDensity = RenderSettings.fogDensity;
    }

    private void Start()
    {
        StartCoroutine(TimeUpdate());
        StartCoroutine(DayAndNightCycle());
    }

    private void Update()
    {
        if (isDayTime)
        {
            if (m_currentFogDensity <= m_dayFogDensity) return;
            
            m_currentFogDensity -= fogDensityIncrement * Time.deltaTime;
            RenderSettings.fogDensity = m_currentFogDensity;
        }
        else
        {
            if (m_currentFogDensity >= maxFogDensity) return;
            
            m_currentFogDensity += fogDensityIncrement * Time.deltaTime;
            RenderSettings.fogDensity = m_currentFogDensity;
        }
    }

    private IEnumerator TimeUpdate()
    {
        while (true)
        {
            minute++;
            if (minute == 60)
            {
                hour++;
                minute = 0;
            }
            if (hour == 24)
            {
                day++;
                hour = 0;
            }

            if (!isDayTime)
            {
                switch (hour)
                {
                    case NIGHT_START_TIME + 1:
                        sun.SetActive(false);
                        break;
                    case DAY_START_TIME - 1:
                        sun.SetActive(true);
                        break;
                    case DAY_START_TIME:
                        isDayTime = true;
                        break;
                }
            }
            else
            {
                switch (hour)
                {
                    case DAY_START_TIME + 1:
                        moon.SetActive(false);
                        break;
                    case NIGHT_START_TIME - 1:
                        moon.SetActive(true);
                        break;
                    case NIGHT_START_TIME:
                        isDayTime = false;
                        break;
                }
                
                
            }

            onTimeUpdate?.Invoke();

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private IEnumerator DayAndNightCycle()
    {
        while (true)
        {
            sun.transform.Rotate(-Vector3.right, 0.25f);
            moon.transform.Rotate(-Vector3.right, 0.25f);
            
            yield return new WaitForSeconds(updateInterval);
        }
    }
}
