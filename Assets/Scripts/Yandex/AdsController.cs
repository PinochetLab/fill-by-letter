using System;
using UnityEngine;
using YG;

namespace Yandex
{
    public class AdsController : MonoBehaviour
    {
        public void ShowRewarded(string id, Action callback)
        {
            YG2.RewardedAdvShow(id, callback);
        }

        public void ShowInterstitial()
        {
            YG2.InterstitialAdvShow();
        }
    }
}