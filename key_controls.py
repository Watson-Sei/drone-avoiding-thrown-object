import key_controls

def handle_key(tello, key):
    if key == ord('w'):
        key_controls.move_foward(tello)
    elif key == ord('s'):
        key_controls.move_back(tello)


def move_forward(tello, distance=30):
    try:
        tello.move_forward(distance)
    except Exception as e:
        print(f"前進中にエラーが発生しました: {e}")

def move_back(tello, distance=30):
    try:
        tello.move_back(distance)
    except Exception as e:
        print(f"後退中にエラーが発生しました: {e}")