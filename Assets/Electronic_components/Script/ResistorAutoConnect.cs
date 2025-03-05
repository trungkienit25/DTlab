using UnityEngine;

using System.Linq;

public class ResistorAutoConnect : MonoBehaviour
{
    public Transform pinOther;  // Chân còn lại của con trở
    public LayerMask socketLayer; // Layer của socket
    public float searchRadius = 0.05f; // Bán kính tìm socket

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor currentSocket;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Socket"))  // Khi 1 chân vào socket
        {
            currentSocket = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
            FindAndAttachOtherPin();
        }
    }

    private void FindAndAttachOtherPin()
    {
        Collider[] sockets = Physics.OverlapSphere(pinOther.position, searchRadius, socketLayer);

        if (sockets.Length > 0)
        {
            // Tìm socket gần nhất
            Transform closestSocket = sockets
                .OrderBy(s => Vector3.Distance(pinOther.position, s.transform.position))
                .First().transform;

            // Gắn chân còn lại vào socket gần nhất
            pinOther.position = closestSocket.position;
            pinOther.SetParent(closestSocket);
        }
    }
}
