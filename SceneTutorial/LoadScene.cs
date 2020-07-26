using System.Collections;
using GlobalEnums;
using UnityEngine;

namespace SceneTutorial
{
    public class LoadScene : MonoBehaviour
    {
        private string scene;
        private void Start()
        {
            On.GameManager.EnterHero += GameManagerOnEnterHero;
            On.SceneManager.Start += SceneManagerOnStart;
        }

        private void SceneManagerOnStart(On.SceneManager.orig_Start orig, SceneManager self)
        {
            if (scene == "NewRoom")
            {
                self.sceneType = SceneType.GAMEPLAY;
                self.mapZone = MapZone.ROYAL_GARDENS;
                self.darknessLevel = 0;
                self.saturation = 0.9f;
                self.ignorePlatformSaturationModifiers = false;
                self.isWindy = false;
                self.isTremorZone = false;
                self.environmentType = 0;
                self.noParticles = false;
                self.overrideParticlesWith = MapZone.NONE;
                self.defaultColor = new Color(210f / 255f, 1f, 234f / 255f, 1f);
                self.defaultIntensity = 0.8f;
                self.heroLightColor = new Color(196f / 255f, 1f, 214f / 255f, 136f / 255f);
            }

            orig(self);
        }
        
        private void GameManagerOnEnterHero(On.GameManager.orig_EnterHero orig, GameManager self, bool additivegatesearch)
        {
            scene = self.sceneName;
            Modding.Logger.Log("NAME " + self.sceneName);
            if (self.sceneName == "GG_Atrium")
            {
                CreateGateway("right test", 
                    new Vector2(115f, 60.6f), new Vector2(1f,4f), 
                    "NewRoom", "left test",
                    true,false,
                    GameManager.SceneLoadVisualizations.Default);
            }
            else if (self.sceneName == "NewRoom")
            {
                Modding.Logger.Log("MADE1");
                CreateGateway("left test", 
                    new Vector2(0.5f,6f), new Vector2(1f,4f), 
                    "GG_Atrium", "right test",
                    false,true,
                    GameManager.SceneLoadVisualizations.Default);
                Modding.Logger.Log("MADE1");
            }

            orig(self, additivegatesearch);
        }

        private void CreateGateway(string gateName, Vector2 pos, Vector2 size, string toScene, string entryGate,
            bool right, bool left, GameManager.SceneLoadVisualizations vis)
        {
            GameObject gate = new GameObject(gateName);
            gate.transform.SetPosition2D(pos);
            var tp = gate.AddComponent<TransitionPoint>();
            
            var bc = gate.AddComponent<BoxCollider2D>();
            bc.size = size;
            bc.isTrigger = true;
            tp.targetScene = toScene;
            tp.entryPoint = entryGate;
            
            tp.alwaysEnterLeft = left;
            tp.alwaysEnterRight = right;
            GameObject rm = new GameObject("Hazard Respawn Marker");
            rm.transform.parent = tp.transform;
            rm.transform.position = new Vector2(rm.transform.position.x - 3f, rm.transform.position.y);
            var tmp = rm.AddComponent<HazardRespawnMarker>();
            tp.respawnMarker = rm.GetComponent<HazardRespawnMarker>();
            tp.sceneLoadVisualization = vis;
            
        }
    }
}