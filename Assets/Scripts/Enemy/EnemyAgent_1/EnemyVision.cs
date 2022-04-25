using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VGP142.EnemyVision
{
    /// <summary>
    /// Manages enemy detection of the player, will also spawn a VisionCone and change the state of Enemy.cs based on what is seen
    /// </summary>

    public class EnemyVision : MonoBehaviour
    {
        [Header("Detection")]
        public float visionAngle = 30f;
        public float visionRange = 10f;
        public float visionNearRange = 5f;
        public float visionHeightAbove = 1f; //How far above can they detect
        public float visionHeightBelow = 10f; //How far below can they detect
        public float touchRange = 1f;
        public LayerMask visionMask = ~(0);
        public bool groupDetect = false; //Group detect will make an enemy follow another enemy chasing even if didnt see player

        [Header("Ref")]
        public Transform eye;
        public GameObject vision_prefab;
        public GameObject death_fx_prefab;

        public UnityAction<TargetVision, int> onSeeTarget; //As soon as seen (Patrol->Alert)  int:0=touch, 1=near, 2=far, 3:other
        public UnityAction<TargetVision, int> onDetectTarget; //detect_time seconds after seen (Alert->Chase)  int:0=touch, 1=near, 2=far
        public UnityAction<TargetVision> onTouchTarget;
        public UnityAction<Vector3> onAlert;

        private EnemyPatrol enemy;

        private TargetVision seen_character = null;
        private VisionCone vision;

        private float detect_timer = 0f;
        private float vision_timer = 0f;
        private float wait_timer = 0f;

        private static List<EnemyVision> enemy_list = new List<EnemyVision>();

        private void Awake()
        {
            enemy_list.Add(this);
            enemy = GetComponent<EnemyPatrol>();
            if (enemy != null)
                enemy.onDeath += OnDeath;
        }

        private void OnDestroy()
        {
            enemy_list.Remove(this);
        }

        void Start()
        {
            if (vision_prefab)
            {
                GameObject vis = Instantiate(vision_prefab, GetEye(), Quaternion.identity);
                vis.transform.parent = transform;
                vision = vis.GetComponent<VisionCone>();
                vision.target = this;
                vision.visionAngle = visionAngle;
                vision.visionRange = visionRange;
                vision.visionNearRange = visionNearRange;
            }

            if (enemy != null)
                enemy.ChangeState(EnemyState.Patrol);
            seen_character = null;
        }

        void Update()
        {

            if (enemy == null || enemy.IsPaused())
                return;

            wait_timer -= Time.deltaTime;

            //While patroling, detect targets
            if (enemy.GetState() == EnemyState.Patrol)
            {
                DetectVisionTarget();

                if (groupDetect)
                    DetectOtherEnemies();
                
            }

            if (enemy.GetState() == EnemyState.Wait)
            {
                if (enemy.GetStateTimer() > 0.5f && wait_timer < 0f)
                {
                    ResumeDefault();
                }
            }
        }

        //Look for possible seen targets
        private void DetectVisionTarget()
        {
            if (wait_timer > 0f)
                return;
        }

        //Check if allies running
        private void DetectOtherEnemies()
        {
            if (wait_timer > 0f)
                return;


            foreach (EnemyPatrol oenemy in EnemyPatrol.GetAll())
            {
                if (oenemy != enemy)
                {
                    if (oenemy.follow_target != null)
                    {
                        TargetVision target = oenemy.follow_target.GetComponent<TargetVision>();
                        Chase(target);

                        if (target != null && onDetectTarget != null)
                            onDetectTarget.Invoke(target, 3);

                    }
                }
            }
        }

        //Is the enemy right next to the object ?
        public bool CanTouchObject(GameObject obj)
        {
            Vector3 dir = obj.transform.position - transform.position;
            if (dir.magnitude < touchRange) //In range and in angle
            {
                return true;
            }
            return false;
        }

        //Call this function from another script to manually start chasing the target
        public void Chase(TargetVision target)
        {
            if (target != null)
            {
                seen_character = target;
                vision_timer = 0f;
                if (enemy != null)
                    enemy.Follow(seen_character.gameObject);
            }
        }

        //Call this function from another script to stop chasing, may not work if the target is still in vision because it will just start chasing again
        public void Stop()
        {
            ResumeDefault();
            WaitFor(2f);
        }

        public void ResumeDefault()
        {
            seen_character = null;
            wait_timer = 0f;
            if (enemy != null)
            {
                enemy.ChangeState(EnemyState.Patrol);
            }
        }

        //Stop detecting player for X seconds
        public void PauseVisionFor(float time)
        {
            wait_timer = time;
        }

        //Do nothing for X seconds
        public void WaitFor(float time)
        {
            wait_timer = time;
            if (enemy != null)
            {
                enemy.ChangeState(EnemyState.Wait);
                enemy.StopMove();
            }
        }

        public Vector3 GetEye()
        {
            return eye ? eye.position : transform.position;
        }

        public EnemyPatrol GetEnemy()
        {
            return enemy;
        }

        private void OnDeath()
        {
            
        }

        public static EnemyVision GetNearest(Vector3 pos, float range = 999f)
        {
            float min_dist = range;
            EnemyVision nearest = null;
            foreach (EnemyVision enemy in enemy_list)
            {
                float dist = (enemy.transform.position - pos).magnitude;
                if (dist < min_dist)
                {
                    min_dist = dist;
                    nearest = enemy;
                }
            }
            return nearest;
        }

        public static List<EnemyVision> GetAllInRange(Vector3 pos, float range)
        {
            List<EnemyVision> range_list = new List<EnemyVision>();
            foreach (EnemyVision enemy in enemy_list)
            {
                float dist = (enemy.transform.position - pos).magnitude;
                if (dist < range)
                {
                    range_list.Add(enemy);
                }
            }
            return range_list;
        }

        public static List<EnemyVision> GetAll()
        {
            return enemy_list;
        }
    }

}
