using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
   private static Managers Instance;
   private static bool init = false;

   private PoolManager poolManager = new PoolManager();
   private CSVLoader csvLoader = new CSVLoader();
   private HPBarManager hpBarManager = new HPBarManager();
   private FirestoreManager firestoreManager = new FirestoreManager();
   private UserManager userManager = new UserManager();
   private SaveLoad_Firebase saveLoad = new SaveLoad_Firebase();
   public static PoolManager PoolManager { get { return Instance.poolManager; } }
   public static CSVLoader CSVLoader { get { return Instance.csvLoader; } }
   public static HPBarManager HPBarManager { get { return Instance.hpBarManager; } }
   public static FirestoreManager FirestoreManager { get { return Instance.firestoreManager; } }
   public static UserManager UserManager { get { return Instance.userManager; } }
   public static SaveLoad_Firebase SaveLoadFirebase { get { return Instance.saveLoad; } }

   private void Awake()
   {
      if (init == false)
      {
         init = true;
         GameObject go = GameObject.Find("@Managers");
         if (go == null)
         {
            go = new GameObject("@Managers");
            go.AddComponent<Managers>();
         }
         DontDestroyOnLoad(go);
         Instance = go.GetComponent<Managers>();
      }
   }
   public void Start()
   {
      FirestoreManager.Init();
      CSVLoader.LoadCSV<CreatureData>("CreatureData");
   }
}
