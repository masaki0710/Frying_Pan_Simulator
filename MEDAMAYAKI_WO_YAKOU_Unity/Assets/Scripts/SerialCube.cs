using UnityEngine;
using UnityEngine.UI;

public class SerialCube : MonoBehaviour
{
    public SerialHandler serialHandler;
    public GameObject cube;

    public float smoothness = 0.1f;
    private Quaternion targetRotation;
    private Vector3 basePosition;
    private Rigidbody rb;

    void Start()
    {
        serialHandler.OnDataReceived += OnDataReceived;
        basePosition = cube.transform.position;
        targetRotation = transform.rotation;
        rb = cube.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Quaternion nextRotation = Quaternion.Slerp(rb.rotation, targetRotation, smoothness);
        rb.MoveRotation(nextRotation);
        rb.MovePosition(basePosition);
    }

    void OnDataReceived(string message)
    {
        try
        {
            string[] data = message.Split(',');
            if (data.Length < 5) return;

            float pitch = float.Parse(data[0]);
            float roll = float.Parse(data[1]);

            targetRotation = Quaternion.Euler(pitch, 0, roll);

        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Data Parse Error: " + e.Message);
        }
    }
}