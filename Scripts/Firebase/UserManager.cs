using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;

public class UserManager
{
    public FirebaseUser userData;

    public void SetUserData(FirebaseUser user)
    {
        userData = user;
        Debug.Log($"userName ::: {userData.DisplayName}");
    }
}