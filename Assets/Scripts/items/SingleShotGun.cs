using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

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
                GameObject TrailObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BulletTrail"), itemGameobject.transform.position,Quaternion.identity);
                TrailRenderer trail = TrailObject.GetComponent<TrailRenderer>();
                //TrailRenderer trail = Instantiate(bulletTrail, itemGameobject.transform.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));
                LastShot = Time.time;

                hit.collider.transform.root.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)iteminfo).damage);
                if (hit.collider.transform.root.gameObject.tag != "Player")
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Decal"), hit.point, Quaternion.LookRotation(hit.normal));
               // Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
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
        //float dist = Vector3.Distance(startpos, hit.point);
        //trail.time = trail.time / dist;

       // Debug.Log("Distance: " + dist + " Time: " + );

        while (time < 5)
        {   
            //float t = time * dist;
            
            trail.transform.position = Vector3.Lerp(startpos, hit.point,time);

            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hit.point;
        

        Destroy(trail.gameObject, trail.time);
    }
}
