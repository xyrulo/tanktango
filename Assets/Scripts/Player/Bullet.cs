// Bullet.cs
// Script for the projectile fired from the tank.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip sfxExplode;
    public float speed = 300f;
    
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        src.clip = sfxExplode;
        GetComponent<Rigidbody2D>().AddForce(this.transform.forward * speed);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        src.Play();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<SpriteRenderer>().enabled = false; 
        GetComponent<Collider2D>().enabled = false;
    }
}