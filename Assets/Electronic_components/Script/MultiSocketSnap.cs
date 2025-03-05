// using System.Collections;
// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;

// public class MultiSocketSnap : MonoBehaviour
// {
//     public Transform pinL_Attach;
//     public Transform pinR_Attach;
//     private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    
//     private Transform socketForPinR = null;
//     private Transform socketForPinL = null;

//     void Start()
//     {
//         grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
//         if (grabInteractable != null)
//         {
//             grabInteractable.selectEntered.AddListener(OnGrabbed);
//         }
//         else
//         {
//             Debug.LogError("XRGrabInteractable không tìm thấy trên " + gameObject.name);
//         }
//     }

//     private void OnGrabbed(SelectEnterEventArgs args)
//     {
//         Debug.Log("Object Grabbed: " + gameObject.name);

//         if (pinL_Attach == null || pinR_Attach == null)
//         {
//             Debug.LogError("Chưa gán pinL_Attach hoặc pinR_Attach!");
//             return;
//         }

//         // Tìm socket gần nhất cho chân phải (Pin_R)
//         socketForPinR = FindClosestSocket(pinR_Attach.position);
//         if (socketForPinR != null)
//         {
//             Debug.Log("Chân phải (Pin_R) cắm vào socket: " + socketForPinR.gameObject.name);

//             // Gắn Pin_R vào socket gần nhất
//             transform.position += (socketForPinR.position - pinR_Attach.position);
//             transform.rotation = socketForPinR.rotation;

//             // Tìm socket cho chân trái (Pin_L), nhưng bỏ qua socket của chân phải
//             StartCoroutine(AlignPinL());
//         }
//         else
//         {
//             Debug.LogError("Không tìm thấy socket cho chân phải (Pin_R)!");
//         }
//     }

//     private IEnumerator AlignPinL()
//     {
//         yield return new WaitForSeconds(0.1f);  // Đợi để vị trí cập nhật

//         socketForPinL = FindClosestSocket(pinL_Attach.position, socketForPinR);
//         if (socketForPinL != null)
//         {
//             Debug.Log("Chân trái (Pin_L) cần cắm vào socket: " + socketForPinL.gameObject.name);

//             AdjustRotation();
//         }
//         else
//         {
//             Debug.LogError("Không tìm thấy socket cho chân trái (Pin_L)!");
//         }
//     }

//     //  Hàm điều chỉnh góc quay để khớp với hướng socket
// private void AdjustRotation()
// {
//     if (socketForPinR == null || socketForPinL == null) return;

//     // 🏹 Vector từ chân R đến chân L
//     Vector3 vectorPin = (pinL_Attach.position - pinR_Attach.position).normalized;

//     // 🔌 Vector từ chân R đến socket của chân L
//     Vector3 vectorSocket = (socketForPinL.position - pinR_Attach.position).normalized;

//     Debug.Log("Vector giữa hai chân: " + vectorPin);
//     Debug.Log("Vector giữa chân R và socket L: " + vectorSocket);

//     // 📏 Tính góc xoay
//     float angle = Vector3.SignedAngle(vectorPin, vectorSocket, Vector3.up);
//     Vector3 rotationAxis = Vector3.Cross(vectorPin, vectorSocket).normalized;

//     if (rotationAxis.magnitude < 0.001f)
//     {
//         Debug.Log("⚠️ Trục quay quá nhỏ, không cần xoay!");
//         return;
//     }

//     Debug.Log("Trục quay: " + rotationAxis);
//     Debug.Log("Góc quay: " + angle);

//     // ✅ Thực hiện xoay bằng cách thay đổi rotation của object
//     Quaternion rotationDelta = Quaternion.AngleAxis(angle, rotationAxis);
//     transform.rotation = rotationDelta * transform.rotation;

//     // Đảm bảo vị trí của chân R không thay đổi sau khi xoay
//     Vector3 pinROffset = pinR_Attach.position - transform.position;
//     transform.position = socketForPinR.position - pinROffset;

//     Debug.Log("⚙️ Đã xoay con trở!");
// }


//     // Hàm tìm socket gần nhất, có thể loại trừ một socket
//     private Transform FindClosestSocket(Vector3 position, Transform excludedSocket = null)
//     {
//         UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor[] sockets = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
//         Transform closestSocket = null;
//         float minDistance = Mathf.Infinity;

//         Debug.Log("Tìm socket gần nhất cho vị trí: " + position);

//         foreach (var socket in sockets)
//         {
//             // Loại bỏ socket đã cắm Pin_R
//             if (excludedSocket != null && socket.transform == excludedSocket)
//                 continue;

//             float distance = Vector3.Distance(position, socket.transform.position);
//             Debug.Log("Khoảng cách tới " + socket.gameObject.name + ": " + distance);

//             if (distance < minDistance)
//             {
//                 minDistance = distance;
//                 closestSocket = socket.transform;
//             }
//         }

//         if (closestSocket != null)
//             Debug.Log("Socket gần nhất: " + closestSocket.gameObject.name);
//         else
//             Debug.LogError("Không tìm thấy socket nào!");

//         return closestSocket;
//     }
// }

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MultiSocketSnap : MonoBehaviour
{
    public Transform pinL_Attach;
    public Transform pinR_Attach;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    
    private Transform socketForPinR = null;
    private Transform socketForPinL = null;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
        else
        {
            Debug.LogError("XRGrabInteractable không tìm thấy trên " + gameObject.name);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Object Grabbed: " + gameObject.name);

        if (pinL_Attach == null || pinR_Attach == null)
        {
            Debug.LogError("Chưa gán pinL_Attach hoặc pinR_Attach!");
            return;
        }

        // 📌 Tìm socket gần nhất cho chân phải (Pin_R)
        socketForPinR = FindClosestSocket(pinR_Attach.position);
        if (socketForPinR != null)
        {
            Debug.Log("Chân phải (Pin_R) cắm vào socket: " + socketForPinR.gameObject.name);
            transform.position += (socketForPinR.position - pinR_Attach.position);
            transform.rotation = socketForPinR.rotation;

            // 📌 Tìm socket cho chân trái (Pin_L), nhưng bỏ qua socket của chân phải
            StartCoroutine(AlignPinL());
        }
        else
        {
            Debug.LogError("Không tìm thấy socket cho chân phải (Pin_R)!");
        }
    }

private void OnReleased(SelectExitEventArgs args)
{
    Debug.Log("Object Released: " + gameObject.name);

    if (!gameObject.activeInHierarchy)
    {
        Debug.LogWarning("GameObject đã bị tắt, thực hiện xoay ngay lập tức!");
        AdjustRotation();
        return;
    }

    StartCoroutine(DelayedAdjustRotation());
}


    private IEnumerator AlignPinL()
    {
        yield return new WaitForSeconds(0.1f);  // Đợi để vị trí cập nhật

        socketForPinL = FindClosestSocket(pinL_Attach.position, socketForPinR);
        if (socketForPinL != null)
        {
            Debug.Log("Chân trái (Pin_L) cắm vào socket: " + socketForPinL.gameObject.name);
        }
        else
        {
            Debug.LogError("Không tìm thấy socket cho chân trái (Pin_L)!");
        }
    }

    private IEnumerator DelayedAdjustRotation()
    {
        yield return new WaitForEndOfFrame(); // Đợi tới cuối frame, tránh bị override transform

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            bool wasKinematic = rb.isKinematic;
            rb.isKinematic = true; // Ngăn không cho Unity override vị trí

            AdjustRotation();

            yield return new WaitForSeconds(0.1f); // Chờ sau khi xoay
            rb.isKinematic = wasKinematic; // Khôi phục trạng thái kinematic ban đầu
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Rigidbody, vẫn tiếp tục xoay.");
            AdjustRotation();
        }
    }

    private void AdjustRotation()
    {
        if (socketForPinR == null || socketForPinL == null) return;

        // 🔌 Vector giữa hai socket
        Vector3 vectorSocket = (socketForPinL.position - socketForPinR.position).normalized;

        // 📌 Xoay con trở theo hướng của socket
        transform.rotation = Quaternion.LookRotation(vectorSocket, Vector3.up);
        
        Debug.Log("⚙️ Đã xoay con trở về hướng socket!");
    }

    private Transform FindClosestSocket(Vector3 position, Transform excludedSocket = null)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor[] sockets = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        Transform closestSocket = null;
        float minDistance = Mathf.Infinity;

        Debug.Log("Tìm socket gần nhất cho vị trí: " + position);

        foreach (var socket in sockets)
        {
            if (excludedSocket != null && socket.transform == excludedSocket)
                continue;

            float distance = Vector3.Distance(position, socket.transform.position);
            Debug.Log("Khoảng cách tới " + socket.gameObject.name + ": " + distance);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestSocket = socket.transform; 
            }
        }

        if (closestSocket != null)
            Debug.Log("Socket gần nhất: " + closestSocket.gameObject.name);
        else
            Debug.LogError("Không tìm thấy socket nào!");

        return closestSocket;
    }
}
