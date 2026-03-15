using UnityEngine;

// Quản lý máu và trạng thái sinh tồn của người chơi
public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;

    // Khởi tạo lượng máu ban đầu
    void Start()
    {
        currentHP = maxHP;
    }

    // Xử lý khi người chơi nhận sát thương
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);


        CameraShake shake = Camera.main.GetComponent<CameraShake>();
        if (shake != null)
        {
            shake.Shake(0.15f, 0.25f);
        }
        if (currentHP <= 0)
        {
            Die();
        }
    }

    // Xử lý khi người chơi hết máu
    void Die()
    {
        Debug.Log("Player chet!");
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
