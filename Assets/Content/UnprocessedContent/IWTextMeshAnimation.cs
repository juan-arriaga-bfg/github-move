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
    
    [SerializeField] private bool autoplayOnEnable = false;

    private bool dependsOnLength = true;

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
        
        //Curve();

        StartCoroutine(AnimateScaleCoroutine());
    }

    private void OnEnable()
    {
        // if (Application.isPlaying)
        // {
        //     Curve();
        //     return;
        // }
        
        // if (isAnimating && !autoplayOnEnable) return;

        if (autoplayOnEnable && Application.isPlaying)
        {
            Animate();
        }
        else
        {
            targetColors = null;
            Curve();
        }
    }

    private void OnDisable()
    {
        isAnimating = false;
    }

    private void LateUpdate()
    {
        // if (Application.isPlaying) return;
        
        // if (isAnimating) return;
        
        Curve();
    }
    
    public virtual void Curve()
    {
        textLabel.ForceMeshUpdate();
        var mesh = textLabel.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] defaultVertices = mesh.vertices;
        Color[] colors = mesh.colors;
        Color[] defaultColors = mesh.colors;
        Matrix4x4 matrix;

        int charCount = textLabel.textInfo.characterCount;
        
        if (charCount < 2) return;

        List<int> processedVertexIndex = new List<int>();

        for (int i = 0; i < charCount; i++)
        {
            var charInfo = textLabel.textInfo.characterInfo[i];
            int vertexIndex = charInfo.vertexIndex;

            if (processedVertexIndex.Contains(vertexIndex)) continue;

            processedVertexIndex.Add(vertexIndex);
        }
        
        float defaultMeshWidth = vertices.Length > 0 ? Mathf.Abs(vertices[0].x - vertices[vertices.Length - 1].x) : 0f;
        defaultMeshWidth = dependsOnLength ? defaultMeshWidth / 600f : 1f;

        for (int i = 0; i < processedVertexIndex.Count; i++)
        {
            int vertexIndex = processedVertexIndex[i];

            float percent = i / (float)(processedVertexIndex.Count - 1);

            var coef = curve.Evaluate(percent);
            var rotationCoef = rotationCurve.Evaluate(percent);

            float angle = rotationCoef - 0.5f;

            var targetOffset = new Vector3(0f, coef, 0f);

            Vector2 charMidBasline = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;
            Vector3 offset = charMidBasline;

            vertices[vertexIndex + 0] = defaultVertices[vertexIndex + 0] - offset;
            vertices[vertexIndex + 1] = defaultVertices[vertexIndex + 1] - offset;
            vertices[vertexIndex + 2] = defaultVertices[vertexIndex + 2] - offset;
            vertices[vertexIndex + 3] = defaultVertices[vertexIndex + 3] - offset;

            matrix = Matrix4x4.TRS(targetOffset * curveMultiply * defaultMeshWidth, Quaternion.Euler(0, 0, angle * angleMultiplier), Vector3.one * curveScale);

            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;

            if (targetColors != null)
            {
                colors[vertexIndex + 0] = new Color(defaultColors[vertexIndex + 0].r, defaultColors[vertexIndex + 0].g, defaultColors[vertexIndex + 0].b, targetColors[vertexIndex + 0].a);
                colors[vertexIndex + 1] = new Color(defaultColors[vertexIndex + 1].r, defaultColors[vertexIndex + 1].g, defaultColors[vertexIndex + 1].b, targetColors[vertexIndex + 1].a);
                colors[vertexIndex + 2] = new Color(defaultColors[vertexIndex + 2].r, defaultColors[vertexIndex + 2].g, defaultColors[vertexIndex + 2].b, targetColors[vertexIndex + 2].a);
                colors[vertexIndex + 3] = new Color(defaultColors[vertexIndex + 3].r, defaultColors[vertexIndex + 3].g, defaultColors[vertexIndex + 3].b, targetColors[vertexIndex + 3].a);
            }
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        textLabel.canvasRenderer.SetMesh(mesh);
    }

    private Color[] targetColors = null;

    public virtual IEnumerator AnimateScaleCoroutine()
    {
        float time = 0f;
        float duration = animDuration;

        if (string.IsNullOrEmpty(textLabel.text))
        {
            isAnimating = false;
            time = float.MaxValue;
        }
        
        textLabel.ForceMeshUpdate();
        var mesh = textLabel.mesh;
        
        Color[] colors = mesh.colors;
        Color[] defaultColors = mesh.colors;
        targetColors = colors;

        int charCount = textLabel.textInfo.characterInfo.Length;//textLabel.textInfo.characterCount;

        // Why characterCount may != characterInfo.Length happens ?!
//        while (textLabel.textInfo.characterCount != textLabel.textInfo.characterInfo.Length)
//        {
//            yield return null;
//        }
        
        List<int> processedVertexIndex = new List<int>();

        for (int i = 0; i < charCount; i++)
        {
            var charInfo = textLabel.textInfo.characterInfo[i];
            int vertexIndex = charInfo.vertexIndex;

            if (processedVertexIndex.Contains(vertexIndex)) continue;

            processedVertexIndex.Add(vertexIndex);
        }

        while (time <= duration)
        {
            for (int i = 0; i < processedVertexIndex.Count; i++)
            {
                int vertexIndex = processedVertexIndex[i];

                float range = duration / (float)(processedVertexIndex.Count);
                float fromRange = range * i;
                float toRange = range * (i + 2);
                toRange = Mathf.Clamp(toRange, 0f, duration);

                float targerAlpha = Mathf.Lerp(0f, 1f, (time - fromRange) / (toRange - fromRange));

                targetColors[vertexIndex + 0] = new Color(defaultColors[vertexIndex + 0].r, defaultColors[vertexIndex + 0].g, defaultColors[vertexIndex + 0].b, targerAlpha);
                targetColors[vertexIndex + 1] = new Color(defaultColors[vertexIndex + 1].r, defaultColors[vertexIndex + 1].g, defaultColors[vertexIndex + 1].b, targerAlpha);
                targetColors[vertexIndex + 2] = new Color(defaultColors[vertexIndex + 2].r, defaultColors[vertexIndex + 2].g, defaultColors[vertexIndex + 2].b, targerAlpha);
                targetColors[vertexIndex + 3] = new Color(defaultColors[vertexIndex + 3].r, defaultColors[vertexIndex + 3].g, defaultColors[vertexIndex + 3].b, targerAlpha);
            }

            // mesh.colors = colors;
            // textLabel.canvasRenderer.SetMesh(mesh);

            time = time + Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;

        targetColors = null;
        isAnimating = false;
    }
}
