import asyncio
import websockets
import cv2
import numpy as np
import base64

# WebSocketサーバーのアドレスとポート
HOST = "localhost"
PORT = 8765

async def video_stream_handler(websocket, path):
    print("クライアントが接続しました")
    try:
        async for message in websocket:
            # Base64デコードして画像データに変換
            img_data = base64.b64decode(message)
            np_arr = np.frombuffer(img_data, np.uint8)
            frame = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)

            if frame is not None:
                # OpenCVでリアルタイム処理（例: グレースケール変換）
                gray_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
                cv2.imshow("Video Stream", gray_frame)

                # 'q'キーで終了
                if cv2.waitKey(1) & 0xFF == ord("q"):
                    break
            else:
                print("フレームが無効です")
    except websockets.ConnectionClosedError as e:
        print(f"接続が切断されました: {e}")
    finally:
        print("クライアントが切断されました")
        cv2.destroyAllWindows()

async def start_server():
    async with websockets.serve(video_stream_handler, HOST, PORT):
        print(f"サーバーが ws://{HOST}:{PORT} で起動しました")
        await asyncio.Future()  # サーバーを終了しないために無限待機

try:
    asyncio.run(start_server())
except KeyboardInterrupt:
    print("サーバーを停止しました")
