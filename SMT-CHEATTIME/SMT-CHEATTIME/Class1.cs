using BepInEx;
using System;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace SMT.CheatTime
{
    [BepInPlugin("com.saysaa.smt_cheattime", "SMT CheatTime", "1.5")]
    public class TrainerPlugin : BaseUnityPlugin
    {
        private bool showMenu = true;
        private bool noClip = false;
        private string luckyMessage = "";
        private float messageTimer = 0f;
        private Vector2 scrollPos = Vector2.zero;

        private float speedValue = 1.0f;
        private int currentTab = 0;
        private bool unlockedLucky = false;

        private void Awake() => Logger.LogInfo("==== >_ SMT.CheatTime! | F3 to toggle menu ====");

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                showMenu = !showMenu;
                Cursor.visible = showMenu;
                Cursor.lockState = showMenu ? CursorLockMode.None : CursorLockMode.Locked;
                if (Camera.main != null && Camera.main.GetComponent("PlayerObjectController") is MonoBehaviour cam) cam.enabled = !showMenu;
            }

            if (messageTimer > 0)
            {
                messageTimer -= Time.deltaTime;
                if (messageTimer <= 0) luckyMessage = "";
            }

            if (noClip)
            {
                var p = GameObject.FindAnyObjectByType<PlayerObjectController>();
                var cam = Camera.main;
                if (p != null && cam != null)
                {
                    Vector3 dir = Vector3.zero;
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z)) dir += cam.transform.forward;
                    if (Input.GetKey(KeyCode.S)) dir -= cam.transform.forward;
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q)) dir -= cam.transform.right;
                    if (Input.GetKey(KeyCode.D)) dir += cam.transform.right;

                    dir.y = 0;

                    if (dir != Vector3.zero)
                    {
                        p.transform.position += dir.normalized * (0.15f * speedValue);
                    }
                }
            }
        }

        private void SetAllCharactersScale(float scale)
        {
            var players = GameObject.FindObjectsByType<PlayerObjectController>(FindObjectsSortMode.None);
            foreach (var p in players)
            {
                p.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        private void LuckyCheats() //I am Lucky button
        {
            System.Random rng = new System.Random();

            int maxChoices = unlockedLucky ? 6 : 5;
            int choice = rng.Next(0, maxChoices);

            messageTimer = 1.5f;

            switch (choice)
            {
                case 0:
                    if (GameData.Instance != null) GameData.Instance.NetworkgameFunds += 10000000f;
                    luckyMessage = "+10000000 $";
                    break;
                case 1:
                    if (GameData.Instance != null) GameData.Instance.NetworkgameFunds = 0f;
                    luckyMessage = "nooooo bye bye money";
                    break;
                case 2:
                    SetAllCharactersScale(3.5f);
                    luckyMessage = "GIANTTTT Mode !";
                    break;
                case 3:
                    SetAllCharactersScale(0.5f);
                    luckyMessage = "tiny Mode !";
                    break;
                case 4:
                    luckyMessage = "boummmm ahhhhhhh";
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    break;
                case 5:
                    var player = GameObject.FindAnyObjectByType<PlayerObjectController>();
                    if (player != null)
                    {
                        luckyMessage = "lost in space...";
                        Vector3 SpawnPosition = new Vector3(100000f, 100000f, 100000f);
                        player.transform.position = SpawnPosition;
                    }
                    break;
            }
        }

        private void ToggleNoClip()
        {
            noClip = !noClip;
            var p = GameObject.FindAnyObjectByType<PlayerObjectController>();
            if (p != null)
            {
                Component col = p.GetComponent("Collider");
                if (col != null)
                {
                    PropertyInfo prop = col.GetType().GetProperty("enabled");
                    if (prop != null) prop.SetValue(col, !noClip, null);
                }

                Component rb = p.GetComponent("Rigidbody");
                if (rb != null)
                {
                    PropertyInfo gravProp = rb.GetType().GetProperty("useGravity");
                    if (gravProp != null) gravProp.SetValue(rb, !noClip, null);
                }
            }
        }

        private void OnGUI()
        {
            if (!showMenu) return;

            if (messageTimer > 0)
            {
                GUIStyle style = new GUIStyle();
                style.fontSize = 24;
                style.normal.textColor = Color.yellow;
                style.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(0, Screen.height / 4, Screen.width, 50), luckyMessage, style);
            }

            float x = Screen.width - 290f;
            float y = 100f;
            float boxWidth = 250f;
            float boxHeight = 350f;

            GUI.Box(new Rect(x, y, boxWidth, boxHeight), "SMT.CheatTime! 1.5");

            if (GUI.Button(new Rect(x + 10, y + 30, 110, 25), "Cheats")) currentTab = 0;
            if (GUI.Button(new Rect(x + 130, y + 30, 110, 25), "Settings")) currentTab = 1;

            scrollPos = GUI.BeginScrollView(new Rect(x + 10, y + 60, boxWidth - 20, boxHeight - 70),
                                            scrollPos, new Rect(0, 0, boxWidth - 40, 550));

            switch (currentTab)
            {
                case 0: //cheats
                    if (GUI.Button(new Rect(0, 0, 210, 35), "[+] | 1M Money"))
                        GameData.Instance.NetworkgameFunds += 1000000f;

                    string ncLabel = noClip ? "[+] | No-Clip (ON)" : "[-] | No-Clip (OFF)";
                    if (GUI.Button(new Rect(0, 45, 210, 35), ncLabel))
                        ToggleNoClip();

                    if (GUI.Button(new Rect(0, 90, 210, 35), "[+] | 10 Franchise Points"))
                        GameData.Instance.NetworkgameFranchisePoints += 10;

                    if (GUI.Button(new Rect(0, 135, 210, 35), "[+] | Max Franchise Points"))
                        GameData.Instance.NetworkgameFranchisePoints += 10000;

                    if (unlockedLucky)
                    {
                        if (GUI.Button(new Rect(0, 180, 210, 35), "[?] | I am Lucky ? [ON]"))
                            LuckyCheats();
                    }
                    else
                    {
                        GUI.enabled = false;
                        GUI.Button(new Rect(0, 180, 210, 35), "[?] | I am Lucky ? [OFF]");
                        GUI.enabled = true;
                    }

                    GUI.Label(new Rect(0, 225, 210, 20), "[+] | SpeedHack : " + speedValue.ToString("F1") + "x");
                    speedValue = GUI.HorizontalSlider(new Rect(0, 250, 210, 15), speedValue, 1.0f, 10.0f);
                    Time.timeScale = speedValue;

                    if (GUI.Button(new Rect(0, 275, 210, 35), "[+] | Anti-Jail"))
                    {
                        GameObject worldBarriers = GameObject.Find("TheCoolRoom/Jail");

                        if (worldBarriers != null && worldBarriers.activeSelf)
                        {
                            worldBarriers.SetActive(false);
                            luckyMessage = "Jail Disabled ;)";
                            messageTimer = 2.0f;
                        }
                        else if (worldBarriers == null)
                        {
                            luckyMessage = "The Jail Doesn't Exist";
                            messageTimer = 2.0f;
                        }
                        else if (!worldBarriers.activeSelf)
                        {
                            luckyMessage = "The Jail is already disabled";
                            messageTimer = 2.0f;
                        }
                    }

                    if (GUI.Button(new Rect(0, 320, 210, 35), "[+] | Grab all steals"))
                    {
                        StolenProductSpawn[] allCheckouts = FindObjectsByType<StolenProductSpawn>(FindObjectsSortMode.None);

                        foreach (StolenProductSpawn checkout in allCheckouts)
                        {
                            checkout.CmdRecoverStolenProduct();
                        }

                        luckyMessage = "Grabbed all Stolen Products";
                        messageTimer = 2.0f;
                    }

                    if (GUI.Button(new Rect(0, 365, 210, 35), "[+] | Disable Barrier"))
                    {
                        GameObject worldBarriers = GameObject.Find("Level_Exterior/Colliders");

                        if (worldBarriers != null && worldBarriers.activeSelf)
                        {
                            worldBarriers.SetActive(false);
                            luckyMessage = "World Borders Disabled";
                        }
                        else if (worldBarriers == null)
                        {
                            luckyMessage = "World Borders Don't Exist";
                        }
                        else if (!worldBarriers.activeSelf)
                        {
                            luckyMessage = "World Borders already got disabled";
                        }
                    }

                    if (GUI.Button(new Rect(0, 410, 210, 35), "[+] | Free DLC"))
                    {
                        GameObject worldBarriers = GameObject.Find("TheCoolRoom/AddonCollider");
                        GameObject posesAndDancesText = GameObject.Find("TheCoolRoom/Canvas_Skins/Container/PosesAndDancesText");
                        GameObject characterNumber = GameObject.Find("TheCoolRoom/Canvas_Skins/Container/CharacterNumber");
                        GameObject hatNumber = GameObject.Find("TheCoolRoom/Canvas_Skins/Container/HatNumber");
                        GameObject poses = GameObject.Find("TheCoolRoom/Canvas_Skins/Container/Poses");
                        GameObject tvController = GameObject.Find("TheCoolRoom/Canvas_TVsHook/Container/");

                        if (posesAndDancesText != null && !posesAndDancesText.activeSelf) posesAndDancesText.SetActive(true);
                        if (characterNumber != null && !characterNumber.activeSelf) characterNumber.SetActive(true);
                        if (hatNumber != null && !hatNumber.activeSelf) hatNumber.SetActive(true);
                        if (poses != null && !poses.activeSelf) poses.SetActive(true);
                        if (tvController != null && !tvController.activeSelf) tvController.SetActive(true);

                        if (worldBarriers != null) worldBarriers.SetActive(false);

                        Builder_Main builderMain = GameObject.Find("GameCanvas").GetComponent<Builder_Main>();
                        if (builderMain != null)
                        {
                            builderMain.playerIsCool = true;
                            Traverse.Create(builderMain).Field("isCool").SetValue(true);
                            Traverse.Create(builderMain).Field("canPlace").SetValue(true);
                            builderMain.ActivateUIInfo(true);
                        }

                        luckyMessage = "Cool mode activated!";
                    }

                    if (GUI.Button(new Rect(0, 455, 210, 35), "[+] | Random Teleport"))
                    {
                        var allPlayers = GameObject.FindObjectsByType<PlayerObjectController>(FindObjectsSortMode.None);
                        Vector3 teleportOffset = new Vector3(15f, 5f, 15f);

                        foreach (var player in allPlayers)
                        {
                            if (player != null)
                            {
                                var controller = player.GetComponent<CharacterController>();

                                if (controller != null)
                                {
                                    controller.enabled = false;
                                    player.transform.position += teleportOffset;
                                    controller.enabled = true;
                                }
                                else
                                {
                                    player.transform.position += teleportOffset;
                                }
                            }
                        }
                        luckyMessage = "Everyone Pushed !";
                        messageTimer = 2.0f;
                    }
                    break;

                case 1: //settings
                    string unlock = unlockedLucky ? "I am Lucky mod [ON]" : "I am Lucky mod [OFF]";
                    if (GUI.Button(new Rect(0, 0, 210, 35), unlock))
                    {
                        unlockedLucky = !unlockedLucky;
                    }

                    if (GUI.Button(new Rect(0, 40, 210, 35), "SMT.CheatTime Github Repo"))
                        System.Diagnostics.Process.Start("https://github.com/saiitanaa/SMT.CheatTime");
                    break;
            }

            GUI.EndScrollView();
        }
    }
}