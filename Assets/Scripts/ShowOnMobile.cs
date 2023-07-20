using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnMobile : MonoBehaviour
{
  private void Awake()
  {
    #if (!UNITY_ANDROID && !UNITY_IOS)
      Destroy(gameObject);
    #endif
  }
}
