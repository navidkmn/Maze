using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class menuscript : MonoBehaviour {


 public Button startText;
 public Button exitText;
 public InputField myField;
 public static string userName;
 // Use this for initialization
 void Start () {

  startText = startText.GetComponent<Button> ();
  exitText = exitText.GetComponent<Button> ();
  myField = myField.GetComponent<InputField> ();

 }


 public void StartLevel(){
  userName= myField.text;
  Application.LoadLevel (1);
  }

 public void ExitGame(){
  Application.Quit ();
 
 }
}