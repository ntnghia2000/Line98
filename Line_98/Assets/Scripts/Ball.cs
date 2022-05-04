using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private SpriteRenderer ball;
    [SerializeField] private float speed = 5.0f;
    private Queue<Vector2> mWayPoints = new Queue<Vector2>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Coroutine_MoveTo());
    }

    public void AddWayPoint(float x, float y) {
        AddWayPoints(new Vector2(x, y));
    }

    public void AddWayPoints(Vector2 p) {
        mWayPoints.Enqueue(p);
    }

    IEnumerator Coroutine_MoveTo() {
        while (true) {
            while (mWayPoints.Count > 0) {
                yield return StartCoroutine(Coroutine_MoveToPoint(mWayPoints.Dequeue(), speed));
            }
            yield return null;
        }
    }

    IEnumerator Coroutine_MoveToPoint(Vector2 endP, float speed) {
        Vector3 p = new Vector3(endP.x, endP.y, transform.position.z);
        float duration = (transform.position - p).magnitude / speed;
        yield return StartCoroutine(Coroutine_MoveInSeconds(p, duration));
    }

    IEnumerator Coroutine_MoveInSeconds(Vector3 endP, float duration) {
        float elapsedTime = 0.0f;
        Vector3 startP = transform.position;
        while (elapsedTime < duration) {
            transform.position = Vector3.Lerp(startP, endP, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endP;
    }

    public Vector3 GetBallPos () {
        return transform.position;
    }
}
