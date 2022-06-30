using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] bool followX;
    [SerializeField] bool followY;
    [SerializeField] bool followZ;

    [Header("Pixel Perfect Settings")]
    [SerializeField] bool pixelPerfect;
    [SerializeField] float pixelsPerUnit = 100;

    void Update()
    {
        if (target)
        {
            if (pixelPerfect)
                SetNewPosition(PixelPerfectClamp(target.position, pixelsPerUnit));
            else
                SetNewPosition(target.position + offset);
        }
    }

    private void SetNewPosition(Vector3 position)
    {
        Vector3 newPos = transform.position;
        if (followX) newPos.x = position.x;
        if (followY) newPos.y = position.y;
        if (followZ) newPos.z = position.z;
        transform.position = newPos;
    }

    private Vector3 PixelPerfectClamp(Vector3 moveVector, float pixelsPerUnit)
    {
        Vector3 vectorInPixels = new Vector3(Mathf.CeilToInt(moveVector.x * pixelsPerUnit + offset.x), Mathf.CeilToInt(moveVector.y * pixelsPerUnit + offset.y), Mathf.CeilToInt(moveVector.z * pixelsPerUnit + offset.z));
        return vectorInPixels / pixelsPerUnit;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }    
}