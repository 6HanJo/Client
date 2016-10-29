﻿/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnergyBarToolkit {

public class EnergyBarSpawnerUGUI : MonoBehaviour {

    #region Public Fields

    public EnergyBarUGUIBase barPrefab;
    public Transform attachPoint;

    public ObjectFinder canvas = new ObjectFinder(typeof(Canvas), "/Canvas", "", ObjectFinder.Method.ByPath);
    public ObjectFinder worldCamera = new ObjectFinder(typeof(Camera), "/Main Camera", "MainCamera", ObjectFinder.Method.ByTag);

    public bool updateLookupReference;

    public bool networkInstantiate;
    public int networkGroup;

    [HideInInspector] [NonSerialized]
    public EnergyBarUGUIBase instance;

    #endregion

    #region Private Fields

    #endregion

    #region Public Methods

    public static EnergyBarUGUIBase Instantiate(Object parent, EnergyBarUGUIBase barPrefab, ObjectFinder canvasFinder, ObjectFinder cameraFinder, Transform attachPoint, bool networkInstantiate, int networkGroup, bool updateLookupReference) {
        EnergyBarUGUIBase bar;

#if !(UNITY_FLASH || UNITY_NACL || UNITY_METRO || UNITY_WP8 || UNITY_WEBGL)
        if (networkInstantiate) {
            bar = Network.Instantiate(barPrefab, barPrefab.transform.position, barPrefab.transform.rotation, networkGroup) as EnergyBarUGUIBase;
        } else {
            bar = Instantiate(barPrefab) as EnergyBarUGUIBase;
        }
#else
        bar = Instantiate(barPrefab) as EnergyBarUGUIBase;
#endif
        if (canvasFinder.chosenMethod == ObjectFinder.Method.ByType) {
            bar.transform.SetParent((canvasFinder.Lookup<Canvas>(parent)).transform, true);
        } else {
            bar.transform.SetParent((canvasFinder.Lookup<Transform>(parent)), true);
        }

        var followObject = bar.GetComponent<EnergyBarFollowObject>();
        if (followObject == null) {
            followObject = bar.gameObject.AddComponent<EnergyBarFollowObject>();
        }

        followObject.worldCamera = cameraFinder;
        followObject.followObject = attachPoint.gameObject;
        followObject.updateLookupReference = updateLookupReference;

        return bar;
    }

    #endregion

    #region Unity Methods

    void OnEnable() {
        if (instance == null) {
            instance = Instantiate(this, barPrefab, canvas, worldCamera, attachPoint, networkInstantiate, networkGroup, updateLookupReference);
        }

        if (instance != null) {
            instance.gameObject.SetActive(true);
        }
    }

    void OnDisable() {
        if (instance != null) {
            instance.gameObject.SetActive(false);
        }
    }

    void OnDestroy() {
        if (instance != null) {

#if !(UNITY_FLASH || UNITY_NACL || UNITY_METRO || UNITY_WP8 || UNITY_WEBGL)
            if (networkInstantiate) {
                Network.Destroy(instance.gameObject);
            } else {
                Destroy(instance.gameObject);
            }
#else
            Destroy(instance.gameObject);
#endif
        }
    }

    #endregion

    #region Inner and Anonymous Classes

    public enum FindCanvasMethod {
        ByTag,
        ByPath,
        ByType,
    }

    #endregion
}

}