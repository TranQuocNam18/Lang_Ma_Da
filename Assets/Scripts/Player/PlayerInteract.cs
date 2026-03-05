using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;

    public GameObject interactText;

    public Camera playerCamera; // camera player

    void Update()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            NoteInteraction note = hit.collider.GetComponent<NoteInteraction>();

            if (note != null)
            {
                interactText.SetActive(true);
                interactText.GetComponent<TextMeshProUGUI>().text = "Đọc [F]";
                return;
            }

            interactText.SetActive(false);

            MonkNPC monk = hit.collider.GetComponent<MonkNPC>();

            if (monk != null)
            {
                interactText.SetActive(true);
                interactText.GetComponent<TextMeshProUGUI>().text = "Nói chuyện [F]";
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