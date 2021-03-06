﻿using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{

    // enemy movement speed
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;


    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;



    // Use this for initialization
    protected virtual void Start() {

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;

    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit) {
        // determines start and end positions
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        // makes sure unit doesn't hit own collider
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null) {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }


    protected IEnumerator SmoothMovement(Vector3 end)
    {

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // moves enemy unit
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

    }

    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component;
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        // if nothing was hit
        if (hit.transform == null)
            return;
        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hit.transform != null)
            OnCantMove(hitComponent);

    }
 

    protected abstract void OnCantMove <T> (T component)
        where T : Component;

}

