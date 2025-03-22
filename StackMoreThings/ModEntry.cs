using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace StackMoreThings
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            CommonUtils.config = this.Config;
            CommonUtils.monitor = this.Monitor;

            var patches = new List<Type>
            {
                typeof(StackTacklePatches),
                typeof(StackRingsPatches),
                typeof(StackMeleeWeaponPatches),
                typeof(StackTrinketPatches),
                typeof(StackFurniturePatches),
                typeof(StackBootsPatches),
                typeof(StackHatPatches),
                typeof(StackClothingPatches),
                typeof(StackWallpaperPatches),
                typeof(MaxStackSizeGeneral),
                typeof(StackColorQuality),
            };

            // Is it stupid to patch Items.canStackWith every time? Probably.
            // Does it make it eaiser to think about? Yes
            foreach (var patch in patches)
            {
                foreach (var method in patch.GetMethods())
                {
                    bool isPrefix = method.Name.EndsWith("_Prefix");
                    bool isPostfix = method.Name.EndsWith("_Postfix");
                    if (isPrefix || isPostfix)
                    {
                        var arguments = method.GetParameters();
                        bool foundThingToBePatched = false;
                        foreach (var argument in arguments)
                        {
                            if (argument.Name == "__instance")
                            {
                                if (isPrefix)
                                {
                                    harmony.Patch(
                                        original: AccessTools.Method(
                                            argument.ParameterType,
                                            method.Name[..^7]
                                        ),
                                        prefix: new HarmonyMethod(
                                            patch,
                                            method.Name,
                                            method.GetParameters().Types()
                                        )
                                    );
                                }
                                else
                                {
                                    harmony.Patch(
                                        original: AccessTools.Method(
                                            argument.ParameterType,
                                            method.Name[..^8]
                                        ),
                                        postfix: new HarmonyMethod(
                                            patch,
                                            method.Name,
                                            method.GetParameters().Types()
                                        )
                                    );
                                }
                                foundThingToBePatched = true;
                                break;
                            }
                        }
                        if (!foundThingToBePatched)
                        {
                            this.Monitor.Log(
                                $"Failed to patch {method.Name} in {patch.Name}. No __instance parameter found.",
                                LogLevel.Error
                            );
                        }
                    }
                }
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(
                "spacechase0.GenericModConfigMenu"
            );
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("MaxStackSize"),
                tooltip: () => this.Helper.Translation.Get("MaxStackSizeTooltip"),
                getValue: () => this.Config.MaxStackSize.ToString(),
                setValue: value =>
                {
                    int stackSize = 9999;
                    int.TryParse(value, out stackSize);
                    if (stackSize < 0)
                    {
                        stackSize = 9999;
                    }
                    this.Config.MaxStackSize = stackSize;
                }
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("EnableComplexPatches"),
                tooltip: () => this.Helper.Translation.Get("EnableComplexPatchesTooltip"),
                getValue: () => this.Config.EnableComplexPatches,
                setValue: value => this.Config.EnableComplexPatches = value
            );

            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("Stacks")
            );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("SettingsInstructions")
            );

            Dictionary<string, string> configNames =
                new()
                {
                    {
                        "Tackle",
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12858")
                    },
                    { "Rings", Game1.content.LoadString("Strings\\StringsFromCSFiles:Ring.cs.1") },
                    { "Weapons", this.Helper.Translation.Get("Weapons") },
                    { "Trinkets", Game1.content.LoadString("Strings\\1_6_Strings:Trinket") },
                    {
                        "Furniture",
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12859")
                    },
                    {
                        "Boots",
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:Boots.cs.12501")
                    },
                    {
                        "Clothing",
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:category_clothes")
                    },
                };
            foreach (var configName in configNames)
            {
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => configNames[configName.Key],
                    getValue: () =>
                        this.Config.GetType().GetProperty(configName.Key)?.GetValue(this.Config)
                            as bool?
                        ?? false,
                    setValue: value =>
                        this.Config.GetType()
                            .GetProperty(configName.Key)
                            ?.SetValue(this.Config, value)
                );
            }
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("Keybinds")
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("ColorMerge"),
                tooltip: () => this.Helper.Translation.Get("ColorMergeTooltip"),
                getValue: () => this.Config.ColorMergeKey,
                setValue: (value) => this.Config.ColorMergeKey = value
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("QualityReduce"),
                tooltip: () => this.Helper.Translation.Get("QualityReduceTooltip"),
                getValue: () => this.Config.QualityReduceKey,
                setValue: (value) => this.Config.QualityReduceKey = value
            );
        }
    }
}
