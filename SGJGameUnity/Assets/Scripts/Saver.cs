using UnityEngine;

public abstract class Saver : MonoBehaviour
{
    public string uniqueIdentifier;
    public string objectType;
    public SaveData saveData;


    protected string key;


    private SceneController sceneController;


    private void Awake()
    {
        sceneController = FindObjectOfType<SceneController>();

        //if(!sceneController)
        //    throw new UnityException("Scene Controller could not be found, ensure that it exists in the Persistent scene.");
        
        key = SetKey ();
    }


    private void OnEnable()
    {
        //SaveController.instance.RegisterObject(gameObject, objectType, true);
        if (sceneController)
        {
            sceneController.Saving += Save;
            sceneController.Loading += Load;
        }
    }


    private void OnDisable()
    {
        //SaveController.instance.RegisterObject(gameObject, objectType, false);
        if (sceneController)
        {
            sceneController.Saving -= Save;
            sceneController.Loading -= Load;
        }
    }


    protected abstract string SetKey ();


    protected abstract void Save ();


    protected abstract void Load ();
}
