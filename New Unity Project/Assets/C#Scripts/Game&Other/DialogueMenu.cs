using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMenu : MonoBehaviour
{
    public static DialogueMenu dialogueMenu;

    public Text textName;
    public Text textDialogue;

    // Start is called before the first frame update
    void Start()
    {
        hideDialogue();
        dialogueMenu = this;
    }


    public void UpdateDialogue(string name, string text)
    {
        textName.text = name;
        textDialogue.text = text;

    }

    public void showDialogue()
    {
        this.gameObject.SetActive(true);
    }

    public void hideDialogue()
    {
        this.gameObject.SetActive(false);
    }

}
