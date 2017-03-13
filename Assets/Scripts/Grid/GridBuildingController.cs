using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GridBuildingController : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    public GridInfo.CellData buildingInfo;

    public Material creationMaterial;
    public Material destroyingMaterial;
    public Material whiteMaterial;

    private Vector3 originalPosition;

    private float creationLength = 2f;
    private Dictionary<Renderer, Material> defaultMaterialMap;
    private bool isInAnimation = false;

    private Func<Renderer, Material> currentMapFunction;

    void OnEnable()
    {
        defaultMaterialMap = new Dictionary<Renderer, Material>();
        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            defaultMaterialMap[r] = r.material;
        }

        currentMapFunction = getDefaultMaterial;
    }

	void Start () {
        tag = "clickable";
        originalPosition = transform.position;

        //Setup box collider
        var collider = gameObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(35f/transform.localScale.x, 24f / transform.localScale.y, 35f / transform.localScale.z);
        collider.center = new Vector3(0, 12f / transform.localScale.y, 0);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void MarkObject(bool selected)
    {
        //var material = selected ? materialAfterSelection : materialBeforeSelection;
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.material.color = (selected ? Color.yellow : Color.white);
        }
    }

    public void AllowTextures(bool value)
    {
        if (value)
            currentMapFunction = getDefaultMaterial;
        else
            currentMapFunction = getWhiteMaterial;

        UpdateMaterials();
    }

    private Material getDefaultMaterial(Renderer r)
    {
        return defaultMaterialMap[r];
    }

    private Material getWhiteMaterial(Renderer r)
    {
        return whiteMaterial;
    }

    public void StartCreationAnimation()
    {
        StartCoroutine(MaterialAnimation(creationMaterial));
    }

    public void StartDestroyingAnimation(Action callback)
    {
        StartCoroutine(DestroyAfterAnimation(MaterialAnimation(destroyingMaterial), callback));
    }

    private IEnumerator DestroyAfterAnimation(IEnumerator animation, Action callback)
    {
        yield return StartCoroutine(animation);
        callback();
    }

    private IEnumerator MaterialAnimation(Material material)
    {
        var dict = new Dictionary<Renderer, Material>();
        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach(var r in renderers)
        {
            r.material = material;
        }

        isInAnimation = true;
        yield return new WaitForSeconds(creationLength);
        isInAnimation = false;
        UpdateMaterials();
        
    }

    private void UpdateMaterials()
    {
        if(isActiveAndEnabled && !isInAnimation)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                var m = currentMapFunction(r);
                r.material = m;
            }
        }
    }
}
