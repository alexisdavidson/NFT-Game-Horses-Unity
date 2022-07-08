using TMPro;
using UnityEngine;
using System.Runtime.InteropServices;

public class GameController : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public void ReceiveReactData(int data)
    {
        Debug.Log($"Receive {data} data!");
        text.text = data.ToString();
    }


    [DllImport("__Internal")]
    private static extern void HandleRequestData();
    void Start()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
    HandleRequestData ();
#endif
    }
}
