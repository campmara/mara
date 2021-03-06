using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Mara.MrTween {
    public class StraightLineSplineSolver : AbstractSplineSolver {
        private Dictionary<int, float> _segmentStartLocations;
        private Dictionary<int, float> _segmentDistances;
        private int _currentSegment;


        public StraightLineSplineSolver(List<Vector3> nodes) {
            _nodes = nodes;
        }


        #region AbstractGoSplineSolver

        public override void BuildPath() {
            // we need at least 3 nodes (more than 1 segment) to bother with building
            if (_nodes.Count < 3)
                return;

            // we dont care about the first node for distances because they are always t:0 and len:0 and we dont need the first or last for locations
            _segmentStartLocations = new Dictionary<int, float>(_nodes.Count - 2);
            _segmentDistances = new Dictionary<int, float>(_nodes.Count - 1);

            for (var i = 0; i < _nodes.Count - 1; i++) {
                // calculate the distance to the next node
                var distance = Vector3.Distance(_nodes[i], _nodes[i + 1]);
                _segmentDistances.Add(i, distance);
                _pathLength += distance;
            }


            // now that we have the total length we can loop back through and calculate the segmentStartLocations
            var accruedRouteLength = 0f;
            for (var i = 0; i < _segmentDistances.Count - 1; i++) {
                accruedRouteLength += _segmentDistances[i];
                _segmentStartLocations.Add(i + 1, accruedRouteLength / _pathLength);
            }
        }


        public override void ClosePath() {
            // add a node to close the route if necessary
            if (_nodes[0] != _nodes[_nodes.Count - 1])
                _nodes.Add(_nodes[0]);
        }


        public override Vector3 GetPoint(float t) {
            return GetPointOnPath(t);
        }


        public override Vector3 GetPointOnPath(float t) {
            // we need at least 3 nodes (more than 1 segment) to bother using the look up tables. else we just lerp directly from
            // node 1 to node 2
            if (_nodes.Count < 3)
                return Vector3.Lerp(_nodes[0], _nodes[1], t);

            int[] keysSegmentStartLocations = new int[_segmentStartLocations.Keys.Count];
            _segmentStartLocations.Keys.CopyTo(keysSegmentStartLocations, 0);

            // which segment are we on?
            _currentSegment = 0;
            for (int k = 0; k < keysSegmentStartLocations.Length; ++k) {
                int key = keysSegmentStartLocations[k];
                float value = _segmentStartLocations[key];

                if (value < t) {
                    _currentSegment = key;
                    continue;
                }

                break;
            }

            // now we need to know the total distance travelled in all previous segments so we can subtract it from the total
            // travelled to get exactly how far along the current segment we are
            var totalDistanceTravelled = t * _pathLength;
            var i = _currentSegment - 1; // we want all the previous segment lengths
            while (i >= 0) {
                totalDistanceTravelled -= _segmentDistances[i];
                --i;
            }

            return Vector3.Lerp(_nodes[_currentSegment], _nodes[_currentSegment + 1], totalDistanceTravelled / _segmentDistances[_currentSegment]);
        }

        #endregion
    }
}