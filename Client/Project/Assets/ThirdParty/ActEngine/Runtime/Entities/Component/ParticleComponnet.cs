using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{

    public class ParticleComponnet : Act.Component
    {
        List<ActControlParticlePlayable> particleList;
        GameObject gameObject;

        uint particleRandomSeed;
        const int k_MaxRandInt = 10000;

        protected override void Start()
        {
            gameObject = (Owner as GameObjectEntity).GameObject;
            particleList = new List<ActControlParticlePlayable>();

            List<ParticleSystem> particles = new List<ParticleSystem>(GetControllableParticleSystems(gameObject));
            foreach (var item in particles)
            {
                var control = new ActControlParticlePlayable();
                control.Initialize(item);
                particleList.Add(control);
            }
        }

        public void OpenDebug()
        {
            if (particleRandomSeed == 0)
                particleRandomSeed = (uint)UnityEngine.Random.Range(1, k_MaxRandInt);

            foreach (var item in particleList)
            {
                var control = new ActControlParticlePlayable();
                control.OpenDebug(particleRandomSeed);
            }
        }

        public void Play()
        {
            gameObject.SetActive(true);
            for (int i = 0; i < particleList.Count; i++)
            {
                ActControlParticlePlayable particle = particleList[i];
                particle.Play();
            }
        }

        public void Simulate(float time)
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                ActControlParticlePlayable particle = particleList[i];
                particle.Update(time);
            }
        }

        public void SetSpeed(float speed = 1)
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                ActControlParticlePlayable particle = particleList[i];
                var main = particle.particleSystem.main;
                main.simulationSpeed = speed;
            }
        }
        public void SetLoop(bool loop)
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                ActControlParticlePlayable particle = particleList[i];
                ParticleSystem.MainModule mainModule = particle.particleSystem.main;
                mainModule.loop = loop;
            }
        }

        public void Pause()
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                var particle = particleList[i];
                particle.Pause();
            }
        }

        public void Resume()
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                var particle = particleList[i];
                particle.Play();
            }
        }

        public void Stop()
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                var particle = particleList[i];
                particle.Stop();
            }
        }

        static readonly HashSet<ParticleSystem> s_SubEmitterCollector = new HashSet<ParticleSystem>();
        public static IList<ParticleSystem> GetControllableParticleSystems(GameObject go)
        {
            var roots = new List<ParticleSystem>();

            // searchHierarchy will look for particle systems on child objects.
            // once a particle system is found, all child particle systems are controlled with playables
            // unless they are subemitters

            //if (go.GetComponent<ParticleSystem>() != null)
            //{
            GetControllableParticleSystems(go.transform, roots, s_SubEmitterCollector);
            s_SubEmitterCollector.Clear();
            //}

            return roots;
        }


        public static void GetControllableParticleSystems(Transform t, ICollection<ParticleSystem> roots, HashSet<ParticleSystem> subEmitters)
        {
            var ps = t.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                if (!subEmitters.Contains(ps))
                {
                    roots.Add(ps);
                    CacheSubEmitters(ps, subEmitters);
                }
            }

            for (int i = 0; i < t.childCount; ++i)
            {
                GetControllableParticleSystems(t.GetChild(i), roots, subEmitters);
            }
        }

        static void CacheSubEmitters(ParticleSystem ps, HashSet<ParticleSystem> subEmitters)
        {
            if (ps == null)
                return;

            for (int i = 0; i < ps.subEmitters.subEmittersCount; i++)
            {
                subEmitters.Add(ps.subEmitters.GetSubEmitterSystem(i));
                // don't call this recursively. subEmitters are only simulated one level deep.
            }
        }

    }


    public class ActControlParticlePlayable
    {
        uint m_RandomSeed = 1;
        public ParticleSystem particleSystem { get; set; }

        const float kUnsetTime = float.MaxValue;
        float m_LastPlayableTime = kUnsetTime;
        float m_LastParticleTime = kUnsetTime;


        public void Initialize(ParticleSystem ps)
        {
            particleSystem = ps;
        }

        public void OpenDebug(uint randomSeed)
        {
            m_RandomSeed = System.Math.Max(1, randomSeed);
            SetRandomSeed(particleSystem, m_RandomSeed);
        }

        public void Play()
        {
            particleSystem.Simulate(0F);
            particleSystem.Clear();
            particleSystem.Play();
        }

        public void Update(float time)
        {
            var particleTime = particleSystem.time;

            // if particle system time has changed externally, a re-sync is needed
            if (m_LastPlayableTime > time || !Mathf.Approximately(particleTime, m_LastParticleTime))
                Simulate(particleSystem, time, true);
            else if (m_LastPlayableTime < time)
                Simulate(particleSystem, time - m_LastPlayableTime, false);

            m_LastPlayableTime = time;
            m_LastParticleTime = particleSystem.time;
        }

        public void Stop()
        {
            particleSystem.Stop();
        }

        public void Pause()
        {
            particleSystem.Pause();
        }

        private void Simulate(ParticleSystem particleSystem, float time, bool restart)
        {
            const bool withChildren = false;
            const bool fixedTimeStep = false;
            float maxTime = Time.maximumDeltaTime;

            if (restart)
                particleSystem.Simulate(0, withChildren, true, fixedTimeStep);

            // simulating by too large a time-step causes sub-emitters not to work, and loops not to
            // simulate correctly
            while (time > maxTime)
            {
                particleSystem.Simulate(maxTime, withChildren, false, fixedTimeStep);
                time -= maxTime;
            }

            if (time > 0)
                particleSystem.Simulate(time, withChildren, false, fixedTimeStep);
        }



        static void SetRandomSeed(ParticleSystem particleSystem, uint randomSeed)
        {
            if (particleSystem == null)
                return;

            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (particleSystem.useAutoRandomSeed)
            {
                particleSystem.useAutoRandomSeed = false;
                particleSystem.randomSeed = randomSeed;
            }

            for (int i = 0; i < particleSystem.subEmitters.subEmittersCount; i++)
            {
                SetRandomSeed(particleSystem.subEmitters.GetSubEmitterSystem(i), ++randomSeed);
            }
        }

    }
}

