using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuMessage : MonoBehaviour
{
  public static string Message;
  public TextMeshProUGUI MessageLabel;

  private void Start()
  {
    if (Message != null && MessageLabel != null) { 
      MessageLabel.text = Message;
      Message = null;
    }
  }
}
