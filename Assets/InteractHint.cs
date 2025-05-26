using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum InteractHintType
{
    None,
    Rest,
    Pickup,
    Use,
    Talk
}

public class InteractHint : MonoBehaviour
{
    public InteractHintType interactHintType;
    public TextMeshProUGUI interactHintText;

    private void Start()
    {
        switch (interactHintType)
        {
            case InteractHintType.Rest:
                interactHintText.text = "Press 'E' to Rest";
                break;
            case InteractHintType.Pickup:
                interactHintText.text = "Press 'E' to Pickup";
                break;
            case InteractHintType.Use:
                interactHintText.text = "Press 'E' to Use";
                break;
            case InteractHintType.Talk:
                interactHintText.text = "Press 'E' to Talk";
                break;
            default:
                interactHintText.text = "";
                break;
        }
    }

    private void Update()
    {
        Vector3 dir = transform.position - Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(dir);

        if (GameManager.Instance.isResting)
        {
            gameObject.SetActive(false);
        }
    }
}
