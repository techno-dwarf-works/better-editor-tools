# [Deprecated] Better Editor Tools

# !! New Repo !!
This plugin is deprecated, replaced with - [Better Commons](https://github.com/techno-dwarf-works/better-commons)

[![openupm](https://img.shields.io/npm/v/com.uurha.bettereditortools?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.uurha.bettereditortools/)

Collections of useful tools for Unity Editor. Such as:
1. DrawerHelpers
2. Better drop down
3. MultiEditor

## MultiEditor Usage

```c#
[BetterEditor(typeof(CustomClass), true, Order = 999, OverrideDefaultEditor = false)]
public class CustomEditor : EditorExtension
{
    public CustomEditor(Object target, SerializedObject serializedObject) : base(target, serializedObject)
    {
    }

    public override void OnDisable()
    {
       //This method called than editor disables
    }

    public override void OnEnable()
    {
       //This method called just right after instance created
    }

    public override void OnInspectorGUI()
    {
       //Use to draw your inspector
    }

    public override void OnChanged()
    {
       //Called when object data in editor is changed
    }
}
```

## Install
[How to install](https://github.com/uurha/BetterPluginCollection/wiki/How-to-install)
