using System.Collections.Generic;
using UnityEngine;
using PuzzlePiece;
using UnityEngine.Pool;

namespace Particles
{
    public class ParticleSystemPool
    {
        private PoolableParticleSystem _poolableParticleSystem;
        private Transform _parentTransform;
        private ObjectPool<ParticleSystem> _pool;

        public ObjectPool<ParticleSystem> Pool => _pool;

        public ParticleSystemPool(PoolableParticleSystem poolableParticleSystem, Transform parentTransform)
        {
            _poolableParticleSystem = poolableParticleSystem;
            _parentTransform = parentTransform;

            _pool = new ObjectPool<ParticleSystem>(
                CreatePooledItem,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject
            );
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
            ParticleSystem particleSystem = _pool.Get();
            particleSystem.transform.position = pieceTransform.position;
            particleSystem.transform.localScale = Vector3.one * pieceTransform.localScale.x;
            particleSystem.Play();
        }

        private ParticleSystem CreatePooledItem()
        {
            var poolable = Object.Instantiate(_poolableParticleSystem, _parentTransform);

            poolable.SetPool(_pool);

            return poolable.ParticleSystem;
        }

        private void OnReturnedToPool(ParticleSystem particleSystem)
        {
            particleSystem.gameObject.SetActive(false);
        }

        private void OnTakeFromPool(ParticleSystem particleSystem)
        {
            particleSystem.gameObject.SetActive(true);
        }

        private void OnDestroyPoolObject(ParticleSystem particleSystem)
        {
            Object.Destroy(particleSystem.gameObject);
        }

        public void ReturnParticle(ParticleSystem particleSystem)
        {
            _pool.Release(particleSystem);
        }

    }
}