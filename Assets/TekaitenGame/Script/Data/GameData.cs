using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameData : MonoBehaviour {

	public static List<GameLevelInfo> LoadGameLevelInfo()
	{
		List<GameLevelInfo> gameLevelInfoList = new List<GameLevelInfo>();

		TextAsset textAssetGameLevelData = Resources.Load("Text/TargetData") as TextAsset;
		char[] charNewLine ={'\r', '\n'};
		string[] gameLevelDataStrings = textAssetGameLevelData.text.Split(charNewLine);

		foreach(var entityData in gameLevelDataStrings)
		{
			string[] detailData = entityData.Split(',');
			GameLevelInfo levelInfo = new GameLevelInfo();

			levelInfo.GameClearCountLevel = int.Parse(detailData[0]);
			levelInfo.MaxPlayGameTimeRimit = float.Parse(detailData[1]);
			levelInfo.MinGameClearCount = int.Parse(detailData[2]);
			levelInfo.Mass = float.Parse(detailData[3]);
			levelInfo.Drag = float.Parse(detailData[4]);
			levelInfo.AngularDrag = float.Parse(detailData[5]);
			levelInfo.TargetGameObjectName = detailData[6];

			gameLevelInfoList.Add(levelInfo);
		}
		return gameLevelInfoList;
	}

	public static GameLevelInfo SetLadGameLevelInfo(List<GameLevelInfo> targetList, int clearCount)
	{
		IEnumerable<GameLevelInfo> list = from item in targetList
			where item.GameClearCountLevel == clearCount
			select item;

		int index = Random.Range(0, list.Count());
		List<GameLevelInfo> hoge = list.ToList();
		GameLevelInfo info = hoge[index];
		return info;
	}
}