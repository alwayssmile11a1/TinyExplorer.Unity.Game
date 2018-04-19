using UnityEngine;

public class RotateWithMath : MonoBehaviour {
    enum RunType { Circle, Tilt_Toward_A_Side, Flower};
    [Header("Run Type")]
    public bool Circel;
    public bool Tilt_Toward_A_Side;
    public bool Flower;
    private RunType runType;

    [Header("General")]
    public float speed = 2;
    public float radius = 2;
    public Transform origin;
    [SerializeField]
    private Vector2 offsetFromZero;

    [Header("Oval")]
    public float tiltOffsetY;
    public float tiltOffsetX;

    [Header("Flower")]
    public float frequency;
    // Use this for initialization
    void Start () {
        offsetFromZero = origin.position;
        if (Circel)
            runType = RunType.Circle;
        else if (Tilt_Toward_A_Side)
            runType = RunType.Tilt_Toward_A_Side;
        else if (Flower)
            runType = RunType.Flower;
        else
            runType = RunType.Circle;
	}
	
	// Update is called once per frame
	void Update () {
        float value = Time.timeSinceLevelLoad * speed;
        switch (runType)
        {
            case RunType.Circle:
                transform.position = (new Vector2(Mathf.Sin(value) * radius, Mathf.Cos(value)*radius)) + offsetFromZero;
                break;
            case RunType.Tilt_Toward_A_Side:
                transform.position = (new Vector2(Mathf.Sin(value + tiltOffsetX) * radius, Mathf.Cos(value + tiltOffsetY) * radius)) + offsetFromZero;
                break;
            case RunType.Flower:
                transform.position = new Vector2(Mathf.Sin(value) * (radius + Mathf.Cos(value * frequency)), Mathf.Cos(value) * (radius + Mathf.Cos(value * frequency))) + offsetFromZero;
                break;
        }
    }
}
