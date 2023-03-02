using System;
using System.Collections;
using ColorFall.Core;
using ColorFall.Game;
using DG.Tweening;
using UnityEngine;

namespace ColorFall.Mechanics
{
    [SelectionBase]
    public class Player : ColorableObject
    {
        private const int BaseHealth = 5;
        private const float BaseScale = 0.5f;
        private const float MaxScale = 2f;
        private const float SpeedReward = 0.1f;
        private const float SpeedPenalty = 1f;
        private const float ScaleReward = 0.01f;
        private const float ScalePenalty = 0.1f;
        private const float AdditionalChargedSpeed = 5f;

        private readonly Quaternion SpeedVFX_VerticalRotation = Quaternion.Euler(new Vector3(-90,0,0));
        private readonly Quaternion SpeedVFX_HorizontalRotation = Quaternion.Euler(new Vector3(0,90,0));
        private readonly Vector3 SpeedVFX_VerticalPosition = new(0, -30, 0);
        private readonly Vector3 SpeedVFX_HorizontalPosition = new(20,0,0);

        [SerializeField] private float jumpForce = 9f;
        [SerializeField] private ParticleSystem colorChangeVfx;
        [SerializeField] private ParticleSystem chargedModeVfx;
        [SerializeField] private ParticleSystem speedVFX;
        [SerializeField] private ParticleSystem confettiVFX;
        [SerializeField] private ParticleSystem collectVFX;
        private TrailRenderer _trailRenderer;
        private Rigidbody _rigidbody;
        private MeshRenderer _meshRenderer;
        private BoxCollider _collider;
        private bool _isBoost;
        private Coroutine _rgbAnimationCoroutine;
        private float _speedToScale;
        private int _currentStackRowsCount;
        private int _correctDropsDestroyedInCurrentStack;

        private float _bonusSpeed;
        private float BonusSpeed
        {
            get => _bonusSpeed;
            set
            {
                _bonusSpeed = value;
                if (_bonusSpeed < 0) _bonusSpeed = 0;
            }
        }
        
        private float _bonusScale;
        private float BonusScale
        {
            get => _bonusScale;
            set
            {
                _bonusScale = value;
                if (_bonusScale < 0) _bonusScale = 0;
                if (CurrentScale > MaxScale) _bonusScale = MaxScale - BaseScale;
            }
        }

        private float CurrentScale => BaseScale + BonusScale;

        private float BaseSpeed => Math.Abs(Physics.gravity.y);
        public float CurrentSpeed
        {
            get => Math.Abs(_rigidbody.velocity.y);
            private set => _rigidbody.velocity = Vector3.up * value;
        }
        private float CurrentBoostSpeed
        {
            get => Math.Abs(_rigidbody.velocity.x);
            set => _rigidbody.velocity = Vector3.right * value;
        }

        private float MaxSpeed => BaseSpeed + _bonusSpeed;

        private new void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<BoxCollider>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _trailRenderer = GetComponent<TrailRenderer>();
            EventManager.AddListener<SecondChanceEvent>(OnSecondChance);
            EventManager.AddListener<ChargedModeOnEvent>(ChargedModeOn);
            EventManager.AddListener<ChargedModeOffEvent>(ChargedModeOff);
            EventManager.AddListener<BoostEvent>(OnBoost);
            EventManager.AddListener<PlayerWinEvent>(OnWin);
            base.Awake();
        }
        
        private new void OnDestroy()
        {
            EventManager.RemoveListener<ChargedModeOnEvent>(ChargedModeOn);
            EventManager.RemoveListener<ChargedModeOffEvent>(ChargedModeOff);
            EventManager.RemoveListener<SecondChanceEvent>(OnSecondChance);
            EventManager.RemoveListener<PlayerWinEvent>(OnWin);
            EventManager.RemoveListener<BoostEvent>(OnBoost);
            base.OnDestroy();
        }

        private void Start()
        {
            chargedModeVfx.Stop();
            speedVFX.transform.rotation = SpeedVFX_VerticalRotation;
            speedVFX.transform.localPosition = SpeedVFX_VerticalPosition;
            _currentStackRowsCount = 0;
            _correctDropsDestroyedInCurrentStack = 0;
            GetComponent<BoxCollider>().size = new Vector3(0.4f, 0.85f, 0.4f);
        }

        private void Update()
        {
            if (Managers.Gameplay.GameOver) return;
            
            CheckVelocity();
            CalcScale();
            if (Managers.Energy.IsCharged) MagnetCollectables();
            if (!Managers.Gameplay.GameOver) Managers.Gameplay.CalculateLevelProgress(transform.position.y);
            if (CurrentSpeed >= 18 && !speedVFX.isPlaying) speedVFX.Play();
            if (CurrentSpeed < 18 && speedVFX.isPlaying) speedVFX.Clear();
        }

        private void MagnetCollectables()
        {
            Vector3 boxSize = new Vector3(5, 1.5f, 7);
            var collectables = Physics.BoxCastAll(
                transform.position,
                boxSize,
                Vector3.down,
                Quaternion.identity,
                0.01f,
                LayerMask.GetMask("Collectable"));
            foreach (var item in collectables)
            {
                var collectable = item.collider.gameObject.GetComponent<ICollectable>();
                if (!collectable.IsAnimating) collectable.Collect(transform);
            }
        }

        private void CheckVelocity()
        {
            if (_isBoost) return;
            if (_rigidbody.velocity.magnitude >= MaxSpeed)
                _rigidbody.velocity = _rigidbody.velocity.normalized * MaxSpeed;
        }

        private void CalcScale()
        {
            transform.localScale = Vector3.one * (CurrentScale);
            _trailRenderer.startWidth = 0.4f * CurrentScale;
            _trailRenderer.endWidth = 0f;
        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            if (Managers.Gameplay.GameOver) return;

            switch (otherCollider.tag)
            {
                case "BouncePlatform":
                    DrawSmudge(otherCollider);
                    Bounce(otherCollider.transform.position.y, true);
                    break;
                case "Unbreakable":
                    Death();
                    break;
                case "Money":
                    MoneyCollect(otherCollider);
                    break;
                case "Drop":
                    DropCollect(otherCollider);
                    break;
                case "PassCollider":
                    OnPassCollider(otherCollider);
                    break;
                case "ColorChanger":
                    ChangeColor(otherCollider);
                    break;
                case "FinishPlatform":
                    OnFinish();
                    break;
                case "FinishCannon":
                    OnCannon(otherCollider);
                    break;
                case "MultiplyPlatform":
                    OnMultiplier(otherCollider);
                    break;
            }
        }

        private void Bounce(float posY, bool withEvent = false)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (!withEvent) return;
            BouncedOffPlatformEvent evt = new(){ posY = posY };
            EventManager.Broadcast(evt);
        }

        private void OnFinish()
        {
            PlayerFinishEvent evt = new() { endSpeed = CurrentSpeed };
            EventManager.Broadcast(evt);
            transform.DOMoveZ(-3f, 1f);
        }

        private void OnCannon(Collider otherCollider)
        {
            EventManager.Broadcast(Events.CannonEvent);
            if (Managers.Energy.IsCharged) CurrentSpeed -= AdditionalChargedSpeed;
            GetComponent<BoxCollider>().size = new Vector3(0.01f, 0.8f, 0.01f);
            CurrentSpeed = 0;
            var toCenter = transform.position + Vector3.down * (otherCollider.bounds.size.y / 1.5f);
            transform.DOMove(toCenter, 1f);
            speedVFX.transform.localPosition = SpeedVFX_HorizontalPosition;
            speedVFX.transform.rotation = SpeedVFX_HorizontalRotation;
        }

        private void OnBoost(BoostEvent evt)
        {
            // Block preventing max speed
            _isBoost = true;
           // Unfreeze positionX 
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
            var forceVector = (Vector3.right * (evt.power * 1.5f)) + (Vector3.up * 10);
            _rigidbody.AddForce(forceVector, ForceMode.Impulse);
        }
        
        private void Death()
        {
            EventManager.Broadcast(Events.PlayerLoseEvent);
            gameObject.SetActive(false);
        }

        private void OnPassCollider(Collider otherCollider)
        {
            PassCollider passCollider = otherCollider.GetComponent<PassCollider>();
            if (Camera.main != null) Camera.main.GetComponent<CameraMotion>().Unlock();

            if (passCollider.transform.parent.CompareTag("FirstPlatform"))
            {
                GetComponent<BoxCollider>().size = new Vector3(0.85f, 0.85f, 0.85f);
            }

            if (passCollider.isStartOfStack)
            {
                _currentStackRowsCount = otherCollider.GetComponentInParent<DropsStack>().GetRowsCount();
            }

            if (passCollider.isEndOfStack)
            {
                int stackPercent =
                    Mathf.RoundToInt((_correctDropsDestroyedInCurrentStack * 100) / _currentStackRowsCount);
                _correctDropsDestroyedInCurrentStack = 0;
                _currentStackRowsCount = 0;
                ComboEvent evt = new() { messageModifier = stackPercent };
                EventManager.Broadcast(evt);
                // Remove passed stack to optimize the game. Use delay, so player won't see removal
                this.Invoke(() =>
                {
                    var stack = passCollider.GetComponentInParent<DropsStack>();
                    Destroy(stack.gameObject);
                }, 0.5f);
            }
        }

        private void MoneyCollect(Collider otherCollider)
        {
            otherCollider.GetComponent<Money>().Collect(transform);
            CollectMoneyEvent evt = new CollectMoneyEvent
            {
                screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position)
            };
            EventManager.Broadcast(evt);
        }

        private void DropCollect(Collider otherCollider)
        {
            Drop drop = otherCollider.GetComponent<Drop>();
                    
            if (!Managers.Energy.IsCharged && drop.GamingColor != GamingColor && !drop.IsAnimatedWhileCharge)
                BadColor(drop);
            else
                GoodColor(drop);
        }

        private void OnMultiplier(Collider otherCollider)
        {
            Managers.Audio.PlaySound(Sound.Bounce);
            DrawSmudge(otherCollider, true);
            colorChangeVfx.Play();

            if (_speedToScale == 0f)
                _speedToScale = CurrentBoostSpeed / CurrentScale;

            if (CurrentScale > BaseScale)
            {
                _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                CurrentBoostSpeed -= _speedToScale * 0.3f;
                BonusScale -= 0.3f;
                return;
            }

            _rigidbody.isKinematic = true;
            _collider.enabled = false;
            _meshRenderer.enabled = false;
            var multiplier = otherCollider.GetComponent<MultiplyPlatform>();
            multiplier.StartAnimation();
            this.Invoke(() =>
            {
                PlayerWinEvent evt = new() { multiplier = multiplier.multiplier };
                EventManager.Broadcast(evt);
                gameObject.SetActive(false);
            }, 2f);
        }

        private void ChangeColor(Collider otherCollider)
        {
            ColorChanger colorChanger = otherCollider.GetComponent<ColorChanger>();
            GamingColor = colorChanger.GamingColor;
            colorChangeVfx.Play();
            Managers.Audio.PlaySound(Sound.ChangeColor);
            // Remove passed color changer to optimize the game. Use delay, so player won't see removal
            this.Invoke(() => Destroy(colorChanger.gameObject), 0.5f);
        }

        private void OnSecondChance(SecondChanceEvent evt)
        {
            gameObject.SetActive(true);
            transform.position = Utils.SetCoordinate(transform.position, transform.position.y + 5f);
            Bounce(transform.position.y);
        }

        private void OnWin(PlayerWinEvent evt)
        {
            _meshRenderer.enabled = false;
            confettiVFX.Play();
        }

        private void GoodColor(Drop drop)
        {
            CollectDropEvent evt = new()
            {
                collectedDrop = CollectedDrop.Correct,
                screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position)
            };
            EventManager.Broadcast(evt);
            drop.Collect(transform);
            _correctDropsDestroyedInCurrentStack++;
            BonusSpeed += SpeedReward;
            BonusScale += ScaleReward;
            collectVFX.Play();
        }
        
        private void BadColor(Drop drop)
        {
            CollectDropEvent evt = new()
            {
                collectedDrop = CollectedDrop.Incorrect,
                screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position)
            };
            EventManager.Broadcast(evt);
            drop.Collect(transform);
            BonusSpeed -= SpeedPenalty;
            BonusScale -= ScalePenalty;
            float factor = BaseHealth + (Managers.Gameplay.CollectedDropsCount / 5f) - Managers.Gameplay.IncorrectDropsCount;
            if (factor < 0) Death();
        }

        private void ChargedModeOn(ChargedModeOnEvent evt)
        {
            chargedModeVfx.Play();
            BonusSpeed += AdditionalChargedSpeed;
            _rgbAnimationCoroutine = StartCoroutine(RgbAnimation());
        }

        private void ChargedModeOff(ChargedModeOffEvent evt)
        {
            chargedModeVfx.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            BonusSpeed -= AdditionalChargedSpeed;
            if (_rgbAnimationCoroutine != null)
            {
                StopCoroutine(_rgbAnimationCoroutine);
                _rgbAnimationCoroutine = null;
            }
            ChangeMaterial();
        }

        protected override void ChangeMaterial()
        {
            if (_rgbAnimationCoroutine != null) return;
            
            GetComponent<Renderer>().material = ColorManager.GetMaterial(gamingColor);
            _trailRenderer.materials = new []{ColorManager.GetMaterial(gamingColor, MaterialType.Sprite)};
            ParticleSystem.MainModule colorChangeEffectSettings = colorChangeVfx.main;
            ParticleSystem.MainModule chargedEffectSettings = chargedModeVfx.main;
            ParticleSystem.MainModule collectEffectSettings = collectVFX.main;
            colorChangeEffectSettings.startColor = ColorManager.GetColor(gamingColor);
            chargedEffectSettings.startColor = ColorManager.GetColor(gamingColor);
            collectEffectSettings.startColor = ColorManager.GetColor(gamingColor);
        }
        
        private void DrawSmudge(Collider other, bool isBoost = false)
        {
            float offset = other.bounds.size.y / 2;
            float yPos = other.transform.position.y + offset;
            Vector3 smudgePosition = transform.position;
            float offsetY = 0.001f;
            if (other.CompareTag("MultiplyPlatform")) offsetY = 0.51f;
            smudgePosition.y = yPos + offsetY;
            var smudgeObj = Instantiate(GameObjectsLoader.GetPrefab<Smudge>(), smudgePosition, Quaternion.Euler(90f, 0, 0));
            smudgeObj.transform.parent = other.transform;
            var smudge = smudgeObj.GetComponent<Smudge>();
            smudge.GamingColor = GamingColor;
            smudge.Size = isBoost ? 5f : transform.localScale.x * 2;
        }

        private IEnumerator RgbAnimation()
        {
            GetComponent<Renderer>().material.color = ColorManager.StandardRed;
            _trailRenderer.materials = new []{ColorManager.GetMaterial(GamingColor.Red, MaterialType.Sprite)};
            ParticleSystem.MainModule chargedEffectSettings = chargedModeVfx.main;
            ParticleSystem.MainModule collectEffectSettings = collectVFX.main;
            chargedEffectSettings.startColor = ColorManager.GetColor(GamingColor.Red);
            collectEffectSettings.startColor = ColorManager.GetColor(GamingColor.Red);

            while (true)
            {
                var newColor = ColorManager.GetNextChargedModeColor(GetComponent<Renderer>().material.color);
                GetComponent<Renderer>().material.color = newColor;
                _trailRenderer.materials[0].color = newColor;
                chargedEffectSettings.startColor = newColor;
                collectEffectSettings.startColor = newColor;
                yield return new WaitForSeconds(0.001f);
            }
        }
    }
}
