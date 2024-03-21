/* PlayerController.cs
 * Controls the player movement, dying, respawning, collision
 */

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    public Bullet bulletPrefab;

    [SerializeField] private float turnSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 spawn1pos;
    [SerializeField] private Vector2 spawn2pos;
    [SerializeField] private Transform barrelOffset;
    private float _turnDir;
    private bool _forward;
    private bool _reverse;
    private Vector2 purgatory;
    private Vector2 spawnpos;
    private bool isDead;
    private int sfxCount;
    [SerializeField] private AudioSource engsrc, src;
    [SerializeField] private AudioClip sfxMove, sfxDie, sfxSpawn, sfxBump;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) {
            // If this player isn't owner, color them red and disable their controls on this user
            GetComponent<SpriteRenderer>().color = Color.red;
            this.enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        engsrc.clip = sfxMove;
        isDead = false;

        // Spawn Player at appropriate location
        if (IsHost) {
            spawnpos = spawn1pos;
            this.transform.position = spawn1pos;
            purgatory = spawn1pos * 9999;
        } else {
            spawnpos = spawn2pos;
            this.transform.position = spawn2pos;
            purgatory = spawn2pos * 9999;            
        }
    }

    void Update()
    {
        if (isDead) Die();

        // Forward/Back input
        _forward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow); 
        if (!_forward) _reverse = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        // Turning input
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) _turnDir = 1.0f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) _turnDir = -1.0f;
        else _turnDir = 0.0f;
    }

    private void FixedUpdate() {
        // Forward input
        if (_forward && _turnDir == 0) {
            rb.AddForce(this.transform.up * this.moveSpeed);
            sfxCount++;
        }

        // Reverse input
        if (_reverse && _turnDir == 0) {
            rb.AddForce(this.transform.up * this.moveSpeed * -1f);
            sfxCount++;
        }

        // Turn input
        if (_turnDir != 0) {
            rb.AddTorque(_turnDir * this.turnSpeed);
            sfxCount++;
        }

        // Engine sound
        if (sfxCount >= 4) {
            sfxCount = 0;
            engsrc.Play();
        }

    }

    // Detect bullets
    void OnTriggerEnter2D(Collider2D collider) {
        //if (!IsServer) return;
        if (collider.GetComponent<Bullet>()) {
            isDead = true;
        }
    }


    // Death/Respawn handling. Super basic, just push offscreen and teleport after wait period
    void Die() {
        src.PlayOneShot(sfxDie);
        isDead = true;
        this.transform.position = purgatory;
        Debug.Log("Dead");

        StartCoroutine(Respawn());
        isDead = false;
    }
    
    IEnumerator Respawn() {
        Debug.Log("Respawning");
        yield return new WaitForSeconds(3);
        src.PlayOneShot(sfxSpawn);
        this.transform.position = spawnpos;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        src.PlayOneShot(sfxBump);
    }
}
