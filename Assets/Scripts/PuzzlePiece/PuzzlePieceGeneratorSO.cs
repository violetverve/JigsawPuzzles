using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using System.Linq;
using PuzzlePiece.Features;
using Utilities;

namespace PuzzlePiece
{
    [CreateAssetMenu(menuName = "PuzzlePiece/PuzzlePieceGenerator")]
    public class PuzzlePieceGeneratorSO : ScriptableObject
    {
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private int _pointsPerSpline = 40;
        [SerializeField] private Material _outlineMaterial;
        [SerializeField] private float _outlineWidth = 0.01f;

        // Test special shape in-build
        [SerializeField] private SplineContainer _specialShape;

        private Dictionary<PieceConfiguration, Mesh> _meshCache = new Dictionary<PieceConfiguration, Mesh>();


        public Piece CreatePiece(PieceConfiguration pieceConfiguration, Vector2Int gridPosition, Vector2Int grid, Material material)
        {
            var points = GetPointsFromConfig(pieceConfiguration);

            var pieceObject = CreatePieceObject(gridPosition);

            if (!_meshCache.TryGetValue(pieceConfiguration, out var mesh))
            {
                mesh = GenerateMesh(points, gridPosition, grid);
                _meshCache[pieceConfiguration] = mesh;
            } 
            else 
            {
                mesh = new Mesh
                {
                    vertices = mesh.vertices,
                    triangles = mesh.triangles,
                    uv = CalculateUVs(mesh.vertices, gridPosition, grid)
                };
            }
            
            AddMeshComponents(pieceObject, material, mesh);
            CreateOutline(pieceObject, points);
            
            var piece = pieceObject.AddComponent<Piece>();

            return piece;
        }

        private GameObject CreatePieceObject(Vector2Int gridPosition)
        {
            return new GameObject(GeneratePieceName(gridPosition));
        }

        private string GeneratePieceName(Vector2Int gridPosition)
        {
            return $"PuzzlePiece {gridPosition.x}_{gridPosition.y}";
        }

        private void AddMeshComponents(GameObject pieceObject, Material material, Mesh mesh)
        {
            var meshFilter = pieceObject.AddComponent<MeshFilter>();
            var meshRenderer = pieceObject.AddComponent<MeshRenderer>();
            meshFilter.mesh = mesh;
            meshRenderer.material = material;
        }

        public void CreateOutline(GameObject piece, PieceConfiguration pieceConfiguration)
        {
            CreateOutline(piece, GetPointsFromConfig(pieceConfiguration));
        }

        private void CreateOutline(GameObject piece, IEnumerable<Vector2> points)
        {
            var lineRenderer = piece.AddComponent<LineRenderer>(); 

            lineRenderer.material = _outlineMaterial;
            lineRenderer.widthMultiplier = _outlineWidth;
            lineRenderer.positionCount = points.Count();
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;

            int index = 0;
            foreach (var point in points)
            {
                lineRenderer.SetPosition(index++, point);
            }
        }

        public IEnumerable<Vector2> GetPointsFromConfig(PieceConfiguration pieceConfiguration)
        {
            return new []
            {
                GetPointsFromFeature(pieceConfiguration.Left, FeaturePosition.Left),
                GetPointsFromFeature(pieceConfiguration.Top, FeaturePosition.Top),
                GetPointsFromFeature(pieceConfiguration.Right, FeaturePosition.Right),
                GetPointsFromFeature(pieceConfiguration.Bottom, FeaturePosition.Bottom)
            }.SelectMany(points => points).Distinct();
        }

        private Vector2[] CalculateUVs(Vector3[] vertices, Vector2Int gridPosition, Vector2Int grid)
        {
            int totalColumns = grid.x;
            int totalRows = grid.y;

            float uvPieceWidth = 1f / totalColumns;
            float uvPieceHeight = 1f / totalRows;
            float uvStartX = gridPosition.x * uvPieceWidth;
            float uvStartY = gridPosition.y * uvPieceHeight;

            Vector2 vertexMin = new Vector2(-1, -1);
            Vector2 vertexMax = new Vector2(1, 1);
            Vector2 rangeSize = vertexMax - vertexMin;

            Vector2[] uv = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                float normalizedU = (vertices[i].x - vertexMin.x) / rangeSize.x;
                float normalizedV = (vertices[i].y - vertexMin.y) / rangeSize.y;

                uv[i] = new Vector2(
                    uvStartX + normalizedU * uvPieceWidth,
                    uvStartY + normalizedV * uvPieceHeight
                );
            }

            return uv;
        }

        private Mesh GenerateMesh(IEnumerable<Vector2> points, Vector2Int gridPosition, Vector2Int grid)
        {
            var vertices = points.Select(point => new Vector3(point.x, point.y, 0)).ToArray();
            var uv = CalculateUVs(vertices, gridPosition, grid);

            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = new Triangulator(points.ToArray()).Triangulate(),
                uv = uv
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        } 

        private IEnumerable<Vector2> GetPointsFromFeature(FeatureType featureType, FeaturePosition featurePosition)
        {
            var points2D = InitializePointsByFeatureType(featureType);
            return ApplyFeaturePositionTransformations(points2D, featurePosition);
        }

        private IEnumerable<Vector2> InitializePointsByFeatureType(FeatureType featureType)
        {
            if (featureType == FeatureType.Hole || featureType == FeatureType.Knob)
            {
                var points = GetSplinePoints2D(_splineContainer.Spline);
                return featureType == FeatureType.Hole
                    ? points.Select(p => new Vector2(-p.x, -p.y)).Reverse()
                    : points;
            }
            else
            {
                return new List<Vector2> {new Vector2(-1, 0), new Vector2(1, 0)};
            }
        }   

        private IEnumerable<Vector2> ApplyFeaturePositionTransformations(IEnumerable<Vector2> points, FeaturePosition featurePosition)
        {
            return points.Select(point => featurePosition switch
            {
                FeaturePosition.Top => new Vector2(point.x, point.y + 1),
                FeaturePosition.Bottom => new Vector2(-point.x, -point.y - 1),
                FeaturePosition.Left => new Vector2(-point.y - 1, point.x),
                FeaturePosition.Right => new Vector2(point.y + 1, -point.x),
                _ => point
            });
        }

        private IEnumerable<Vector2> GetSplinePoints2D(Spline spline)
        {
            for (int i = 0; i <= _pointsPerSpline; i++)
            {
                float interpolationFactor = i / (float)_pointsPerSpline;
                Vector3 position = spline.EvaluatePosition(interpolationFactor);
                yield return new Vector2(position.x, position.y);
            }
        }

    }
}
