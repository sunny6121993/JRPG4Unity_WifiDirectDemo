using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WifiP2pController : MonoBehaviour, ICombatSystemController, IPLayerAttackHandler, ITurnHandler, IEnemyAttackHandler {


	public GUIText guiText;
	public GUIText statusText;
	public GUIStyle guiStyle;
	public GUIContent content;
	public WifiP2pHandler wifiP2pObject;

	//
	public bool isGettingDeviceInfo = false;
	public List<string> deviceInfo;
	public string[] deviceList;
	public string[] deviceListName;
	public bool connected = false;
	public bool isOpenedSocket = false;
	public bool isGroupOwner = false;
	public bool isStartServer = false;
	public bool isOpenSocket = false;
	public string previousString;
	int deviceInfoLength = 0;
	//
	public Rect searchButton = new Rect(0,0,400,75);
	public Rect connectButton = new Rect(0,75,400,75);
	public Rect serverButton = new Rect(0,150,400,75);
	public Rect closeSocketButton = new Rect(0,225,400,75);
	public Rect disconnectButton = new Rect(0,0,400,75);
	public Rect stopDiscoverButton = new Rect(0,300,400,75);
	//Combat System

	public bool isCombatStart = false;
	//
	public enum Turn { Player, Player2 }
	public enum Force { Player, Player2 }
	
	public int partyMemberNum = 1;
	public int enemyNum = 1;
	public GameObject enemy;
	
	private const string GAME_OVER_TEXT = "Game Over";
	private const string PLAYER_WIN_TEXT = "You Win";
	
	private WifiP2pPlayerController playerController;
	private WifiP2pEnemyController[] enemyControllers;
	public Turn currentTurn;
	private bool isCombatEnd;
	private string resultText;
	private GUIStyle style;
	private Rect resultRect;
	//
	
	void Awake(){
		
	}
	
	// Use this for initialization
	void Start () {
		wifiP2pObject = new WifiP2pHandler(Random.Range(1,100).ToString());
		wifiP2pObject.registerWifiP2pReceiver();
		//You Can Put Record Here
		//
		wifiP2pObject.registerService();
		deviceInfo = new List<string>();
		connected = false;
		guiStyle = new GUIStyle();
		guiStyle.fontSize = 30;
		//Combat System
		isCombatEnd = false;
	}
	void OnDestroy(){
		wifiP2pObject.cancelWifiP2pConnection();
	}
	// Update is called once per frame
	void Update () {
		if(isGettingDeviceInfo&&!connected){
			deviceInfoLength = wifiP2pObject.getDeviceInfoLength();
			if(deviceInfoLength>0){
				isGettingDeviceInfo = false;
				deviceInfo.Add(wifiP2pObject.getDeviceInfo(0));
				if(deviceInfo.Capacity>0){
					deviceListName = new string[deviceInfo.Capacity];
					for(int i=0;i<deviceInfo.Capacity;i++){
						int cutOff = 0;
						cutOff = deviceInfo[0].IndexOf("/");
						deviceListName[0] = deviceInfo[0].Substring(0,cutOff);

						if(i+1==1){
							break;
						}
					}
					guiText.text = "Found Device Name: "+deviceListName[0].ToString();
				}

			}else{
				guiText.text = "";
			}




			
		}

		connected = wifiP2pObject.isWifiP2pConnect();

		if(connected){
			statusText.text = "Connected";
			isGroupOwner = wifiP2pObject.getIsGroupOwner();
			bool tempBool =  wifiP2pObject.socketIsConnected(isGroupOwner);
			if(tempBool!=null){
				if(tempBool == false){
					Debug.Log("waiting for start server and client");
					/*if(!isOpenSocket){
						wifiP2pObject.startServerClient(isGroupOwner);
						isStartServer = true;
						isOpenSocket = true;
					}*/
				}else{
					if(!isCombatStart){
						isCombatStart = true;
						//Combat System
						isCombatEnd = false;
						
						playerController = gameObject.AddComponent<WifiP2pPlayerController>();
						playerController.PartyMemberNum = partyMemberNum;
						playerController.PlayerAttackHandler = this;
						playerController.TurnHandler = this;
						playerController.SystemController = this;
						playerController.WifiP2pObject = this.wifiP2pObject;
						playerController.wifiP2pController = this;
						Debug.Log("updatetesting1");
						enemyControllers = new WifiP2pEnemyController[enemyNum];
						
						GameObject enemyClone = null;
						for (int i = 0; i < enemyNum; i++)
						{
							enemyClone = Instantiate(enemy) as GameObject;
							enemyControllers[i] = enemyClone.GetComponent<WifiP2pEnemyController>();
							enemyControllers[i].EnemyAttackHandler = this;
							enemyControllers[i].TurnHandler = this;
							enemyControllers[i].SystemController = this;

						}
						Debug.Log("updatetesting2");
						if(isGroupOwner){
							Debug.Log("isGroupOwner, current turn is player");
							currentTurn = Turn.Player;
						}else{
							Debug.Log("!isGroupOwner, current turn is player2");
							currentTurn = Turn.Player2;
						}
						style = new GUIStyle();
						style.alignment = TextAnchor.MiddleCenter;
						style.fontStyle = FontStyle.Bold;
						style.fontSize = 30;
						resultRect = new Rect(Screen.width / 2f - 50, Screen.height / 2f - 15, 100, 30);
						
						
						//
					}else{
						switch (currentTurn)
						{
						case Turn.Player:
							playerController.ShowCommand();
							break;
						case Turn.Player2:
							break;
						}
					}
				}
			}
			if(isStartServer){
				readMessage();
			}

		}
		//
	}
	void OnGUI(){
		if(!isCombatStart){
			//guiStyle = "Discover Peer";
			if(GUI.Button(searchButton,"Discover Peer")){
				wifiP2pObject.discoverService();
				isGettingDeviceInfo = true;
			}
			if(GUI.Button(connectButton,"Connect")){
				if(!connected){
					wifiP2pObject.connectToPeer(deviceListName[0]);
					statusText.text = "Connecting";
				}
			}

			if(GUI.Button(serverButton,"Server/Client")){
				if(connected){
					
					wifiP2pObject.startServerClient(isGroupOwner);
					isStartServer = true;
				
				}
			}
			if(GUI.Button(closeSocketButton,"CloseSocket")){
				if(connected&&isStartServer){
					wifiP2pObject.closeSocket(isGroupOwner);
					isStartServer = false;
				}
			}

			if(GUI.Button(stopDiscoverButton,"stopPeerDiscovery")){
				wifiP2pObject.stopPeerDiscovery();
				
			}
		}

		if (isCombatEnd)
		{
			GUI.Label(resultRect, resultText, style);
			if(GUI.Button(disconnectButton,"Disconnect")){
				if(connected){
					wifiP2pObject.cancelWifiP2pConnection();
					statusText.text = "disconnected";
				}
			}
		}
	}

	private void readMessage(){
		string readString = wifiP2pObject.getMessage(isGroupOwner);
		if(readString!=previousString&&readString!=null){
			previousString = readString;
			string cutOffString = readString.Substring(0,readString.IndexOf('/'));


			if(cutOffString.CompareTo("Hurt")==0){


				Debug.Log("Hurt"+readString.Substring(readString.IndexOf('/')+1,readString.IndexOf('#')-readString.IndexOf('/')-1));
				playerController.Hurt(0, calculateDamage(int.Parse(readString.Substring(readString.IndexOf('/')+1,readString.IndexOf('#')-readString.IndexOf('/')-1)), playerController.GetPlayerDef(0)));
				enemyControllers[0].changeTurn((int)currentTurn);
				//enemyControllers[0].isDefending = false;
				//enemyControllers[0].currentEnemyState = WifiP2pEnemyController.EnemyState.Attack;
			}
			if(cutOffString.CompareTo("Def")==0){
				enemyControllers[0].changeTurn((int)currentTurn);
				enemyControllers[0].isDefending = true;
				enemyControllers[0].currentEnemyState = WifiP2pEnemyController.EnemyState.Defense;
			}
		}else{
			
		}
	}
	public void playerDefense(){
	}
	public void noticePlayerDefense(int currentPlayer){
		//WifiP2p
		wifiP2pObject.sendMessage(isGroupOwner,"Def/"+currentPlayer+"#"+Random.Range(0,1000000));
	}
	//Combat System
	private int calculateDamage(int atk, int def)
	{
		return atk - def + Random.Range(atk, def);
	}
	
	public void OnPlayerAttack(int playerAtk)
	{
		//WifiP2p
		wifiP2pObject.sendMessage(isGroupOwner,"Hurt/"+playerAtk+"#"+Random.Range(0,1000000));
		//
		//enemyControllers[0].Hurt(calculateDamage(playerAtk, enemyControllers[0].EnemyDef));
	}
	
	public int GetCurrentTurn()
	{
		return (int)currentTurn;
	}
	
	public void OnTurnEnd(int currentTurn)
	{
		switch (currentTurn)
		{
		case (int)Turn.Player:
			this.currentTurn = Turn.Player2;
			break;
		case (int)Turn.Player2:
			this.currentTurn = Turn.Player;
			break;
		}
	}
	
	public void OnEnemyAttack(int enemyAtk)
	{
		playerController.Hurt(0, calculateDamage(enemyAtk, playerController.GetPlayerDef(0)));
	}
	
	public void OnCombatEnd(int loseForce)
	{
		isCombatEnd = true;
		switch (loseForce)
		{
		case (int)Force.Player:
			resultText = GAME_OVER_TEXT;
			style.normal.textColor = Color.red;
			break;
		case (int)Force.Player2:
			resultText = PLAYER_WIN_TEXT;
			style.normal.textColor = Color.blue;
			break;
		}
	}
	
}
