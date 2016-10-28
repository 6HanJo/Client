/// <Licensing>
/// ©2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

namespace PathologicalGames
{
    /// <summary>
	///	Contains only an EventInfoList list, like EventFireControllers have, but for easy use by 
	/// any script.
    /// </summary>
	[AddComponentMenu("Path-o-logical/TriggerEventPRO/Utilities/EventInfoList (Standalone)")]
    public class EventInfoListStandalone : MonoBehaviour
    {
        /// <summary>
		/// A list of EventInfo structs which hold one or more descriptions
        /// of how a Target can be affected. To alter this from code. Get the list, edit it, then 
        /// set the whole list back. (This cannot be edited "in place").
        /// </summary>
		// Encodes / Decodes EventInfos to and from EventInfoGUIBackers
        public EventInfoList eventInfoList
        {
            get
            {
                var returnInfoList = new EventInfoList();
                foreach (var infoBacker in this._eventInfoListGUIBacker)
                {
                    // Create and add a struct-form of the backing-field instance
                    returnInfoList.Add
                    (
                        new EventInfo
                        {
                            name = infoBacker.name,
                            value = infoBacker.value,
                            duration = infoBacker.duration,
                        }
                    );
                }

                return returnInfoList;
            }

            set
            {
                // Clear and set the backing-field list also used by the GUI
                this._eventInfoListGUIBacker.Clear();

                EventInfoListGUIBacker infoBacker;
                foreach (var info in value)
                {
                    infoBacker = new EventInfoListGUIBacker(info);
                    this._eventInfoListGUIBacker.Add(infoBacker);
                }
            }
        }
		/// <summary>
		/// Public for Inspector use only.
		/// </summary>
		public List<EventInfoListGUIBacker> _eventInfoListGUIBacker = new List<EventInfoListGUIBacker>();
		
        // Keeps the state of each individual foldout item during the editor session
        public Dictionary<object, bool> _inspectorListItemStates = new Dictionary<object, bool>();
    }
}