using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// makes the canvas always visible in the Frustum camera.
/// canvas follows the camera without being locked in front of the camera
/// </summary>
public class SimpleTagAlong : MonoBehaviour
{
    //Simple tag along seeks to stay at a fixed distance from the Camera.
    [Tooltip("The distance in meters from the camera for the Tagalong to seek when updating its position.")]
    public float tagAlongDistance = 2.0f;

    [Tooltip("If true, forces the Tagalong to be TagalongDistance from the camera, even if it didn't need to move otherwise.")]
    private bool enforceDistance = true;

    [Tooltip("The speed at which to move the Tagalong when updating its position (meters/second).")]
    private float positionUpdateSpeed = 9.8f;

    [Tooltip("When true, the Tagalong's motion is smoothed.")]
    public bool smoothMotion = true;

    [Tooltip("Will keep in front of the objects which has this layer")]
    public LayerMask floatOnLayer = 0;

    [Tooltip("Off sets the scale ratio so that text does not scale down too much. (Set to zero for linear scaling)")]
    public float SizeRatio = 0;

    // The ratio between the transform's local scale and its starting
    // distance from the camera.
    private float startingDistance;
    private Vector3 startingScale;

    public float fovStandard = 37;
    public float fovChangeFactor = 0.03f;

    [Range(0.0f, 1.0f), Tooltip("The factor applied to the smoothing algorithm. 1.0f is super smooth. But slows things down a lot.")]
    private float smoothingFactor = 0.75f;

    private Vector3 scale;


    // The BoxCollider represents the volume of the object that is tagging
    // along. It is a required component.
    protected BoxCollider tagAlongCollider;

    // The Interpolator is a helper class that handles various changes to an
    // object's transform. It is used by Tagalong to adjust the object's
    // transform.position.
    protected Interpolator interpolator;

    // This is an array of planes that define the camera's view frustum along
    // with some helpful indices into the array. The array is updated each
    // time through FixedUpdate().
    protected Plane[] frustumPlanes;
    protected const int frustumLeft = 0;
    protected const int frustumRight = 1;
    protected const int frustumBottom = 2;
    protected const int frustumTop = 3;

    private Vector3 cameraPos = Vector3.zero;
    private Vector3 selfPos = Vector3.zero;

    // Use this for initialization
    void Start () {

        // Make sure the Tagalong object has a BoxCollider.
        tagAlongCollider = GetComponent<BoxCollider>();
        if (!tagAlongCollider)
        {
            Debug.LogWarning("There must be a collider in tagAlone object!");
            // If we can't find one, disable the script.
            enabled = false;
            
        }

        tagAlongCollider.enabled = false;

        // Add an Interpolator component and set some default parameters for
        // it. These parameters can be adjusted in Unity's Inspector.
        interpolator = GetComponent<Interpolator>();
        if (interpolator == null)
            interpolator = gameObject.AddComponent<Interpolator>();
        interpolator.SmoothLerpToTarget = smoothMotion;
        interpolator.SmoothPositionLerpRatio = smoothingFactor;

        cameraPos = Camera.main.transform.position;
        selfPos = transform.position;
        Ray ray = new Ray(cameraPos, selfPos - cameraPos);
        transform.position = GetRayPosition(ray);

        startingScale = transform.localScale;

        startingDistance = tagAlongDistance;
        startingScale = transform.localScale;

        SetSizeRatio(SizeRatio);

    }

    protected virtual void Update()
    {
        //SPhysics.IgnoreCollision(gameObject.GetComponent<BoxCollider>(), MainApp);

        // Retrieve the frustum planes from the camera.
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        // Determine if the Tagalong needs to move based on whether its
        // BoxCollider is in or out of the camera's view frustum.
        Vector3 tagalongTargetPosition;
        if (CalculateTagalongTargetPosition(transform.position, out tagalongTargetPosition))
        {
            // Derived classes will use the same Interpolator and may have
            // adjusted its PositionUpdateSpeed for some other purpose.
            // Restore the value we care about and tell the Interpolator
            // to move the Tagalong to its new target position.
            interpolator.PositionPerSecond = positionUpdateSpeed;
            interpolator.SetTargetPosition(tagalongTargetPosition);
        }
        else if (!interpolator.Running && enforceDistance)
        {
            // If the Tagalong is inside the camera's view frustum, and it is
            // supposed to stay a fixed distance from the camera, force the
            // tagalong to that location (without using the Interpolator).
            cameraPos = Camera.main.transform.position;
            selfPos = transform.position;
            Ray ray = new Ray(cameraPos, selfPos - cameraPos);

            transform.position = GetRayPosition(ray);
        }
    }

    private Vector3 GetRayPosition(Ray ray)
    {

        RaycastHit hitInfo;
        float dis = tagAlongDistance;
        if (Physics.Raycast(ray, out hitInfo, 1000, floatOnLayer))
        {
            if (hitInfo.distance < tagAlongDistance)
            {
                dis = hitInfo.distance;
            }
        }

        return ray.GetPoint(dis);
    }

    protected virtual bool CalculateTagalongTargetPosition(Vector3 fromPosition, out Vector3 toPosition)
    {
        // Check to see if any part of the Tagalong's BoxCollider's bounds is
        // inside the camera's view frustum. Note, the bounds used are an Axis
        // Aligned Bounding Box (AABB).
        bool needsToMove = !GeometryUtility.TestPlanesAABB(frustumPlanes, tagAlongCollider.bounds);

        // Calculate a default position where the Tagalong should go. In this
        // case TagalongDistance from the camera along the gaze vector.
        toPosition = Camera.main.transform.position + Camera.main.transform.forward * tagAlongDistance;

        // If we already know we don't need to move, bail out early.
        if (!needsToMove)
        {
            return false;
        }

        // Create a Ray and set it's origin to be the default toPosition that
        // was calculated above.
        Ray ray = new Ray(toPosition, Vector3.zero);
        Plane plane = new Plane();
        float distanceOffset = 0f;

        // Determine if the Tagalong needs to move to the right or the left
        // to get back inside the camera's view frustum. The normals of the
        // planes that make up the camera's view frustum point inward.
        bool moveRight = frustumPlanes[frustumLeft].GetDistanceToPoint(fromPosition) < 0;
        bool moveLeft = frustumPlanes[frustumRight].GetDistanceToPoint(fromPosition) < 0;
        if (moveRight)
        {
            // If the Tagalong needs to move to the right, that means it is to
            // the left of the left frustum plane. Remember that plane and set
            // our Ray's direction to point towards that plane (remember the
            // Ray's origin is already inside the view frustum.
            plane = frustumPlanes[frustumLeft];
            ray.direction = -Camera.main.transform.right;
        }
        else if (moveLeft)
        {
            // Apply similar logic to above for the case where the Tagalong
            // needs to move to the left.
            plane = frustumPlanes[frustumRight];
            ray.direction = Camera.main.transform.right;
        }
        if (moveRight || moveLeft)
        {
            // If the Tagalong needed to move in the X direction, cast a Ray
            // from the default position to the plane we are working with.
            plane.Raycast(ray, out distanceOffset);

            // Get the point along that ray that is on the plane and update
            // the x component of the Tagalong's desired position.
            toPosition.x = ray.GetPoint(distanceOffset).x;
        }

        // Similar logic follows below for determining if and how the
        // Tagalong would need to move up or down.
        bool moveDown = frustumPlanes[frustumTop].GetDistanceToPoint(fromPosition) < 0;
        bool moveUp = frustumPlanes[frustumBottom].GetDistanceToPoint(fromPosition) < 0;
        if (moveDown)
        {
            plane = frustumPlanes[frustumTop];
            ray.direction = Camera.main.transform.up;
        }
        else if (moveUp)
        {
            plane = frustumPlanes[frustumBottom];
            ray.direction = -Camera.main.transform.up;
        }
        if (moveUp || moveDown)
        {
            plane.Raycast(ray, out distanceOffset);
            toPosition.y = ray.GetPoint(distanceOffset).y;
        }

        // Create a ray that starts at the camera and points in the direction
        // of the calculated toPosition.
        cameraPos = Camera.main.transform.position;
        selfPos = transform.position;
        ray = new Ray(cameraPos, toPosition - cameraPos);

        // Find the point along that ray that is the right distance away and
        // update the calculated toPosition to be that point.
        toPosition = GetRayPosition(ray);

        // If we got here, needsToMove will be true.
        return needsToMove;
    }

    /// <summary>
    /// Manually update the OverrideSizeRatio during runtime or through UnityEvents in the editor
    /// </summary>
    /// <param name="ratio"> 0 - 1 : Use 0 for linear scaling</param>
    public void SetSizeRatio(float ratio)
    {
        if (ratio == 0)
        {
            if (startingDistance > 0.0f)
            {
                // set to a linear scale ratio
                SizeRatio = 1 / startingDistance;
            }
            else
            {
                // If the transform and the camera are both in the same
                // position (that is, the distance between them is zero),
                // disable this Behaviour so we don't get a DivideByZero
                // error later on.
                enabled = false;
#if UNITY_EDITOR
                Debug.LogWarning("The object and the camera are in the same position at Start(). The attached FixedAngularSize Behaviour is now disabled.");
#endif // UNITY_EDITOR
            }
        }
        else
        {
            SizeRatio = ratio;
        }
    }

    private void LateUpdate()
    {
        float distanceToHologram = Vector3.Distance(Camera.main.transform.position, transform.position);
        float fov = Camera.main.fieldOfView;

        // create an offset ratio based on the starting position. This value creates a new angle that pivots
        // on the starting position that is more or less drastic than the normal scale ratio.
        float curvedRatio = 1 - startingDistance * SizeRatio;

        float f = 1 + (fov - fovStandard) * fovChangeFactor;
        if (f < 0.3f)
            f = 0.3f;

        transform.localScale = startingScale * (distanceToHologram * SizeRatio + curvedRatio) * f;
    }
}

