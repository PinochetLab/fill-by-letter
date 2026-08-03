using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Money
{
    public class RewardSpawner : MonoBehaviour
    {
        [SerializeField] private List<Reward> rewards;

        private Reward TakeReward()
        {
            var reward = rewards.Find(c => !c.gameObject.activeSelf);

            if (reward == null)
            {
                throw new NotImplementedException("Not enough rewards");
            }
            
            reward.gameObject.SetActive(true);
            return reward;
        }
        
        private void ReleaseReward(Reward reward)
        {
            reward.gameObject.SetActive(false);
        }

        public void SpawnReward(Vector3 position, Vector2 coinSize, int money)
        {
            var reward = TakeReward();
            
            reward.SetUp(money, coinSize);
            
            reward.Move(position).OnComplete(() => ReleaseReward(reward));
        }
    }
}