using TMPro;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerHorse[] playerHorses;
    [SerializeField] TMP_Text text;
    [SerializeField] string testData = "";
    List<int> finishers = new List<int>();

    void StartGame(string data)
    {
        ParseData(data);
        ActivateHorses();
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
