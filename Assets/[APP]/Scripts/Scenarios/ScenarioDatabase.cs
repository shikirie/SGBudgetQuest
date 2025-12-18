using System.Collections.Generic;
using UnityEngine;
using Modules;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = nameof(ScenarioDatabase), menuName = "[APP]/Database/ScenarioDatabase")]
public class ScenarioDatabase : BaseDatabase<ScenarioData>
{
    [System.Serializable]
    public class ScenarioPair : DatabaseItemPair<ScenarioData>
    {
        public ScenarioPair(string key, ScenarioData value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [Header("Scenarios")]
    [SerializeField] private List<ScenarioPair> items = new List<ScenarioPair>();

    private readonly Dictionary<string, ScenarioData> lookup = new Dictionary<string, ScenarioData>();

    public override ScenarioData[] GetAllItems()
    {
        ScenarioData[] arr = new ScenarioData[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            arr[i] = items[i]?.Value;
        }
        return arr;
    }

    public override ScenarioData GetItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return null;
        return items[index]?.Value;
    }

    public override ScenarioData GetItem(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        EnsureIndex();
        if (lookup.TryGetValue(id, out var val))
            return val;

        return null;
    }

    public override ScenarioData GetRandom()
    {
        if (items == null || items.Count == 0)
            return null;
        int idx = UnityEngine.Random.Range(0, items.Count);
        return items[idx]?.Value;
    }

#if UNITY_EDITOR
    protected override void Add(ScenarioData value)
    {
        if (value == null)
            return;

        var key = GetKey(value);
        if (string.IsNullOrEmpty(key))
            return;

        // Prevent duplicates by key
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].Key == key)
                return;
        }

        items.Add(new ScenarioPair(key, value));
        // update lookup so runtime queries work without extra rebuilds
        if (!lookup.ContainsKey(key))
            lookup.Add(key, value);
    }

    protected override void Clear()
    {
        items.Clear();
        lookup.Clear();
    }

    protected override void Sort()
    {
        items.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;
            return string.Compare(a.Key, b.Key, System.StringComparison.Ordinal);
        });

        // Rebuild lookup to match sorted items
        RebuildIndex();
    }
#endif

    private static string GetKey(ScenarioData data)
    {
        return data != null ? data.ScenarioId : null;
    }

    private void EnsureIndex()
    {
        if (lookup.Count == items.Count)
            return;
        RebuildIndex();
    }

    private void RebuildIndex()
    {
        lookup.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            var p = items[i];
            if (p == null || p.Value == null)
                continue;
            var key = p.Key;
            if (string.IsNullOrEmpty(key))
                key = GetKey(p.Value);
            if (string.IsNullOrEmpty(key))
                continue;
            if (!lookup.ContainsKey(key))
                lookup.Add(key, p.Value);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScenarioDatabase))]
public class ScenarioDatabaseEditor : Editor
{
    private ScenarioDatabase script;

    private void OnEnable()
    {
        script = (ScenarioDatabase)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.Space();
        if (GUILayout.Button("Setup"))
        {
            script.Setup();
            EditorUtility.SetDirty(script);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
