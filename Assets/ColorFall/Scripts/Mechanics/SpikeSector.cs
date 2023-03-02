using ColorFall.Core;
using ColorFall.Game;
using DG.Tweening;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public class SpikeSector : MonoBehaviour
    {
        private const float SpikesLowPosYOffset = -0.06f;
        private GameObject _spikes;
        private Vector3 _spikesStartPos;
        
        protected void Awake()
        {
            _spikes = transform.GetChild(0).gameObject;
            _spikesStartPos = _spikes.transform.position;
            _spikes.tag = "Unbreakable";
            EventManager.AddListener<ChargedModeOnEvent>(ChargedModeOn);
            EventManager.AddListener<ChargedModeOffEvent>(ChargedModeOff);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<ChargedModeOnEvent>(ChargedModeOn);
            EventManager.RemoveListener<ChargedModeOffEvent>(ChargedModeOff);
        }
        
        private void ChargedModeOn(ChargedModeOnEvent evt)
        {
            _spikes.transform.DOMoveY(transform.position.y + SpikesLowPosYOffset, 0.5f);
            _spikes.GetComponent<MeshCollider>().enabled = false;
            this.Invoke(() =>
            {
                _spikes.GetComponent<MeshRenderer>().enabled = false;
            }, 0.5f);
        }
    
        private void ChargedModeOff(ChargedModeOffEvent evt)
        {
            _spikes.transform.DOMoveY(_spikesStartPos.y, 0.5f);
            _spikes.GetComponent<MeshRenderer>().enabled = true;
            _spikes.GetComponent<MeshCollider>().enabled = true;
        }
    }
}
