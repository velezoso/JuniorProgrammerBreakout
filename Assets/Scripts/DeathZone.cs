using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.JuniorProgrammer.Breakout
{
    public class DeathZone : MonoBehaviour
    {
        public LevelController levelController;

        private void OnCollisionEnter(Collision other)
        {
            Destroy(other.gameObject);
            levelController.GameOver();
        }
    }
}