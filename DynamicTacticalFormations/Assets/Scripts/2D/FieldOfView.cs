using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float g_ViewRadius;
    
    [Range(0,360)] public float g_ViewAngle;

    public LayerMask g_TargetMask;
    public LayerMask g_ObstaclesMask;

    [SerializeField] float m_MeshResolution;
    [SerializeField] int m_EdgeResolveIterations;
    [SerializeField] float m_EdgeDistanceThreshold;

    public MeshFilter g_ViewMeshFilter;
    Mesh m_ViewMesh;

    public List<Transform> visibleTargets = new List<Transform>();


    private void Start()
    {
        m_ViewMesh = new Mesh();
        m_ViewMesh.name = "View Mesh";
        g_ViewMeshFilter.mesh = m_ViewMesh;

        
        StartCoroutine("FindTargetsWithDelay", 0.2f);
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }


    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, g_ViewRadius,g_TargetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            //If it is within the field of view
            if (Vector3.Angle(transform.forward,dirToTarget) < g_ViewAngle / 2 )
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                //No obstacles in the way thus we can see the target
                //Do something with it!

                //if target is not occluded by obstacles
                if (!Physics.Raycast(transform.position,dirToTarget,distToTarget,g_ObstaclesMask))
                {
                    visibleTargets.Add(target);
                    Debug.Log("Found him!");
                }
            }
        }

      
    }

    private void Update()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(g_ViewAngle * m_MeshResolution);
        float stepAngleSize = g_ViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - g_ViewAngle / 2 + stepAngleSize * i;
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle,true) * g_ViewRadius, Color.red);
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > m_EdgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);

                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }

                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }
            
            
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }


        m_ViewMesh.Clear();
        m_ViewMesh.vertices = vertices;
        m_ViewMesh.triangles = triangles;
        m_ViewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;


        for (int i = 0; i < m_EdgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > m_EdgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }

        }

        return new EdgeInfo(minPoint,maxPoint);

    }


    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 direction = DirFromAngle(globalAngle, true);

        RaycastHit hit;


        if (Physics.Raycast(transform.position,direction,out hit, g_ViewRadius, g_ObstaclesMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
            return new ViewCastInfo(false, transform.position + direction * g_ViewRadius, g_ViewRadius, globalAngle);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

public struct ViewCastInfo
{
    public bool hit;
    public Vector3 point;
    public float distance;
    public float angle;

    public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
    {
        this.hit = hit;
        this.point = point;
        this.distance = distance;
        this.angle = angle;
    }
}


public struct EdgeInfo
{
    public Vector3 pointA;
    public Vector3 pointB;

    public EdgeInfo(Vector3 pointA, Vector3 pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
    }
}

