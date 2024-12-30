using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BlueEyes.Interactions;
using BlueEyes.NarrativeBrain;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeggarsStealer_Road96_Mod
{
    [BepInEx.BepInPlugin(mod_guid, "BeggarsStealer", version)]
    [BepInEx.BepInProcess("Road 96.exe")]
    public class BeggarsStealerMod : BasePlugin
    {
        private const string mod_guid = "miroxy12.beggarsstealer";
        private const string version = "1.1";
        private readonly Harmony harmony = new Harmony(mod_guid);
        internal static new ManualLogSource Log;
        public static string currentscene;
        public static bool stealingfood = false;
        public static GameObject newint;
        public static GameObject pf;

        public override void Load()
        {
            Log = base.Log;
            Log.LogInfo(mod_guid + " started, version: " + version);
            harmony.PatchAll(typeof(PayCostHook));
            harmony.PatchAll(typeof(RemoveMoneyHook));
            harmony.PatchAll(typeof(BeginInteractionStateHook));
            harmony.PatchAll(typeof(LoadSceneAsyncHook));
            harmony.PatchAll(typeof(AddTextConstraintUIHook));
            AddComponent<ModMain>();
        }
        // ngl this is a bad code LOL, i'll rework all of my mods when i'll find another good way to take the logic scene and grab a gameobject from it
        // should use recursivity
        // I can't use GameObject.Find because there are multiples loaded scenes
        public static GameObject FindBeggarINT(GameObject parent)
        {
            GameObject current = null;
            for (int i = 0; i < parent.transform.childCount; i++) {
                if (parent.transform.GetChild(i).name.Contains("NPC_Beggar")) {
                    current = parent.transform.GetChild(i).gameObject;
                    break;
                }
            }
            if (current != null) {
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name == "Stuff_To_Buy") {
                        current = current.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name == "PF_Food_Buy") {
                        current = current.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name == "INT_Eat") {
                        current = current.transform.GetChild(i).gameObject;
                        return current;
                    }
                }
            } else {
                for (int i = 0; i < parent.transform.childCount; i++) {
                    if (parent.transform.GetChild(i).name.Equals("Nobodies")) {
                        current = parent.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                if (current == null) { return null; }
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name.Contains("NPC_Beggar")) {
                        current = current.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name == "Stuff_To_Buy") {
                        current = current.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name == "PF_Food_Buy") {
                        current = current.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name == "INT_Eat") {
                        current = current.transform.GetChild(i).gameObject;
                        return current;
                    }
                }
            }
            return null;
        }
        public static GameObject FindBeggarPF(GameObject parent)
        {
            GameObject current = null;
            for (int i = 0; i < parent.transform.childCount; i++) {
                if (parent.transform.GetChild(i).name.Contains("NPC_Beggar")) {
                    current = parent.transform.GetChild(i).gameObject;
                    break;
                }
            }
            if (current != null) {
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name == "Stuff_To_Buy") {
                        current = current.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name == "PF_Food_Buy") {
                        current = current.transform.GetChild(i).gameObject;
                        return current;
                    }
                }
            } else {
                for (int i = 0; i < parent.transform.childCount; i++) {
                    if (parent.transform.GetChild(i).name.Equals("Nobodies")) {
                        current = parent.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                if (current == null) { return null; }
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name.Contains("NPC_Beggar")) {
                        current = current.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name == "Stuff_To_Buy") {
                        current = current.transform.GetChild(i).gameObject;
                        break;
                    }
                }
                for (int i = 0; i < current.transform.childCount; i++) {
                    if (current.transform.GetChild(i).name == "PF_Food_Buy") {
                        current = current.transform.GetChild(i).gameObject;
                        return current;
                    }
                }
            }
            return null;
        }
    }

    public class ModMain : MonoBehaviour
    {
        void Awake()
        {
            BeggarsStealerMod.Log.LogInfo("loading BeggarsStealer");
        }
        void OnEnable()
        {
            BeggarsStealerMod.Log.LogInfo("enabled BeggarsStealer");
        }
        void Update()
        {
            if (BeggarsStealerMod.currentscene != "") {
                Scene scene = SceneManager.GetSceneByName(BeggarsStealerMod.currentscene);
                GameObject npclogic = null;
                if (scene.isLoaded) {
                    GameObject[] gos = scene.GetRootGameObjects();
                    foreach (var go in gos) {
                        if (go.name.Contains("_NPC")) {
                            npclogic = go;
                            GameObject beggarfoodint = BeggarsStealerMod.FindBeggarINT(npclogic);
                            BeggarsStealerMod.pf = BeggarsStealerMod.FindBeggarPF(npclogic);
                            if (beggarfoodint != null) {
                                //    UnityEngine.Debug.Log(beggarfoodint.name);
                                BeggarsStealerMod.newint = GameObject.Instantiate(beggarfoodint);
                                BeggarsStealerMod.newint.name = "BeggarFoodSteal_INT";
                                BeggarsStealerMod.newint.GetComponent<BlueEyes.Interactions.Interaction>()._content = "Steal food";
                            }
                            BeggarsStealerMod.currentscene = null;
                            break;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(InteractionUI), "AddTextConstraintUI", new System.Type[] { typeof(string), typeof(bool) })]
    public class AddTextConstraintUIHook
    {
        static void Prefix(InteractionUI __instance, ref string text, bool isMoney)
        {
            if (__instance._interaction.name.Equals("BeggarFoodSteal_INT")) {
                BeggarsStealerMod.newint.GetComponent<BlueEyes.Interactions.Interaction>()._areConditionsFulfiled = true;
                BeggarsStealerMod.newint.GetComponent<BlueEyes.Interactions.Interaction>()._areConstraintConditionsFulfiled = true;
                BeggarsStealerMod.newint.GetComponent<BlueEyes.Interactions.Interaction>()._areStateConditionsFulfiled = true;
                text = "-0.2 Karma";
            }
        }
    }
    [HarmonyPatch(typeof(BlueEyes.Interactions.Interaction), "BeginInteractionState", new System.Type[] { typeof(InteractionController) })]
    public class BeginInteractionStateHook
    {
        static void Prefix(BlueEyes.Interactions.Interaction __instance, InteractionController controller)
        {
            BeggarsStealerMod.stealingfood = false;
            if (__instance?.name == "BeggarFoodSteal_INT") {
                BeggarsStealerMod.stealingfood = true;
            }
        }
    }
    [HarmonyPatch(typeof(RessourceImpact), "PayCost", new System.Type[] { typeof(NarrativeContext), typeof(NarrativeSettings), typeof(bool) })]
    public class PayCostHook
    {
        static void Prefix(RessourceImpact __instance, NarrativeContext context, NarrativeSettings settings, bool skipFeedback)
        {
            if (__instance.name.Contains("Eat") && !BeggarsStealerMod.stealingfood) {
                BeggarsStealerMod.newint.SetActive(false);
            }
            if (__instance.name.Contains("Eat") && BeggarsStealerMod.stealingfood) {
                context.ImpactKarma(-0.20f, settings, false);
                BeggarsStealerMod.pf.SetActive(false);
            }
        }
    }
    [HarmonyPatch(typeof(NarrativeContext), "RemoveMoney", new System.Type[] { typeof(int) })]
    public class RemoveMoneyHook
    {
        static void Prefix(NarrativeContext __instance, ref int amount)
        {
            if (BeggarsStealerMod.stealingfood) {
                amount = 0;
            }
        }
    }
    [HarmonyPatch(typeof(SceneManager), "LoadSceneAsync", new System.Type[] { typeof(string), typeof(LoadSceneMode) })]
    public class LoadSceneAsyncHook
    {
        static void Postfix(string sceneName, LoadSceneMode mode)
        {
            // These are scenes that are also logic scenes but without the "_Logic" at the end of the name
            string[] logicscene = {
                    "000_Game/Scenes/JAROD_9/JAROD_9",
                    "000_Game/Scenes/ZOE_4/ZOE_4",
                    "000_Game/Scenes/BORDERS/BORDER_ZOE",
                    "000_Game/Scenes/BORDERS/BorderExit_Zoe/BORDEREXIT_ZOE",
                    "000_Game/Scenes/BORDERS/BorderExit_CreditGold/BorderExit_CreditGold",
                    "000_Game/Scenes/SONYA_8/SONYA_8",
                    "000_Game/Scenes/BORDERS/BORDER_FINAL",
                    "000_Game/Scenes/ZOE_1/ZOE_1",
                    "000_Game/Scenes/ALEX_7/ALEX_7",
                    "000_Game/Scenes/FANNY_9/FANNY_9",
                    "000_Game/Scenes/STANMITCH_9/STANMITCH_9"
            };
            string[] tokens = null;
            bool islogicscene = false;

            if (sceneName.ToString().Contains("Logic") || sceneName.ToString().Contains("LOGIC")) {
                tokens = sceneName.Split('/');
                if (SceneManager.GetSceneByName(sceneName) != null) {
                    BeggarsStealerMod.currentscene = tokens[3];
                }
            } else {
                foreach (string i in logicscene) {
                    if (sceneName == i) {
                        islogicscene = true;
                        break;
                    }
                }
                if (islogicscene) {
                    tokens = sceneName.Split('/');
                    if (SceneManager.GetSceneByName(sceneName) != null) {
                        BeggarsStealerMod.currentscene = tokens[3];
                    }
                }
            }
        }
    }
}
