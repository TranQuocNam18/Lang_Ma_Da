using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public GameObject interactText;
    public Camera playerCamera;

    void Update()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            MonkNPC monk = hit.collider.GetComponentInParent<MonkNPC>();

            if (monk != null)
            {
                interactText.SetActive(true);
                interactText.GetComponent<TextMeshProUGUI>().text = "Nói chuyện [F]";

                if (Input.GetKeyDown(KeyCode.F))
                {
                    // nếu đang nói chuyện thì chuyển câu
                    if (DialogueManager.Instance.isTalking)
                    {
                        DialogueManager.Instance.NextLine();
                    }
                    else
                    {
                        monk.StartTalk();
                    }
                }

                return;
            }

            interactText.SetActive(false);
        }
        else
        {
            interactText.SetActive(false);
        }
    }
}