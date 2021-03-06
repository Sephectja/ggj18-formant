﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveFollower : MonoBehaviour {
    private Rigidbody Rigidbody;
    private BulletGroup BulletGroup;

    public float fireRatePerSec = 1.0f;
    float fireTimer = 0.0f;

    public float ControllerForce;
    public float WavePull;

    public AudioClip DeathAudioClip;
    public GameObject ExplosionFlash;

    public Game Game;

	// Use this for initialization
	void Start () {
        Rigidbody = GetComponent<Rigidbody>();
        BulletGroup = GetComponent<BulletGroup>();
	}
	
	// Update is called once per frame
	void Update () {
        var inputX = Input.GetAxis("Horizontal");
        var inputY = Input.GetAxis("Vertical");
        var inputMagnitude = new Vector2(inputX, inputY).magnitude;
        if (inputMagnitude > 1.0f)
        {
            inputX /= inputMagnitude;
            inputY /= inputMagnitude;
            inputMagnitude = 1.0f;
        }

        Rigidbody.AddForce(new Vector3(-inputX, inputY, 0.0f) * ControllerForce * Time.deltaTime, ForceMode.Acceleration);

        
        if (inputMagnitude > 0.1f)
        {
            var inputAngle = Mathf.Atan2(-inputX, -inputY) + Mathf.PI;
            var currentAngle = Rigidbody.rotation.eulerAngles.z * Mathf.PI / 180.0f;
            var difference = inputAngle - currentAngle;
            if (difference > Mathf.PI) difference -= Mathf.PI * 2.0f;
            if (difference < -Mathf.PI) difference += Mathf.PI * 2.0f;
            difference *= 100.0f;
            Rigidbody.AddTorque(0.0f, 0.0f, difference, ForceMode.Acceleration);
        }

        var flameMagnitude = inputMagnitude * 0.5f + 0.5f;
        transform.GetChild(0).localScale = new Vector3(flameMagnitude * 0.2f, flameMagnitude * 0.7f, 0.0f);
        transform.GetChild(1).localScale = new Vector3(flameMagnitude * 0.25f, flameMagnitude * 0.5f, 0.0f);
        transform.GetChild(0).GetComponent<AudioSource>().volume = inputMagnitude * 0.3f;

        var magnitude = Rigidbody.position.magnitude;
        var normal = Rigidbody.position.normalized;
        var angle = Mathf.Atan2(Rigidbody.position.x, Rigidbody.position.y);

        IWaveRenderer closestWave = null;
        var distanceToClosestWave = float.PositiveInfinity;
        foreach (var wave in Game.Waves) wave.IsClosest = false;
        foreach (var wave in Game.Waves)
        {
            var distanceToWave = Mathf.Abs(wave.RadiusAt(angle) - magnitude);
            if (distanceToWave >= distanceToClosestWave) continue;
            closestWave = wave;
            distanceToClosestWave = distanceToWave;
        }
        closestWave.IsClosest = true;

        var wavePullRolloff = (closestWave.RadiusAt(angle) - magnitude);
        Rigidbody.AddForce(normal * WavePull * Time.deltaTime * wavePullRolloff, ForceMode.Acceleration);

        Vector3 fireDirection = new Vector3(Input.GetAxis("Fire Horizontal"), Input.GetAxis("Fire Vertical"));
        if (Input.GetKey(KeyCode.I))
            fireDirection.y = 1;
        if (Input.GetKey(KeyCode.K))
            fireDirection.y = -1;
        if (Input.GetKey(KeyCode.L))
            fireDirection.x = -1;
        if (Input.GetKey(KeyCode.J))
            fireDirection.x = 1;
        

        if (fireDirection.magnitude > 0.5f)
        {
            if (fireTimer == 0)
            {
                fireDirection.Normalize();
                BulletGroup.Fire(Rigidbody.position, fireDirection, 12.5f);
            }

            fireTimer += Time.deltaTime;
            if (fireTimer > 1.0f / fireRatePerSec)
                fireTimer = 0;
        }
        else
        {
            fireTimer = 0;
        }

        if (Mathf.Abs(transform.position.x) > 5.0f || transform.position.y < 0.5f || magnitude > 6.5f)
        {
            AudioSource.PlayClipAtPoint(DeathAudioClip, Vector3.Lerp(transform.position, GameObject.Find("Main Camera").transform.position, 0.9f));
            var explosionFlash = Instantiate(ExplosionFlash);
            explosionFlash.transform.position = transform.position;
            Destroy(gameObject);
            GameObject.Find("UICanvas").GetComponent<UI>().PlayerDied();
        }
    }
}
