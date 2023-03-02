using System.Collections;
using ColorFall.Core;
using ColorFall.Game;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColorFall.Mechanics
{
    public class CameraMotion : MonoBehaviour
    {
        private bool _motionLocked = true;
        private bool _gameStarted = false;
        private Coroutine _coroutine;
        private Vector3 _offsetVec = new(0, 4, -10);

        private GameObject _sphere;
        private Camera _camera;
        private readonly Vector3 _finishPosition = new(-8, 7, -3);
        private readonly Vector3 _finishRotation = new(30, 80, 0);

        private const int StandardFieldOfView = 90;
        private const int ChargedFieldOfView = 110;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventManager.AddListener<BouncedOffPlatformEvent>(LockMotion);
            EventManager.AddListener<PlayerFinishEvent>(OnFinish);
            EventManager.AddListener<PlayerWinEvent>(OnWin);
            EventManager.AddListener<ChargedModeOnEvent>(ChargedModeStart);
            EventManager.AddListener<ChargedModeOffEvent>(ChargedModeEnd);
            DontDestroyOnLoad(gameObject);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "_Preloader") return;

            _sphere = GameObject.FindWithTag("Player");
            _camera = GetComponent<Camera>();
            _camera.fieldOfView = StandardFieldOfView;
            _coroutine = null;
            _gameStarted = false;
            _motionLocked = true;
            _offsetVec = new Vector3(0, 4, -10);
            InitStartingPosition();
        }

        private void InitStartingPosition()
        {
            Vector3 firstPlatform = GameObject.FindGameObjectWithTag("FirstPlatform").transform.position;
            var platformHalfHeight = 0.25f; // magic number (collider height = 0.5f)
            var offsetY = firstPlatform.y + platformHalfHeight;
            transform.position = _offsetVec + (Vector3.up * offsetY);
            transform.rotation = Quaternion.Euler(new Vector3(30, 0, 0));
        }

        void Update()
        {
            if (_motionLocked) return;
            if (!_gameStarted)
            {
                // The same as if (_coroutine == null)
                _coroutine ??= StartCoroutine(SmoothStart());

                return;
            }

            transform.position = _sphere.transform.position + _offsetVec;
        }

        private void LockMotion(BouncedOffPlatformEvent evt)
        {
            _motionLocked = true;
            transform.position = Utils.SetCoordinate(transform.position, evt.posY + _offsetVec.y);
        }

        public void Unlock()
        {
            _motionLocked = false;
        }

        private void ChargedModeStart(ChargedModeOnEvent evt)
        {
            DOTween.To(() => _camera.fieldOfView, value =>
            {
                _camera.fieldOfView = value;
            }, ChargedFieldOfView, 0.5f);
        }

        private void ChargedModeEnd(ChargedModeOffEvent evt)
        {
            DOTween.To(() => _camera.fieldOfView, value =>
            {
                _camera.fieldOfView = value;
            }, StandardFieldOfView, 0.5f);
        }

        private void OnFinish(PlayerFinishEvent evt)
        {
            var duration = 2f;
            transform.DORotate(_finishRotation, duration);
            DOTween.To(() => _offsetVec, value =>
            {
                _offsetVec = value;
            }, _finishPosition, duration);
        }

        private void OnWin(PlayerWinEvent evt)
        {
            _motionLocked = true;
            DOTween.To(() => transform.position.y, value =>
            {
                transform.position = Utils.SetCoordinate(transform.position, value);
            }, transform.position.y + 2, 2f);
            DOTween.To(() => transform.rotation.eulerAngles.x, value =>
            {
                transform.rotation = Quaternion.Euler(Utils.SetCoordinate(transform.rotation.eulerAngles, value, "x"));
            }, 10, 2f);
        }

        private IEnumerator SmoothStart()
        {
            var offset = 2.5f;
            var delta = 0f; 
            while (delta < offset)
            {
                transform.position = Utils.SetCoordinate(transform.position, _sphere.transform.position.y + (_offsetVec.y - delta));
                delta += 2f * Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _offsetVec.y -= offset;
            _gameStarted = true;
        }
    }
}
