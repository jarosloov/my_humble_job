/********************************************************/
/**  © 2019 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public enum ExplosionForce2DMode { Simple, Adaptive }
public static class GM
{
    static int props_Color;
    static int props_EmissionColor;
    static bool isSetup;

    static void Init()
    {
        props_Color = Shader.PropertyToID("_Color");
        props_EmissionColor = Shader.PropertyToID("_EmissionColor");
        isSetup = true;
    }

    /// <summary>
    /// Найти ближайший объект, относительно указанной позиции.
    /// </summary>
    /// <param name="position">Целевая позиция.</param>
    /// <param name="array">Массив поиска.</param>
    public static Collider FindNearTarget(Vector3 position, Collider[] array)
    {
        float dist = Mathf.Infinity;
        Collider target = null;

        foreach (Collider t in array)
        {
            float curDist = (t.transform.position - position).sqrMagnitude;

            if (curDist < dist)
            {
                target = t;
                dist = curDist;
            }
        }

        return target;
    }

    /// <summary>
    /// Найти ближайший объект, относительно указанной позиции.
    /// </summary>
    /// <param name="position">Целевая позиция.</param>
    /// <param name="array">Массив поиска.</param>
    public static Collider2D FindNearTarget(Vector3 position, Collider2D[] array)
    {
        float dist = Mathf.Infinity;
        Collider2D target = null;

        foreach (Collider2D t in array)
        {
            float curDist = (t.transform.position - position).sqrMagnitude;

            if (curDist < dist)
            {
                target = t;
                dist = curDist;
            }
        }

        return target;
    }

    /// <summary>
    /// Рассчитать размеры холста для игрового пространства.
    /// </summary>
    /// <param name="canvas">Целевой объект.</param>
    /// <param name="widthInMeters">Требуемая ширина.</param>
    public static void CalculateCanvasScale(Canvas canvas, float widthInMeters)
    {
        if (canvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.Log(canvas + " --> Выбран неверный режим, необходимо установить его в 'World Space'.");
            return;
        }

        RectTransform tr = canvas.GetComponent<RectTransform>();
        float size = widthInMeters / tr.sizeDelta.x;
        tr.localScale = new Vector3(size, size, size);
    }

    /// <summary>
    /// Эффект взрыва для 2D игры.
    /// </summary>
    /// <param name="explosionForce">Сила взрыва.</param>
    /// <param name="explosionPosition">Точка взрыва.</param>
    /// <param name="explosionRadius">Радиус взрыва.</param>
    /// <param name="mode">Режим эффекта. Simple - не учитывает преграды. Adaptive - учитывает окружение.</param>
    /// <param name="layerMask">Фильтр по маске слоя.</param>
    public static void AddExplosionForce2D(float explosionForce, Vector3 explosionPosition, float explosionRadius, ExplosionForce2DMode mode = ExplosionForce2DMode.Simple, int layerMask = Physics2D.AllLayers)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, layerMask);

        foreach (Collider2D hit in colliders)
        {
            if (hit.attachedRigidbody != null)
            {
                Vector3 direction = hit.transform.position - explosionPosition;
                direction.z = 0;

                if (CanUse(explosionPosition, hit.attachedRigidbody, mode))
                {
                    hit.attachedRigidbody.AddForce(direction.normalized * explosionForce);
                }
            }
        }
    }

    static bool CanUse(Vector3 position, Rigidbody2D body, ExplosionForce2DMode mode)
    {
        if (mode == ExplosionForce2DMode.Simple) return true;

        RaycastHit2D hit = Physics2D.Linecast(position, body.position);

        if (hit.rigidbody != null && hit.rigidbody == body)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Сериализация данных.
    /// </summary>
    /// <typeparam name="T">Целевой объект.</typeparam>
    /// <param name="data">Целевой объект.</param>
    /// <param name="path">Путь к файлу.</param>
    public static void SaveBinary<T>(T data, string path)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        binary.Serialize(stream, data);
        stream.Close();
        Debug.Log(" [Serializator] --> Сохранение по адресу: " + path);
    }

    /// <summary>
    /// Десериализация данных.
    /// </summary>
    /// <typeparam name="T">Целевой объект.</typeparam>
    /// <param name="path">Путь к файлу.</param>
    /// <returns></returns>
    public static T LoadBinary<T>(string path)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);
        T data = (T)binary.Deserialize(stream);
        stream.Close();
        Debug.Log(" [Serializator] --> Загрузка данных из файла: " + path);
        return data;
    }

    /// <summary>
    /// Установить orthographic 2D камеры так, чтобы спрайт помещался по ширине экрана.
    /// </summary>
    /// <param name="cam">Текущая камера.</param>
    /// <param name="spriteWidth">Размер спрайта по ширине.</param>
    public static void OrthographicSizeByWidth(Camera cam, float spriteWidth)
    {
        if (!cam.orthographic) return;
        cam.orthographicSize = spriteWidth * Screen.height / Screen.width * 0.5f;
    }

    /// <summary>
    /// Выстроить объекты по кругу, относительно заданного радиуса.
    /// </summary>
    /// <param name="center">Центр круга.</param>
    /// <param name="array">Массив объектов.</param>
    /// <param name="radius">Радиус круга.</param>
    public static void CreateCircleByRadius(Vector3 center, Transform[] array, float radius)
    {
        if (array.Length == 0 || radius <= 0) return;

        float step = 360f / array.Length;
        float j = step;

        for (int i = 0; i < array.Length; i++)
        {
            float angle = j * Mathf.PI / 180f;
            float x = (center.x + radius * Mathf.Cos(angle));
            float z = (center.z + radius * Mathf.Sin(angle));
            array[i].position = new Vector3(x, center.y, z);
            j += step;
        }
    }

    /// <summary>
    /// Выстроить объекты по кругу, где радиус круга будет рассчитан, относительно размера объекта.
    /// </summary>
    /// <param name="center">Центр круга.</param>
    /// <param name="array">Массив объектов.</param>
    /// <param name="size">Размер объекта.</param>
    public static void CreateCircleBySize(Vector3 center, Transform[] array, float size)
    {
        if (array.Length == 0 || size <= 0) return;
        float radius = (array.Length / Mathf.PI) / 2f * size;
        CreateCircleByRadius(center, array, radius);
    }

    /// <summary>
    /// Вычислить длину массива перечислений.
    /// </summary>
    /// <typeparam name="T">Класс Enum</typeparam>
    public static int EnumToLength<T>() where T : System.Enum
    {
        return System.Enum.GetNames(typeof(T)).Length;
    }

    /// <summary>
    /// Установить базовый цвет материала.
    /// </summary>
    /// <param name="mesh">Текущий MeshRenderer.</param>
    /// <param name="props">Текущий PropertyBlock.</param>
    /// <param name="color">Желаемый базовый цвет.</param>
    public static void SetMaterialPropertyColor(MeshRenderer mesh, MaterialPropertyBlock props, Color color)
    {
        if (!isSetup) Init();
        mesh.GetPropertyBlock(props);
        props.SetColor(props_Color, color);
        mesh.SetPropertyBlock(props);
    }

    /// <summary>
    /// Установить базовый цвет и эмиссию материала.
    /// </summary>
    /// <param name="mesh">Текущий MeshRenderer.</param>
    /// <param name="props">Текущий PropertyBlock.</param>
    /// <param name="color">Желаемый базовый цвет.</param>
    /// <param name="emission">Желаемый цвет эмиссии.</param>
    public static void SetMaterialPropertyColor(MeshRenderer mesh, MaterialPropertyBlock props, Color color, Color emission)
    {
        if (!isSetup) Init();
        mesh.GetPropertyBlock(props);
        props.SetColor(props_Color, color);
        props.SetColor(props_EmissionColor, emission);
        mesh.SetPropertyBlock(props);
    }

    /// <summary>
    /// Получить базовый цвет материала.
    /// </summary>
    /// <param name="mesh">Текущий MeshRenderer.</param>
    /// <param name="props">Текущий PropertyBlock.</param>
    public static Color GetMaterialPropertyColor(MeshRenderer mesh, MaterialPropertyBlock props)
    {
        if (!isSetup) Init();
        mesh.GetPropertyBlock(props);
        return props.GetColor(props_Color);
    }

    /// <summary>
    /// Получить цвет эмиссии материала.
    /// </summary>
    /// <param name="mesh">Текущий MeshRenderer.</param>
    /// <param name="props">Текущий PropertyBlock.</param>
    public static Color GetMaterialPropertyEmissionColor(MeshRenderer mesh, MaterialPropertyBlock props)
    {
        if (!isSetup) Init();
        mesh.GetPropertyBlock(props);
        return props.GetColor(props_EmissionColor);
    }

    /// <summary>
    /// Получить минимальное значение массива и номер индекса этого массива.
    /// </summary>
    /// <param name="values">Текущий массив.</param>
    /// <param name="index">Возвращаемый индекс массива.</param>
    public static float Min(float[] values, out int index)
    {
        index = 0;

        int length = values.Length;

        if (length == 0)
            return 0.0f;

        float num = values[0];

        for (int i = 1; i < length; ++i)
        {
            if ((double)values[i] < (double)num)
            {
                index = i;
                num = values[i];
            }
        }

        return num;
    }

    /// <summary>
    /// Получить максимальное значение массива и номер индекса этого массива.
    /// </summary>
    /// <param name="values">Текущий массив.</param>
    /// <param name="index">Возвращаемый индекс массива.</param>
    public static float Max(float[] values, out int index)
    {
        index = 0;
        int length = values.Length;

        if (length == 0)
            return 0.0f;

        float num = values[0];

        for (int i = 1; i < length; ++i)
        {
            if ((double)values[i] > (double)num)
            {
                index = i;
                num = values[i];
            }
        }

        return num;
    }

    /// <summary>
    /// Получить максимальное значение массива и номер индекса этого массива.
    /// </summary>
    /// <param name="values">Текущий массив.</param>
    /// <param name="index">Возвращаемый индекс массива.</param>
    public static int Max(int[] values, out int index)
    {
        index = 0;
        int length = values.Length;

        if (length == 0)
            return 0;

        int num = values[0];

        for (int i = 1; i < length; ++i)
        {
            if (values[i] > num)
            {
                index = i;
                num = values[i];
            }
        }

        return num;
    }

    /// <summary>
    /// Получить минимальное значение массива и номер индекса этого массива.
    /// </summary>
    /// <param name="values">Текущий массив.</param>
    /// <param name="index">Возвращаемый индекс массива.</param>
    public static int Min(int[] values, out int index)
    {
        index = 0;
        int length = values.Length;

        if (length == 0)
            return 0;

        int num = values[0];

        for (int i = 1; i < length; ++i)
        {
            if ((double)values[i] < (double)num)
            {
                index = i;
                num = values[i];
            }
        }

        return num;
    }

    /// <summary>
    /// Аналог функции Vector3.MoveTowards, но с возвратом magnitude вектора. Если цель достигнута magnitude будет равен нулю.
    /// </summary>
    /// <param name="current">Текущая позиция.</param>
    /// <param name="target">Точка назначения.</param>
    /// <param name="maxDistanceDelta">Шаг перемещения за вызов.</param>
    /// <param name="magnitude">Возвращаемое значение.</param>
    public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta, out float magnitude)
    {
        Vector3 a = target - current;
        magnitude = a.magnitude;
        
        if (magnitude <= maxDistanceDelta || magnitude == 0f)
        {
            magnitude = 0;
            return target;
        }
        
        return current + a / magnitude * maxDistanceDelta;
    }

    /// <summary>
    /// Проверить слой GameObject, есть ли он в списке LayerMask.
    /// </summary>
    /// <param name="layer">Номер слоя.</param>
    /// <param name="layerMask">Проверяемый LayerMask.</param>
    public static bool CheckObjectLayer(int layer, LayerMask layerMask)
    {
        if (((1 << layer) & layerMask) != 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Проверить наличие тега в указанном массиве.
    /// </summary>
    /// <param name="tag">Текущий тег.</param>
    /// <param name="tagList">Проверяемый массив.</param>
    public static bool CheckObjectTag(string tag, string[] tagList)
    {
        foreach (string l in tagList)
        {
            if (tag.CompareTo(l) == 0) return true;
        }

        return false;
    }

    /// <summary>
    /// Нарисовать плавную линию с помощью модуля LineRenderer. Для работы алгоритма необходимо, чтобы на сцене присутствовала карта навигации NavMesh.
    /// </summary>
    /// <param name="start">Начало позиции.</param>
    /// <param name="end">Конец позиции.</param>
    /// <param name="lineRenderer">Целевой объект.</param>
    /// <param name="gradient">Градиент линии.</param>
    /// <param name="offset">Минимальный шаг между точками.</param>
    public static void DrawPathWithNavMesh(Vector3 start, Vector3 end, LineRenderer lineRenderer, Gradient gradient, float offset = 1)
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(start, end, -1, path);

        if (path.corners.Length < 2) return;

        List<Vector3> points = new List<Vector3>();
        points.Add(path.corners[0]);
        Vector3 lastPoint = path.corners[0];

        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(path.corners[i], lastPoint) > offset)
            {
                points.Add(path.corners[i]);
            }

            lastPoint = path.corners[i];
        }

        Vector3[] smoothedPoints = SmoothLine(points.ToArray(), .15f);
        lineRenderer.colorGradient = gradient;
        lineRenderer.positionCount = smoothedPoints.Length;
        lineRenderer.SetPositions(smoothedPoints);
        lineRenderer.gameObject.SetActive(true);
    }

    /// <summary>
    /// Нарисовать плавную линию с помощью модуля LineRenderer. Для работы алгоритма необходимо, чтобы на сцене присутствовала карта навигации NavMesh.
    /// </summary>
    /// <param name="start">Начало позиции.</param>
    /// <param name="end">Конец позиции.</param>
    /// <param name="lineRenderer">Целевой объект.</param>
    /// <param name="offset">Минимальный шаг между точками.</param>
    public static void DrawPathWithNavMesh(Vector3 start, Vector3 end, LineRenderer lineRenderer, float offset = 1)
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(start, end, -1, path);

        if (path.corners.Length < 2) return;

        List<Vector3> points = new List<Vector3>();
        points.Add(path.corners[0]);
        Vector3 lastPoint = path.corners[0];

        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(path.corners[i], lastPoint) > offset)
            {
                points.Add(path.corners[i]);
            }

            lastPoint = path.corners[i];
        }

        Vector3[] smoothedPoints = SmoothLine(points.ToArray(), .15f);
        lineRenderer.positionCount = smoothedPoints.Length;
        lineRenderer.SetPositions(smoothedPoints);
        lineRenderer.gameObject.SetActive(true);
    }

    static Vector3[] SmoothLine(Vector3[] inputPoints, float segmentSize)
    {
        AnimationCurve curveX = new AnimationCurve();
        AnimationCurve curveY = new AnimationCurve();
        AnimationCurve curveZ = new AnimationCurve();

        Keyframe[] keysX = new Keyframe[inputPoints.Length];
        Keyframe[] keysY = new Keyframe[inputPoints.Length];
        Keyframe[] keysZ = new Keyframe[inputPoints.Length];

        for (int i = 0; i < inputPoints.Length; i++)
        {
            keysX[i] = new Keyframe(i, inputPoints[i].x);
            keysY[i] = new Keyframe(i, inputPoints[i].y);
            keysZ[i] = new Keyframe(i, inputPoints[i].z);
        }

        curveX.keys = keysX;
        curveY.keys = keysY;
        curveZ.keys = keysZ;

        for (int i = 0; i < inputPoints.Length; i++)
        {
            curveX.SmoothTangents(i, 0);
            curveY.SmoothTangents(i, 0);
            curveZ.SmoothTangents(i, 0);
        }

        List<Vector3> lineSegments = new List<Vector3>();

        for (int i = 0; i < inputPoints.Length; i++)
        {
            lineSegments.Add(inputPoints[i]);

            if (i + 1 < inputPoints.Length)
            {
                float distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i + 1]);
                int segments = (int)(distanceToNext / segmentSize);

                for (int s = 1; s < segments; s++)
                {
                    float time = (s / (float)segments) + (float)i;
                    Vector3 newSegment = new Vector3(curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time));
                    lineSegments.Add(newSegment);
                }
            }
        }

        return lineSegments.ToArray();
    }

    /// <summary>
    /// Проверить, точка внутри окна RectTransform или нет.
    /// </summary>
    /// <param name="position">Проверяемая точка.</param>
    /// <param name="rectTransform">Целевой объект.</param>
    public static bool IsPointInside(Vector2 position, RectTransform rectTransform)
    {
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);
        if (position.x > worldCorners[0].x && position.x < worldCorners[2].x
            && position.y > worldCorners[0].y && position.y < worldCorners[2].y)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Заменить обозначение новой линии "\n" на "<br>".
    /// </summary>
    /// <param name="str">Проверяемый текст.</param>
    public static string ReplaceNewLine(string str)
    {
        return Regex.Replace(str, "\n", "<br>");
    }

    /// <summary>
    /// Заменить HTML код "<br>" на "\n".
    /// </summary>
    /// <param name="str">Проверяемый текст.</param>
    public static string ReplaceBR(string str)
    {
        return Regex.Replace(str, "<br>", "\n");
    }

    /// <summary>
    /// Загрузить сцену по имени, если это возможно.
    /// </summary>
    /// <param name="name">Имя сцены.</param>
    public static void LoadSceneNow(string name)
    {
        if (Application.CanStreamedLevelBeLoaded(name))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(name);
        }
    }

    /// <summary>
    /// Конвертировать текст в значение Enum.
    /// </summary>
    /// <typeparam name="T">Целевой Enum.</typeparam>
    /// <param name="value">Текст.</param>
    public static T ParseEnum<T>(string value) where T : System.Enum
    {
        return (T)System.Enum.Parse(typeof(T), value, true);
    }

    /// <summary>
    /// Округлить значение до десятых, сотых, тысячных и т.д.
    /// </summary>
    /// <param name="f">Текущее значение.</param>
    /// <param name="to">Округляем до.</param>
    public static float RoundTo(float f, int to)
    {
        return Mathf.Round(f * (float)to) / (float)to;
    }

    /// <summary>
    /// Генерировать MD5 хеш код.
    /// </summary>
    /// <param name="str">Текст.</param>
    public static string Hash(string str)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in GetHash(str))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }

    static byte[] GetHash(string str)
    {
        HashAlgorithm algorithm = MD5.Create();
        return algorithm.ComputeHash(Encoding.UTF8.GetBytes(str));
    }
}
