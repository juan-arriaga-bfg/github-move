using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class IWTextMeshAnimation : MonoBehaviour
{
    private TMP_Text textLabel;

    [SerializeField] private AnimationCurve curve;

    [SerializeField] private AnimationCurve rotationCurve;

    [SerializeField] private float curveMultiply = 10f;

    [SerializeField] private float angleMultiplier = 1f;

    [SerializeField] private float curveScale = 1f;

    [SerializeField] private float animDuration = 1f;

    protected bool isAnimating = false;

    public bool IsAnimating
    {
        get { return isAnimating; }
    }

    protected virtual void Awake()
    {
        this.textLabel = GetComponent<TMP_Text>();
    }

    [ContextMenu("Animate")]
    public virtual void Animate()
    {
        if (isAnimating) return;

        isAnimating = true;
        
        Curve();

        StartCoroutine(AnimateScaleCoroutine());
    }

    
#if UNITY_EDITOR
    
    private void OnEnable()
    {
        if (Application.isPlaying) return;
        
        if (isAnimating) return;
        
        Curve();
    }

    private void OnDisable()
    {
        isAnimating = false;
    }

    private void Update()
    {
        if (Application.isPlaying) return;
        
        if (isAnimating) return;
        
        Curve();
    }
    
#endif

    public virtual void Curve()
    {
        textLabel.ForceMeshUpdate();
        var mesh = textLabel.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] defaultVertices = mesh.vertices;
        Matrix4x4 matrix;

        int charCount = textLabel.textInfo.characterCount;

        List<int> processedVertexIndex = new List<int>();

        for (int i = 0; i < charCount; i++)
        {
            var charInfo = textLabel.textInfo.characterInfo[i];
            int vertexIndex = charInfo.vertexIndex;

            if (processedVertexIndex.Contains(vertexIndex)) continue;

            processedVertexIndex.Add(vertexIndex);
        }

        for (int i = 0; i < processedVertexIndex.Count; i++)
        {
            int vertexIndex = processedVertexIndex[i];

            float percent = i / (float)(processedVertexIndex.Count - 1);

            var coef = curve.Evaluate(percent);
            var rotationCoef = rotationCurve.Evaluate(percent);

            float angle = rotationCoef - 0.5f;// (i >= processedVertexIndex.Count * 0.5f) ? 1f * rotationCoef : -1f * rotationCoef;

            var targetOffset = new Vector3(0f, coef, 0f);

            Vector2 charMidBasline = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;
            Vector3 offset = charMidBasline;

            vertices[vertexIndex + 0] = defaultVertices[vertexIndex + 0] - offset;
            vertices[vertexIndex + 1] = defaultVertices[vertexIndex + 1] - offset;
            vertices[vertexIndex + 2] = defaultVertices[vertexIndex + 2] - offset;
            vertices[vertexIndex + 3] = defaultVertices[vertexIndex + 3] - offset;

            matrix = Matrix4x4.TRS(targetOffset * curveMultiply, Quaternion.Euler(0, 0, angle * angleMultiplier), Vector3.one * curveScale);

            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        mesh.vertices = vertices;
        textLabel.canvasRenderer.SetMesh(mesh);
    }

    public virtual IEnumerator AnimateScaleCoroutine()
    {
        textLabel.ForceMeshUpdate();
        var mesh = textLabel.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] defaultVertices = mesh.vertices;
        Color[] colors = mesh.colors;
        Color[] defaultColors = mesh.colors;
        Matrix4x4 matrix;
        int charCount = textLabel.textInfo.characterCount;

        List<int> processedVertexIndex = new List<int>();

        for (int i = 0; i < charCount; i++)
        {
            var charInfo = textLabel.textInfo.characterInfo[i];
            int vertexIndex = charInfo.vertexIndex;

            if (processedVertexIndex.Contains(vertexIndex)) continue;

            processedVertexIndex.Add(vertexIndex);
        }

        float time = 0f;
        float duration = animDuration;

        while (time < duration)
        {

            for (int i = 0; i < processedVertexIndex.Count; i++)
            {
                int vertexIndex = processedVertexIndex[i];

                float percent = i / (float)(processedVertexIndex.Count - 1);

                var coef = curve.Evaluate(percent);
                var rotationCoef = rotationCurve.Evaluate(percent);

                float range = duration / (float)(processedVertexIndex.Count);
                float fromRange = range * i;
                float toRange = range * (i + 2);
                toRange = Mathf.Clamp(toRange, 0f, duration);
                float targetScale = Mathf.Lerp(0f, 1f, (time - fromRange) / (toRange - fromRange));
                float targerAlpha = Mathf.Lerp(0f, 1f, (time - fromRange) / (toRange - fromRange));

                float angle = rotationCoef - 0.5f;// (i >= processedVertexIndex.Count * 0.5f) ? 1f * rotationCoef : -1f * rotationCoef;

                var targetOffset = new Vector3(0f, coef, 0f);

                Vector2 charMidBasline = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;
                Vector3 offset = charMidBasline;

                vertices[vertexIndex + 0] = defaultVertices[vertexIndex + 0] - offset;
                vertices[vertexIndex + 1] = defaultVertices[vertexIndex + 1] - offset;
                vertices[vertexIndex + 2] = defaultVertices[vertexIndex + 2] - offset;
                vertices[vertexIndex + 3] = defaultVertices[vertexIndex + 3] - offset;

                matrix = Matrix4x4.TRS(targetOffset * curveMultiply * targetScale, Quaternion.Euler(0, 0, angle * angleMultiplier), new Vector3(targetScale, targetScale, targetScale) * curveScale);

                vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
                vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
                vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
                vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

                vertices[vertexIndex + 0] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;

                colors[vertexIndex + 0] = new Color(defaultColors[vertexIndex + 0].r, defaultColors[vertexIndex + 0].g, defaultColors[vertexIndex + 0].b, targerAlpha);
                colors[vertexIndex + 1] = new Color(defaultColors[vertexIndex + 1].r, defaultColors[vertexIndex + 1].g, defaultColors[vertexIndex + 1].b, targerAlpha);
                colors[vertexIndex + 2] = new Color(defaultColors[vertexIndex + 2].r, defaultColors[vertexIndex + 2].g, defaultColors[vertexIndex + 2].b, targerAlpha);
                colors[vertexIndex + 3] = new Color(defaultColors[vertexIndex + 3].r, defaultColors[vertexIndex + 3].g, defaultColors[vertexIndex + 3].b, targerAlpha);
            }

            mesh.vertices = vertices;
            mesh.colors = colors;
            textLabel.canvasRenderer.SetMesh(mesh);

            time = time + Time.deltaTime;
            yield return null;
        }

        yield return null;

        isAnimating = false;
    }
}
