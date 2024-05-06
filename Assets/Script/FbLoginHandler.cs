using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FbLoginHandler : MonoBehaviour
{
    public static FbLoginHandler inst;
    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
            DontDestroyOnLoad(inst);
        }
        else
        {
            if (inst != this)
            {
                DestroyImmediate(this.gameObject);
            }
        }
    }
    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    public string userName = "";
    public string userId = "";
    public void LoginUsingFacebook()
    {
        var perms = new List<string>() { "public_profile", "email" };
//#if UNITY_ANDROID
//        FB.setLoginBehavior("web_only");
//#endif
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }
   
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            Debug.LogError("FB login success");
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            //Debug.Log(aToken.UserId);
            //result.RawResult.byte
            // Print current access token's granted permissions
            ////Debug.Log(result.ResultDictionary["email"].ToString());
            ////Debug.Log(result.ResultDictionary["id"].ToString());
            ////Debug.Log(result.ResultDictionary["name"].ToString());
            //var profile = FB.Mobile.CurrentProfile();
            //if (profile != null)
            //{
            //    this.userName = profile.Name;
            //    this.userId = profile.UserID;
            //    this.userEmail = profile.Email;
            //    this.profileImageUrl = profile.ImageURL;
            //    this.userBirthday = profile.Birthday.ToString();
            //    this.userAgeRange = profile.AgeRange;
            //    this.userFriendIDs = profile.FriendIDs;
            //    this.userGender = profile.Gender;
            //    this.userLink = profile.LinkURL;
            //    this.userHometown = profile.Hometown;
            //    this.userLocation = profile.Location;
            //}
            foreach (string perm in aToken.Permissions)
            {
                //Debug.Log(perm);
            }
            DealWithFbMenus(FB.IsLoggedIn);
        }
        else
        {
            Debug.Log("User cancelled login");
           // NotificationHandler.Instance.ShowNotification("Facebook login failed!");
        }
    }

    void DealWithFbMenus(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            //Debug.LogError("DealWithFbMenus......User logged in with fb..");
            FB.API("/me?fields=id,first_name,email", HttpMethod.GET, OnFacebookUserDataCallback);  
            //FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
        }
        else
        {
            //Debug.LogError("DealWithFbMenus......User Not logged in..");
        }
    }

    private void OnFacebookUserDataCallback(IGraphResult result)
    {
        if (string.IsNullOrEmpty(result.Error))
        {
            IDictionary<string, object> userData = result.ResultDictionary;
            if (userData.TryGetValue("id",out object idObj))
            {
                userId = idObj.ToString();
            }
            if (userData.TryGetValue("first_name", out object nameObj))
            {
                userName = nameObj.ToString();
            }

            // Check if the email key is present in the dictionary
            if (userData.TryGetValue("email", out object emailObj))
            {
                string email = emailObj.ToString();
                //Debug.Log("Facebook email: " + email);
            }
            else
            {
                //Debug.Log("Email not available from Facebook data.");
            }
            CheckForAllComplete();
        }
        else
        {
            //Debug.Log("Failed to get Facebook user data: " + result.Error);
        }
    }

    //private void EmailCallback(IGraphResult result)
    //{
    //    if (string.IsNullOrEmpty(result.Error))
    //    {
    //        string email = result.ResultDictionary["email"].ToString();
    //        //Debug.Log("Facebook email: " + email);
    //    }
    //    else
    //    {
    //        //Debug.Log("Failed to get Facebook user data: " + result.Error);
    //    }
    //}
    //void DisplayUsername(IResult result)
    //{
    //    if (result.Error == null)
    //    {
    //        string name = "" + result.ResultDictionary["first_name"];
    //        //Debug.LogError(name);
    //        //string email = "" + result.ResultDictionary["user_email"];
    //        ////Debug.LogError(email);

    //        string lastname = "" + result.ResultDictionary["last_name"];

    //        DataHandler.Instance.UserName = name;
    //        //Debug.LogError(lastname);
                
    //        //Debug.Log("" + name);
    //    }

    //    else
    //    {
    //        //Debug.Log(result.Error);
    //    }
    //}
    //void GetFacebookInfo(IResult result)
    //{
    //    if (result.Error == null)
    //    {
    //        string name = "" + result.ResultDictionary["name"];
    //        string lastname = "" + result.ResultDictionary["email"];

    //        DataHandler.Instance.UserName = name;
    //        //Debug.LogError("Name : "+name);
    //        //Debug.LogError("Email : "+lastname);
    //    }

    //    else
    //    {
    //        //Debug.Log(result.Error);
    //    }
    //}
    //void DisplayProfilePic(IGraphResult result)
    //{
    //    if (result.Texture != null)
    //    {
    //        //Debug.Log("Profile Pic" + DataHandler.Instance.playerProfilePath);
    //        if (Directory.Exists(DataHandler.Instance.playerProfilePath) == false)
    //        {
    //            Directory.CreateDirectory(DataHandler.Instance.playerProfilePath);
    //        }
    //        byte[] bytes = result.Texture.EncodeToJPG();

    //        //FB_useerDp.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
    //        File.WriteAllBytes(DataHandler.Instance.playerProfilePath + "/profile.png", bytes);
    //    }

    //    else
    //    {
    //        //Debug.Log(result.Error);
    //    }
    //}

    void CheckForAllComplete()
    {
        //if (string.IsNullOrEmpty(userName))
        //{
        //    return;
        //}
        //if (string.IsNullOrEmpty(userId))
        //{
        //    return;
        //}
        //Sender.CreateUserSendData userDetail = new Sender.CreateUserSendData();
        //userDetail.auth_type = "facebook";
        //userDetail.name = userName;
        //userDetail.email = userId;
        //userDetail.coins = DataHandler.Instance.Coins;
      
        //if (DataHandler.Instance.NoAds == 1)
        //{
        //    userDetail.adpurchased = true;
        //}
        //else
        //{
        //    userDetail.adpurchased = false;
        //}

        //userDetail.username = userId;
        ////Debug.Log("OnSuccess");
        //if (string.IsNullOrEmpty(DataHandler.Instance.LoginToken) || DataHandler.Instance.LoginStatus == 0)
        //{
        //    LogInHandler.inst.RegisterNewUserGoogle(userDetail);
        //}
        //else
        //{
        //    LogInHandler.inst.ConnectGoogleUserWithGuest(userDetail);
        //}
    }
    Coroutine coSignOut;
    public void SignOutFb()
    {
        //if (DataHandler.Instance.userDetailData != null)
        //{
        //    if (DataHandler.Instance.userDetailData.auth_type == "facebook")
        //    {
        //        if (coSignOut == null)
        //        {
        //            coSignOut = StartCoroutine(FBLogout());
        //        }
        //    }
        //}
    }

    IEnumerator FBLogout()
    {
        FB.LogOut();

        while (FB.IsLoggedIn)
        {
            print("Logging Out");

            yield return null;
        }

        print("Logout Successful");
        coSignOut = null;
        //FB_useerDp.sprite = null;

        //FB_userName.text = "";
    }
}


