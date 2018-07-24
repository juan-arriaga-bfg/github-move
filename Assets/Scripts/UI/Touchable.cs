using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Touchable : Graphic
{
    public override bool Raycast(Vector2 sp, Camera eventCamera)
    {
        return gameObject.activeSelf;
    }

    public override Texture mainTexture
    {
        get { return s_WhiteTexture; }
    }

    [Obsolete("Use OnPopulateMesh(VertexHelper vh) instead.", false)]
    protected override void OnPopulateMesh(Mesh m)
    {
        Vector2 corner1 = new Vector2(0f, 0f);
        Vector2 corner2 = new Vector2(0f, 0f);

        corner1.x -= rectTransform.pivot.x;
        corner1.y -= rectTransform.pivot.y;
        corner2.x -= rectTransform.pivot.x;
        corner2.y -= rectTransform.pivot.y;

        corner1.x *= rectTransform.rect.width;
        corner1.y *= rectTransform.rect.height;
        corner2.x *= rectTransform.rect.width;
        corner2.y *= rectTransform.rect.height;

        Vector4 uv = Vector4.zero;

        using (var vh = new VertexHelper())
        {
            vh.AddVert(new Vector3(corner1.x, corner1.y), color, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector3(corner1.x, corner2.y), color, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(corner2.x, corner2.y), color, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(corner2.x, corner1.y), color, new Vector2(uv.z, uv.y));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);

            vh.FillMesh(m);
        }
    }
}