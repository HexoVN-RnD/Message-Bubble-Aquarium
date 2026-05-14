using UnityEngine;
using System.Collections;

public class GlobalFlock : MonoBehaviour
{
	public static GlobalFlock instance;

	public GameObject defaultFish;
	public GameObject[] fishPrefabs;
	public GameObject fishSchool;

	[SerializeField]
	private Collider tankCollider;
	private Bounds tankBounds;

	public int numFish = 30;
	public static GameObject[] allFish;
	public static Vector3 goalPos = Vector3.zero;

	// Use this for initialization
	void Start()
	{
		instance = this;

		// Validate tank collider is assigned
		if (tankCollider == null)
		{
			Debug.LogError("[GlobalFlock] tankCollider is not assigned in the Inspector. Please assign a Collider component to define tank boundaries.");
			return;
		}

		// Cache the collider bounds for fish spawning and boundary calculations
		tankBounds = tankCollider.bounds;

		allFish = new GameObject[numFish];
		for (int i = 0; i < numFish; i++)
		{
			// Generate spawn position within collider bounds
			Vector3 pos = new Vector3(
				Random.Range(tankBounds.min.x, tankBounds.max.x),
				Random.Range(tankBounds.min.y, tankBounds.max.y),
				Random.Range(tankBounds.min.z, tankBounds.max.z)
			);
			GameObject fish = (GameObject)Instantiate(
				fishPrefabs[Random.Range(0, fishPrefabs.Length)], pos, Quaternion.identity);
			fish.transform.parent = fishSchool.transform;
			allFish[i] = fish;
		}
	}

	// Update is called once per frame
	void Update()
	{
		HandleGoalPos();
	}

	/// <summary>
	/// Returns the bounds of the tank collider for use by other scripts.
	/// </summary>
	public Bounds GetTankBounds()
	{
		return tankBounds;
	}

	void HandleGoalPos()
	{
		if (Random.Range(1, 10000) < 50)
		{
			// Generate goal position within collider bounds
			goalPos = new Vector3(
				Random.Range(tankBounds.min.x, tankBounds.max.x),
				Random.Range(tankBounds.min.y, tankBounds.max.y),
				Random.Range(tankBounds.min.z, tankBounds.max.z)
			);
		}
	}
}
