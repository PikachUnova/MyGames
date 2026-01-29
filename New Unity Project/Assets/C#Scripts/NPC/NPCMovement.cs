using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 180f; // degrees per second
    [SerializeField] private float facingThreshold = 2f; // degrees

    public NPCConversation conversation;
    private bool isTalking = false;
    private GameObject player;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsFacingPlayer() && isTalking)
            anim.SetFloat("Direction", 1f, 1f, Time.deltaTime);
        else
            anim.SetFloat("Direction", 0f, 1f, Time.deltaTime);
    }

    public void Talk()
    {
        if (isTalking)
            return;
        isTalking = true;
        StartCoroutine(Turn());
    }

    IEnumerator Turn()
    {
        while (!IsFacingPlayer())
        {
            LookAtPlayer();
            yield return null; // wait one frame
        }
        ConversationManager.Instance.StartConversation(conversation);
    }

    bool IsFacingPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        return angle < facingThreshold;
    }

    void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }


    public void Speak() // Trigger event
    {
        this.GetComponent<Animator>().Play("Talk");
    }

    public void SetIsTalking(bool talking) // Trigger event
    {
        isTalking = talking;
        player.GetComponent<PlayerMovement>().SetIsTalking(talking);
    }
}
