using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSystemManager : SingletonMonoBehaviour<GameSystemManager>{

	#region inspector
	private float sumDiffEuler;
	private FlickStatus beforeFlickStatus;
	private int gameClearCount;
	private GameData gameData;
	private List<GameLevelInfo> gameLevelInfoListMaster;
	private GameLevelInfo presentGameLevelInfo;
	private float __playGameTimer;
	#endregion

	#region property
	public bool IsGameStart{get; private set;}
	public int TotalTargetGameRotationCount{get; private set;}
	public int TargetRotationCount{get; private set;}
	public float PlayGameTimer{
		get
		{
			return this.__playGameTimer;
		}
		private set
		{
			this.__playGameTimer = value;
		}
	}
	public int CountDownLog{get; private set;}
	public string Log{get; set;}
	#endregion

	#region unity
	private void Awake()
	{
		if(this != Instance)
		{
			Destroy(this);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
		InvalidPlayGame(false);

		//全ゲーム情報リストをあらかじめ用意する.
		gameLevelInfoListMaster = GameData.LoadGameLevelInfo();
	}

	/// <summary>
	/// ゲームの管理に使用.
	/// </summary>
	private void Update()
	{
		if(!IsGameStart)
		{
			return;
		}

		PlayGameTimer -= Time.deltaTime;

		//制限時間経過時に実行.
		if(PlayGameTimer < 0)
		{
			Debug.Log("test okay");
			if(!IsGameStart)
			{
				return;
			}
			InvalidPlayGame(false, VariableUtility.gameStopScale);
			Debug.Log("game end");
			//勝ち負け判定.
			JudgmentVictoryOrDefeat();
		}
	}
	#endregion

	#region public
	/// <summary>
	/// ターゲットのRigidbodyのパラメータを設定.
	/// </summary>
	/// <returns>The rigidbody.</returns>
	/// <param name="targetGameObject">Target game object.</param>
	public Rigidbody InitRigidbodyAndGameLevel(GameObject targetGameObject)
	{
		//テキストからRigidbodyの情報を取得する.
		presentGameLevelInfo = GameData.SetLadGameLevelInfo(gameLevelInfoListMaster, gameClearCount);

		//制限時間を設定.
		this.PlayGameTimer = presentGameLevelInfo.MaxPlayGameTimeRimit;

		Debug.Log("<<init rigidbody!!>>");
		Rigidbody rigidbody = targetGameObject.GetComponent<Rigidbody>() as Rigidbody;
		if(rigidbody == null)
		{
			Debug.Log("rigidbody add");
			rigidbody = targetGameObject.AddComponent<Rigidbody>();
		}

		//Rigidbodyのステータスを設定.
		rigidbody.mass = presentGameLevelInfo.Mass;
		rigidbody.drag = presentGameLevelInfo.Drag;
		rigidbody.useGravity = false;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

		//プレハブのモデルを設定.
		string pathTarget = string.Format("Prefab/{0}", presentGameLevelInfo.TargetGameObjectName);
		Object objectCharacter = Resources.Load(pathTarget);
		GameObject cloneObj = Instantiate(objectCharacter) as GameObject;
		if(cloneObj != null)
		{
			Debug.Log("game object setting...");
			cloneObj.transform.parent = targetGameObject.transform;
			cloneObj.transform.localPosition = Vector3.zero;
		}
		else
		{
			Debug.LogError("not have prefab... please check text file."+pathTarget);
		}

		return rigidbody;
	}

	public IEnumerator StartCountDown()
	{
		yield return new WaitForSeconds(1f);
		CountDownLog = 3;
		yield return new WaitForSeconds(1f);
		CountDownLog = 2;
		yield return new WaitForSeconds(1f);
		CountDownLog = 1;
		yield return new WaitForSeconds(1f);
		CountDownLog = 0;
		IsGameStart = true;
	}

	/// <summary>
	/// ビューポートから差分のVector3を算出. ターゲットのRigidbodyに回転のトルクを与える.
	/// </summary>
	/// <returns>The rotation torque speed.</returns>
	/// <param name="camera">Camera.</param>
	/// <param name="vectorStart">Vector start.</param>
	/// <param name="vectorPresent">Vector present.</param>
	public void AddTargetGameObjectTorque(Camera camera, FlickStatus startViewportFlick, FlickStatus presentViewportFlick, Rigidbody targetRigidBody, float addSpeed = 1000)
	{
		//タップした初期の座標と現在の座標からビューボードを導き指す.
		Vector3 viewportVectorStart = startViewportFlick.TouchViewportVector;
		Vector3 viewportVectorPresent = presentViewportFlick.TouchViewportVector;

		//初期値と現在の座標から差分を出す.
		Vector3 viewportVectorDiff = viewportVectorStart - viewportVectorPresent;

		//xとyの座標（値）を入れ替える.
		float viewportDiffY = viewportVectorDiff.y;
		viewportVectorDiff.y = viewportVectorDiff.x;
		viewportVectorDiff.x = viewportDiffY;

		//距離と時間から指の移動速度を求める(きはじの法則). *加算速度.
		viewportVectorDiff.y *= (addSpeed * Time.deltaTime);
		targetRigidBody.AddTorque(viewportVectorDiff);
	}


	/// <summary>
	/// ターゲットが左右のどちらかを回転しているかを確認、それに合わせてスコアの管理を行うメソッド.
	/// </summary>
	/// <param name="startFlickStatus">回転を開始した時のFlickStatus.</param>
	/// <param name="presentFlickStatus">現在のFlickStatus.</param>
	public void ConfirmationTargetRotation(ref FlickStatus startFlickStatus, ref FlickStatus presentFlickStatus, Rigidbody targetRigidbody)
	{
		//50fps前のFlickStatusを初期化
		if(beforeFlickStatus == null)
		{
			Debug.Log("defore init");
			beforeFlickStatus = new FlickStatus();
		}

		//Y軸の差分を算出.
		float beforeEulerY = beforeFlickStatus.Quater.eulerAngles.y;
		float presentEulerY = presentFlickStatus.Quater.eulerAngles.y;
		float diffEulerY = Mathf.DeltaAngle(beforeEulerY, presentEulerY);

		//差分の値から見回転方向を求める（右左）.
		presentFlickStatus.Type = SetPresentFlickType(targetRigidbody);

		//ターゲットが回転していないところから処理する.
		if(startFlickStatus.Type == FlickType.None)
		{
			startFlickStatus.Type = SetPresentFlickType(targetRigidbody);
		}

		//ターゲットの現在の回転方向が別の回転を行った時にスタートのFlickStatusを更新する.
		if(startFlickStatus.Type != presentFlickStatus.Type)
		{
			startFlickStatus.RenewalStatus(
				presentFlickStatus.Quater,
				presentFlickStatus.TouchViewportVector,
				presentFlickStatus.Type);
		}

		//スコア加算の処理.
		AddScore(startFlickStatus, presentFlickStatus, diffEulerY);

		//前のフレームのステータスを更新.
		beforeFlickStatus.RenewalStatus(
			presentFlickStatus.Quater,
			presentFlickStatus.TouchViewportVector,
			presentFlickStatus.Type);
	}
	#endregion

	#region private
	/// <summary>
	/// RigidbodyのangularVelocityの回転値から回転している方向を導き出す.
	/// </summary>
	/// <returns>The present flick type.</returns>
	/// <param name="targetRigidbody">Target rigidbody.</param>
	private FlickType SetPresentFlickType(Rigidbody targetRigidbody)
	{
		if(targetRigidbody.angularVelocity.y > 0)
		{
			return FlickType.Right;
		}
		else if(targetRigidbody.angularVelocity.y < 0)
		{
			return FlickType.Left;
		}
		return FlickType.None;
	}

	/// <summary>
	/// スコアを算出するメソッド.
	/// </summary>
	/// <param name="startFlickStatus">回転を開始したFlickStatus.</param>
	/// <param name="presentFlickStatus">現在のFlickStatus.</param>
	/// <param name="presentDiffEuler">現在の差分.</param>
	private void AddScore(FlickStatus startFlickStatus, FlickStatus presentFlickStatus, float presentDiffEuler)
	{
		//ターゲットが逆回転したら、現在のスコアと円の値をかけて合計回転数の内容を書き換える.
		if(startFlickStatus.Type != presentFlickStatus.Type)
		{
			sumDiffEuler = VariableUtility.angleOf360DegreesFloat * TargetRotationCount;
			return;
		}

        sumDiffEuler += Mathf.Abs(presentDiffEuler);
		int count = Mathf.FloorToInt(sumDiffEuler) / VariableUtility.angleOf360DegreesInt;

		//割った値つまり回転数が現在の回転数よりも多かったら1加算する.
		if(count <= TargetRotationCount)
		{
			return;
		}

		TargetRotationCount ++;
	}

	/// <summary>
	/// 勝敗判定をするメソッド.
	/// </summary>
	private void JudgmentVictoryOrDefeat()
	{
		//勝ち負け判定.
		if(presentGameLevelInfo.MinGameClearCount < TargetRotationCount)
		{
			Debug.Log("level up!!");
			TotalTargetGameRotationCount += TargetRotationCount;
			this.gameClearCount++;
			//回転数の初期化.
			TargetRotationCount = 0;
			sumDiffEuler = 0;
			return;
		}

		Debug.Log("game over...");
	}

	/// <summary>
	/// ゲーム終了時にステータスを治す.
	/// </summary>
	/// <param name="flag">If set to <c>true</c> flag.</param>
	/// <param name="timeScale">Time scale.</param>
	private void InvalidPlayGame(bool flag, float timeScale = VariableUtility.gamePlayScale)
	{
		this.IsGameStart = flag;
		Time.timeScale = timeScale;
	}
	#endregion

}