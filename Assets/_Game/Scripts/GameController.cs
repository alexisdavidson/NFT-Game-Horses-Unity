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
    List<int> finishers = new List<int>();
    bool raceStarted;

    float updateCameraParentDelay = 0.2f;
    float updateCameraParentLast;

    struct curve
    {
        public float a;
        public float b;
        public float xToReach10;
    }
    List<curve> curves;

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
            Camera.main.transform.parent = mostAheadHorse?.transform;

            updateCameraParentLast = Time.time;
        }
    }

    void StartGame(string data)
    {
        ParseData(data);
        ComputeCurves();
        ActivateHorses();
        raceStarted = true;
    }

    void ComputeCurves()
    {
        // f(x) = x + a sin(b x)
        // f(x) = 10
        // 10 - x = a sin(bx)
        // -x = a sin(bx) - 10
        // x/a = sin(bx) - 10/a
        // x/a - sin(bx) = -10/a
        curves = new List<curve>();
        foreach(var finisher in finishers)
        {
            var c = new curve();
            c.a = Random.Range(0.0f, 1);
            c.b = Random.Range(0.0f, 1);
            curves.Add(c);
        }
    }

    void ActivateHorses()
    {
        for(int i = 0; i < playerHorses.Length; i ++)
        {
            if (i < finishers.Count)
                playerHorses[i].Activate(true, finishers[i].ToString());
            else playerHorses[i].Activate(false, string.Empty);
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
