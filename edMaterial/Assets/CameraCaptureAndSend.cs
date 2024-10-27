using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

public class CameraCaptureAndSend : MonoBehaviour
{
    public Camera mainCamera;
    public string serverUrl = "ws://localhost:8765";
    private ClientWebSocket webSocket;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        ConnectWebSocket();
        StartCoroutine(CaptureAndSend());
    }

    async void ConnectWebSocket()
    {
        webSocket = new ClientWebSocket();
        try
        {
            await webSocket.ConnectAsync(new Uri(serverUrl), CancellationToken.None);
            Debug.Log("WebSocketに接続しました");
        }
        catch (Exception e)
        {
            Debug.LogError("WebSocketの接続に失敗しました: " + e);
        }
    }

    IEnumerator CaptureAndSend()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.033f); // 約30FPSでキャプチャ

            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            mainCamera.targetTexture = renderTexture;
            Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            mainCamera.Render();
            RenderTexture.active = renderTexture;
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();

            byte[] imageBytes = screenshot.EncodeToJPG();
            string base64Image = Convert.ToBase64String(imageBytes);

            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                // WebSocket送信を別の非同期メソッドに分けて呼び出し
                SendImageData(base64Image);
            }

            Destroy(screenshot);
            RenderTexture.active = null;
            mainCamera.targetTexture = null;
            Destroy(renderTexture);
        }
    }

    // 画像データをWebSocketで送信する非同期メソッド
    async void SendImageData(string base64Image)
    {
        var buffer = Encoding.UTF8.GetBytes(base64Image);
        try
        {
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            Debug.Log("画像データを送信しました");
        }
        catch (Exception e)
        {
            Debug.LogError("画像データの送信に失敗しました: " + e);
        }
    }

    private async void OnApplicationQuit()
    {
        if (webSocket != null)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "アプリケーションの終了", CancellationToken.None);
            webSocket.Dispose();
            Debug.Log("WebSocketを閉じました");
        }
    }
}
