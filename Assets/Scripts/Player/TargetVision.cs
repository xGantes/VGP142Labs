using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.EnemyVision
{
    public class TargetVision : MonoBehaviour
    {
        public bool visible = true;

        private static List<TargetVision> targetList = new List<TargetVision>();

        private void Awake()
        {
            targetList.Add(this);
        }

        private void OnDestroy()
        {
            targetList.Remove(this);
        }

        public bool CanBeSeen()
        {
            return visible;
        }

        public static List<TargetVision> GetAll()
        {
            return targetList;
        }
    }

}