using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.EnemyVision
{

    public class TargetVision : MonoBehaviour
    {
        public bool visible = true;

        private static List<TargetVision> target_list = new List<TargetVision>();

        private void Awake()
        {
            target_list.Add(this);
        }

        private void OnDestroy()
        {
            target_list.Remove(this);
        }

        public bool CanBeSeen()
        {
            return visible;
        }

        public static List<TargetVision> GetAll()
        {
            return target_list;
        }
    }

}