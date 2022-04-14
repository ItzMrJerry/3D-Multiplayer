using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun;
public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    [SerializeField]
    private bool HasBulletSpread = true;
    [SerializeField]
    private float BulletSpreadVarience;
    //private Vector3 BulletSpreadVarience = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField]
    private TrailRenderer bulletTrail;
    [SerializeField]
    private ParticleSystem impactParticleSystem;
    [SerializeField]
    private float ShootDelay = 0.5f;
    private float LastShot;

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        if (LastShot + ShootDelay < Time.time)
        {

            
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = cam.transform.position;

            Vector3 dir = GetDirection();

            if (Physics.Raycast(cam.transform.position,dir, out RaycastHit hit,float.MaxValue))
            {
                //TrailRenderer trail = PhotonNetwork.Instantiate();
                TrailRenderer trail = Instantiate(bulletTrail, itemGameobject.transform.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));
                LastShot = Time.time;
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)iteminfo).damage);
            }
        }
    }

    private Vector3 GetDirection()
    {
        if (!HasBulletSpread) return cam.transform.forward;

        Vector3 direction = cam.transform.forward;

        direction += new Vector3(
            Random.Range(-BulletSpreadVarience, BulletSpreadVarience), 
            Random.Range(-BulletSpreadVarience, BulletSpreadVarience), 
            Random.Range(-BulletSpreadVarience, BulletSpreadVarience));

        direction.Normalize();
        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail,RaycastHit hit)
    {
        float time = 0;
        Vector3 startpos = trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startpos, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = hit.point;
        //stantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }
}
