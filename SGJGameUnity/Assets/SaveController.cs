using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveController : MonoBehaviour {
    public static SaveController instance;

    public SaveData saveData;
    public bool doLoad = false;

    [System.Serializable]
    public class ObjectPrefabEntry
    {
        public string objectType;
        public GameObject obj;
    }
    public ObjectPrefabEntry[] knownObjects;

    private List<ObjectPrefabEntry> registeredObjects = new List<ObjectPrefabEntry>();

    void Awake()
    {
        if (SaveController.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        SaveController.instance = this;
    }

    ObjectPrefabEntry findPrefabByType(string objectType)
    {
        foreach (var objectEntry in knownObjects)
        {
            if (objectEntry.objectType != objectType)
                continue;

            return objectEntry;
        }

        throw new System.Exception("Unknown object type: " + objectType);
    }

    ObjectPrefabEntry findEntryByObject(GameObject obj)
    {
        foreach (var objectEntry in knownObjects)
        {
            if (objectEntry.obj != obj)
                continue;

            return objectEntry;
        }

        Debug.LogError("Object is not registered.", obj);
        throw new System.Exception("Unknown object.");
    }

    public void RegisterObject(GameObject obj, string objectType, bool register)
    {
        var prefabEntry = findPrefabByType(objectType);

        if (register)
        {
            registeredObjects.Add(new ObjectPrefabEntry { objectType = objectType, obj = obj });
            return;
        }

        for (var i = 0; i < registeredObjects.Count; ++i)
        {
            var existing = registeredObjects[i];
            if (existing.obj != obj)
                continue;

            registeredObjects.RemoveAt(i);
            return;
        }
        //Debug.LogError("Object is not registered: " + objectType, obj);
    }

    public void Load()
    {
        foreach (var existing in registeredObjects)
        {
            Destroy(existing.obj);
        }
        registeredObjects = new List<ObjectPrefabEntry>();

        var objTypes = new List<string>();
        var objPositions = new List<Vector3>();
        saveData.Load("object-types", ref objTypes);
        saveData.Load("object-positions", ref objPositions);

        for (var i = 0; i < objTypes.Count; ++i)
        {
            var objType = objTypes[i];
            var objPos = objPositions[i];
            var prefabEntry = findPrefabByType(objType);
            var obj = Instantiate(prefabEntry.obj, objPos, Quaternion.identity);
            var objSaver = obj.GetComponent<Saver>();
            objSaver.uniqueIdentifier = i.ToString();
        }
    }

    public void Save()
    {
        var objTypes = new List<string>();
        var objPositions = new List<Vector3>();

        foreach (var existing in registeredObjects)
        {
            objTypes.Add(existing.objectType);
            objPositions.Add(existing.obj.transform.position);
        }

        saveData.Save("object-types", objTypes);
        saveData.Save("object-positions", objPositions);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (doLoad)
        {
            doLoad = false;
            //Load();
        }
	}
}
