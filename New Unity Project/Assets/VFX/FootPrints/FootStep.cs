using UnityEngine;
using UnityEngine.Rendering.Universal;


public class FootStep : MonoBehaviour
{
    public float lifeTime = 8f;     // Time before fade starts
    public float fadeDuration = 2f; // How long the fade lasts
    public GameObject parent;

    private DecalProjector decal;
    private float timer;

    void Start()
    {
        Destroy(parent.gameObject, 10.0f);
        decal = GetComponent<DecalProjector>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifeTime)
        {
            float t = (timer - lifeTime) / fadeDuration;
            decal.fadeFactor = Mathf.Lerp(1f, 0f, t);
        }
    }
}
