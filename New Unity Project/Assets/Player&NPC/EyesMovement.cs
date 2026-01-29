using UnityEngine;

public class EyesMovement : MonoBehaviour
{
    public GameObject eyeR;
    public GameObject eyeL;
    private Renderer renderEyeL, renderEyeR;

    public Transform eyePivot;
    public Transform pivotLookAt;

    public float eyeLMin;
    public float eyeLMax;
    public float eyeRMin;
    public float eyeRMax;


    public float eyeY_min;
    public float eyeY_max;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderEyeL = eyeL.GetComponent<Renderer>();
        renderEyeR = eyeR.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        eyePivot.LookAt(pivotLookAt);
        Vector2 tempEyeRot = new Vector2(eyePivot.localRotation.y, eyePivot.localRotation.x);

        float tempEyeL_limit = Mathf.Clamp(tempEyeRot.x, eyeLMin, eyeLMax);
        float tempEyeR_limit = Mathf.Clamp(tempEyeRot.x, eyeRMin, eyeRMax);
        float tempEyeY_limit = Mathf.Clamp(tempEyeRot.y, eyeY_min, eyeY_max);

        renderEyeL.material.mainTextureOffset = new Vector2(tempEyeL_limit, tempEyeY_limit);
        renderEyeR.material.mainTextureOffset = new Vector2(tempEyeR_limit, tempEyeY_limit);
    }
}
