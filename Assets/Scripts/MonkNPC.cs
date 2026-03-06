using UnityEngine;

public class MonkNPC : MonoBehaviour
{
    public DialogueManager dialogueManager;

    public string[] dialogueLines;   // thêm dòng này

    bool playerInRange = false;
    bool isTalking = false;

    void Update()
    {
        if (playerInRange && !isTalking)
        {
            // Kiểm tra thêm: dialoguePanel phải đang tắt
            // Tránh trường hợp nhấn F kết thúc hội thoại xong bị bắt đầu lại ngay
            bool dialogueActive = DialogueManager.Instance != null &&
                                  DialogueManager.Instance.IsDialogueActive();

            if (!dialogueActive && Input.GetKeyDown(KeyCode.F))
            {
                StartTalk();
            }
        }
    }

    void StartTalk()
    {
        isTalking = true;

        dialogueManager.StartDialogue(dialogueLines); // truyền lines
    }

    public void EndTalk()
    {
        isTalking = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}