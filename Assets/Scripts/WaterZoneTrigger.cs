using UnityEngine;

public class WaterZoneTrigger : MonoBehaviour
{
    public MadaFollowerAI mada;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            mada.SetPlayerInWater(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            mada.SetPlayerInWater(false);
    }
}
