using UnityEngine;
using System.Collections;

public class GameLevelInfo {
	public int GameClearCountLevel{get; set;}
	public int MinGameClearCount{get; set;}
	public float MaxPlayGameTimeRimit{get; set;}
	public float Mass{get; set;}
	public float Drag{get; set;}
	public float AngularDrag{get; set;}
	public string TargetGameObjectName{get; set;}

	public GameLevelInfo( int gameClearCount = 0, int minGameCount = 9, float maxPlayGameTimer = 30f, float mass = 1f, float drag = 0, float angularDrag = 0.05f, string targetGameObjectName = "")
	{
		this.GameClearCountLevel = gameClearCount;
		this.MinGameClearCount = minGameCount;
		this.MaxPlayGameTimeRimit = maxPlayGameTimer;
		this.Mass = mass;
		this.Drag = drag;
		this.AngularDrag = angularDrag;
		this.TargetGameObjectName = targetGameObjectName;
	}

	public override string ToString ()
	{
		return string.Format ("[GameLevelInfo: GameClearCountLevel={0}, MinGameClearCount={1}, MaxPlayGameTimeRimit={2}, Mass={3}, Drag={4}, AngularDrag={5}, TargetGameObjectName={6}]", GameClearCountLevel, MinGameClearCount, MaxPlayGameTimeRimit, Mass, Drag, AngularDrag, TargetGameObjectName);
	}
}
