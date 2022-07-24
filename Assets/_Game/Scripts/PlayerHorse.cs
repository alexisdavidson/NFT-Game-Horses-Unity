using System.Collections;
using UnityEngine;

public class PlayerHorse : MonoBehaviour
{
    [SerializeField] Animator animatorHorse;
    [SerializeField] Animator animatorJockey;
    [SerializeField] TextMesh textMesh;
    [SerializeField] float delayBeforeMove = 2.0f;
    [SerializeField] float speed = 4.0f;

    Vector3 originalPosition;
    Curve curve;
    float startTime;
    bool moving;
    float goalX;

    public void Activate(Curve curve, float goalX, string playerName)
    {
        Debug.Log("Activate horse " + playerName + " with curve a=" + curve.a + ", b=" + curve.b + ", xToReach10=" + curve.xToReachGoal);
        this.curve = curve;
        this.goalX = goalX;
        originalPosition = transform.position;
        moving = false;
        gameObject.SetActive(true);
        textMesh.text = playerName;

        const string animationToPlay = "Idle02";
        animatorHorse.Play(animationToPlay);
        animatorJockey.Play(animationToPlay);

        if (gameObject.activeSelf)
            StartCoroutine(StartMovingAfterTime());
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    IEnumerator StartMovingAfterTime()
    {
        yield return new WaitForSeconds(delayBeforeMove);
        moving = true;
        startTime = Time.time;
        yield return null;
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            var x = Time.time - startTime;
            var y = (x + curve.a * Mathf.Sin(curve.b * x)) * goalX * speed / curve.goalX;
            gameObject.transform.position = originalPosition + new Vector3(y, 0, 0);
        }
    }
}
