import cv2
from djitellopy import Tello
import time
import key_controls

def main():
    tello = Tello()

    print("Connecting to Tello...")
    tello.connect()

    battery = tello.get_battery()
    print(f"Battery Level: {battery}%")

    tello.streamon()

    frame_read = tello.get_frame_read()

    tello.takeoff()
    time.sleep(2)

    try:
        while True:
            frame = frame_read.frame

            frame = cv2.resize(frame, (960, 720))

            height = tello.get_height()
            cv2.putText(frame, f"Height: {height} cm", (10, 30),
                        cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
            
            cv2.imshow("Tello Camera", frame)

            key = cv2.waitKey(1) & 0xFF

            if not key_controls.handle_key(tello, key):
                print("プログラムを終了します...")
                break

    except KeyboardInterrupt:
        print("ユーザーによって中断されました")

    finally:
        tello.land()
        tello.streamoff()
        cv2.destroyAllWindows()

if __name__ == "__main__":
    main()

