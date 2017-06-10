using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using LitJson;

public class StickyManager : MonoBehaviour {
	//マネージャーがリストを持つ
	StickyList stickyList;
	GameObject imageCanvas;
	//Resouceに置いてアタッチしないでプレハブを使う方法もある
	//そうするとstatic内で使える
	public GameObject stickyImagePrefab;

	void Awake() {
		imageCanvas = GameObject.Find ("Canvas");
		stickyList = new StickyList();
		StartCoroutine(GetText());
	}

	IEnumerator GetText() {
		var request = UnityWebRequest.Get("https://sticky-virtual.herokuapp.com/stickies");
		//Sendで通信が実行
		//コルーチンで通信が終わるまで待つ
		yield return request.Send();
		//downloadhandler.textでデータを取得
		string json = request.downloadHandler.text;
		//JSONテキストをオブジェクトにデコード
		JsonData jsonData = LitJson.JsonMapper.ToObject (json);
		//データの取得
		for (int i = 0; i < jsonData.Count; ++i) {
			stickyList.m_list.Add (new StickyItem () { keyword = (string)jsonData[i]["content"] });
		}
		this.setStickyList ();
		yield return null;
	}

	// Use this for initialization
	void Start () {
	}

	void setStickyList() {
		//プレハブからゲームオブジェクトを動的生成
		for (int i = 1; i <= stickyList.m_list.Count; i++) {
			//配列からItemインスタンスを取得
			StickyItem item = stickyList.m_list [i-1];
			//プレハブから生成(指定のゲームオブジェクトの子要素として出力したい)
			GameObject stickyImage = Instantiate (stickyImagePrefab) as GameObject;
			//Canvasの子要素にする
			//親要素に設定した時、Untiyがポジションとスケールを自動で変換してしまうのでポジションとスケールを初期化する
			//ポジションはlocalPositionを使った方がいい
			stickyImage.transform.parent = imageCanvas.transform;
			stickyImage.transform.localPosition = new Vector3 (0, -200, 0);
			stickyImage.transform.localScale = Vector3.one; //1,1,1
			//位置をずらす
			Vector3 pos = stickyImage.transform.localPosition;
			pos.y += i * 120;
			stickyImage.transform.localPosition = pos;
			//テキストをItemのプロパティを使う
			Text stickyText = stickyImage.transform.Find ("StickyText").GetComponent<Text> ();
			stickyText.text = item.keyword;
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
