using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SpecialShape : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;

    private const int _pointsPerShape = 20;

    private void Start()
    {
        var shape = _splineContainer.Spline;

        // Step 2: Get the bounds of the spline
        Bounds bounds = GetSplineBounds(shape);
        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float minY = bounds.min.y;
        float maxY = bounds.max.y;

        // Step 3: Define split points (center of width and height)
        float midX = (minX + maxX) / 2;
        float midY = (minY + maxY) / 2;


        var points = GetSplinePoints2D(shape);

        List<List<Vector2>> splitPointsPerQuarter = SplitByAxes(points, 0, 0);
    }

    private List<List<Vector2>> SplitByAxes(IEnumerable<Vector2> points, float midX, float midY)
    {
        List<Vector2> firstQuarter = new List<Vector2>();
        List<Vector2> secondQuarter = new List<Vector2>();
        List<Vector2> thirdQuarter = new List<Vector2>();
        List<Vector2> fourthQuarter = new List<Vector2>();

        foreach (var point in points)
        {
            if (point.x < midX && point.y < midY)
            {
                firstQuarter.Add(point);
            }
            else if (point.x >= midX && point.y < midY)
            {
                secondQuarter.Add(point);
            }
            else if (point.x < midX && point.y >= midY)
            {
                thirdQuarter.Add(point);
            }
            else if (point.x >= midX && point.y >= midY)
            {
                fourthQuarter.Add(point);
            }
        }

        Debug.Log("First quarter: ");
        PlotList(firstQuarter);
        Debug.Log("Second quarter: ");
        PlotList(secondQuarter);
        Debug.Log("Third quarter: ");
        PlotList(thirdQuarter);
        Debug.Log("Fourth quarter: ");
        PlotList(fourthQuarter);

        return new List<List<Vector2>> { firstQuarter, secondQuarter, thirdQuarter, fourthQuarter };

    }

    private void PlotList(List<Vector2> points)
    {
        foreach (var point in points)
        {
            Debug.Log(point);
        }
    }

    Bounds GetSplineBounds(Spline spline)
    {
        Vector3 min = spline[0].Position;
        Vector3 max = spline[0].Position;

        for (int i = 1; i < spline.Count; i++)
        {
            Vector3 point = spline[i].Position;
            min = Vector3.Min(min, point);
            max = Vector3.Max(max, point);
        }

        Debug.Log("Min: " + min + " Max: " + max);

        return new Bounds((min + max) / 2, max - min);
    }

    private IEnumerable<Vector2> GetSplinePoints2D(Spline spline)
    {
        for (int i = 0; i <= _pointsPerShape; i++)
        {
            float interpolationFactor = i / (float)_pointsPerShape;
            Vector3 position = spline.EvaluatePosition(interpolationFactor);
            yield return new Vector2(position.x, position.y);
        }
    }

    private float GetYForX(Spline spline, float x)
    {
        float closestY = float.NaN;
        float closestDistance = float.MaxValue;

        for (int i = 0; i <= _pointsPerShape; i++)
        {
            float interpolationFactor = i / (float)_pointsPerShape;
            Vector3 position = spline.EvaluatePosition(interpolationFactor);

            float distance = Mathf.Abs(position.x - x);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestY = position.y;
            }
        }

        return closestY;
    }
}
