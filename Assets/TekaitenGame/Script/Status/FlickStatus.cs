using UnityEngine;
using System.Collections;

public class FlickStatus
{
	private Quaternion __quater;
	private Vector3 __touchViewportVector;
	private FlickType __type;
	private EulerAnglesType eulerType;

	public Quaternion Quater{get; set;}
	public Vector3 TouchViewportVector{get; set;}
	public FlickType Type{get; set;}

	//ステータスが所持している角度から角度のタイプを自動的に割り出すように設計したプロパティ.
	public EulerAnglesType EulerType
	{
		get
		{
			this.eulerType = EulerAnglesType.None;
			if(this.Quater.eulerAngles.y >= VariableUtility.angleOf0DegreesFloat && this.Quater.eulerAngles.y < VariableUtility.angleOf180DegreesFloat)
			{
				this.eulerType = EulerAnglesType.FirstHalf;
			}
			if(this.Quater.eulerAngles.y >= VariableUtility.angleOf180DegreesFloat && this.Quater.eulerAngles.y < VariableUtility.angleOf360DegreesFloat)
			{
				this.eulerType = EulerAnglesType.LatterHalf;
			}
			return this.eulerType;
		}
	}

	//コンストラクターをオーバーロード.
	public FlickStatus()
	{
		this.Quater = new Quaternion();
		this.TouchViewportVector = new Vector3();
		this.Type = FlickType.None;
	}

	public FlickStatus(Quaternion qua, FlickType type)
	{
		this.Quater = qua;
		this.TouchViewportVector = new Vector3();
		this.Type = type;
	}

	//ターゲットの回転の方向が変更された時に開始地点を記憶時に使用.
	public void RenewalStatus(Quaternion quaternion, Vector3 touchViewportVector, FlickType flickType = FlickType.None)
	{
		this.Quater = quaternion;
		this.TouchViewportVector = touchViewportVector;
		this.Type = flickType;
	}

	//オーバーライドでステータス確認.
	public override string ToString ()
	{
		return string.Format ("[FlickStatus: Quater={0}, TouchVector={1}, Type={2} EulerType={3}]", Quater.eulerAngles, TouchViewportVector, Type, EulerType);
	}


}
