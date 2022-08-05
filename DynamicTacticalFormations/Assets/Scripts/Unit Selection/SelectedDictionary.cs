using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This script is responsible for handling the dictionary of selected units.
It can:
    * Add a gameObject to the dictionary
    * Remove a gameObject from the dictionary
    * Remove all gameObject from the dictionary

It also adds a selected component to each selected game object.

*/


public class SelectedDictionary : MonoBehaviour
{
    public Dictionary<int, GameObject> g_SelectedTable = new Dictionary<int, GameObject>();

    /// <summary>
    /// Adds a gameObject to the dictionary. Adds a selectedComponent script.
    /// </summary>
    /// <param name ="go"></param>
    public void AddSelected(GameObject go)
    {
        int id = go.GetInstanceID();

        if (!(g_SelectedTable.ContainsKey(id)))
        {
            g_SelectedTable.Add(id, go);
            go.AddComponent<SelectedComponent>();
            //Debug.Log("Added " + id + " to selected dict");
        }
    }

    /// <summary>
    /// Removes selectionComponent script. Removes gameObject from dictionary based on id
    /// </summary>
    /// <param name="id"></param>
    public void Deselect(int id)
    {
        Destroy(g_SelectedTable[id].GetComponent<SelectedComponent>());
        g_SelectedTable.Remove(id);
    }

    /// <summary>
    /// Removes the selectionComponent script from all gameObjects in the dictionary. Clears dictionary.
    /// </summary>
    public void DeselectAll()
    {
        foreach(KeyValuePair<int,GameObject> pair in g_SelectedTable)
        {
            if(pair.Value != null)
            {
                Destroy(g_SelectedTable[pair.Key].GetComponent<SelectedComponent>());
            }
        }
        g_SelectedTable.Clear();
    }
}
