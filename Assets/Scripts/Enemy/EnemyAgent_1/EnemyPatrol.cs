using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace VGP142.EnemyVision
{
    public enum EnemyState
    {
        None = 0,
        Patrol = 2,
        Alert = 5,
        Chase = 10,
        Confused = 15,
        Wait = 20,
    }

    public enum EnemyPatrolType
    {
        Loop = 2,
    }

    [RequireComponent(typeof(Rigidbody))]
    public class EnemyPatrol : MonoBehaviour
    {
        public float moveSpeed = 2f;
        public float runSpeed = 4f;
        public float rotateSpeed = 120f;
        public float fallSpeed = 5f;
        public LayerMask obstacleMask = ~(0);
        public bool usePathfind = false;

        //Enemy states
        [Header("State")]
        public EnemyState state = EnemyState.Patrol;

        //Enemy patrol
        [Header("Patrol")]
        public EnemyPatrolType type;
        public float waitTime = 1f;
        public GameObject[] patrolPath;

        //Enemy alert
        [Header("Alert")]
        public float alertWaitTime = 3f;
        public float alertWalkTime = 10f;

        //Enemy follow
        [Header("Follow")]
        public GameObject followTarget;
        public float memoryDuration = 4f;

        //enemy death
        public UnityAction onDeath;

        //components
        private Rigidbody rigid;
        private NavMeshAgent navAgent;

        //agents position
        private Vector3 startPos;
        private Vector3 moveVect;
        private Vector3 faceVect;
        private float rotateVal;
        private float stateTimer = 0f;
        private bool paused = false;

        //agents alert function
        private Vector3 moveTarget;
        private Vector3 alertTarget;
        private float   currentSpeed = 1f;
        private Vector3 currentMove;
        private Vector3 currentRotTarget;
        private float currentRotMult = 1f;
        private bool waiting = false;
        private float waitTimer = 0f;

        //agents path
        private int currentPath = 0;
        private bool pathRewind = false;
        private bool usingNavmesh = false;

        //agents return path
        private Vector3 lastSeenPos;
        private GameObject lastTarget;
        private float memoryTimer = 0f;

        private List<Vector3> pathList = new List<Vector3>();
        private static List<EnemyPatrol> enemyList = new List<EnemyPatrol>();

        private void Awake()
        {
            enemyList.Add(this);

            rigid = GetComponent<Rigidbody>();
            navAgent = GetComponent<NavMeshAgent>();

            moveVect = Vector3.zero;
            startPos = transform.position;
            moveTarget = transform.position;
            currentRotTarget = transform.position + transform.forward;
            alertTarget = followTarget ? followTarget.transform.position : transform.position;
            lastSeenPos = transform.position;
            currentSpeed = moveSpeed;
            rotateVal = 0f;

            foreach (GameObject patrol in patrolPath)
            {
                if (patrol)
                    pathList.Add(patrol.transform.position);
            }

            currentPath = 0;
            if (pathList.Count >= 2)
                currentPath = 1; //Dont start at start pos
        }

        private void OnDestroy()
        {
            enemyList.Remove(this);
        }

        private void FixedUpdate()
        {
            if (paused)
                return;

            bool fronted = CheckFronted(transform.forward);
            bool grounded = CheckGrounded(Vector3.down);

            Vector3 dist_vect = (moveTarget - transform.position);
            moveVect = dist_vect.normalized * currentSpeed * Mathf.Min(dist_vect.magnitude, 1f);

            if (usePathfind && navAgent && usingNavmesh && dist_vect.magnitude > 1f)
            {
                navAgent.enabled = true;
                navAgent.speed = currentSpeed;
                navAgent.SetDestination(moveTarget);
                rigid.velocity = Vector3.zero;
            }
            else
            {
                if (fronted)
                    moveVect = Vector3.zero;
                if (!grounded)
                    moveVect += Vector3.down * fallSpeed;

                if (navAgent && navAgent.enabled)
                    navAgent.enabled = false;

                currentMove = Vector3.MoveTowards(currentMove, moveVect, moveSpeed * 10f * Time.fixedDeltaTime);
                rigid.velocity = currentMove;
            }
        }

        private void Update()
        {
            if (paused)
                return;

            stateTimer += Time.deltaTime;
            waitTimer += Time.deltaTime;

            if (state == EnemyState.Alert)
            {
                UpdateAlert();
            }

            if (state == EnemyState.Patrol)
            {
                UpdatePatrol();
            }

            if (state == EnemyState.Chase)
            {
                UpdateFollow();
            }

            if (state == EnemyState.Confused)
            {
                UpdateConfused();
            }

            //Manual Rotation
            bool controlled_by_agent = usePathfind && navAgent && navAgent.enabled && usingNavmesh && navAgent.hasPath;
            rotateVal = 0f;

            if (!controlled_by_agent && state != EnemyState.None && state != EnemyState.Wait)
            {
                Vector3 dir = currentRotTarget - transform.position;
                dir.y = 0f;
                if (dir.magnitude > 0.1f)
                {
                    Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);
                    Quaternion reachedRotation = Quaternion.RotateTowards(transform.rotation, target, rotateSpeed * currentRotMult * Time.deltaTime);
                    rotateVal = Quaternion.Angle(transform.rotation, target);
                    faceVect = dir.normalized;
                    transform.rotation = reachedRotation;
                }
            }
        }

        private void UpdateAlert()
        {
            if (stateTimer < alertWaitTime)
            {
                FaceToward(alertTarget);
            }
            else if (stateTimer < alertWaitTime + alertWalkTime)
            {
                MoveTo(alertTarget, moveSpeed);
            }
        }

        private void UpdateConfused()
        {
            
            if (waitTimer > alertWaitTime)
            {
                waitTimer = 0f;
                waiting = false;
                alertTarget = GetRandomLookTarget();
                MoveTo(alertTarget);
            }
        }

        private void UpdatePatrol()
        {
            float dist = (transform.position - startPos).magnitude;
            bool is_far = dist > 0.5f;

            //Facing only
            if (!waiting)
            {
                if (is_far)
                {
                    //Return to starting pos
                    MoveTo(startPos, moveSpeed);
                    FaceToward(startPos);
                }
                else
                {
                    //Rotate only
                    Vector3 targ = pathList[currentPath];
                    FaceToward(targ);
                    CheckIfFacingReachedTarget(targ);
                }
            }

            //Regular patrol
            if (!waiting)
            {
                //Move following path
                Vector3 targ = pathList[currentPath];
                MoveTo(targ, moveSpeed);
                FaceToward(GetNextTarget());

                //Check if reached target
                Vector3 dist_vect = (targ - transform.position);
                dist_vect.y = 0f;
                if (dist_vect.magnitude < 0.1f)
                {
                    waiting = true;
                    waitTimer = 0f;
                }

                //Check if obstacle ahead
                bool fronted = CheckFronted(dist_vect.normalized);
                if (fronted && waitTimer > 2f)
                {
                    waitTimer = 0f;
                }
            }

            //Waiting
            if (waiting)
            {
                //Wait a bit
                if (waitTimer > waitTime)
                {
                    GoToNextPath();
                    waiting = false;
                    waitTimer = 0f;
                }
            }
        }

        private void UpdateFollow()
        {
            Vector3 targ = followTarget ? followTarget.transform.position : lastSeenPos;

            //Use memory if no more target
            if (followTarget == null && lastTarget != null && memoryDuration > 0.1f)
            {
                memoryTimer += Time.deltaTime;
                if (memoryTimer < memoryDuration)
                {
                    lastSeenPos = lastTarget.transform.position;
                    targ = lastSeenPos;
                }
            }

            //Move to target
            MoveTo(targ, runSpeed);
            FaceToward(GetNextTarget(), 2f);

            if (followTarget != null)
            {
                lastTarget = followTarget;
                lastSeenPos = followTarget.transform.position;
                memoryTimer = 0f;
            }
        }

        //---- Patrol -----

        private void GoToNextPath()
        {
            if (type == EnemyPatrolType.Loop)
            {
                currentPath = (currentPath + 1) % pathList.Count;
                currentPath = Mathf.Clamp(currentPath, 0, pathList.Count - 1);
            }
            else
            {
                if (currentPath <= 0 || currentPath >= pathList.Count - 1)
                    pathRewind = !pathRewind;
                currentPath += pathRewind ? -1 : 1;
                currentPath = Mathf.Clamp(currentPath, 0, pathList.Count - 1);
            }
        }

        //---- Chase -----

        public void SetAlertTarget(Vector3 pos)
        {
            alertTarget = pos;
        }

        public void SetFollowTarget(GameObject atarget)
        {
            followTarget = atarget;
            if (followTarget != null)
            {
                lastSeenPos = followTarget.transform.position;
                memoryTimer = 0f;
            }
        }

        //---- Actions -----

        public void Alert(Vector3 pos)
        {
            if (state != EnemyState.Chase)
            {
                ChangeState(EnemyState.Alert);
                SetAlertTarget(pos);
                StopMove();
            }
        }

        public void Follow(GameObject target)
        {
            ChangeState(EnemyState.Chase);
            SetFollowTarget(target);
            usingNavmesh = true;
        }

        public void MoveTo(Vector3 pos, float speed = 1f)
        {
            moveTarget = pos;
            currentSpeed = speed;
            usingNavmesh = true;
        }

        public void FaceToward(Vector3 pos, float speed_mult = 1f)
        {
            currentRotTarget = pos;
            currentRotMult = speed_mult;
        }

        public void StopMove()
        {
            usingNavmesh = false;
            moveTarget = rigid.position;
            currentMove = Vector3.zero;
            rigid.velocity = Vector3.zero;
            if (navAgent && navAgent.enabled)
                navAgent.ResetPath(); //Cancel previous path
        }

        public void Kill()
        {
            if (onDeath != null)
                onDeath.Invoke();

            Destroy(gameObject);
        }

        public void ChangeState(EnemyState state)
        {
            this.state = state;
            stateTimer = 0f;
            waitTimer = 0f;
            waiting = false;
        }

        public void Pause()
        {
            paused = true;
        }

        public void UnPause()
        {
            paused = false;
        }

        //---- Check state -----

        public bool CheckFronted(Vector3 dir)
        {
            Vector3 origin = transform.position + Vector3.up * 1f;
            RaycastHit hit;
            bool success = Physics.Raycast(new Ray(origin, dir.normalized), out hit, dir.magnitude, obstacleMask.value);
            return success && (followTarget == null || !hit.collider.transform.IsChildOf(followTarget.transform));
        }

        public bool CheckGrounded(Vector3 dir)
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            RaycastHit hit;
            return Physics.Raycast(new Ray(origin, dir.normalized), out hit, dir.magnitude, obstacleMask.value);
        }

        private void CheckIfFacingReachedTarget(Vector3 targ)
        {
            //Check if reached target
            Vector3 dist_vect = (targ - transform.position);
            dist_vect.y = 0f;
            float dot = Vector3.Dot(transform.forward, dist_vect.normalized);
            if (dot > 0.99f)
            {
                waiting = true;
                waitTimer = 0f;
            }
        }

        //---- Getters ------

        public bool HasReachedTarget()
        {
            Vector3 targ = followTarget ? followTarget.transform.position : lastSeenPos;
            if (state == EnemyState.Alert)
                targ = alertTarget;
            return (targ - transform.position).magnitude < 0.5f;
        }

        private Vector3 GetRandomLookTarget()
        {
            Vector3 center = transform.position;
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            return center + offset;
        }

        public EnemyState GetState()
        {
            return state;
        }

        public float GetStateTimer()
        {
            return stateTimer;
        }

        public Vector3 GetMove()
        {
            return moveVect;
        }

        public Vector3 GetFacing()
        {
            return faceVect;
        }

        public float GetRotationVelocity()
        {
            return rotateVal;
        }

        public bool IsRunning()
        {
            return state == EnemyState.Chase;
        }

        public Vector3 GetNextTarget()
        {
            if (usePathfind && navAgent && navAgent.enabled && usingNavmesh && navAgent.hasPath)
                return navAgent.nextPosition;
            return moveTarget;
        }

        public bool IsPaused()
        {
            return paused;
        }

        public static EnemyPatrol GetNearest(Vector3 pos, float range = 999f)
        {
            float min_dist = range;
            EnemyPatrol nearest = null;
            foreach (EnemyPatrol enemy in enemyList)
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
        
        public static List<EnemyPatrol> GetAllInRange(Vector3 pos, float range)
        {
            List<EnemyPatrol> range_list = new List<EnemyPatrol>();
            foreach (EnemyPatrol enemy in enemyList)
            {
                float dist = (enemy.transform.position - pos).magnitude;
                if (dist < range)
                {
                    range_list.Add(enemy);
                }
            }
            return range_list;
        }

        public static List<EnemyPatrol> GetAll()
        {
            return enemyList;
        }

        //----- Debug Gizmos -------

        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 prev_pos = transform.position;

            foreach (GameObject patrol in patrolPath)
            {
                if (patrol)
                {
                    Gizmos.DrawLine(prev_pos, patrol.transform.position);
                    prev_pos = patrol.transform.position;
                }
            }

            if (type == EnemyPatrolType.Loop)
            Gizmos.DrawLine(prev_pos, transform.position);
            

            foreach (GameObject patrol in patrolPath)
            {
                if (patrol)
                {
                    Gizmos.DrawLine(transform.position, patrol.transform.position);
                }
            }
        }
    }

}