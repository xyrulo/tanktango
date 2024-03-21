// BulletSpawner.cs
// Spawns the bullets from the attached player using serverRPC.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform initTransform;
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip sfxFire;
    public float lifetime = 3f;

    public override void OnNetworkSpawn() {
        src.clip = sfxFire;
    }

    void Update()
    {
        // Firing
        if (Input.GetKeyDown(KeyCode.Space) && IsOwner) {
            SpawnBulletServerRPC(initTransform.position, initTransform.rotation);
        }
    }

    [ServerRpc]
    private void SpawnBulletServerRPC(Vector3 pos, Quaternion rot, ServerRpcParams serverRpcParams = default) {
        src.Play();
        GameObject instBullet = Instantiate(bullet, pos, rot);
        instBullet.GetComponent<Rigidbody2D>().AddForce(this.transform.up * 400f); 
        instBullet.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
        Destroy(instBullet, lifetime);
    }
}
