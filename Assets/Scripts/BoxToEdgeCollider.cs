using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxToEdgeCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            // Adds 4 corners to the array
            Vector2[] boxColliderPoints = new Vector2[5];

            // Top right
            boxColliderPoints[0] = new Vector2(boxCollider.bounds.extents.x / transform.lossyScale.x,
                boxCollider.bounds.extents.y / transform.lossyScale.y);

            // Bottom right
            boxColliderPoints[1] = new Vector2(boxCollider.bounds.extents.x / transform.lossyScale.x,
                -boxCollider.bounds.extents.y / transform.lossyScale.y);

            // Bottom left
            boxColliderPoints[2] = new Vector2(-boxCollider.bounds.extents.x / transform.lossyScale.x,
                -boxCollider.bounds.extents.y / transform.lossyScale.y);

            // Top left
            boxColliderPoints[3] = new Vector2(-boxCollider.bounds.extents.x / transform.lossyScale.x,
                boxCollider.bounds.extents.y / transform.lossyScale.y);

            // Top right
            boxColliderPoints[4] = new Vector2(boxCollider.bounds.extents.x / transform.lossyScale.x,
                boxCollider.bounds.extents.y / transform.lossyScale.y);

            // Adds the edge collider and deletes the box collider
            EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            edgeCollider.points = boxColliderPoints;
            edgeCollider.offset = boxCollider.offset;
            DestroyImmediate(boxCollider);
        }
    }

}
