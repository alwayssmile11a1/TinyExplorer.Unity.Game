using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RotateAroundTarget : MonoBehaviour {

    public Transform target;
    public Vector3 rotatingPointFromTargetOffset;
    public float speed = -10f;

    [Tooltip("If set, the start position of the gameobject will be calculated based on distanceFromRotatingPoint and startAngleOffset")]
    public bool useModifierVariables = false;
    public float distanceFromRotatingPoint = 1f;
    public float startAngleOffset = 0;


    private void Awake()
    {
        if (useModifierVariables)
        {
            //set transform position
            transform.position = target.position + rotatingPointFromTargetOffset + Vector3.right * distanceFromRotatingPoint;
            transform.RotateAround(target.position + rotatingPointFromTargetOffset, Vector3.forward, startAngleOffset);
        }
    }

    // Update is called once per frame
    private void Update () {

        transform.RotateAround(target.position + rotatingPointFromTargetOffset, Vector3.forward, 10 * speed * Time.deltaTime);

	}


    public void SetDistanceFromRotatingPoint(float distanceFromRotatingPoint)
    {
        this.distanceFromRotatingPoint = distanceFromRotatingPoint;

        transform.position = (target.position + rotatingPointFromTargetOffset - transform.position).normalized * distanceFromRotatingPoint;
    }

    public void SetStartPosition(float angleOffset, float distanceFromRotatingPoint)
    {
        startAngleOffset = angleOffset;
        this.distanceFromRotatingPoint = distanceFromRotatingPoint;

        //set transform position
        transform.position = target.position + rotatingPointFromTargetOffset + Vector3.right * distanceFromRotatingPoint;
        transform.RotateAround(target.position + rotatingPointFromTargetOffset, Vector3.forward, startAngleOffset);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {

        if (target == null) return;

        Handles.color = new Color(0, 1f, 0, 0.5f);
        Handles.DrawSolidDisc(target.position + rotatingPointFromTargetOffset, Vector3.back, 0.1f);

    }
#endif
}
