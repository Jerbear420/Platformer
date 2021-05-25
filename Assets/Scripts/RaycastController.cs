using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : PlatformerSystem
{



    public LayerMask collisionMask;

    [HideInInspector]
    public BoxCollider2D _collider;
    public RaycastOrigins raycastOrigins;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticleRaySpacing;
    public const float skinWidth = .015f;
    // Start is called before the first frame update
    public virtual void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();

    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(skinWidth * -2);
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticleRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope, descendingSlope;
        public Vector3 velocityOld;
        public float slopeAngle, slopeAngleOld;
        public int faceDir;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
