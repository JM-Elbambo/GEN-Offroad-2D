using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class SpriteShapeTerrain : MonoBehaviour
{
	private SpriteShapeController controller;

	[Min(3)]
	[SerializeField] int maxChunks = 5;
	[SerializeField] int pointsPerChunk = 10;
	[SerializeField] int loadChunkAhead = 1;
	[SerializeField] float minY = 0;
	[SerializeField] float maxY = 7;
	[SerializeField] float pointDistance = 5f;
	[SerializeField] Vector3 leftTangent = new Vector3(-2, 0, 0);
	[SerializeField] Vector3 rightTangent = new Vector3(2, 0, 0);
	[SerializeField] float noiseIntensity = 0.1f;
	[SerializeField] float safeY = 2.5f;
	[SerializeField] float minSafeX = -10;
	[SerializeField] float maxSafeX = 10;

	int chunks = 0;
	float updateRightX;
	float updateLeftX;
	float loadDistance;
	float seed;

	// Bonus score spawning
	[SerializeField] GameObject bonusPrefab;
	[Range(0, 1)]
	[SerializeField] float bonusSpawnChance = 0.2f;
	[SerializeField] float yOffset = 2f;
	float farthestX = 0;

	private void Awake()
	{
		controller = GetComponent<SpriteShapeController>();
	}

	// Start is called before the first frame update
	void Start()
	{
		loadDistance = loadChunkAhead * pointsPerChunk * pointDistance;
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void UpdateTerrain(float posX)
    {
		if (posX > updateRightX)
        {
			UpdatePointsRight();

			// Set new bounds
			updateRightX = GetEndX() - loadDistance;
			updateLeftX = GetStartX() + loadDistance;
		}
		else if (posX < updateLeftX)
		{
			UpdatePointsLeft();

			// Set new bounds
			updateRightX = GetEndX() - loadDistance;
			updateLeftX = GetStartX() + loadDistance;
		}
	}

	void UpdatePointsRight()
	{
		int startIndex = controller.spline.GetPointCount() - 2;
		for (int i = startIndex; i < startIndex + pointsPerChunk; i++)
		{
			// Insert surface point
			float posX = controller.spline.GetPosition(i).x + pointDistance;
			Vector3 newPointPosition = new Vector3(
				posX,
				(posX >= minSafeX && posX <= maxSafeX) ? safeY :
				minY + (Mathf.PerlinNoise(posX * noiseIntensity, seed) * (maxY - minY)),
				0
			);
			controller.spline.InsertPointAt(i + 1, newPointPosition);

			// Smooth the curve by using continuous tangent
			controller.spline.SetTangentMode(i + 1, ShapeTangentMode.Continuous);
			controller.spline.SetLeftTangent(i + 1, leftTangent);
			controller.spline.SetRightTangent(i + 1, rightTangent);

			// Spawn a bonus point
			if (newPointPosition.x > farthestX)
			{
				if (Random.value <= bonusSpawnChance)
				{
					Vector3 bonusPosition = transform.position + newPointPosition + new Vector3(0, yOffset, 0);
					Bonus bonus = Instantiate(bonusPrefab, bonusPosition, Quaternion.identity, transform).GetComponent<Bonus>();
					bonus.RandomizeValue();
				}
			}
		}

		// Move lower right corner point
		int lrcIndex = controller.spline.GetPointCount() - 1;
		Vector3 lrcPosition = new Vector3(
			controller.spline.GetPosition(controller.spline.GetPointCount() - 2).x,
			-1,
			0
		);
		controller.spline.SetPosition(lrcIndex, lrcPosition);
		farthestX = lrcPosition.x;

		if (++chunks > maxChunks)
		{
			// Delete far left points
			for (int i = 0; i < pointsPerChunk; i++)
			{
				controller.spline.RemovePointAt(1);
			}


			// Move lower left corner point
			int llcIndex = 0;
			Vector3 llcPosition = new Vector3(controller.spline.GetPosition(1).x, -1, 0);
			controller.spline.SetPosition(llcIndex, llcPosition);
			
			chunks--;
		}
    }

	void UpdatePointsLeft()
	{
		for (int i = 1; i < pointsPerChunk + 1; i++)
		{
			// Insert surface point
			float posX = controller.spline.GetPosition(1).x - pointDistance;
			Vector3 newPointPosition = new Vector3(
				posX,
				(posX >= minSafeX && posX <= maxSafeX) ? safeY :
				minY + (Mathf.PerlinNoise(posX * noiseIntensity, seed) * (maxY - minY)),
				0
			);
			controller.spline.InsertPointAt(1, newPointPosition);

			// Smooth the curve by using continuous tangent
			controller.spline.SetTangentMode(1, ShapeTangentMode.Continuous);
			controller.spline.SetLeftTangent(1, leftTangent);
			controller.spline.SetRightTangent(1, rightTangent);
		}

		// Move lower left corner point
		int llcIndex = 0;
		Vector3 llcPosition = new Vector3(controller.spline.GetPosition(1).x, -1, 0);
		controller.spline.SetPosition(llcIndex, llcPosition);

		if (++chunks > maxChunks)
		{
			// Delete far right points
			for (int i = 0; i < pointsPerChunk; i++)
			{
				controller.spline.RemovePointAt(controller.spline.GetPointCount() - 2);
			}

			// Move lower right corner point
			int lrcIndex = controller.spline.GetPointCount() - 1;
			Vector3 lrcPosition = new Vector3(
				controller.spline.GetPosition(controller.spline.GetPointCount() - 2).x,
				-1,
				0
			);
			controller.spline.SetPosition(lrcIndex, lrcPosition);

			chunks--;
		}
	}

	public float GetStartX()
    {
		return controller.spline.GetPosition(0).x;
	}

	public float GetEndX()
    {
		int lrcIndex = controller.spline.GetPointCount() - 1;
		return controller.spline.GetPosition(lrcIndex).x;
    }

	public void RandomizeSeed()
    {
		seed = Random.value * 1000;
    }
}
