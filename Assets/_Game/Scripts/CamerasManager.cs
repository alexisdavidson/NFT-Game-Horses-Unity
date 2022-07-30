using System.Collections;
using UnityEngine;

public class CamerasManager : MonoBehaviour
{
    [SerializeField] GameObject[] cameras;
    [SerializeField] float cooldownToChange = 3.5f;
    [SerializeField] GameController gameController;

    int currentActiveCamera = 0;

    void Start()
    {
        StartCoroutine(FinishRaceInSeconds());
    }

    IEnumerator FinishRaceInSeconds()
    {
        while(!gameController.RaceFinished)
        {
            yield return new WaitForSeconds(cooldownToChange);
            if (!gameController.RaceFinished)
            {
                cameras[currentActiveCamera].SetActive(false);
                currentActiveCamera++;
                if (currentActiveCamera >= cameras.Length)
                    currentActiveCamera = 0;
                cameras[currentActiveCamera].SetActive(true);

                gameController.SetShowNftIdsOnHorses(currentActiveCamera == 0);
            }
        }
        yield return null;
    }
}
