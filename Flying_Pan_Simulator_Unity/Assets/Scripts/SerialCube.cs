using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SerialCube : MonoBehaviour
{
    public SerialHandler serialHandler;

    public Text text;
    public GameObject cube;

    public GameObject eggPrefab;
    public Transform spawnPoint;
    private int lastButtonState = 0;

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

    // シリアルデータを受信したときの処理
    void OnDataReceived(string message)
    {
        //Debug.Log("届いたデータ: " + message);
        try
        {
            string[] data = message.Split(',');

            // データが足りない場合は無視する
            if (data.Length < 5)
            {
                Debug.LogWarning("データが足りません: " + message);
                return;
            }

            // 前後の傾き（X軸）
            float pitch = float.Parse(data[0]);
            // 左右の傾き（Z軸）
            float roll = float.Parse(data[1]);
            // ボタン
            int currentButtonState = int.Parse(data[4]);

            targetRotation = Quaternion.Euler(pitch, 0, roll);

            if (currentButtonState == 1 && lastButtonState == 0)
            {
                DropEgg();
            }

            lastButtonState = currentButtonState;

            if (text != null)
            {
                text.text = $"Pitch(X): {pitch:F1}\nRoll(Y): {roll:F1}";
            }
            
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Data Parse Error: " + e.Message);
        }
    }

    void DropEgg()
    {
        if (eggPrefab != null && spawnPoint != null)
        {
            Instantiate(eggPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
