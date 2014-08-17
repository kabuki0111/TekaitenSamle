using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour {
	private float maxFlickTimer;
	private Rigidbody rigidbody;
	private bool isGetMouseButtonDown;
	private bool isGetMouseButton;
	private FlickStatus startFlickStatus;
	private FlickStatus presentFlickStatus;
	private bool isGameStart = false;

	// Use this for initialization
	void Start () {
		rigidbody = GameSystemManager.Instance.InitRigidbodyAndGameLevel(this.gameObject);
		startFlickStatus = new FlickStatus(this.transform.localRotation, FlickType.None);
		presentFlickStatus = new FlickStatus(startFlickStatus.Quater, FlickType.None);
		StartCoroutine(GameSystemManager.Instance.StartCountDown());
	}

	private void Update()
	{
		if(!GameSystemManager.Instance.IsGameStart)
		{
			Debug.Log("can not play now update");
			return;
		}

		if(Input.GetMouseButtonDown(0))
		{
			startFlickStatus.TouchViewportVector = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		}


		if(Input.GetMouseButton(0))
		{
			maxFlickTimer += Time.deltaTime;
			if(maxFlickTimer >= VariableUtility.maxFlicktimer){
				return;
			}
			presentFlickStatus.TouchViewportVector = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			GameSystemManager.Instance.AddTargetGameObjectTorque(Camera.main, startFlickStatus, presentFlickStatus, rigidbody, 500f);
		}

		if(Input.GetMouseButtonUp(0))
		{
			maxFlickTimer = 0;
		}

	}


	private void FixedUpdate()
	{
		//現在の回転角度からターゲットの回転方向を割り出す.
		presentFlickStatus.Quater = this.transform.localRotation;
		GameSystemManager.Instance.ConfirmationTargetRotation(ref startFlickStatus, ref presentFlickStatus, rigidbody);
	}


	private void OnGUI()
	{
		GUI.Label(new Rect(10, 150, 400, 50), string.Format("total count--->{0}", GameSystemManager.Instance.TotalTargetGameRotationCount));
		if(GameSystemManager.Instance.IsGameStart)
		{
			GUI.Label(new Rect(10, 10, 250, 50), string.Format("rotation count ---> {0}", GameSystemManager.Instance.TargetRotationCount));
			GUI.Label(new Rect(10, 40, 250, 50), string.Format("play timer ---> {0}", GameSystemManager.Instance.PlayGameTimer));
		}
		else
		{
			GUI.Label(new Rect(10, 10, 250, 50), string.Format("count down --->{0}", GameSystemManager.Instance.CountDownLog));
		}

		if(Time.timeScale == 0)
		{
			if( GUI.Button(new Rect(10, 100, 300, 50),"reset"))
			{
				Time.timeScale = 1;
				Application.LoadLevel("Main");
			}
		}
	}

	private void test()
	{
		GameObject obj = Resources.Load("Character00") as GameObject;
		obj.transform.parent = this.transform;
	}
}
