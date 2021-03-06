using UnityEngine;
using System.Collections.Generic;


namespace Mara.MrTween {
    public class CatmullRomSplineSolver : AbstractSplineSolver {
        public CatmullRomSplineSolver(List<Vector3> nodes) {
            _nodes = nodes;
        }


        #region AbstractSplineSolver

        // closing a Catmull-Rom spline: http://cl.ly/GOZv
        public override void ClosePath() {
            // first, remove the control points
            _nodes.RemoveAt(0);
            _nodes.RemoveAt(_nodes.Count - 1);

            // if the first and last node are not the same add one
            if (_nodes[0] != _nodes[_nodes.Count - 1])
                _nodes.Add(_nodes[0]);


            // figure out the distances from node 0 to the first node and the second to last node (remember above
            // we made the last node equal to the first so node 0 and _nodes.Count - 1 are identical)
            var distanceToFirstNode = Vector3.Distance(_nodes[0], _nodes[1]);
            var distanceToLastNode = Vector3.Distance(_nodes[0], _nodes[_nodes.Count - 2]);


            // handle the first node. we want to use the distance to the LAST (opposite segment) node to figure out where this control point should be
            var distanceToFirstTarget = distanceToLastNode / Vector3.Distance(_nodes[1], _nodes[0]);
            var lastControlNode = (_nodes[0] + (_nodes[1] - _nodes[0]) * distanceToFirstTarget);


            // handle the last node. for this one, we want the distance to the first node for the control point but it should
            // be along the vector to the last node
            var distanceToLastTarget = distanceToFirstNode / Vector3.Distance(_nodes[_nodes.Count - 2], _nodes[0]);
            var firstControlNode = (_nodes[0] + (_nodes[_nodes.Count - 2] - _nodes[0]) * distanceToLastTarget);

            _nodes.Insert(0, firstControlNode);
            _nodes.Add(lastControlNode);
        }


        public override Vector3 GetPoint(float t) {
            var numSections = _nodes.Count - 3;
            var currentNode = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            var u = t * (float)numSections - (float)currentNode;

            var a = _nodes[currentNode];
            var b = _nodes[currentNode + 1];
            var c = _nodes[currentNode + 2];
            var d = _nodes[currentNode + 3];

            return 0.5f *
                (
                    (-a + 3f * b - 3f * c + d) * (u * u * u)
                    + (2f * a - 5f * b + 4f * c - d) * (u * u)
                    + (-a + c) * u
                    + 2f * b
                );
        }


        public override void DrawGizmos() {
            if (_nodes.Count < 2)
                return;

            // draw the control points
            var originalColor = Gizmos.color;
            Gizmos.color = new Color(1, 1, 1, 0.3f);

            Gizmos.DrawLine(_nodes[0], _nodes[1]);
            Gizmos.DrawLine(_nodes[_nodes.Count - 1], _nodes[_nodes.Count - 2]);

            Gizmos.color = originalColor;
        }

        #endregion
    }
}