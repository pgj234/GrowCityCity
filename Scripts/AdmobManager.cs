using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour {

    public bool isTestMode;

    void Start() {
        LoadBannerAd();
    }

    AdRequest GetAdRequest() {
        return new AdRequest.Builder().AddTestDevice("1167D6F9C4D16378").Build();
    }

    #region  배너 광고

    const string bannerTestID = "ca-app-pub-3940256099942544/6300978111";
    const string bannerID = "";
    BannerView bannerAd;

    void LoadBannerAd() {
        bannerAd = new BannerView(isTestMode ? bannerTestID : bannerID, AdSize.SmartBanner, AdPosition.Bottom);
        bannerAd.LoadAd(GetAdRequest());

    }

    //public void ToggleBannerAd(bool b) {
    //    if (b) {
    //        bannerAd.Show();
    //    }
    //    else {
    //        bannerAd.Hide();
    //    }
    //}

    #endregion
}