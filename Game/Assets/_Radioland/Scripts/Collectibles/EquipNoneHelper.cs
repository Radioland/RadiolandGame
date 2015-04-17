using UnityEngine;
using System.Collections;

public class EquipNoneHelper : MonoBehaviour
{
    public void EquipNone() {
        CosmeticsManager.UnequipAll();
    }
}
