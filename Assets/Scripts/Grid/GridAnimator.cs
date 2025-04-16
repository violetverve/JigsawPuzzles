using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuzzlePiece;
using GameManagement;
using System.Linq;

namespace Grid
{
    public class GridAnimator : MonoBehaviour
    {
        [SerializeField] private GridInteractionController _gridInteractionController;
        
        private void OnEnable()
        {
            ProgressManager.EdgesCollected += HandleEdgesCollected;
            GridInteractionController.PiecesCollected += HandlePiecesCollected;
        }

        private void OnDisable()
        {
            ProgressManager.EdgesCollected -= HandleEdgesCollected;
            GridInteractionController.PiecesCollected -= HandlePiecesCollected;
        }

        private void HandleEdgesCollected()
        {
            Debug.Log("Edges Collected");

            var corePieces = _gridInteractionController.CorePieces;

            var wholeGroup = _gridInteractionController.CollectedPieces;
            StartEdgeMaterialAnimation(corePieces, wholeGroup);
        }

        private void HandlePiecesCollected(List<Piece> corePieces, List<Piece> wholeGroup)
        {
            StartMaterialAnimation(corePieces, wholeGroup);
        }

        public void StartMaterialAnimation(List<Piece> corePieces, List<Piece> wholeGroup)
        {
            List<Piece> neighbourPieces = wholeGroup
                .Where(piece => !corePieces.Contains(piece) && 
                                corePieces.Any(corePiece => piece != corePiece && piece.IsNeighbour(corePiece.GridPosition)))
                .ToList();

            corePieces.ForEach(piece => piece.StartMaterialAnimation(1f));
            neighbourPieces.ForEach(piece => piece.StartMaterialAnimation(0.5f));
        }

        public static List<Vector2Int> GetNeighbors(Vector2Int piece, HashSet<Vector2Int> grid)
        {
            int x = piece.x;
            int y = piece.y;
            List<Vector2Int> neighbors = new List<Vector2Int>
            {
                new Vector2Int(x - 1, y), new Vector2Int(x + 1, y),
                new Vector2Int(x, y - 1), new Vector2Int(x, y + 1)
            };

            neighbors.RemoveAll(p => !grid.Contains(p));
            return neighbors;
        }

        public static (List<Vector2Int>, List<Vector2Int>) Bfs(Vector2Int start, HashSet<Vector2Int> grid)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            List<Vector2Int> path1 = new List<Vector2Int>();
            List<Vector2Int> path2 = new List<Vector2Int>();
            bool turn = true;

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                List<Vector2Int> neighbors = GetNeighbors(current, grid);

                foreach (Vector2Int neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);

                        if (turn)
                        {
                            path1.Add(neighbor);
                        }
                        else
                        {
                            path2.Add(neighbor);
                        }

                        turn = !turn;
                    }
                }
            }

            return (path1, path2);
        }

        public static (List<Vector2Int>, List<Vector2Int>) SplitGridPaths(List<Vector2Int> grid, Vector2Int corePiece)
        {
            HashSet<Vector2Int> gridSet = new HashSet<Vector2Int>(grid);
            return Bfs(corePiece, gridSet);
        }


        public void StartEdgeMaterialAnimation(List<Piece> corePieces, List<Piece> wholeGroup)
        {
            var coreEdges = corePieces
                .Where(piece => piece.IsEdgePiece)
                .ToList();

            var corePiece = coreEdges.First();

            // Debug.Log("Core piece position " + corePiece.GridPosition);

            var allOtherEdges = wholeGroup
                .Where(piece => piece.IsEdgePiece && piece != corePiece)
                .ToList();

            allOtherEdges.Sort((a, b) =>
            {
                int result = b.GridPosition.y.CompareTo(a.GridPosition.y); // Compare y-coordinates descending
                if (result == 0)
                {
                    result = a.GridPosition.x.CompareTo(b.GridPosition.x); // Compare x-coordinates ascending if y-coordinates are equal
                }
                return result;
            });

            var allOtherEdgesPositions = allOtherEdges.Select(piece => piece.GridPosition).ToList();

            var (path1, path2) = SplitGridPaths(allOtherEdgesPositions, corePiece.GridPosition);

            // Convert Vector2Int paths to Piece paths
            var path1Pieces = path1
                .Select(position => allOtherEdges.First(piece => piece.GridPosition == position))
                .ToList();
            
            var path2Pieces = path2
                .Select(position => allOtherEdges.First(piece => piece.GridPosition == position))
                .ToList();
            
            StartCoroutine(EdgesAmination(corePiece));
            StartCoroutine(StartChainReaction(path1Pieces));
            StartCoroutine(StartChainReaction(path2Pieces));
        }
        
        private IEnumerator EdgesAmination(Piece corePiece)
        {
            yield return new WaitForSeconds(0.4f);

            corePiece.StartMaterialAnimation(0.5f);
        }

        private IEnumerator StartChainReaction(List<Piece> pieces)
        {
            float chainReactionDelay = 0.15f;

            // for connect animation to end
            yield return new WaitForSeconds(0.4f + chainReactionDelay);

            foreach (var piece in pieces)
            {
                piece.StartMaterialAnimation(0.5f);
                yield return new WaitForSeconds(chainReactionDelay);
            }
        }

    }

}
