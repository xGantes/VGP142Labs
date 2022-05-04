using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.EnemyVision
{
    public class EnemyDetector : MonoBehaviour
    {
        Mesh mesh;
        Collider[] colliders = new Collider[50];
        public Color meshColor = Color.red;
        public LayerMask Interact;
        public float distance = 10;
        public float angle = 10;
        public float height = 1.5f;


        public int scanFrequency = 30;
        int count;
        float scanInterval;
        float scanTimer;


        private void Start()
        {
            scanInterval = 1.0f / scanFrequency;
        }

        private void Update()
        {
            scanTimer -= Time.deltaTime;
            if (scanTimer < 0)
            {
                scanTimer += scanInterval;
                Scan();
            }
        }

        void Scan()
        {
            count = Physics.OverlapSphereNonAlloc(transform.position, distance,
                colliders, Interact, QueryTriggerInteraction.Collide);
        }

        Mesh CreateWedgeMesh()
        {
            mesh = new Mesh();

            int segments = 10;
            int numTriangles = (segments * 4) + 2 + 2;
            int numVertices = numTriangles * 3;

            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            Vector3 botCenter = Vector3.zero;
            Vector3 botLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
            Vector3 botRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

            Vector3 topCenter = botCenter + Vector3.up * height;
            Vector3 topLeft = botLeft + Vector3.up * height;
            Vector3 topRight = botRight + Vector3.up * height;

            //track all triangles
            int verts = 0;

            //left side
            vertices[verts++] = botCenter;
            vertices[verts++] = botLeft;
            vertices[verts++] = topLeft;

            vertices[verts++] = topLeft;
            vertices[verts++] = topCenter;
            vertices[verts++] = botCenter;

            //right side
            vertices[verts++] = botCenter;
            vertices[verts++] = topCenter;
            vertices[verts++] = topRight;

            vertices[verts++] = topRight;
            vertices[verts++] = botRight;
            vertices[verts++] = botCenter;

            float currentAngle = -angle;
            float deltaAngle = (angle * 2) / segments;

            for (int i = 0; i < segments; i++)
            {
                botLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
                botRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

                topLeft = botLeft + Vector3.up * height;
                topRight = botRight + Vector3.up * height;

                //front side
                vertices[verts++] = botLeft;
                vertices[verts++] = botRight;
                vertices[verts++] = topRight;

                vertices[verts++] = topRight;
                vertices[verts++] = topLeft;
                vertices[verts++] = botLeft;

                //top side
                vertices[verts++] = topCenter;
                vertices[verts++] = topLeft;
                vertices[verts++] = topRight;

                //bottom side
                vertices[verts++] = botCenter;
                vertices[verts++] = botLeft;
                vertices[verts++] = botRight;

                currentAngle += deltaAngle;
            }



            for (int x = 0; x < numVertices; x++)
            {
                triangles[x] = x;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private void OnValidate()
        {
            mesh = CreateWedgeMesh();
        }

        private void OnDrawGizmos()
        {
            if (mesh == null) { return; }
            else
            {
                Gizmos.color = meshColor;
                Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
            }

            Gizmos.DrawWireSphere(transform.position, distance);
            for (int i = 0; i < count; i++)
            {
                Gizmos.DrawSphere(colliders[i].transform.position, 0.2f);

            }
        }
    }

}
