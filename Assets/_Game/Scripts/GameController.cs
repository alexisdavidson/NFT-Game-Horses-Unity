using TMPro;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerHorse[] playerHorses;
    [SerializeField] TMP_Text text;
    [SerializeField] string testData = "";
    [SerializeField] float goalX = 500;
    [SerializeField] float curveGoal = 10;
    [SerializeField] float delayToShowResults = 2.0f;
    [SerializeField] GameObject endMenu;
    [SerializeField] TMP_Text winnerText;
    [SerializeField] Transform camerasManagerTransform;
    List<int> finishers = new List<int>();
    List<Curve> curves;

    [SerializeField] float updateCameraParentDelay = 0.5f;
    float updateCameraParentLast;

    [SerializeField] float checkRaceFinishedDelay = 0.5f;
    float checkRaceFinishedLast;

    public bool RaceFinished { get { return raceFinished; } }
    bool raceStarted;
    bool raceFinished;

    void StartGame(string data)
    {
        playerHorses = playerHorses.OrderBy(x => Random.Range(0, 10)).ToArray();

        ParseData(data);
        ComputeCurves();
        ActivateHorses();
        raceStarted = true;
    }

    void FixedUpdate()
    {
        if (raceStarted && !raceFinished)
        {
            UpdateCameraParent();
            CheckIfRaceFinished();
        }
    }

    IEnumerator FinishRaceInSeconds()
    {
        raceFinished = true;
        yield return new WaitForSeconds(delayToShowResults);
        endMenu.SetActive(true);
        winnerText.text = "Horse " + finishers[0].ToString();
        Time.timeScale = 0.0f;
        yield return null;
    }

    void CheckIfRaceFinished()
    {
        if (Time.time - checkRaceFinishedLast > checkRaceFinishedDelay)
        {
            var horsesNotAtTheEnd = playerHorses.FirstOrDefault(item => item.transform.position.x < goalX && item.gameObject.activeSelf);

            if (horsesNotAtTheEnd == null)
            {
                Debug.Log("Race finished!");
                StartCoroutine(FinishRaceInSeconds());
            }

            checkRaceFinishedLast = Time.time;
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
                    camerasManagerTransform.parent = null;
                else
                    camerasManagerTransform.parent = mostAheadHorse.transform;
            }

            updateCameraParentLast = Time.time;
        }
    }

    public void SetShowNftIdsOnHorses(bool active)
    {
        foreach(var horse in playerHorses)
            horse.SetShowNftIdsOnHorses(active);
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
