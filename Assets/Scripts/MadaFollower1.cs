using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MadaFollowerAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public Rigidbody playerRb;
    public MicListener playerMic;

    [Header("Movement")]
    public float moveSpeed = 2.2f;
    public float rotateSpeed = 6f;
    public float followDistance = 3f;

    [Header("Attack")]
    public float attackDistance = 1.2f;
    public float jumpscareDuration = 0.8f;
    public GameObject gameOverUI;

    [Header("Vision")]
    public float viewDistance = 12f;
    public float eyeHeight = 1.5f;
    public LayerMask obstacleMask;

    [Header("Hearing")]
    public float hearThreshold = 0.02f;

    [Header("Player Look")]
    public float stopDot = 0.6f;
    public float disappearTime = 2.5f;

    [Header("Teleport (Terrain Safe)")]
    public float groundRayHeight = 25f;
    public LayerMask groundMask;

    [Header("Animator")]
    public string movingBool = "isMoving";
    public string chasingBool = "isChasing";

    Rigidbody rb;
    Animator animator;
    Renderer[] renderers;
    Collider[] colliders;

    bool isVisible;
    bool isFrozen;
    bool isChasing;
    bool isAttacking;
    float lookTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        HideMada();
    }

    void FixedUpdate()
    {
        if (!player || isAttacking) return;

        // ===== PLAYER NHÌN → FREEZE =====
        if (IsPlayerLookingAtMada())
        {
            lookTimer += Time.fixedDeltaTime;
            Freeze();
            SetAnim(false, false);

            if (lookTimer >= disappearTime)
                HideMada();

            return;
        }

        lookTimer = 0f;
        UnFreeze();

        // ===== CHƯA HIỆN → TELEPORT =====
        if (!isVisible)
        {
            TeleportBehindPlayerSafe();
            return;
        }

        // ===== SENSE =====
        bool canSee = CanSeePlayer();
        bool canHear = CanHearPlayer();
        isChasing = canSee || canHear;

        if (!isChasing)
        {
            SetAnim(false, false);
            return;
        }

        // ===== CHASE =====
        ChasePlayer();
    }

    // ================= CHASE & ATTACK =================

    void ChasePlayer()
    {
        Vector3 target = player.position;
        target.y = transform.position.y;

        Vector3 dir = target - transform.position;
        float dist = dir.magnitude;

        // ATTACK
        if (dist <= attackDistance)
        {
            StartCoroutine(JumpscareSequence());
            return;
        }

        Vector3 move = dir.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        Quaternion rot = Quaternion.LookRotation(dir);
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.fixedDeltaTime));

        SetAnim(true, true); // Crawl
    }

    // ================= SENSE =================

    bool CanSeePlayer()
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 dir = (player.position - origin).normalized;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, viewDistance, ~obstacleMask))
            return hit.transform == player;

        return false;
    }

    bool CanHearPlayer()
    {
        return playerMic && playerMic.loudness > hearThreshold;
    }

    bool IsPlayerLookingAtMada()
    {
        if (!isVisible) return false;

        Vector3 dir = (transform.position - player.position).normalized;
        float dot = Vector3.Dot(player.forward, dir);
        return dot > stopDot;
    }

    // ================= TELEPORT SAFE (FPS + TERRAIN) =================

    void TeleportBehindPlayerSafe()
    {
        Camera cam = Camera.main;
        if (!cam) return;

        Vector3 backDir = -player.forward;
        backDir.y = 0f;
        backDir.Normalize();

        Vector3 desiredPos = player.position + backDir * followDistance;

        // Không teleport nếu còn trong FOV
        Vector3 vp = cam.WorldToViewportPoint(desiredPos);
        if (vp.z > 0 && vp.x > 0 && vp.x < 1 && vp.y > 0 && vp.y < 1)
            return;

        Vector3 rayOrigin = desiredPos + Vector3.up * groundRayHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit,
            groundRayHeight * 2f, groundMask))
        {
            transform.position = hit.point;
            transform.LookAt(player);
            ShowMada();
        }
    }

    // ================= JUMPSCARE =================

    IEnumerator JumpscareSequence()
    {
        isAttacking = true;

        rb.linearVelocity = Vector3.zero;
        SetAnim(false, false);

        // Khóa player
        if (playerRb)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        // Đưa MADA sát camera
        Camera cam = Camera.main;
        if (cam)
        {
            transform.position = cam.transform.position + cam.transform.forward * 0.5f;
            transform.LookAt(cam.transform);
        }

        yield return StartCoroutine(CameraShake(0.4f, 0.25f));
        yield return new WaitForSeconds(jumpscareDuration);

        if (gameOverUI)
            gameOverUI.SetActive(true);

        Time.timeScale = 0f;
    }

    IEnumerator CameraShake(float duration, float magnitude)
    {
        Camera cam = Camera.main;
        if (!cam) yield break;

        Transform t = cam.transform;
        Vector3 originalPos = t.localPosition;

        float time = 0f;
        while (time < duration)
        {
            t.localPosition = originalPos +
                new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * magnitude;

            time += Time.deltaTime;
            yield return null;
        }

        t.localPosition = originalPos;
    }

    // ================= ANIM =================

    void SetAnim(bool moving, bool chasing)
    {
        if (!animator) return;
        animator.SetBool(movingBool, moving);
        animator.SetBool(chasingBool, chasing);
    }

    // ================= FREEZE =================

    void Freeze()
    {
        if (isFrozen) return;
        isFrozen = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        animator.speed = 0f;
    }

    void UnFreeze()
    {
        if (!isFrozen) return;
        isFrozen = false;
        animator.speed = 1f;
    }

    // ================= VISIBILITY =================

    void HideMada()
    {
        isVisible = false;
        isChasing = false;
        lookTimer = 0f;
        SetAnim(false, false);

        foreach (var r in renderers) r.enabled = false;
        foreach (var c in colliders) c.enabled = false;
    }

    void ShowMada()
    {
        isVisible = true;

        foreach (var r in renderers) r.enabled = true;
        foreach (var c in colliders) c.enabled = true;
    }
}
