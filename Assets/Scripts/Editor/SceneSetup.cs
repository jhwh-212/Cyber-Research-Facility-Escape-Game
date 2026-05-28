using UnityEngine;
using UnityEditor;

public class SceneSetup : EditorWindow
{
    [MenuItem("Tools/Setup Escape Game Scene")]
    public static void SetupScene()
    {
        // ── Tags ──────────────────────────────────────────────────────────────
        AddTag("Player");

        // ── GameManager ───────────────────────────────────────────────────────
        GameObject gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();

        // ── Ghost (Player) ────────────────────────────────────────────────────
        GameObject ghost = new GameObject("Ghost");
        ghost.tag = "Player";
        ghost.AddComponent<SpriteRenderer>();
        var rb = ghost.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        var ghostCol = ghost.AddComponent<CircleCollider2D>();
        ghostCol.isTrigger = false;
        ghost.AddComponent<PlayerController>();
        ghost.transform.position = Vector3.zero;

        // ── Light 1 ───────────────────────────────────────────────────────────
        GameObject light1 = CreateLight("Light_1", new Vector3(-3f, 2f, 0f));

        // ── Light 2 ───────────────────────────────────────────────────────────
        GameObject light2 = CreateLight("Light_2", new Vector3(3f, 2f, 0f));

        // ── Switch ────────────────────────────────────────────────────────────
        GameObject sw = new GameObject("Switch");
        sw.AddComponent<SpriteRenderer>();
        var swCol = sw.AddComponent<BoxCollider2D>();
        swCol.isTrigger = true;
        var switchScript = sw.AddComponent<Switch>();
        switchScript.controlledLights = new GameObject[] { light1, light2 };
        sw.transform.position = new Vector3(0f, -3f, 0f);

        // ── Souls (5) ─────────────────────────────────────────────────────────
        Vector3[] soulPositions = new Vector3[]
        {
            new Vector3(-4f,  0f, 0f),
            new Vector3( 4f,  0f, 0f),
            new Vector3( 0f,  3f, 0f),
            new Vector3(-2f, -2f, 0f),
            new Vector3( 2f, -2f, 0f),
        };

        for (int i = 0; i < soulPositions.Length; i++)
        {
            GameObject soul = new GameObject("Soul_" + (i + 1));
            soul.AddComponent<SpriteRenderer>();
            var soulCol = soul.AddComponent<CircleCollider2D>();
            soulCol.isTrigger = true;
            soul.AddComponent<Soul>();
            soul.transform.position = soulPositions[i];
        }

        // Set total souls on GameManager
        gm.GetComponent<GameManager>().totalSouls = soulPositions.Length;

        // ── Main Camera ───────────────────────────────────────────────────────
        GameObject camObj = GameObject.FindWithTag("MainCamera");
        if (camObj == null)
        {
            camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            var cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6f;
        }
        var follow = camObj.GetComponent<CameraFollow>() ?? camObj.AddComponent<CameraFollow>();
        follow.target = ghost.transform;
        follow.offset = new Vector3(0f, 0f, -10f);
        follow.smoothSpeed = 5f;
        camObj.transform.position = new Vector3(0f, 0f, -10f);

        Debug.Log("Scene setup complete! Assign sprites in the Inspector and press Play.");
        EditorUtility.DisplayDialog("Setup Complete",
            "All GameObjects created!\n\nNext steps:\n" +
            "1. Assign sprites to each SpriteRenderer\n" +
            "2. Assign UI Text to GameManager\n" +
            "3. Press Play", "OK");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static GameObject CreateLight(string name, Vector3 pos)
    {
        GameObject light = new GameObject(name);
        light.AddComponent<SpriteRenderer>();
        light.AddComponent<LightDetection>();
        light.transform.position = pos;

        // Detection zone child
        GameObject zone = new GameObject("DetectionZone");
        zone.transform.SetParent(light.transform);
        zone.transform.localPosition = Vector3.zero;
        zone.AddComponent<SpriteRenderer>(); // assign cone sprite here
        var col = zone.AddComponent<PolygonCollider2D>();
        col.isTrigger = true;
        // Default cone shape pointing downward
        col.SetPath(0, new Vector2[]
        {
            new Vector2(0f,    0f),
            new Vector2(-1.5f, -3f),
            new Vector2( 1.5f, -3f),
        });

        return light;
    }

    static void AddTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tags = tagManager.FindProperty("tags");

        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return; // already exists

        tags.arraySize++;
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }
}
