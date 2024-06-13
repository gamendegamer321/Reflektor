using System.Collections;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using Reflektor.Patches;
using Reflektor.Windows;
using SpaceWarp;
using SpaceWarp.API.Mods;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Reflektor;

[BepInPlugin(ModGuid, ModName, ModVer)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class Reflektor : BaseSpaceWarpPlugin
{
    [PublicAPI] public const string ModGuid = "Reflektor";
    [PublicAPI] public const string ModName = "Reflektor";
    [PublicAPI] public const string ModVer = "0.2.1.2";

    // Instance stuff
    public static Reflektor Instance;

    private const string RootName = "_InspectorRoot";
    public static GameObject Root => GameObject.Find(RootName) ?? new GameObject(RootName);

    // Events
    public static event Action<SelectKey, bool> PropertyChangedEvent;

    // Config
    private ConfigEntry<KeyCode> _toggleAllUIShortcut;
    private ConfigEntry<KeyCode> _raycastShortcut;

    private KeyboardShortcut? _toggleAllKey;
    private KeyboardShortcut? _raycastShortcutKey;

    private Harmony _harmonyInstance;

    public ConfigEntry<int> MaxLogs;
    public ConfigEntry<int> MaxMessages;

    public override void OnInitialized()
    {
        base.OnInitialized();
        Log("Initializing");
        Instance = this;

        var rootGameObject = new GameObject("_InspectorRoot");
        DontDestroyOnLoad(rootGameObject);

        SpaceWarp.API.Game.Messages.StateChanges.GameStateChanged += (_, _, _) => Utils.ResetSorting();
        SpaceWarp.API.Game.Messages.StateChanges.GameStateChanged += (_, _, _) => RefreshBrowser();

        _toggleAllUIShortcut = Config.Bind(
            new ConfigDefinition("Settings", "Toggle UI Key"),
            KeyCode.F7,
            new ConfigDescription("Toggle all the reflektor UI with this key."));

        _raycastShortcut = Config.Bind("Settings", "Fire Raycast Key",
            KeyCode.R,
            "Fire a raycast with: this key + L SHIFT + L ALT");

        MaxLogs = Config.Bind("Settings", "Max logs count", 300,
            "The maximum amount of logs that can be stored before the oldest get removed");
        MaxMessages = Config.Bind("Settings", "Max messages count", 150,
            "The maximum amount of messages that can be stored before the oldest get removed");


        Config.SettingChanged += (_, _) => SetKeyboardShortcuts();
        Config.ConfigReloaded += (_, _) => SetKeyboardShortcuts();
        SetKeyboardShortcuts();

        _harmonyInstance = new Harmony(ModGuid);
        _harmonyInstance.PatchAll(typeof(BepInExLogPatch));
        _harmonyInstance.PatchAll(typeof(MessagesPatch));

        Log("Initialized");

        // Add Default Tabs
#if DEBUG
        Inspector.SwitchTab(new SelectKey(GameObject.Find("/GameManager")));

        GameObject g = new GameObject("[Testing]");
        DontDestroyOnLoad(g);
        ZZZ_TestClass t = g.AddComponent<ZZZ_TestClass>();
        if (t is not null)
        {
            Inspector.SwitchTab(new SelectKey(t));
        }
#endif

        ToggleDisplay();
    }

    private static void RefreshBrowser()
    {
        if (Instance == null)
        {
            return;
        }

        IEnumerator coroutine = RefreshBrowserAfterWait(3);
        Instance.StartCoroutine(coroutine);
    }

    private static IEnumerator RefreshBrowserAfterWait(int numFrames)
    {
        for (int i = 0; i < numFrames; i++)
        {
            yield return 0;
        }

        Browser.Refresh();
    }

    public void Update()
    {
        if (_toggleAllKey != null && _toggleAllKey.Value.IsDown())
        {
            ToggleDisplay();
        }

        if (_raycastShortcutKey != null && _raycastShortcutKey.Value.IsDown())
        {
            FireRay();
        }
    }

    public void FireRay()
    {
        PointerEventData pointerEventData = new(null);
        pointerEventData.position = Input.mousePosition;
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        var objects = raycastResults.Select(result => result.gameObject).ToList();

        Browser.ShowRaycastResults(objects);
    }

    public static void FirePropertyChangedEvent(SelectKey key, bool triggeredManually = false)
    {
        if (Instance is null)
        {
            return;
        }

        Instance.StartCoroutine(FireEventAfterWait((selectKey, b) =>
        {
            GameObject g = selectKey.Target.GetGameObject();
            if (g is not null)
            {
                PropertyChangedEvent?.Invoke(new SelectKey(g), b);
                foreach (Component c in g.GetComponents<Component>())
                {
                    PropertyChangedEvent?.Invoke(new SelectKey(c), b);
                }
            }
            else
            {
                PropertyChangedEvent?.Invoke(selectKey, b);
            }
        }, key, triggeredManually, 3));
    }

    private static IEnumerator FireEventAfterWait(Action<SelectKey, bool> eventAction, SelectKey obj, bool b,
        int numFrames)
    {
        for (int i = 0; i < numFrames; i++)
        {
            yield return 0;
        }

        eventAction.Invoke(obj, b);
    }

    public static void Inspect(SelectKey key)
    {
        if (Instance is not null)
        {
            Inspector.SwitchTab(key);
        }
    }

    private void SetKeyboardShortcuts()
    {
        if (_toggleAllUIShortcut != null)
        {
            _toggleAllKey =
                new KeyboardShortcut(_toggleAllUIShortcut.Value);
        }

        if (_raycastShortcut != null)
        {
            _raycastShortcutKey =
                new KeyboardShortcut(_raycastShortcut.Value, KeyCode.LeftShift, KeyCode.LeftAlt);
        }
    }

    public static void Log(object msg)
    {
        if (msg is IEnumerable<object> enumerable)
        {
            Log(enumerable);
        }

        StringBuilder sb = new();
        string list = sb.AppendJoin(", ", msg).ToString();
        Debug.Log($"<color=#00FF77>{ModName}: {list}</color>");
    }

    private static void Log(IEnumerable<object> msg)
    {
        StringBuilder sb = new();
        string list = sb.AppendJoin(", ", msg).ToString();
        Debug.Log($"<color=#00FF77>{ModName}: {list}</color>");
    }

    private void ToggleDisplay()
    {
        Navigator.UpdateValue(Navigator.WindowId.Inspector, false);
        Navigator.UpdateValue(Navigator.WindowId.Browser, false);
        Navigator.UpdateValue(Navigator.WindowId.Logger, false);
        Navigator.UpdateValue(Navigator.WindowId.Messages, false);

        Navigator.ToggleDisplay();
    }
}