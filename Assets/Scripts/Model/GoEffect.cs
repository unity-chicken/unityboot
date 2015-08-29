using UnityEngine;
using System.Collections;

public class GoEffect : GoItem {
    public float lifeTime = 3;
    public bool emit { get; private set; }

    public virtual void Play() {
        this.StartParticle();
        StartCoroutine(DieAfter(lifeTime));
    }

    protected IEnumerator DieAfter(float wait) {
        yield return new WaitForSeconds(wait);
        this.StopParticle();
        yield return new WaitForSeconds(2f);

        this.ClearParticle();
        yield return new WaitForEndOfFrame();
        Service.goPooler.Return(this);
    }

    public override void OnGettingOutPool() { 
    }

    public override void OnGoingIntoPool() { 
        this.ClearParticle();
        StopAllCoroutines();
    }

    public void StartParticle() {
        emit = true;
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem ps in particleSystems) {
            ps.time = 0;
            ps.Play();
        }
    }

    public void StopParticle() {
        emit = false;
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem ps in particleSystems) {
            ps.Stop();
        }
    }

    public void ClearParticle() {
        emit = false;
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem ps in particleSystems) {
            ps.Clear();
            ps.time = 0;
        }
    }
}
