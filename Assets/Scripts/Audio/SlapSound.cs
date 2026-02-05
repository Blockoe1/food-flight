/*****************************************************************************
// File Name : SlapSound.cs
// Author : Brandon Koederitz
// Creation Date : 2/5/2026
// Last Modified : 2/5/2026
//
// Brief Description :  Plays the slap sound when the player slaps an opponent.
*****************************************************************************/
using CustomAttributes;
using NUnit.Framework;
using UnityEngine;

namespace FoodFlight
{
    public class SlapSound : MonoBehaviour
    {
        [SerializeField] private AudioRelay audioRelay;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Slapable slapable))
            {
                audioRelay.Play();
            }
        }
    }
}
