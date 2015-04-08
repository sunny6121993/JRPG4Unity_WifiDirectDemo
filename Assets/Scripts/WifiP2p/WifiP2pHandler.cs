using UnityEngine;
using System.Collections;

public class WifiP2pHandler{

	private AndroidJavaClass wifiP2pPlugin;
	private AndroidJavaObject activity;
	private AndroidJavaObject wifiP2pObject;


	public WifiP2pHandler(string deviceName){
		wifiP2pPlugin = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		activity = wifiP2pPlugin.GetStatic<AndroidJavaObject>("currentActivity");
		wifiP2pObject = new AndroidJavaObject("com.dirbil.wifip2plibrary.WifiP2pLibraryActivity",activity,deviceName);

		wifiP2pObject.Call("cancelWifiP2pConnection");
		wifiP2pObject.Call("stopPeerDiscovery");
	}
	
	public void registerWifiP2pReceiver(){
		wifiP2pObject.Call("registerWifiP2pReceiver");
	}

	public void registerService(){
		wifiP2pObject.Call("registerService");
	}
	public void stopPeerDiscovery(){
		wifiP2pObject.Call("stopPeerDiscovery");
	}
	public void putRecord(string key,string data){
		wifiP2pObject.Call("putRecord",key,data);
	}
	public void putRecord(string key,int data){
		wifiP2pObject.Call("putRecord",key,data);
	}
	public int getDeviceInfoLength(){
		return wifiP2pObject.Call<int>("getDeviceInfoLength");
	}
	public string getDeviceInfo(int deviceInfoNum){
		return wifiP2pObject.Call<string>("getDeviceInfo",deviceInfoNum);
	}
	public bool isWifiP2pConnect(){
		return wifiP2pObject.Call<bool>("isWifiP2pConnect");
	}
	public bool getIsGroupOwner(){
		return wifiP2pObject.Call<bool>("getIsGroupOwner");
	}
	public bool socketIsConnected(bool isGroupOwner){
		//Debug.Log("socketIsConnected : "+isGroupOwner);
		bool tempBoolean = wifiP2pObject.Call<bool>("socketIsConnected",isGroupOwner);
		//Debug.Log("socketIsConnected return values : "+tempBoolean);
		return tempBoolean;
	}
	public string getMessage(bool isGroupOwner){
		return wifiP2pObject.Call<string>("getMessage",isGroupOwner);
	}
	public void discoverService(){
		wifiP2pObject.Call("discoverService");
	}
	public void connectToPeer(string deviceName){
		wifiP2pObject.Call("connectToPeer",deviceName);
	}
	public void startServerClient(bool isGroupOwner){
		wifiP2pObject.Call("startServerClient",isGroupOwner);
	}
	public void sendMessage(bool isGroupOwner,string message){
		wifiP2pObject.Call("sendMessage",isGroupOwner,message);
	}
	public void closeSocket(bool isGroupOwner){
		wifiP2pObject.Call("closeSocket",isGroupOwner);
	}
	public void cancelWifiP2pConnection(){
		wifiP2pObject.Call("cancelWifiP2pConnection");
	}


}
