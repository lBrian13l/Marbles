namespace UnityEngine.AI
{
    public class WalkablePlatform : WalkableFigure
    {
        private float _defaultMeshWidth = 4f;
        private float _defaultMeshHeight = 0.5f;
        private float _defaultAdditionalMeshLength = 8f;
        private float _defaultMeshOffset = 0.2f;
        private float _additionalLinkWidth = 2f;

        public override void SetupLinks()
        {
            for (int i = 0; i < _links.Length && i < _meshes.Length; i++)
            {
                NavMeshLink link = _links[i];
                Transform meshTransform = _meshes[i].transform;
                float scaleX = transform.lossyScale.x;
                float scaleY = transform.lossyScale.y;
                float scaleZ = transform.lossyScale.z;
                GameObject previousSide;
                GameObject nextSide;
                float meshOffset = 0f;
                float additionalMeshLength = _defaultAdditionalMeshLength;

                if (i == 0)
                {
                    previousSide = _links[_links.Length - 1].gameObject;
                    nextSide = _links[i + 1].gameObject;
                }
                else if (i == _links.Length - 1)
                {
                    previousSide = _links[i - 1].gameObject;
                    nextSide = _links[0].gameObject;
                }
                else
                {
                    previousSide = _links[i - 1].gameObject;
                    nextSide = _links[i + 1].gameObject;
                }

                if (previousSide.activeInHierarchy != nextSide.activeInHierarchy)
                {
                    if (previousSide.activeInHierarchy)
                    {
                        meshOffset = _defaultMeshOffset;
                    }
                    else
                    {
                        meshOffset = -_defaultMeshOffset;
                    }

                    additionalMeshLength /= 2;
                }
                else if (previousSide.activeInHierarchy == false)
                {
                    additionalMeshLength = 0;
                }

                if (i % 2 == 1)
                {
                    link.width = scaleX + _additionalLinkWidth;
                    meshTransform.localScale = new Vector3((scaleX + additionalMeshLength) / scaleX, _defaultMeshHeight / scaleY, _defaultMeshWidth / scaleZ);
                }
                else
                {
                    link.width = scaleZ + _additionalLinkWidth;
                    meshTransform.localScale = new Vector3((scaleZ + additionalMeshLength) / scaleZ, _defaultMeshHeight / scaleY, _defaultMeshWidth / scaleX);
                }

                meshTransform.localPosition = new Vector3(meshOffset, _defaultMeshHeight - meshTransform.localScale.y / 2, 0.5f + meshTransform.localScale.z / 2);
                link.startPoint = new Vector3(-meshOffset * 10, link.startPoint.y, link.startPoint.z);
                link.endPoint = new Vector3(-meshOffset * 10, link.startPoint.y - scaleY, link.endPoint.z);
            }
        }
    }
}
