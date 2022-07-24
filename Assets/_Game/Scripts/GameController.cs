using TMPro;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerHorse[] playerHorses;
    [SerializeField] TMP_Text text;
    [SerializeField] string testData = "";
    [SerializeField] float goalX = 500;
    [SerializeField] float curveGoal = 10;
    List<int> finishers = new List<int>();
    bool raceStarted;
    List<Curve> curves;

    [SerializeField] float updateCameraParentDelay = 0.5f;
    float updateCameraParentLast;

    void StartGame(string data)
    {
        ParseData(data);
        ComputeCurves();
        ActivateHorses();
        raceStarted = true;
    }

    void FixedUpdate()
    {
        if (raceStarted)
        {
            UpdateCameraParent();
        }
    }

    void UpdateCameraParent()
    {
        if (Time.time - updateCameraParentLast > updateCameraParentDelay)
        {
            var mostAheadHorse = playerHorses.OrderByDescending(item => item.transform.position.x).FirstOrDefault();
            if (mostAheadHorse != null)
            {
                if (mostAheadHorse.transform.position.x > goalX)
                    Camera.main.transform.parent = null;
                else Camera.main.transform.parent = mostAheadHorse.transform;
            }

            updateCameraParentLast = Time.time;
        }
    }

    void ComputeCurves()
    {
        curves = new List<Curve>();
        foreach(var finisher in finishers)
        {
            var curve = new Curve();
            curve.a = Random.Range(0.1f, 0.9f);
            curve.b = Random.Range(0.1f, 0.9f);
            curve.goalX = curveGoal;
            curves.Add(curve);
        }

        float step = 0.01f;
        foreach (var curve in curves)
        {
            float currentX = 0;
            float currentY = 0;
            while (currentY < curve.goalX)
            {
                // f(x) = x + a sin(b x)
                currentY = currentX + curve.a * Mathf.Sin(curve.b * currentX);
                currentX += step;
            }
            curve.xToReachGoal = currentX;
        }

        curves = curves.OrderBy(x => x.xToReachGoal).ToList();
    }

    void ActivateHorses()
    {
        for(int i = 0; i < playerHorses.Length; i ++)
        {
            if (i < finishers.Count)
                playerHorses[i].Activate(curves[i], goalX, finishers[i].ToString());
            else playerHorses[i].Deactivate();
        }
    }

    public void ReceiveReactData(string data)
    {
        Debug.Log($"Receive {data} data!");
        text.text = data;
        StartGame(data);
    }

    [DllImport("__Internal")]
    private static extern void HandleRequestData();
    void Start()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
    HandleRequestData ();
#endif

        if (testData != "")
        {
            StartGame(testData);
        }
    }

    void ParseData(string data)
    {
        data = data.Substring(1, data.Length - 2);
        var finishersString = data.Split(",");
        foreach (var finisherString in finishersString)
            finishers.Add(int.Parse(finisherString));

        //Debug.Log("finishers: ");
        //foreach(var finisher in finishers)
        //    Debug.Log(finisher);
    }
}
