using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionChecker2D : MonoBehaviour
{
    [SerializeField] string[] tagsToCheck;

    public UnityAction<Collider2D> OnTrigger2D;
    public UnityAction<Collider2D> OnCollision2D;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (tagsToCheck.Contains(collider.tag))
        {
            OnTrigger2D?.Invoke(collider);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        if (tagsToCheck.Contains(collider.tag))
        {
            OnCollision2D?.Invoke(collision.collider);
        }
    }
}
