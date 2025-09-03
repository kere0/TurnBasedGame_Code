using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;

[FirestoreData]
public class SaveCreatureInfo
{
    [FirestoreProperty] public string ID { get; set; }
    [FirestoreProperty] public int Star { get; set; }
    [FirestoreProperty] public Rarity Rarity { get; set; }
    [FirestoreProperty] public bool isUsing { get; set; } = false;
}
[FirestoreData]
public class MyCreatureDataList
{
    [FirestoreProperty] public List<SaveCreatureInfo> CreatureData { get; set; }
}