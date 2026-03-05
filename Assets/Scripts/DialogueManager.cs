using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public bool isTalking = false;

    private string[] lines;
    private int index = 0;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            NextLine();
        }
    }

    public void StartDialogue(string[] dialogueLines)
    {
        // tránh lỗi nếu mảng rỗng
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogError("Dialogue lines is empty!");
            return;
        }

        lines = dialogueLines;
        index = 0;

        dialoguePanel.SetActive(true);
        dialogueText.text = lines[index];
        isTalking = true;

        if (GameManager.Instance != null)
            GameManager.Instance.StartDialogue();
    }

    public void NextLine()
    {
        index++;

        if (index < lines.Length)
        {
            dialogueText.text = lines[index];
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.EndDialogue();
        isTalking = false;

        // chỉ bắt đầu nhiệm vụ 1 lần
        if (!ObjectiveManager.Instance.objectiveStarted)
        {
            ObjectiveManager.Instance.StartObjective();
        }

        // reset trạng thái NPC
        MonkNPC monk = FindObjectOfType<MonkNPC>();
        if (monk != null)
        {
            monk.EndTalk();
        }
    }

    /// <summary>
    /// Trả về true nếu dialogue panel đang hiển thị.
    /// Dùng để MonkNPC kiểm tra trước khi cho phép bắt đầu hội thoại mới.
    /// </summary>
    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }
}