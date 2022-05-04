using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VGP142.EnemyVision
{

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

        [Header("Alert")]
        public float detectTime = 1f;
        public float alertedTime = 10f;

        [Header("Chase")]
        public float followTime = 10f;
        public bool dontReturn = false;

        [Header("Ref")]
        public Transform eye;
        public GameObject visionPrefab;
        public GameObject deathFxPrefab;

        public UnityAction<TargetVision, int> onSeeTarget; //As soon as seen (Patrol->Alert)  int:0=touch, 1=near, 2=far, 3:other
        public UnityAction<TargetVision, int> onDetectTarget; //detect_time seconds after seen (Alert->Chase)  int:0=touch, 1=near, 2=far
        public UnityAction<TargetVision> onTouchTarget;
        public UnityAction<Vector3> onAlert;
        public UnityAction onDeath;

        private EnemyPatrol enemy;

        private TargetVision seenCharacter = null;
        private VisionCone vision;

        private float detectTimer = 0f;
        private float visionTimer = 0f;
        private float waitTimer = 0f;

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
            if (visionPrefab)
            {
                GameObject vis = Instantiate(visionPrefab, GetEye(), Quaternion.identity);
                vis.transform.parent = transform;
                vision = vis.GetComponent<VisionCone>();
                vision.target = this;
                vision.vision_angle = visionAngle;
                vision.vision_range = visionRange;
                vision.vision_near_range = visionNearRange;
            }

            if (enemy != null)
                enemy.ChangeState(EnemyState.Patrol);
            seenCharacter = null;
        }

        void Update()
        {
            //In case using vision without enemy behavior
            if (enemy == null)
                DetectVisionTargetOnly();

            if (enemy == null || enemy.IsPaused())
                return;

            waitTimer -= Time.deltaTime;

            //While patroling, detect targets
            if (enemy.GetState() == EnemyState.Patrol)
            {
                DetectVisionTarget();

                if (groupDetect)
                    DetectOtherEnemies();
            }

            //When just seen the VisionTarget, enemy alerted
            if (enemy.GetState() == EnemyState.Alert)
            {
                TargetVision target_seen = CanSeeAnyVisionTarget();

                visionTimer += target_seen ? Time.deltaTime : -Time.deltaTime;

                if (target_seen != null && visionTimer < -0.5f)
                {
                    Alert(target_seen);

                    int distance = GetSeenDistance(target_seen.gameObject);
                    if (onSeeTarget != null)
                        onSeeTarget.Invoke(target_seen, distance);
                }

                if (target_seen != null)
                    enemy.SetAlertTarget(target_seen.transform.position);

                if (target_seen != null && visionTimer > detectTime)
                {
                    Chase(target_seen);

                    if (onDetectTarget != null)
                        onDetectTarget.Invoke(target_seen, 2);
                }

                if (target_seen != null && enemy.GetStateTimer() > 0.2f && CanSeeVisionTargetNear(target_seen))
                {
                    Chase(target_seen);

                    bool is_touch = CanTouchObject(target_seen.gameObject);
                    if (onDetectTarget != null)
                        onDetectTarget.Invoke(target_seen, is_touch ? 0 : 1);
                }

                if (enemy.HasReachedTarget() || enemy.GetStateTimer() > alertedTime)
                {
                    ResumeDefault();
                }

                if (groupDetect)
                    DetectOtherEnemies();
            }

            //If seen long enough (detect time), will go into a chase
            if (enemy.GetState() == EnemyState.Chase)
            {
                bool can_see_target = CanSeeVisionTarget(seenCharacter);

                visionTimer += can_see_target ? -Time.deltaTime : Time.deltaTime;
                visionTimer = Mathf.Max(visionTimer, 0f);

                if (enemy.GetStateTimer() > 0.5f)
                {
                    enemy.SetFollowTarget(can_see_target ? seenCharacter.gameObject : null);
                }

                if (visionTimer > followTime)
                {
                    ResumeDefault();
                }

                if (enemy.HasReachedTarget() && !can_see_target)
                    enemy.ChangeState(EnemyState.Confused);

                if (seenCharacter == null)
                    enemy.ChangeState(EnemyState.Confused);

                DetectTouchTarget();
            }

            //After the chase, if VisionTarget is unseen, enemy will be confused
            if (enemy.GetState() == EnemyState.Confused)
            {
                TargetVision target_seen = CanSeeAnyVisionTarget();
                if (target_seen != null && target_seen == seenCharacter)
                {
                    Chase(target_seen);

                    int distance = GetSeenDistance(target_seen.gameObject);
                    if (onDetectTarget != null)
                        onDetectTarget.Invoke(target_seen, distance);
                }

                if (target_seen != null && target_seen != seenCharacter)
                {
                    Alert(target_seen);

                    int distance = GetSeenDistance(target_seen.gameObject);
                    if (onSeeTarget != null)
                        onSeeTarget.Invoke(target_seen, distance);
                }

                if (!dontReturn && enemy.GetStateTimer() > alertedTime)
                {
                    ResumeDefault();
                }

                if (groupDetect)
                    DetectOtherEnemies();
            }

            if (enemy.GetState() == EnemyState.Wait)
            {
                if (enemy.GetStateTimer() > 0.5f && waitTimer < 0f)
                {
                    ResumeDefault();
                }
            }
        }

        //In case using vision without enemy behavior, detect without changing state
        private void DetectVisionTargetOnly()
        {
            detectTimer += Time.deltaTime;
            TargetVision target = CanSeeAnyVisionTarget();
            if (target != null && detectTimer > detectTime)
            {
                int dist = GetSeenDistance(target.gameObject);
                onSeeTarget.Invoke(target, dist);
                visionTimer = 0f;
            }
        }

        //Look for possible seen targets
        private void DetectVisionTarget()
        {
            if (waitTimer > 0f)
                return;

            //Detect character
            foreach (TargetVision character in TargetVision.GetAll())
            {
                if (character == seenCharacter)
                    continue;

                if (CanSeeVisionTarget(character))
                {
                    Alert(character);

                    int distance = GetSeenDistance(character.gameObject);
                    if (onSeeTarget != null)
                        onSeeTarget.Invoke(character, distance);
                }
            }
        }

        //Check if the enemy is in touch range of a target
        private void DetectTouchTarget()
        {
            if (waitTimer > 0f)
                return;

            //Detect character touch
            foreach (TargetVision character in TargetVision.GetAll())
            {
                if (CanTouchObject(character.gameObject))
                {
                    if (onTouchTarget != null)
                        onTouchTarget.Invoke(character);
                }
            }
        }

        //Check if allies running
        private void DetectOtherEnemies()
        {
            if (waitTimer > 0f)
                return;

            foreach (EnemyPatrol oenemy in EnemyPatrol.GetAll())
            {
                if (oenemy != enemy && oenemy.GetState() == EnemyState.Chase)
                {
                    if (oenemy.followTarget != null && CanSeeObject(oenemy.gameObject, visionRange, visionAngle))
                    {
                        TargetVision target = oenemy.followTarget.GetComponent<TargetVision>();
                        Chase(target);

                        if (target != null && onDetectTarget != null)
                            onDetectTarget.Invoke(target, 3);
                    }
                }
            }
        }

        //Can see any vision target
        public TargetVision CanSeeAnyVisionTarget()
        {
            foreach (TargetVision character in TargetVision.GetAll())
            {
                if (CanSeeVisionTarget(character))
                {
                    return character;
                }
            }
            return null;
        }

        //Can the enemy see a vision target?
        public bool CanSeeVisionTarget(TargetVision target)
        {
            return target != null && target.CanBeSeen()
                && (CanSeeObject(target.gameObject, visionRange, visionAngle) || CanTouchObject(target.gameObject));
        }

        public bool CanSeeVisionTargetNear(TargetVision target)
        {
            return target != null && target.CanBeSeen()
                && (CanSeeObject(target.gameObject, visionNearRange, visionAngle) || CanTouchObject(target.gameObject));
        }

        //Can the enemy see an object ?
        public bool CanSeeObject(GameObject obj, float see_range, float see_angle)
        {
            Vector3 forward = transform.forward;
            Vector3 dir = obj.transform.position - GetEye();
            Vector3 dir_touch = dir; //Preserve Y for touch
            dir.y = 0f; //Remove Y for cone vision range

            float vis_range = see_range;
            float vis_angle = see_angle;
            float losangle = Vector3.Angle(forward, dir);
            float losheight = obj.transform.position.y - GetEye().y;
            bool can_see_cone = losangle < vis_angle / 2f && dir.magnitude < vis_range && losheight < visionHeightAbove && losheight > -visionHeightBelow;
            bool can_see_touch = dir_touch.magnitude < touchRange;
            if (obj.activeSelf && (can_see_cone || can_see_touch)) //In range and in angle
            {
                RaycastHit hit;
                bool raycast = Physics.Raycast(new Ray(GetEye(), dir.normalized), out hit, dir.magnitude, visionMask.value);
                if (!raycast)
                    return true; //No obstacles in the way (in case character not in layer)
                if (raycast && (hit.collider.gameObject == obj || hit.collider.transform.IsChildOf(obj.transform))) //See character
                    return true; //The only obstacles is the character
            }
            return false;
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

        //Return seen distance of target: 0:touch,  1:near,  2:far,  3:other
        public int GetSeenDistance(GameObject target)
        {
            bool is_near = CanSeeObject(target, visionNearRange, visionAngle);
            bool is_touch = CanTouchObject(target);
            int distance = is_touch ? 0 : (is_near ? 1 : 2);
            return distance;
        }

        //Call this function from another script to manually alert of the target presense
        public void Alert(TargetVision target)
        {
            if (target != null)
            {
                Alert(target.transform.position);
            }
        }

        //Alert with a position instead of object (such as noise)
        public void Alert(Vector3 target)
        {
            if (enemy != null)
                enemy.Alert(target);
            visionTimer = 0f;

            if (onAlert != null)
                onAlert.Invoke(target);
        }

        //Call this function from another script to manually start chasing the target
        public void Chase(TargetVision target)
        {
            if (target != null)
            {
                seenCharacter = target;
                visionTimer = 0f;
                if (enemy != null)
                    enemy.Follow(seenCharacter.gameObject);
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
            seenCharacter = null;
            waitTimer = 0f;
            if (enemy != null)
            {
                if (dontReturn)
                    enemy.ChangeState(EnemyState.Confused);
                else
                    enemy.ChangeState(EnemyState.Patrol);
            }
        }

        //Stop detecting player for X seconds
        public void PauseVisionFor(float time)
        {
            waitTimer = time;
        }

        //Do nothing for X seconds
        public void WaitFor(float time)
        {
            waitTimer = time;
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
            if (vision)
                vision.gameObject.SetActive(false);

            if (onDeath != null)
                onDeath.Invoke();
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
