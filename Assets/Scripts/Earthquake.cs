using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class Earthquake : MonoBehaviour
{
    public ShakeData explosionShakeData;
    public ParticleSystem debrisParticles;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))    
        {
            CameraShakerHandler.Shake(explosionShakeData);

            debrisParticles.Stop(); 
            debrisParticles.Play();
        }
    }
}