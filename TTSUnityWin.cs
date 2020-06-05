using UnityEngine;
using System.Collections;
using SpeechLib;
using System.Xml;
using System.IO;
using UnityEngine.Video;
using UnityEngine.UI;

/**************************************************************************************
 * ******************Text to Speech for Windows SAPI *********************************
 * ************************************************************************************
 * v 1.0 - 01/01/2016
 * Marco Martinelli 
 * marco.m@gamesource.it
 * 
 * 
 * Tested on Windows 10, SAPI 4.0 | Windows 8.1
 * More info on www.finalmarco.com
/*************************************************************************************/



public class TTSUnityWin : MonoBehaviour {

	private SpVoice voice;
    public bool intro = false,  task1 = false, task2= false;
    bool intro1 = false, intro2 = false, intro3=false;
    StudyManager sm;
    public int count = 0;
    VideoPlayer vp;
    //public Text text;
    public GameObject plane;

/// CODE FOR LOAD XML OR OTHER TEXT FILES IN THE SISTEM FROM THE FOLDER RESOURCE

	string loadXMLStandalone (string fileName) {
		
		string path  = Path.Combine("Resources", fileName);
		path = Path.Combine (Application.dataPath, path);
		Debug.Log ("Path:  " + path);
		StreamReader streamReader = new StreamReader (path);
		string streamString = streamReader.ReadToEnd();
		Debug.Log ("STREAM XML STRING: " + streamString);
		return streamString;
	}
///




	//Resources.Load('builtIn.xml') as Texture;

/// PRINT ON SCREEN SAPI VOICES INSTALLED IN THE OS
	//void OnGUI() {
	
	//	SpObjectTokenCategory tokenCat = new SpObjectTokenCategory();
	//	tokenCat.SetId(SpeechLib.SpeechStringConstants.SpeechCategoryVoices, false);
	//	ISpeechObjectTokens tokens = tokenCat.EnumerateTokens(null, null);
		
	//	int n = 0;
	//	foreach (SpObjectToken item in tokens)
	//	{
	//			GUILayout.Label( "Voice"+ n +" ---> "+ item.GetDescription(0));
	//		    n ++;
	//	}
	//	GUILayout.Label( "There are "+ n +" SAPI voices installed in your OS | Press SPACE for start TEST");
	
	//	//Set a voice (if not using XML)
	////	voice.Voice = (tokens.Item (3)); // Comment this line if you use XML parser for choice voices, force a voice over the def one.

	//}



	string BuiltAsset = "";

	void Start(){
        //print (loadXMLStandalone ("fottiti.xml"));	
        sm = gameObject.GetComponent<StudyManager>();
		voice = new SpVoice();
        vp = gameObject.GetComponent<VideoPlayer>();
        //TextAsset txt = (TextAsset)Resources.Load("readme", typeof(TextAsset));
        //	TextAsset txt = (TextAsset)Resources.Load("builtIn.xml", typeof(XmlText));
        //	TextAsset textXML = (TextAsset)Resources.Load("builtIn", typeof(TextAsset));
        //	BuiltAsset = textXML.text;
        //TextAsset textAsset = (TextAsset) Resources.Load("builtIn.xml");  
        plane.SetActive(false);
	}
	

    IEnumerator startintro()
    {
        intro = false;
        plane.SetActive(true);
        voice.Volume = 100; // Volume (no xml)
        voice.Rate = 0;
        var x = loadXMLStandalone("readme.txt");
        voice.Speak(x, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        yield return new WaitForSeconds(10);
        vp.Play();
        
        
        yield return new WaitForSeconds(60);
        vp.Stop();
        sm.currentController = PositionManager.controllerEnum.sphere;
        sm.task = 3;
        count++;
        vp.url = "D:/AR/Assets/controller.mp4";
        intro1 = true;
    }
    IEnumerator startintro1()
    {
        intro1 = false;
        count++;
        voice.Volume = 100; // Volume (no xml)
        voice.Rate = 0;
        var x = loadXMLStandalone("readme1.txt");
        voice.Speak(x, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        sm.currentController = PositionManager.controllerEnum.controller;
        //text.text = "Please pick up the controller. This is the second interface that we are testing. With this interface you use the controller in the exact same way as you did the sphere previously."+
        //    "Play around with the interface until you are familiar with it. Then try to place the opaque object in front of you so that it overlaps with the blue transparent object."
        //    +"If you are close enough to the desired orientation, the blue object will turn green and disappear after a few seconds if you hold your object in place.";
        vp.Play();
        yield return new WaitForSeconds(30);

        vp.url = "D:/AR/Assets/controller_menu.mp4";
        sm.task = 3;
        intro2 = true;

    }
    IEnumerator startintro2()
    {
        intro2 = false;
        count++;
        voice.Volume = 100; // Volume (no xml)
        voice.Rate = 0;
        var x = loadXMLStandalone("readme3.txt");
        voice.Speak(x, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        sm.currentController = PositionManager.controllerEnum.controller_menu;
        vp.Play();
        //text.text = "The third interaction method also uses the controller. But here you can use the buttons. As long as you press any button of the controller the 3D object will follow it. If a rotation gets into a uncomfortable position just release the button, reposition the controller and rotate further."+
        //    "To scale you only have to swipe up or down on the touchpad. Swiping up makes the object bigger, swiping down smaller."+
        //    "Play around with the interface until you are familiar with it. Then try to place the opaque object in front of you so that it overlaps with the blue transparent object. If you are close enough to the desired orientation, the blue object will turn green and disappear after a few seconds if you hold your object in place.";
        yield return new WaitForSeconds(50);
        vp.Stop();
        vp.url = "D:/AR/Assets/controller1.mp4";
        sm.task = 3;
        intro3 = true;

    }
    IEnumerator startintro3()
    {
        intro3 = false;
        voice.Volume = 100; // Volume (no xml)
        voice.Rate = 0;
        var x = loadXMLStandalone("readme2.txt");
        voice.Speak(x, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        sm.currentController = PositionManager.controllerEnum.controller_arcball;
        vp.Play();
        //text.text = "The third interaction method also uses the controller. But here you can use the buttons. As long as you press any button of the controller the 3D object will follow it. If a rotation gets into a uncomfortable position just release the button, reposition the controller and rotate further."+
        //    "To scale you only have to swipe up or down on the touchpad. Swiping up makes the object bigger, swiping down smaller."+
        //    "Play around with the interface until you are familiar with it. Then try to place the opaque object in front of you so that it overlaps with the blue transparent object. If you are close enough to the desired orientation, the blue object will turn green and disappear after a few seconds if you hold your object in place.";
        yield return new WaitForSeconds(37);
        vp.Stop();

        sm.task = 3;
        yield return new WaitForSeconds(10);
        plane.SetActive(false);

    }
    public void controllerChange(PositionManager.controllerEnum e)
    {
        var x = "";
        if (e == PositionManager.controllerEnum.sphere)
        {
            x = loadXMLStandalone("sphere_controller.txt");
        }
        else if(e == PositionManager.controllerEnum.controller_arcball)
        {
            x = loadXMLStandalone("vive_controller.txt");
        }
        else if (e == PositionManager.controllerEnum.controller)
        {
            x = loadXMLStandalone("button_controller.txt");
        }
        else
        {
            x = loadXMLStandalone("menu_controller.txt");
        }
        
        voice.Speak(x, SpeechVoiceSpeakFlags.SVSFlagsAsync);
    }

    IEnumerator t1()
    {

        task1 = false;
        var x = loadXMLStandalone("task1.txt");
        voice.Speak(x, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        

        yield return new WaitForSeconds(35);
        sm.task = 1;

    }
    IEnumerator t2()
    {

        task2 = false;
        var x = loadXMLStandalone("task2.txt");
        voice.Speak(x, SpeechVoiceSpeakFlags.SVSFlagsAsync);


        yield return new WaitForSeconds(28);
        sm.task = 2;

    }

    // Update is called once per frame
    void Update()
	{
        if (intro)
        {
            StartCoroutine(startintro());
        }
        if(intro1 && sm.task == 0 )
        {
            StartCoroutine(startintro1());
            
        }

        if (intro2 && sm.task == 0)
        {
            StartCoroutine(startintro2());            
        }

        if (intro3 && sm.task == 0)
        {
            StartCoroutine(startintro3());
        }

        if (task1)
        {
            StartCoroutine(t1());
            
        }
        if (task2)
        {
            StartCoroutine(t2());
            
        }

        
		if (Input.GetKeyDown(KeyCode.P))
		{
			voice.Pause();
			
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			voice.Resume();
		}
		
		//TEST PER ANDROID
		/*	if (Input.GetTouch)
		{

			voice.Resume();
		}*/
		
		
	}
}


