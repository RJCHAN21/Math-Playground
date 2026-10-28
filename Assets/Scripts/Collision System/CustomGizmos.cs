using UnityEngine;

/// <summary>
/// This class contains functions that already exist through Gizmos, which is honestly stupid and redundant at the same time.
/// However, for anything that Gizmos can't do by default (such as 2D Shapes), it's probably here.
/// Methods with OLD suffixes are my initial draft that was used to understand the math behind the concept.
/// They work, but OLD methods are a bit less optimal.
/// </summary>
public static class CustomGizmos
{   
    public static void DrawWireCube(Vector3 center, Vector3 size, Color? color = null)
    {
        Vector3 halfSize = size / 2f;
        Vector3 min = center - halfSize;
        Vector3 max = center + halfSize;

        Vector3 A = new Vector3(max.x, max.y, min.z);
        Vector3 B = new Vector3(min.x, max.y, min.z);
        Vector3 C = new Vector3(min.x, max.y, max.z);
        Vector3 D = new Vector3(max.x, max.y, max.z);
        Vector3 E = new Vector3(max.x, min.y, min.z);
        Vector3 F = new Vector3(min.x, min.y, min.z);
        Vector3 G = new Vector3(min.x, min.y, max.z);
        Vector3 H = new Vector3(max.x, min.y, max.z);

        Gizmos.color = color ?? Color.red;

        Gizmos.DrawLine(A,B);
        Gizmos.DrawLine(B,C);
        Gizmos.DrawLine(C,D);
        Gizmos.DrawLine(D,A);
        Gizmos.DrawLine(E,A);
        Gizmos.DrawLine(F,B);
        Gizmos.DrawLine(G,C);
        Gizmos.DrawLine(H,D);
        Gizmos.DrawLine(E,F);
        Gizmos.DrawLine(F,G);
        Gizmos.DrawLine(G,H);
        Gizmos.DrawLine(H,E);
    }

    public static void DrawSquare(Vector2 center, Vector2 size)
    {
        Vector2 halfSize = size / 2f;
        Vector2 min = center - halfSize;
        Vector2 max = center + halfSize;
        Vector2 topLeftCorner = new Vector2(min.x, max.y);
        Vector2 topRightCorner = new Vector2(max.x, max.y);
        Vector2 bottomLeftCorner = new Vector2(min.x, min.y);
        Vector2 bottomRightCorner = new Vector2(max.x, min.y);

        Gizmos.DrawLine(topLeftCorner, topRightCorner);
        Gizmos.DrawLine(topRightCorner, bottomRightCorner);
        Gizmos.DrawLine(bottomRightCorner, bottomLeftCorner);
        Gizmos.DrawLine(bottomLeftCorner, topLeftCorner);
    }

    public static void DrawSquareOLD(Vector2 center, Vector2 size)
    {   
        Vector2 halfSize = size / 2f;
        Vector2 topLeftCorner = new Vector2(center.x - halfSize.x, center.y + halfSize.y);
        Vector2 topRightCorner = new Vector2(center.x + halfSize.x, center.y + halfSize.y);
        Vector2 bottomLeftCorner = new Vector2(center.x - halfSize.x, center.y - halfSize.y);
        Vector2 bottomRightCorner = new Vector2(center.x + halfSize.x, center.y - halfSize.y);

        Gizmos.DrawLine(new Vector3(topLeftCorner.x, topLeftCorner.y), new Vector3(topRightCorner.x, topRightCorner.y));
        Gizmos.DrawLine(new Vector3(topRightCorner.x, topRightCorner.y), new Vector3(bottomRightCorner.x, bottomRightCorner.y));
        Gizmos.DrawLine(new Vector3(bottomRightCorner.x, bottomRightCorner.y), new Vector3(bottomLeftCorner.x, bottomLeftCorner.y));
        Gizmos.DrawLine(new Vector3(bottomLeftCorner.x, bottomLeftCorner.y), new Vector3(topLeftCorner.x, topLeftCorner.y));
    }
}
