using UnityEngine;
using System.Collections.Generic;


namespace Mara.MrTween {
    public class CubicBezierSplineSolver : AbstractSplineSolver {
        public CubicBezierSplineSolver(List<Vector3> nodes) {
            _nodes = nodes;
        }


        #region AbstractSplineSolver

        public override void ClosePath() { }


        public override Vector3 GetPoint(float t) {
            var d = 1f - t;
            return d * d * d * _nodes[0] + 3f * d * d * t * _nodes[1] + 3f * d * t * t * _nodes[2] + t * t * t * _nodes[3];
        }


        public override void DrawGizmos() {
            // draw the control points
            var originalColor = Gizmos.color;
            Gizmos.color = Color.red;

            Gizmos.DrawLine(_nodes[0], _nodes[1]);
            Gizmos.DrawLine(_nodes[2], _nodes[3]);

            Gizmos.color = originalColor;
        }

        #endregion
    }
}