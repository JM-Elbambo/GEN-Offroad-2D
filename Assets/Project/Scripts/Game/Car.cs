using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Car : MonoBehaviour
{
	[SerializeField] Rigidbody2D body;
	[SerializeField] Rigidbody2D rearWheel;
	[SerializeField] Rigidbody2D frontWheel;
	[SerializeField] CollisionChecker2D dieTriggerArea;
	[SerializeField] CollisionChecker2D collectorTriggerArea;

	[Header("Car Properties")]
	[SerializeField] bool allWheelDrive;
	[SerializeField] float accelerationTorque = -8;
	[SerializeField] float reverseTorque = 5;
	[SerializeField] float minWheelAngularVelocity = -1600;
	[SerializeField] float maxWheelAngularVelocity = 1000;
	[SerializeField] float tiltTorque = 20;
	[SerializeField] float maxBodyAngularVelocity = 200;
	float torque;

	// Status
	bool gas;
	bool reverse;
	float tilt;

	public UnityAction OnDie;
	public UnityAction<int> OnBonusCollected;

	// Start is called before the first frame update
	void Start()
	{
		// Initialize car state
		Gas(false);
		Reverse(false);
	}

	// Update is called once per frame
	void Update()
	{
		
	}

    private void FixedUpdate()
    {
		UpdateWheels();
		UpdateTilt();
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
		print(collision.collider.tag);
	}

	public void Initialize()
    {
		dieTriggerArea.OnCollision2D = Die;
		collectorTriggerArea.OnTrigger2D = OnCollect;
	}

	void Die(Collider2D collision)
    {
		OnDie?.Invoke();
    }

	void OnCollect(Collider2D collision)
	{
		Bonus bonus = collision.GetComponent<Bonus>();
		if (bonus != null)
		{
			int value = bonus.Collect();
			OnBonusCollected?.Invoke(value);
			Destroy(bonus.gameObject);
		}
	}

	void UpdateWheels()
    {
		// Calculate new torque
		if (gas)
			torque = accelerationTorque;
		if (reverse)
			torque = reverseTorque;

		// Apply torque to wheels
		if (gas || reverse)
		{
			rearWheel.AddTorque(torque);
			if (allWheelDrive)
				frontWheel.AddTorque(torque);
		}

		// Limit wheel angular velocity
		if (rearWheel.angularVelocity < minWheelAngularVelocity)
			rearWheel.angularVelocity = minWheelAngularVelocity;
		else if (rearWheel.angularVelocity > maxWheelAngularVelocity)
			rearWheel.angularVelocity = maxWheelAngularVelocity;

		if (frontWheel.angularVelocity < minWheelAngularVelocity)
			frontWheel.angularVelocity = minWheelAngularVelocity;
		else if (frontWheel.angularVelocity > maxWheelAngularVelocity)
			frontWheel.angularVelocity = maxWheelAngularVelocity;
	}

	void UpdateTilt()
    {
		float _tiltTorque = tilt * tiltTorque;
		body.AddTorque(_tiltTorque);

		// Limit body angular velocity
		if (body.angularVelocity < -maxBodyAngularVelocity)
			body.angularVelocity = -maxBodyAngularVelocity;
		else if (body.angularVelocity > maxBodyAngularVelocity)
			body.angularVelocity = maxBodyAngularVelocity;
	}

    public void Gas(bool pressed)
	{
		gas = pressed;
	}

	public void Reverse(bool pressed)
	{
		reverse = pressed;
	}

	public Rigidbody2D GetBody()
    {
		return body;
    }

	public void Tilt(float value)
    {
		tilt = value;
    }
}
