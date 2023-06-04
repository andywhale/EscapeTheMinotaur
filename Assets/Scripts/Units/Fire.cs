using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] public LineRenderer lineRenderer;

    [SerializeField] public RectTransform target;

    void Start()
    {
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
    }

    public void Shoot(Transform target)
    {
        Vector2 direction = target.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target.position);
        lineRenderer.enabled = true;

        StartCoroutine(waitForSec(0.6f));
    }

    void EnableLaser()
    {
        lineRenderer.enabled = true;
    }

    private IEnumerator waitForSec(float sec)
    {
        yield return new WaitForSeconds(sec);
        lineRenderer.enabled = false;
    }
}
