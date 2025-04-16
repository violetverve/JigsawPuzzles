using UnityEngine;
using UnityEngine.Pool;

namespace Particles
{
    public class PoolableParticleSystem : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;
        private ObjectPool<ParticleSystem> _pool;

        public ParticleSystem ParticleSystem => _particleSystem;

        private void OnParticleSystemStopped()
        {
            _pool.Release(_particleSystem);
        }

        public void SetPool(ObjectPool<ParticleSystem> pool)
        {
            _pool = pool;
        }
    }
}