using UnityEngine;

namespace ColorFall.Mechanics
{
    public class MultiplyTarget : MonoBehaviour
    {
        [SerializeField] private GameObject smudgePrefab;
        public float multiplier;
        private Player _player;
        
        void Start()
        {
            gameObject.tag = "MultiplyTarget";
            multiplier = 2;
            _player = FindObjectOfType<Player>();
        }

        private void OnTriggerEnter(Collider other)
        {
            DrawSmudge();
        }
        
        private void DrawSmudge()
        {
            Vector3 smudgePosition = _player.transform.position;
            float offsetY = 0.015f;
            smudgePosition.y = -offsetY;
            var smudgeObj = Instantiate(smudgePrefab, smudgePosition, Quaternion.Euler(-90f, 0, 0));
            smudgeObj.transform.parent = transform;
            var smudge = smudgeObj.GetComponent<Smudge>();
            smudge.GamingColor = _player.GamingColor;
            smudge.CancelRemoval();
            smudgeObj.transform.localScale = new Vector3(3, 3, 3);
        }
    }
}
