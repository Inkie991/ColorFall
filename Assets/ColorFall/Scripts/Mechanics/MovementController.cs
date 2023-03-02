using ColorFall.Core;
using ColorFall.Game;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ColorFall.Mechanics
{
    public class MovementController : MonoBehaviour
    {
        private bool _isLocked;
        private bool _isGameStarted;
        private Sequence _sequence;
        private const int MobileLimit = 30;
        private const int DesktopLimit = 5;
        private const int MobileSensitivity = 130;
        private const int DesktopSensitivity = 1000;
        private float FixValue => Time.deltaTime * -1;

        struct InputPoint
        {
            public static float PosX = 0;
            public static float Sign = 0;
            public static float Speed = 0;
            public static float Distance = 0;

            public static void Reset()
            {
                PosX = 0;
                Sign = 0;
                Speed = 0;
                Distance = 0;
            }
        }

        private void Awake()
        {
            EventManager.AddListener<PlayerWinEvent>(OnPlayerWin);
            EventManager.AddListener<PlayerLoseEvent>(OnPlayerLose);
            EventManager.AddListener<SecondChanceEvent>(OnSecondChance);
            _isLocked = false;
            _sequence = DOTween.Sequence();
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<PlayerWinEvent>(OnPlayerWin);
            EventManager.RemoveListener<PlayerLoseEvent>(OnPlayerLose);
            EventManager.RemoveListener<SecondChanceEvent>(OnSecondChance);
        }

        void Update()
        {
            if (_isLocked) return;

            HandleInput();
            
            if (
                EventSystem.current.IsPointerOverGameObject() ||
                (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            ) return;
            
            HandleRotation();
            CheckStop();

            if (_isGameStarted || InputPoint.Sign == 0) return;
            
            _isGameStarted = true;
            EventManager.Broadcast(Events.GameStartedEvent);
        }

        void HandleInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        InputPoint.PosX = touch.position.x;
                        break;
                    case TouchPhase.Moved or TouchPhase.Stationary or TouchPhase.Ended:
                        InputPoint.Distance = touch.position.x - InputPoint.PosX;
                        InputPoint.PosX = touch.position.x;
                        InputPoint.Sign = Sign(InputPoint.Distance);
                        break;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                InputPoint.Sign = Sign(Input.GetAxis("Mouse X"));
                InputPoint.PosX = Input.GetAxis("Mouse X");
            }
        }

        void HandleRotation()
        {
            if (!Application.isMobilePlatform || Mathf.Abs(InputPoint.Distance) > 2f)
            {
                var sensitivity = Application.isMobilePlatform ? MobileSensitivity : DesktopSensitivity;
                var limit = Application.isMobilePlatform ? MobileLimit : DesktopLimit;
                var delta = Application.isMobilePlatform ? (InputPoint.Distance / Screen.width * 100) : InputPoint.PosX;
                var rotationValue = delta * sensitivity * FixValue;

                if (rotationValue == 0) return;
                if (Mathf.Abs(rotationValue) > limit)
                    rotationValue = limit * Sign(rotationValue);

                InputPoint.Speed = rotationValue;

                if (_sequence.active)
                    _sequence.Kill();

                transform.Rotate(Vector3.up * rotationValue);
            }
        }

        void CheckStop()
        {
            if (InputPoint.Sign == 0) return;
            if (!Application.isMobilePlatform && !Input.GetMouseButtonUp(0)) return;
            if (Application.isMobilePlatform && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;
                if (touch.phase != TouchPhase.Ended) return;
            }

            var posY = transform.rotation.eulerAngles.y;
            var endValue = posY + InputPoint.Speed;
            _sequence = DOTween.Sequence();
            _sequence.Append(DOTween.To(() => posY, value =>
            {
                posY = value;
                transform.rotation = Quaternion.Euler(Utils.SetCoordinate(transform.rotation.eulerAngles, posY));
            }, endValue, 0.3f));
            _sequence.Play();
            InputPoint.Reset();
        }

        void OnPlayerWin(PlayerWinEvent e) => LockMovement();
        void OnPlayerLose(PlayerLoseEvent e) => LockMovement();
        void OnSecondChance(SecondChanceEvent evt) => this.Invoke(UnlockMovement, 0.2f);
        void LockMovement()
        {
            _isLocked = true;
            InputPoint.Reset(); // static class will store values for the next context
        }
        
        void UnlockMovement()
        {
            _isLocked = false;
        }

        private int Sign(float value)
        {
            if (value == 0) return 0;
            return (int) Mathf.Sign(value);
        }
    }
}
