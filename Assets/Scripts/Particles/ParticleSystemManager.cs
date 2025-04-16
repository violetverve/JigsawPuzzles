using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using PuzzlePiece;
using Grid;

namespace Particles
{
    public class ParticleSystemManager : MonoBehaviour
    {
        [SerializeField] private PoolableParticleSystem _sparklePrefab;
        private ObjectPool<ParticleSystem> _sparklePool;

        private void OnEnable()
        {
            GridInteractionController.PiecesCollected += HandlePiecesCollected;
        }

        private void OnDisable()
        {
            GridInteractionController.PiecesCollected -= HandlePiecesCollected;
        }
        
        private void Start()
        {
            _sparklePool = new ParticleSystemPool(_sparklePrefab, transform).Pool;
        }

        private void HandlePiecesCollected(List<Piece> corePieces, List<Piece> wholeGroup)
        {
            PlaySparkleEffectList(corePieces);
        }

        private void PlaySparkleEffectList(List<Piece> pieces)
        {
            foreach (var piece in pieces)
            {
                PlaySparkleEffect(piece.transform);
            }
        }

        public void PlaySparkleEffect(Transform pieceTransform)
        {
            ParticleSystem particleSystem = _sparklePool.Get();
            particleSystem.transform.SetParent(pieceTransform);
            particleSystem.transform.position = pieceTransform.position;
            particleSystem.transform.localScale = Vector3.one * pieceTransform.localScale.x;
            particleSystem.Play();
        }

    }
}