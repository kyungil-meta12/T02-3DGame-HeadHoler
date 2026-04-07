using System.Collections.Generic;
using UnityEngine;

public class CarObstacle : MonoBehaviour
{
    public Light[] lights; // 차량 라이트
    private List<float> lightIntensity; // 라이트 밝기 초기 값
    public float alarmDuration; // 차량 경보 지속 시간
    public float lightFlickInterval; // 차량 라이트 깜빡임 간격

    private float durateTime; // 차량 경보 업데이트 시간
    private bool alarmState = false; // 차량 경보 여부

    private float flickTime; // 라이트 깜빡임 업데이트 시간
    private bool lightOn = false; // 차량 라이트 켜짐 여부

    void Start()
    {
        lightIntensity = new List<float>();
        for (int i = 0; i < lights.Length; i++)
        {
            lightIntensity.Add(lights[i].intensity);
        }
        SetLight(false);
    }

    public void Hit()
    {
        durateTime = 0f;
        flickTime = 0f;
        alarmState = true;
    }

    void Update()
    {
        if (alarmState) // alarmState가 활성화 되면 차량 경보 사운드를 재생하면서 전조등을 깜빡인다 
        { // 일정 시간동안 경보가 울리다 시간이 지나면 다시 멈춘다
            durateTime += Time.deltaTime;
            if (durateTime < alarmDuration)
            {
                flickTime += Time.deltaTime;
                if (flickTime >= lightFlickInterval)
                {
                    flickTime -= lightFlickInterval;
                    lightOn = !lightOn;
                    SetLight(lightOn);
                }
            }
            else
            {
                durateTime = 0f;
                flickTime = 0f;
                alarmState = false;
                SetLight(false);
            }
        }
    }

    void SetLight(bool flag)
    {
        for(int i = 0; i < lights.Length; i ++)
        {
            lights[i].intensity = flag ? lightIntensity[i] : 0f;
        }
    }
}
