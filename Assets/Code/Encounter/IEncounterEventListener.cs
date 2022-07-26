using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serves two functions:
// 1. marker interface - only monobehaviours with this interfaces get picked up by the encounter asset
// 2. notification about event happening, no restriction to implement all encounter event types

public interface IEncounterEventListener
{
    void OnEncounterEvent(EncounterEventType eventType);
}