using UnityEngine;

namespace Teatro
{
    class NoiseGenerator
    {
        XXHash _hash1;
        XXHash _hash2;
        XXHash _hash3;
        float _time;
        float _frequency;

        int _fractal = 2;

        public NoiseGenerator(float frequency)
        {
            _hash1 = new XXHash(Random.Range(0, 0x7fffffff));
            _hash2 = new XXHash(Random.Range(0, 0x7fffffff));
            _hash3 = new XXHash(Random.Range(0, 0x7fffffff));
            _frequency = frequency;
        }

        public NoiseGenerator(int seed, float frequency)
        {
            _hash1 = new XXHash(seed);
            _hash2 = new XXHash(seed ^ 0x1327495a);
            _hash3 = new XXHash(seed ^ 0x3cbe84f2);
            _frequency = frequency;
        }

        public void Step()
        {
            _time += _frequency * Time.deltaTime;
        }

        public float Value(int seed2)
        {
            var i1 = _hash1.Range(-100.0f, 100.0f, seed2);
            return Perlin.Fbm(_time + i1, _fractal);
        }

        public Vector3 Vector(int seed2)
        {
            var i1 = _hash1.Range(-100.0f, 100.0f, seed2);
            var i2 = _hash2.Range(-100.0f, 100.0f, seed2);
            var i3 = _hash3.Range(-100.0f, 100.0f, seed2);
            return new Vector3(
                Perlin.Fbm(_time + i1, _fractal),
                Perlin.Fbm(_time + i2, _fractal),
                Perlin.Fbm(_time + i3, _fractal));
        }

        public Quaternion Rotation(int seed2, float angle)
        {
            var i1 = _hash1.Range(-100.0f, 100.0f, seed2);
            var i2 = _hash2.Range(-100.0f, 100.0f, seed2);
            var i3 = _hash3.Range(-100.0f, 100.0f, seed2);
            return Quaternion.Euler(
                Perlin.Fbm(_time + i1, _fractal) * angle,
                Perlin.Fbm(_time + i2, _fractal) * angle,
                Perlin.Fbm(_time + i3, _fractal) * angle);
        }
    }
}
