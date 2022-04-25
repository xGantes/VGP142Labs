using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.EnemyVision
{

    public class VisionCone : MonoBehaviour
    {
        [Header("Linked Enemy")]
        public EnemyVision target;

        [Header("Vision")]
        public float visionAngle = 30f;
        public float visionRange = 5f;
        public float visionNearRange = 3f;
        public LayerMask obstacleMask = ~(0);
        public bool showTwoLevels = false;

        [Header("Material")]
        public Material coneMaterial;
        public Material coneFarMaterial;
        public int sortOrder = 1;

        [Header("Optimization")]
        public int precision = 60;
        public float refreshRate = 0f;

        private MeshRenderer render;
        private MeshFilter mesh;
        private MeshRenderer render_far;
        private MeshFilter mesh_far;
        private float timer = 0f;

        private void Awake()
        {
            render = gameObject.AddComponent<MeshRenderer>();
            mesh = gameObject.AddComponent<MeshFilter>();
            render.sharedMaterial = coneMaterial;
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            render.receiveShadows = false;
            render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            render.allowOcclusionWhenDynamic = false;
            render.sortingOrder = sortOrder;

            if (showTwoLevels)
            {
                GameObject far_cone = new GameObject("FarCone");
                far_cone.transform.position = transform.position;
                far_cone.transform.SetParent(gameObject.transform);

                render_far = far_cone.AddComponent<MeshRenderer>();
                mesh_far = far_cone.AddComponent<MeshFilter>();
                render_far.sharedMaterial = coneFarMaterial;
                render_far.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                render_far.receiveShadows = false;
                render_far.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                render_far.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                render_far.allowOcclusionWhenDynamic = false;
                render.sortingOrder = sortOrder;
            }
        }

        private void Start()
        {
            InitMesh(mesh, false);

            if (showTwoLevels)
                InitMesh(mesh_far, true);
        }

        private void InitMesh(MeshFilter mesh, bool far)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();

            if (!far)
            {
                vertices.Add(new Vector3(0f, 0f, 0f));
                normals.Add(Vector3.up);
                uv.Add(Vector2.zero);
            }

            int minmax = Mathf.RoundToInt(visionAngle / 2f);

            int tri_index = 0;
            float step_jump = Mathf.Clamp(visionAngle / precision, 0.01f, minmax);

            for (float i = -minmax; i <= minmax; i += step_jump)
            {
                float angle = (float)(i + 90f) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle) * visionRange, 0f, Mathf.Sin(angle) * visionRange);

                vertices.Add(dir);
                normals.Add(Vector2.up);
                uv.Add(Vector2.zero);

                if (far)
                {
                    vertices.Add(dir);
                    normals.Add(Vector2.up);
                    uv.Add(Vector2.zero);
                }

                if (tri_index > 0)
                {
                    if (far)
                    {
                        triangles.Add(tri_index);
                        triangles.Add(tri_index + 1);
                        triangles.Add(tri_index - 2);

                        triangles.Add(tri_index - 2);
                        triangles.Add(tri_index + 1);
                        triangles.Add(tri_index - 1);
                    }
                    else
                    {
                        triangles.Add(0);
                        triangles.Add(tri_index + 1);
                        triangles.Add(tri_index);
                    }
                }
                tri_index += far ? 2 : 1;
            }

            mesh.mesh.vertices = vertices.ToArray();
            mesh.mesh.triangles = triangles.ToArray();
            mesh.mesh.normals = normals.ToArray();
            mesh.mesh.uv = uv.ToArray();
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = target.eye.transform.position;
            transform.rotation = target.transform.rotation;

            if (timer > refreshRate)
            {
                timer = 0f;

                float range = visionRange;
                if (showTwoLevels)
                    range = visionNearRange;

                UpdateMainLevel(mesh, range);

                if (showTwoLevels)
                    UpdateFarLevel(mesh_far, visionNearRange, visionRange - visionNearRange);
            }
        }

        private void UpdateMainLevel(MeshFilter mesh, float range)
        {
            List<Vector3> vertices = new List<Vector3>();
            vertices.Add(new Vector3(0f, 0f, 0f));

            int minmax = Mathf.RoundToInt(visionAngle / 2f);
            float step_jump = Mathf.Clamp(visionAngle / precision, 0.01f, minmax);
            for (float i = -minmax; i <= minmax; i += step_jump)
            {
                float angle = (float)(i + 90f) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle) * range, 0f, Mathf.Sin(angle) * range);

                RaycastHit hit;
                Vector3 pos_world = transform.TransformPoint(Vector3.zero);
                Vector3 dir_world = transform.TransformDirection(dir.normalized);
                bool ishit = Physics.Raycast(new Ray(pos_world, dir_world), out hit, range, obstacleMask.value);
                if (ishit)
                    dir = dir.normalized * hit.distance;
                Debug.DrawRay(pos_world, dir_world * (ishit ? hit.distance : range));

                vertices.Add(dir);
            }

            mesh.mesh.vertices = vertices.ToArray();
            mesh.mesh.RecalculateBounds();
        }

        private void UpdateFarLevel(MeshFilter mesh, float offset, float range)
        {
            List<Vector3> vertices = new List<Vector3>();

            int minmax = Mathf.RoundToInt(visionAngle / 2f);
            float step_jump = Mathf.Clamp(visionAngle / precision, 0.01f, minmax);
            for (float i = -minmax; i <= minmax; i += step_jump)
            {
                float angle = (float)(i + 90f) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle) * offset, 0f, Mathf.Sin(angle) * offset);

                RaycastHit hit;
                Vector3 pos_world = transform.TransformPoint(Vector3.zero);
                Vector3 dir_world = transform.TransformDirection(dir.normalized);
                bool ishit = Physics.Raycast(new Ray(pos_world, dir_world), out hit, range + offset, obstacleMask.value);

                float tot_dist = ishit ? hit.distance : range + offset;
                Vector3 dir1 = dir.normalized * offset;
                Vector3 dir2 = dir.normalized * Mathf.Max(tot_dist, offset);

                Debug.DrawRay(pos_world + dir_world * offset, dir_world * Mathf.Max(tot_dist - offset, 0f), Color.blue);

                vertices.Add(dir1);
                vertices.Add(dir2);
            }

            mesh.mesh.vertices = vertices.ToArray();
            mesh.mesh.RecalculateBounds();
        }
    }
}
