﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public Transform player;
    public float speed = 1.25f;
    protected Rigidbody rigidBody;
    protected MeshFilter mesh;

    public AudioClip DeathAudioClip;
    public GameObject ExplosionFlash;

    public int Health = 3;

	// Use this for initialization
	protected virtual void Start ()
    {
        player = GameObject.Find("Ship(Clone)").transform;
        rigidBody = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshFilter>();
	}

    void Update()
    {
        if (player == null) //Assume dead
        {
            player = GameObject.Find("GameObject").transform;
        }
    }

    public void Hurt()
    {
        Health--;
        if (Health <= 0)
        {
            AudioSource.PlayClipAtPoint(DeathAudioClip, Vector3.Lerp(transform.position, GameObject.Find("Main Camera").transform.position, 0.9f));
            var explosionFlash = Instantiate(ExplosionFlash);
            explosionFlash.transform.position = transform.position;
            Destroy(gameObject);
        }
        else
        {
            var takeDamageAudioClips = GameObject.Find("GameObject").GetComponent<EnemyManager>().TakeDamageAudioClips;
            AudioSource.PlayClipAtPoint(takeDamageAudioClips[Random.Range(0, takeDamageAudioClips.Count)], Vector3.Lerp(transform.position, GameObject.Find("Main Camera").transform.position, 0.9f));
        }
    }
}
