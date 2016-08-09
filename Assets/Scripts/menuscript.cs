using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class menuscript : MonoBehaviour {

    public Button startText;
    public Button exitText;
    public InputField myField;
    public InputField myHost;
    public static string userName;
    public static string host;

 // Use this for initialization
 void Start () {

    startText = startText.GetComponent<Button> ();
    exitText = exitText.GetComponent<Button> ();
    myField = myField.GetComponent<InputField> ();
    myHost = myHost.GetComponent<InputField>();
 }

 public void StartLevel(){

    userName= myField.text;
    host = myHost.text;
    Application.LoadLevel (1);
  }

 public void ExitGame(){
    
     Application.Quit ();
 }

}