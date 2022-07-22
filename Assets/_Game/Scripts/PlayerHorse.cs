using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHorse : MonoBehaviour
{
    [SerializeField] Animator animatorHorse;
    [SerializeField] Animator animatorJockey;
    [SerializeField] TextMesh textMesh;
    [SerializeField] float delayBeforeMove = 2.0f;
    [SerializeField] float speed = 4.0f;
    [SerializeField] float speedVariation = 1.3f;

    bool moving;

    public void Activate(bool state, string playerName)
    {
        moving = false;
        gameObject.SetActive(state);
        textMesh.text = playerName;

        const string animationToPlay = "Idle02";
        animatorHorse.Play(animationToPlay);
        animatorJockey.Play(animationToPlay);

        if (gameObject.activeSelf)
            StartCoroutine(StartMovingAfterTime());
    }

    IEnumerator StartMovingAfterTime()
    {
        yield return new WaitForSeconds(delayBeforeMove);
        moving = true;
        speed = Random.Range(speed / speedVariation, speed * speedVariation);
        yield return null;
    }

    private void FixedUpdate()
    {
        if (moving)
            gameObject.transform.position += new Vector3(Time.deltaTime * speed, 0, 0);
    }
}
